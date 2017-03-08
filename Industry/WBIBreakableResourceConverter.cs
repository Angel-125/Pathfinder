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
    [KSPModule("Breakable Resource Converter")]
    public class WBIBreakableResourceConverter : WBIResourceConverter, IOpsView
    {
        private const string kNotEnoughResourcesToRepair = "Unable to repair due to insufficient resources. You need {0:f1} ";
        private const string kInfoRepairSkill = "Best skill for repairs: ";
        private const string kInfoRepairAmount = "Requires {0:f1} {1} to repair";
        private const string kNeedsRepairs = "Needs repairs";
        private float kMessageDuration = 5.0f;

        [KSPField]
        public string progressLabel = "Progress";

        [KSPField]
        public string efficiencyType;

        [KSPField]
        public int harvestType;

        [KSPField(isPersistant = true)]
        public float criticalFailModifier;

        [KSPField(isPersistant = true)]
        public bool isBroken;

        [KSPField]
        public string repairResource;

        [KSPField]
        public float repairAmount;

        [KSPField]
        public string repairSkill;

        [KSPField(isPersistant = true)]
        private float originalCriticalFail = -1;

        [KSPField(isPersistant = true)]
        private float currentCriticalFail = -1;

        private string biomeName;
        private int planetID = -1;
        HarvestTypes harvestID;
        protected string efficiencyString = "Efficiency";

        [KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = true, unfocusedRange = 3f, guiName = "Perform repairs", guiActiveEditor = false)]
        public void PerformRepairs()
        {
            //Do we require resources to fix the scope?
            //If so, make sure the kerbal on EVA has enough resources.
            if (WBIMainSettings.RepairsRequireResources)
            {
                float repairUnits = calculateRepairCost();

                //Not enough resources to effect repairs? Tell the player.
                if (repairUnits < 0.0f)
                {
                    string message = string.Format(kNotEnoughResourcesToRepair, repairAmount) + repairResource;
                    ScreenMessages.PostScreenMessage(message, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }

                //We have enough, deduct the repair cost
                FlightGlobals.ActiveVessel.rootPart.RequestResource(repairResource, repairUnits, ResourceFlowMode.ALL_VESSEL);
            }

            //Finally, unset broken.
            criticalFail = originalCriticalFail;
            currentCriticalFail = criticalFail;
            isBroken = false;
            SetupGUI();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (originalCriticalFail == -1)
            {
                originalCriticalFail = criticalFail;
                currentCriticalFail = criticalFail;
            }

            criticalFail = currentCriticalFail;

            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

            biomeName = biome.name;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                harvestID = (HarvestTypes)harvestType;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            }

            float efficiencyModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestID, efficiencyType);
            if (this.EfficiencyBonus != efficiencyModifier)
                this.EfficiencyBonus = efficiencyModifier;

            SetupGUI();
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (ModuleIsActive() && isBroken)
            {
                StopConverter();
                status = kNeedsRepairs;
            }

            SetGuiVisible(showGUI);
        }

        public override string GetInfo()
        {
            string info = base.GetInfo();

            if (WBIMainSettings.RepairsRequireResources)
                info += kInfoRepairSkill + repairSkill + "\r\n";
            info += string.Format(kInfoRepairAmount, repairAmount, repairResource);

            return info;
        }

        protected float calculateRepairCost()
        {
            if (this.part.vessel == null)
                return -1.0f;

            float repairUnits = repairAmount;
            double totalResources = ResourceHelper.GetTotalResourceAmount(repairResource, this.part.vessel);

            //Anybody can repair the scope, but the right skill can reduce the cost by as much as 60%
            ProtoCrewMember astronaut = FlightGlobals.ActiveVessel.GetVesselCrew()[0];
            if (astronaut.HasEffect(repairSkill))
                repairUnits = repairUnits * (0.9f - (astronaut.experienceTrait.CrewMemberExperienceLevel() * 0.1f));

            //Now make sure the kerbal has enough resources to conduct repairs.
            //Get the resource definition
            PartResourceDefinition definition = ResourceHelper.DefinitionForResource(repairResource);
            if (definition == null)
                return -1.0f;

            //make sure the ship has enough of the resource
            if (totalResources < repairUnits)
                return -1.0f;

            return repairUnits;
        }

        public void SetupGUI()
        {
            SetGuiVisible(showGUI);

            if (isBroken)
            {
                //Enable repair button
                Events["PerformRepairs"].guiActiveUnfocused = true;
                status = kNeedsRepairs;
                return;
            }

            //Hide repair button
            Events["PerformRepairs"].guiActiveUnfocused = false;
            Events["PerformRepairs"].guiActive = false;

            Fields["progress"].guiName = progressLabel;
        }

        protected override void onFailure()
        {
            base.onFailure();

            //The chances of a critical failure increase
            criticalFail += criticalFailModifier;
            currentCriticalFail = criticalFail;
            StopConverter();
        }

        protected override void onSuccess()
        {
        }

        protected override void onCriticalSuccess()
        {
            criticalFail -= criticalFailModifier;

            if (criticalFail < originalCriticalFail)
                criticalFail = originalCriticalFail;

            currentCriticalFail = criticalFail;
        }

        protected override void onCriticalFailure()
        {
            if (!WBIMainSettings.PartsCanBreak)
                return;

            base.onCriticalFailure();

            isBroken = true;
            StopConverter();
            SetupGUI();

            status = kNeedsRepairs;
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public virtual void SetContextGUIVisible(bool isVisible)
        {
            SetGuiVisible(isVisible);
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public virtual List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add(ConverterName);
            return buttonLabels;
        }

        public virtual void DrawOpsWindow(string buttonLabel)
        {
            string absentResource = GetMissingRequiredResource();
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(140));
            if (string.IsNullOrEmpty(absentResource))
                GUILayout.Label("<color=white><b>Status: </b>" + this.status + "</color>");
            else
                GUILayout.Label("<color=white><b>Status: </b>Requires " + absentResource + "</color>");
            GUILayout.Label(string.Format("<color=white><b>" + efficiencyString + ": </b>{0:f2}%</color>", this.EfficiencyBonus * 100f));
            GUILayout.Label("<color=white><b>Time Until Maintennance: </b>" + getTimeUntilCheck() + "</color>");
            GUILayout.Label("<color=white><b>Failure Probability: </b>" + criticalFail + "%</color>");
            GUILayout.EndScrollView();

            if (ModuleIsActive())
            {
                if (GUILayout.Button(StopActionName))
                    StopConverter();
            }

            else if (GUILayout.Button(StartActionName))
            {
                StartConverter();
            }

            GUILayout.EndVertical();
        }

        protected string getTimeUntilCheck()
        {
            double timeUntilCheck = (GetSecondsPerCycle() - elapsedTime);

            if (timeUntilCheck > 21600f)
                return string.Format("{0:f1} days", timeUntilCheck / 21600);
            else
                return string.Format("{0:f1} hrs", timeUntilCheck / 3600f);
        }

        #endregion
    }
}
