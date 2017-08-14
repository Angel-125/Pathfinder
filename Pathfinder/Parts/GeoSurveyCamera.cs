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
    public class GeoSurveyCamera : WBIBasicScienceLab
    {
        private const string kSafeMode = "T.E.R.R.A.I.N. has suffered a malfunction and is now in safe mode. It needs maintennance to resume operations.";
        private const string kToolTip = "Now that you’ve performed a basic scan for resources, why not continue to monitor them? With the T.E.R.R.A.I.N. you can monitor resources from orbit for Science!";
        private const string kToolTipTitle = "Your first planetary scan";
        private const string kNotEnoughResourcesToRepair = "Unable to repair the T.E.R.R.A.I.N. due to insufficient resources. You need {0:f1} ";
        private const string kInfoRepairSkill = "Required to conduct repairs: ";
        private const string kInfoRepairAmount = "Requires {0:f1} {1} to repair";
        private const string kExperimentTitle = "Dirt Watch Report orbiting ";
        private const string kNoScience = "No research data accumulated, check back later.";
        private const string kGenericDirtWatchResults = "Continuous monitoring of the resources has yielded some interesting results that have advanced knowledge about this celestial body.";

        [KSPField(guiActive = true, guiName = "Science Collected")]
        public string scienceCollected;

        ModuleOrbitalSurveyor orbitalSurveyer;
        ModuleOrbitalScanner orbitalScanner;
        ModuleScienceContainer scienceContainer;
        WBIResultsDialogSwizzler swizzler;
        bool monitorSurvey;

        [KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = true, unfocusedRange = 3f, guiName = "Perform orbital survey", guiActiveEditor = false, guiActive = true)]
        public void PerformOrbitalSurvey()
        {
            //Roll to see if the telescope breaks.
            //If so, we're done and we need to repair the telescope.

            //We're good, perform the survey
            monitorSurvey = true;
            orbitalSurveyer.PerformSurvey();
        }

        public override void OnPartFixed(ModuleQualityControl qualityControl)
        {
            base.OnPartFixed(qualityControl);
            SetupGUI();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Grab the survey module
            orbitalSurveyer = this.part.FindModuleImplementing<ModuleOrbitalSurveyor>();
            orbitalScanner = this.part.FindModuleImplementing<ModuleOrbitalScanner>();

            //Verify that the planet is really unlocked
            //verifyPlanetUnlock();

            //Hide stock survey GUI
            if (orbitalSurveyer != null)
            {
                orbitalSurveyer.Events["PerformSurvey"].guiActive = false;
                orbitalSurveyer.Events["PerformSurvey"].guiActiveUnfocused = false;
                orbitalSurveyer.Events["PerformSurvey"].guiActiveEditor = false;
            }

            //Create swizzler
            swizzler = new WBIResultsDialogSwizzler();
            swizzler.onTransmit = transmitData;

            //Setup the science container
            scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();
            scienceContainer.Events["ReviewDataEvent"].guiActiveUnfocused = false;
            scienceContainer.Events["ReviewDataEvent"].guiActive = false;

            //Now setup our own GUI
            botchedResultsMsg = kSafeMode;
            SetupGUI();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //If we started a survey, then watch to see when the planet is unlocked.
            //Once the planet is unlocked, hide our orbital survey button and enable
            //resource monitoring
            if (monitorSurvey)
            {
                if (ResourceMap.Instance.IsPlanetScanned(FlightGlobals.currentMainBody.flightGlobalsIndex))
                {
                    //Check to see if we should show the tooltip
                    if (WBIPathfinderScenario.Instance.HasShownToolTip(this.ClassName) == false)
                    {
                        WBIPathfinderScenario.Instance.SetToolTipShown(this.ClassName);
                        WBIToolTipWindow introWindow = new WBIToolTipWindow(kToolTipTitle, kToolTip);
                        introWindow.SetVisible(true);
                    }
                    Events["PerformOrbitalSurvey"].guiActive = false;
                    Events["PerformOrbitalSurvey"].guiActiveUnfocused = false;
                    monitorSurvey = false;
                    SetupGUI();
                }
            }

            if (ModuleIsActive())
            {
                Fields["scienceCollected"].guiActive = true;
                scienceCollected = string.Format("{0:f2}", scienceAdded);
            }
            else
            {
                Fields["scienceCollected"].guiActive = false;
            }
        }

        public override string GetInfo()
        {
            string info = base.GetInfo();

            info += kInfoRepairSkill + repairSkill + "\r\n";
            info += string.Format(kInfoRepairAmount, repairAmount, repairResource);

            return info;
        }

        public override void ReviewData()
        {
            float scienceCap = WBIBiomeAnalysis.GetScienceCap(this.part);
            float totalScience = scienceAdded;

            if (totalScience < 0.001f)
            {
                ScreenMessages.PostScreenMessage(kNoScience, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            while (totalScience > 0.001f)
            {
                //Set the experiment title
                if (totalScience < scienceCap)
                    dataAmount = totalScience;
                else
                    dataAmount = scienceCap;

                //Generate lab data
                ScienceData data = WBIBiomeAnalysis.CreateData(this.part, dataAmount);
                scienceContainer.AddData(data);

                //Deduct from the total
                totalScience -= dataAmount;
                if (totalScience <= 0.001f)
                    scienceAdded = 0f;
            }

            //Make sure we have some science to transmit.
            if (scienceContainer.GetScienceCount() == 0)
            {
                ScreenMessages.PostScreenMessage(kNoScience, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //Review the data
            scienceContainer.ReviewData();
            swizzler.SwizzleResultsDialog();
        }

        protected bool transmitData(ScienceData data)
        {
            if (data.subjectID.Contains(WBIBiomeAnalysis.kBiomeAnalysisID))
                WBIBiomeAnalysis.ResetScienceGains(this.part);

            return true;
        }

        public override void TransmitResults()
        {
            if (transmitHelper.TransmitToKSC(dataAmount, 0, 0))
            {
                scienceAdded -= dataAmount;
                if (scienceAdded <= 0.001)
                    scienceAdded = 0f;

                if (scienceAdded > 0.001)
                    ReviewData();
            }
        }

        protected override void onFailure()
        {
            base.onFailure();
            status = "Inconclusive";
        }

        protected override void onSuccess()
        {
            base.onSuccess();
            status = "Good results";
        }


        protected override void onCriticalSuccess()
        {
            base.onCriticalSuccess();
            status = "Great results";
        }

        public void SetupGUI()
        {
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            bool planetUnlocked = ResourceMap.Instance.IsPlanetScanned(FlightGlobals.currentMainBody.flightGlobalsIndex);

            //Is the telescope broken? if so, show the repair scope button
            //and don't allow any research
            if (isBroken)
            {
                //Cannot perform an orbital survey...
                Events["PerformOrbitalSurvey"].active = false;

                //Hide survey scanner GUI
                if (orbitalScanner != null)
                    orbitalScanner.DisableModule();

                //No research can be performed.
                SetGuiVisible(false);
                return;
            }

            //Show scanner GUI?
            if (planetUnlocked && orbitalScanner != null)
                orbitalScanner.EnableModule();

            //Show the perform orbital survey button if we've unlocked the planet
            Events["PerformOrbitalSurvey"].active = !planetUnlocked;

            //Ditto for the resource monitoring
            SetGuiVisible(planetUnlocked);
        }

        protected void verifyPlanetUnlock()
        {
            if (orbitalSurveyer == null)
                return;
            if (ResourceMap.Instance == null)
                return;

            bool planetUnlocked = ResourceMap.Instance.IsPlanetScanned(FlightGlobals.currentMainBody.flightGlobalsIndex);

            //Weird edge case where the user hit F9 to reload, planet claims it is unlocked (was unlocked before F9)
            //but it shouldn't be because we just reloaded to a point were the planet wasn't unlocked. 
            //Somehow the ModuleOrbitalSurveyor knows it's not really unlocked.
            //Fortunately if a planet is unlocked, then you shouldn't see the perform survey button.
            if (orbitalSurveyer.Events["PerformSurvey"].guiActive == true && planetUnlocked)
            {
                //Go find the planet scan data in the resource map and delete it.
                List<PlanetScanData> scanData = ResourceMap.Instance.PlanetScanInfo;
                PlanetScanData doomed = null;
                foreach (PlanetScanData data in scanData)
                    if (data.PlanetId == FlightGlobals.currentMainBody.flightGlobalsIndex)
                    {
                        doomed = data;
                        break;
                    }
                if (doomed != null)
                    scanData.Remove(doomed);
            }
        }

    }
}
