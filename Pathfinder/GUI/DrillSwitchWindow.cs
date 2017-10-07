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
    public class DrillSwitchWindow : Dialog<DrillSwitchWindow>
    {
        private const string insufficientResourcesMsg = "Unable to reconfigure the drill. You need {0:f2} {1:s} to reconfigure it.";
        private const string kInsufficientSkill = "Unable to reconfigure the drill. You need a skilled {0:s}. to reconfigure it.";
        private const string kDrillReconfigured = "Drill reconfigured";
        private const int kWindowWidth = 300;
        private const int kWindowHeight = 310;
        private const int kDrillLabelWidth = 220;

        public List<ModuleResourceHarvester> groundDrills;
        public List<string> resourceList = new List<string>();
        public Part part;

        public string requiredResource;
        public float reconfigureCost;
        public string requiredSkill;

        private Vector2 scrollPos;
        private List<int> groundDrillResourceIndexes = new List<int>();
        private bool biomIsUnlocked;

        public DrillSwitchWindow(string title = "Drill Modifications") :
            base(title, kWindowWidth, kWindowHeight)
        {
            WindowTitle = title;
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            int index;
            base.SetVisible(newValue);

            if (newValue)
            {
                //See if the biome is unlocked.
                biomIsUnlocked = Utils.IsBiomeUnlocked(this.part.vessel);

                //Get the list of resources for the biome
                IEnumerator<string> nameEnumerator = ResourceMap.Instance.FetchAllResourceNames(HarvestTypes.Planetary).GetEnumerator();
                while (nameEnumerator.MoveNext())
                {
                    resourceList.Add(nameEnumerator.Current);
                }

                //For each drill, find the index of the resource that it drills for.
                foreach (ModuleResourceHarvester drill in groundDrills)
                {
                    for (index = 0; index < resourceList.Count; index++)
                    {
                        if (drill.ResourceName == resourceList[index])
                        {
                            groundDrillResourceIndexes.Add(index);
                            break;
                        }
                    }
                }
            }
        }

        protected override void DrawWindowContents(int windowId)
        {

            GUILayout.BeginVertical();

            if (biomIsUnlocked)
            {
                if (resourceList.Count > 0)
                {
                    scrollPos = GUILayout.BeginScrollView(scrollPos, new GUIStyle(GUI.skin.textArea));

                    drawGroundDrills();

                    if (GUILayout.Button("Reconfigure Drill"))
                        reconfigureDrill();

                    GUILayout.EndScrollView();
                }

                else
                {
                    GUILayout.Label("Biome appears unlocked but the resource map isn't listing any resources.");
                }
            }

            else
            {
                GUILayout.Label("You need to perform some geology research before you can switch what the drill mines.");
            }
            GUILayout.EndVertical();
        }

        protected void reconfigureDrill()
        {
            //If required, make sure we have the proper skill
            if (WBIMainSettings.RequiresSkillCheck)
            {
                if (FlightGlobals.ActiveVessel.isEVA && Utils.IsExperienceEnabled())
                {
                    Vessel vessel = FlightGlobals.ActiveVessel;
                    ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                    if (astronaut.HasEffect(requiredSkill))
                    {
                        ScreenMessages.PostScreenMessage(string.Format(kInsufficientSkill, requiredSkill), 5.0f, ScreenMessageStyle.UPPER_CENTER);
                        return;
                    }
                }
            }

            //If needed, pay the cost to reconfigure
            if (WBIMainSettings.PayToReconfigure)
            {
                //Make sure we can afford it
                PartResourceDefinition definition = ResourceHelper.DefinitionForResource(requiredResource);

                //Pay for the reconfiguration cost.
                double partsPaid = this.part.RequestResource(definition.id, reconfigureCost, ResourceFlowMode.ALL_VESSEL);

                //Could we afford it?
                if (Math.Abs(partsPaid) / Math.Abs(reconfigureCost) < 0.999f)
                {
                    ScreenMessages.PostScreenMessage(string.Format(insufficientResourcesMsg, reconfigureCost, requiredResource), 6.0f, ScreenMessageStyle.UPPER_CENTER);

                    //Put back what we took
                    this.part.RequestResource(definition.id, -partsPaid, ResourceFlowMode.ALL_VESSEL);
                    return;
                }
            }

            //Now reconfigure the drill.
            ModuleResourceHarvester drill;
            string resourceName;
            for (int drillIndex = 0; drillIndex < groundDrills.Count; drillIndex++)
            {
                drill = groundDrills[drillIndex];
                resourceName = resourceList[groundDrillResourceIndexes[drillIndex]];
                setupDrillGUI(drill, resourceName);
            }
            ScreenMessages.PostScreenMessage(kDrillReconfigured, 5.0f, ScreenMessageStyle.UPPER_CENTER);
        }

        protected void drawGroundDrills()
        {
            ModuleResourceHarvester drill;
            string resourceName;
            int index;

            for (index = 0; index < groundDrills.Count; index++)
            {
                drill = groundDrills[index];

                //Previous resource
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("<", GUILayout.Width(20)))
                {
                    groundDrillResourceIndexes[index] -= 1;

                    if (groundDrillResourceIndexes[index] < 0)
                        groundDrillResourceIndexes[index] = resourceList.Count - 1;
                }

                //Drill labeling
                resourceName = resourceList[groundDrillResourceIndexes[index]];
                GUILayout.Label("Resource: " + resourceName, GUILayout.Width(kDrillLabelWidth));

                //Next resource
                if (GUILayout.Button(">", GUILayout.Width(20)))
                {
                    groundDrillResourceIndexes[index] += 1;

                    if (groundDrillResourceIndexes[index] >= resourceList.Count)
                        groundDrillResourceIndexes[index] = 0;
                }
                GUILayout.EndHorizontal();
            }
        }

        protected void setupDrillGUI(ModuleResourceHarvester drill, string resourceName)
        {
            drill.ResourceName = resourceName;
            drill.StartActionName = "Start " + resourceName + " drill";
            drill.StopActionName = "Stop " + resourceName + " drill";
            drill.Fields["ResourceStatus"].guiName = resourceName + " rate";
        }
    }
}
