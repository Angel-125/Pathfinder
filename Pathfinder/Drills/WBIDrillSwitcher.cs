using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2015, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
If you want to use this code, give me a shout on the KSP forums! :)
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class WBIDrillSwitcher : ExtendedPartModule
    {
        const string kEngineerNeeded = "You need an engineer in order to modify the drill.";
        const string kDrillSwitchTooltip = "Do you always want to drill just for ore? Why not re-engineer the drill and dig up something else?";
        const string kToolTipTitle = "Drilling More Than Ore";

        [KSPField]
        public string requiredResource;

        [KSPField]
        public float reconfigureCost;

        [KSPField]
        public string requiredSkill;

        protected List<ModuleResourceHarvester> groundDrills;
        protected string[] drillResources;
        protected DrillSwitchWindow drillSwitchWindow = new DrillSwitchWindow();

        [KSPEvent(guiActive = true, guiName = "Reset Drill GUI")]
        public void ResetDrillGUI()
        {
            if (groundDrills != null)
            {
                foreach (ModuleResourceHarvester harvester in groundDrills)
                    harvester.Events["StartResourceConverter"].guiActive = true;
            }
        }

        public void OnGUI()
        {
            if (drillSwitchWindow.IsVisible())
                drillSwitchWindow.DrawWindow();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            //Load all the drill info
            drillResources = node.GetValues("drillResource");
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            //Save current drill resource info
            if (groundDrills != null)
            {
                foreach (ModuleResourceHarvester harvester in groundDrills)
                    node.AddValue("drillResource", harvester.ResourceName);
            }
        }

        public override void OnStart(StartState state)
        {
            ModuleResourceHarvester harvester;
            base.OnStart(state);

            //Get the drills
            groundDrills = this.part.FindModulesImplementing<ModuleResourceHarvester>();

            //Setup the drills with the new resource to drill for.
            if (drillResources != null)
            {
                for (int index = 0; index < drillResources.Length; index++)
                {
                    harvester = groundDrills[index];
                    harvester.ResourceName = drillResources[index];
                    harvester.StartActionName = "Start " + drillResources[index] + " Drill";
                    harvester.StopActionName = "Stop " + drillResources[index] + " Drill";
                    harvester.Fields["ResourceStatus"].guiName = drillResources[index] + " rate";
                }
            }

            //Setup the window
            drillSwitchWindow.groundDrills = groundDrills;
            drillSwitchWindow.part = this.part;
            drillSwitchWindow.reconfigureCost = reconfigureCost;
            drillSwitchWindow.requiredResource = requiredResource;
            drillSwitchWindow.requiredSkill = requiredSkill;

            //Setup GUI
            Events["ShowDrillSwitchWindow"].guiActiveUnfocused = Utils.HasResearchedNode(PathfinderAppView.drillTechNode);

            //Tooltip
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (!scenario.HasShownToolTip(this.ClassName))
            {
                scenario.SetToolTipShown(this.ClassName);

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kToolTipTitle, kDrillSwitchTooltip);
                toolTipWindow.SetVisible(true);
            }
        }

        [KSPEvent(guiName = "Modify Drill", guiActive = true, guiActiveEditor = false, guiActiveUnfocused = true, unfocusedRange = 3.0f)]
        public void ShowDrillSwitchWindow()
        {
            //Make sure we have an experienced engineer.
            if (FlightGlobals.ActiveVessel.isEVA && Utils.IsExperienceEnabled())
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                if (astronaut.HasEffect(requiredSkill) && WBIMainSettings.RequiresSkillCheck)
                {
                    ScreenMessages.PostScreenMessage(kEngineerNeeded, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
            }

            drillSwitchWindow.groundDrills = groundDrills;
            drillSwitchWindow.SetVisible(true);
        }
    }
}
