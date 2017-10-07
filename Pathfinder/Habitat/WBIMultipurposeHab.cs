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
    [KSPModule("Multipurpose Hab")]
    public class WBIMultipurposeHab : WBIMultiConverter, IModuleInfo
    {
        private const string kSettingsWindow = "Settings Window";
        private const string kPartsTip = "Don't want to pay to redecorate? No problem. Just press Mod P (the modifier key defaults to the Alt key on Windows) to open the Settings window and uncheck the option.\r\n\r\n";
        private const string kPonderosaOpsView = "Ponderosa Operations";
        private const string kResourceDistributed = "Distributed {0:f2} units of {1:s}";

        [KSPField]
        public string partToolTip;

        [KSPField]
        public string partToolTipTitle;

        [KSPField]
        public string opsViewTitle;

        protected Animation anim;
        protected PartModule impactSeismometer;
        protected PartModule exWorkshop;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsEditor)
                this.part.CrewCapacity = 0;

            foreach (PartModule mod in this.part.Modules)
            {
                if (mod.moduleName == "Seismometer")
                    impactSeismometer = mod;
                else if (mod.moduleName == "ExWorkshop")
                    exWorkshop = mod;
            }

            opsManagerView.WindowTitle = this.part.partInfo.title + " Operations";

            if (string.IsNullOrEmpty(resourcesToKeep))
                resourcesToKeep = "ElectricCharge";

            if (!isInflatable)
                return;
            if (string.IsNullOrEmpty(animationName))
                return;
            anim = this.part.FindModelAnimators(animationName)[0];
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (anim == null)
            {
                checkAndShowToolTip();
                return;
            }

            //We're only interested in the act of inflating the module.
            if (isDeployed == false)
            {
                animationStarted = false;
                return;
            }

            //If we've completed the animation then we are done.
            if (animationStarted == false)
                return;

            //Animation may not be done yet.
            if (anim.isPlaying)
                return;

            //At this point we know that the animation was playing but has now stopped.
            //We also know that the animation was started. Now reset the flag.
            animationStarted = false;

            //Hide seismometer
            if (impactSeismometer != null)
            {
                IScienceDataContainer container = (IScienceDataContainer)impactSeismometer;

                BaseEvent reviewEvent = impactSeismometer.Events["reviewEvent"];
                if (reviewEvent != null)
                {
                    reviewEvent.guiActive = false;
                    reviewEvent.guiActiveUnfocused = false;
                }

                //clear out the impact data if the current template isn't a geology lab
                if (CurrentTemplateName != "Geology Lab")
                {
                    ScienceData[] dataItems = container.GetData();
                    if (dataItems != null)
                        foreach (ScienceData doomed in dataItems)
                            container.DumpData(doomed);
                }
            }

            checkAndShowToolTip();
        }

        public override void UpdateContentsAndGui(string templateName)
        {
            ModuleScienceContainer scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            if (scienceContainer != null)
            {
                ScienceData[] dataQueue = scienceContainer.GetData();
                if (dataQueue != null)
                {
                    if (dataQueue.Length > 0)
                    {
                        scienceContainer.Events["ReviewDataEvent"].guiActiveUnfocused = true;
                        scienceContainer.Events["ReviewDataEvent"].guiActive = true;
                    }
                }
            }

            //Now reconfigure
            base.UpdateContentsAndGui(templateName);

            //Check to see if we've displayed the tooltip for the template.
            //First, we're only interested in deployed modules.
            if (isInflatable && isDeployed == false)
                return;

            //Now check
            checkAndShowToolTip();
        }

        public override void RedecorateModule(bool loadTemplateResources = true)
        {
            base.RedecorateModule(loadTemplateResources);
            updateDrill();
            updateWorkshop();
        }

        protected void updateWorkshop()
        {
            PartModule oseWorkshop = null;
            PartModule oseRecycler = null;
            bool enableWorkshop = false;

            //See if the drill is enabled.
            if (CurrentTemplate.HasValue("enableWorkshop"))
                enableWorkshop = bool.Parse(CurrentTemplate.GetValue("enableWorkshop"));

            //Find the workshop modules
            foreach (PartModule pm in this.part.Modules)
            {
                if (pm.moduleName == "OseModuleWorkshop")
                    oseWorkshop = pm;
                else if (pm.moduleName == "OseModuleRecycler")
                    oseRecycler = pm;
            }

            if (oseWorkshop != null)
            {
                oseWorkshop.enabled = enableWorkshop;
                oseWorkshop.isEnabled = enableWorkshop;
            }

            if (oseRecycler != null)
            {
                oseRecycler.enabled = enableWorkshop;
                oseRecycler.isEnabled = enableWorkshop;
            }
        }

        protected void updateDrill()
        {
            bool enableDrill = false;
            float value;
            string resourceName;
            ModuleResourceHarvester harvester = this.part.FindModuleImplementing<ModuleResourceHarvester>();

            //No drill? No need to proceed.
            if (harvester == null)
                return;

            //See if the drill is enabled.
            if (CurrentTemplate.HasValue("enableDrill"))
                enableDrill = bool.Parse(CurrentTemplate.GetValue("enableDrill"));

            ModuleOverheatDisplay overheat = this.part.FindModuleImplementing<ModuleOverheatDisplay>();
            if (overheat != null)
            {
                overheat.enabled = enableDrill;
                overheat.isEnabled = enableDrill;
            }

            ModuleCoreHeat coreHeat = this.part.FindModuleImplementing<ModuleCoreHeat>();
            if (coreHeat != null)
            {
                coreHeat.enabled = enableDrill;
                coreHeat.isEnabled = enableDrill;
            }

            WBIDrillSwitcher drillSwitcher = this.part.FindModuleImplementing<WBIDrillSwitcher>();
            if (drillSwitcher != null)
            {
                drillSwitcher.enabled = enableDrill;
                drillSwitcher.isEnabled = enableDrill;
            }

            WBIEfficiencyMonitor extractionMonitor = this.part.FindModuleImplementing<WBIEfficiencyMonitor>();
            if (extractionMonitor != null)
            {
                extractionMonitor.enabled = enableDrill;
                extractionMonitor.isEnabled = enableDrill;
            }

            //Update the drill
            if (enableDrill)
                harvester.EnableModule();
            else
                harvester.DisableModule();

            //Setup drill parameters
            if (enableDrill)
            {
                if (CurrentTemplate.HasValue("converterName"))
                    harvester.ConverterName = CurrentTemplate.GetValue("converterName");

                if (CurrentTemplate.HasValue("drillStartAction"))
                {
                    harvester.StartActionName = CurrentTemplate.GetValue("drillStartAction");
                    harvester.Events["StartResourceConverter"].guiName = CurrentTemplate.GetValue("drillStartAction");
                }

                if (CurrentTemplate.HasValue("drillStopAction"))
                {
                    harvester.StopActionName = CurrentTemplate.GetValue("drillStopAction");
                    harvester.Events["StopResourceConverter"].guiName = CurrentTemplate.GetValue("drillStopAction");
                }

                if (CurrentTemplate.HasValue("drillEficiency"))
                    harvester.Efficiency = float.Parse(CurrentTemplate.GetValue("drillEficiency"));

                if (CurrentTemplate.HasValue("drillResource"))
                {
                    resourceName = CurrentTemplate.GetValue("drillResource");
                    harvester.ResourceName = resourceName;
                    harvester.Fields["ResourceStatus"].guiName = resourceName + " rate";
                }

                if (CurrentTemplate.HasValue("drillElectricCharge"))
                {
                    if (float.TryParse(CurrentTemplate.GetValue("drillElectricCharge"), out value))
                    {
                        ResourceRatio[] inputRatios = harvester.inputList.ToArray();
                        for (int inputIndex = 0; inputIndex < inputRatios.Length; inputIndex++)
                        {
                            if (inputRatios[inputIndex].ResourceName == "ElectricCharge")
                                inputRatios[inputIndex].Ratio = value;
                        }
                    }
                }

                harvester.Fields["status"].guiName = "Drill Status";
                MonoUtilities.RefreshContextWindows(this.part);
            }
        }

        protected override void notEnoughParts()
        {
            base.notEnoughParts();

            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;

            //Add first time for redecoration
            if (scenario.HasShownToolTip(kSettingsWindow) == false)
            {
                scenario.SetToolTipShown(kSettingsWindow);

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kSettingsWindow, kPartsTip);
                toolTipWindow.SetVisible(true);
            }
        }

        protected override bool canAffordReconfigure(string templateName, bool deflatedModulesAutoPass = true)
        {
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            bool canAfford = base.canAffordReconfigure(templateName, deflatedModulesAutoPass);
            string requiredName = templateManager[templateName].GetValue(WBIAffordableSwitcher.kRequiredResourceField);

            //If the vessel can't afford to reconfigure the module, then maybe the distribution manager can help.
            if (canAfford == false)
            {
                ScreenMessages.PostScreenMessage("Checking distributors...", 10.0f);
                if (string.IsNullOrEmpty(requiredName))
                    return true;

                double distributedAmount = WBIDistributionManager.Instance.GetDistributedAmount(requiredName);
                Log("Distributors have " + distributedAmount + " units of " + requiredName);
                if (distributedAmount >= reconfigureCost)
                {
                    ScreenMessages.PostScreenMessage("Distributors have enough " + requiredName, 10.0f);
                    return true;
                }
                else
                {
                    ScreenMessages.PostScreenMessage("No active distributors have " + requiredName + " to share. Make sure resource distribution is turned on, and a distributor is sharing " + requiredName + ".", 10.0f);
                    canAfford = false;
                }
            }

            //Add first time for redecoration
            if (!canAfford && scenario.HasShownToolTip(kSettingsWindow) == false)
            {
                scenario.SetToolTipShown(kSettingsWindow);

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kSettingsWindow, kPartsTip);
                toolTipWindow.SetVisible(true);
            }

            return canAfford;
        }

        protected override bool payPartsCost(int templateIndex)
        {
            bool canAffordCost = base.payPartsCost(templateIndex);

            //Maybe the distribution manager can help?
            if (canAffordCost == false)
            {
                double remodelCost = calculateRemodelCost(templateIndex);
                string resourceName = templateManager[templateIndex].GetValue(WBIAffordableSwitcher.kRequiredResourceField);
                if (string.IsNullOrEmpty(resourceName))
                    return true;

                double amountObtained = WBIDistributionManager.Instance.RequestDistributedResource(resourceName, remodelCost, false);
                if (amountObtained > 0f)
                    return true;
            }

            return canAffordCost;
        }

        protected override void recoverResourceCost(string resourceName, double recycleAmount)
        {
            double availableStorage = ResourceHelper.GetTotalResourceSpaceAvailable(resourceName, this.part.vessel);

            //Do we have sufficient space in the vessel to store the recycled parts?
            //If not, distrubute what we don't have space for.
            if (availableStorage < recycleAmount)
            {
                double amountRemaining = recycleAmount - availableStorage;
                string recycleMessage;

                //We'll only recycle what we have room to store aboard the vessel.
                recycleAmount = availableStorage;

                //Distribute the rest.
                List<DistributedResource> recycledResources = new List<DistributedResource>();
                recycleMessage = string.Format(kResourceDistributed, amountRemaining, resourceName);
                recycledResources.Add(new DistributedResource(resourceName, amountRemaining, recycleMessage));
                WBIDistributionManager.Instance.DistributeResources(recycledResources);
            }

            //Store what we have space for.
            this.part.RequestResource(resourceName, -recycleAmount, ResourceFlowMode.ALL_VESSEL);
        }

        protected void checkAndShowToolTip()
        {
            //Now we can check to see if the tooltip for the current template has been shown.
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (scenario.HasShownToolTip(CurrentTemplateName) && scenario.HasShownToolTip(getMyPartName()))
                return;

            //Tooltip for the current template has never been shown. Show it now.
            string toolTipTitle = CurrentTemplate.GetValue("toolTipTitle");
            string toolTip = CurrentTemplate.GetValue("toolTip");

            if (string.IsNullOrEmpty(toolTipTitle))
                toolTipTitle = partToolTipTitle;

            //Add the very first part's tool tip.
            if (scenario.HasShownToolTip(getMyPartName()) == false)
            {
                toolTip = partToolTip + "\r\n\r\n" + toolTip;

                scenario.SetToolTipShown(getMyPartName());
            }

            if (string.IsNullOrEmpty(toolTip) == false)
            {
                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(toolTipTitle, toolTip);
                toolTipWindow.SetVisible(true);

                //Cleanup
                scenario.SetToolTipShown(CurrentTemplateName);
            }
        }

        protected override void hideEditorGUI(PartModule.StartState state)
        {
            base.hideEditorGUI(state);
//            Events["ToggleInflation"].guiActiveEditor = false;
        }

        public override string GetInfo()
        {
            return "Click the Manage Operations button to change the configuration.";
        }

        public string GetModuleTitle()
        {
            return "Multipurpose Hab";
        }

        public string GetPrimaryField()
        {
            return "Inflated Crew Capacity: " + inflatedCrewCapacity.ToString();
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }
    }
}
