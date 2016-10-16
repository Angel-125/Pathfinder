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
    public enum GeologyLabExperiments
    {
        SoilAnalysis,
        MetallurgyAnalysis,
        ChemicalAnalysis,
        BiomeAnalysis,
        Drilling,
        None
    }

    [KSPModule("Geology Lab")]
    public class WBIGeologyLab : WBIBasicScienceLab, IOpsView
    {
        const float kBiomeResearchCost = 100f;
        const float kBiomeAnalysisFactor = 0.75f;
        const string kNoScientistsOrTerrain = "Unable to perform the analysis, there are no scientists staffing the lab, and no T.E.R.R.A.I.N. satellites in orbit.";
        const string kAnalysisUplink = "Soil analysis uplinked to KSC via T.E.R.R.A.I.N. and analyzed at KSC.";
        const string kNoCrew = "At least one cremember must staff the lab in order to perform the analysis.";
        const string kNoScientists = "At least one Scientist must staff the lab in order to perform the analysis.";
        const string kOutOfSamples = "No core samples to start research. Either retrieve core samples from a drill, or generate a biome analysis report.";
        const string kTTTitle = "Research: ";
        const string kTTSoilAnalysis = "Core samples can be analyzed for their nutrient composition to see how well crops would grow in the dirt. Good results will help you grow crops more efficiently and break down waste products. Bad results will have the opposite effect.";
        const string kTTMetalAnalysis = "Core samples can be analyzed for their metallurgic content and can provide clues about how well in-situ resources will perform when creating construction materials. The better the results, the better your construction material creation will be. Bad results will have the opposite effect.";
        const string kTTChemAnalysis = "The chemical composition of core samples can be useful to know when using in-situ resources to induce chemical reactions. Good results will improve your chemical processes, while bad results will worsen them.";
        const string kTTBiomeAnalysis = "The Biome Analysis costs 100 Science to perform, but you have the potential to improve your return on investment with a good result. Similarly, a poor result will result in lost knowledge. When you hit the transmit button, you'll have the option to transmit research for Science, publish it to gain Reputation, or sell it for Funds. Finally, the analysis counts as a core sample when attempting to improve your production efficiencies.";
        const string kTTDrilling = "Core samples can be analyzed to help improve drilling efficiencies. Good results will help you improve your extraction yields, while poor results will worsen them.";
        const string kBetterEfficiency = "<color=lime>Production efficiency improved by <b>{0:f2}%</b> for ";
        const string kWorseEfficiency = "<color=orange>Production efficiency worsened by <b>{0:f2}%</b> for ";
        const string kLifeSupport = "life support processors.";
        const string kManufacturing = "fabrication processors.";
        const string kChemicalProducts = "chemical processeors.";
        const string kExtraction = "extraction rates.";
        const string kNotEnoughScience = "Not enough Science in the budget to continue research. Efforts wasted!";

        [KSPField]
        public int harvestID;

        [KSPField]
        public bool canImproveEfficiency = true;

        protected IParentView parentView = null;
        ModuleBiomeScanner biomeScanner;
        ModuleGPS gps;
        PartModule impactSeismometer;
        IScienceDataContainer impactSensor;
        private Vector2 scrollPosResources;
        List<PlanetaryResource> resourceList;
        string[] experimentTypes = { "Soil", "Metallurgy", "Chemical", "Biome", "Drilling" };
        double[] elapsedTimes = new double[5];
        ModuleScienceContainer scienceContainer;
        WBIResultsDialogSwizzler swizzler;
        GeologyLabExperiments currentExperiment;
        TerainUplinkView terrainUplinkView = new TerainUplinkView();
        private string biomeName;
        private int planetID = -1;
        HarvestTypes harvestType;

        #region Overrides
        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            if (node.HasValue("currentExperiment"))
                currentExperiment = (GeologyLabExperiments)Enum.Parse(typeof(GeologyLabExperiments), node.GetValue("currentExperiment"));
            else
                currentExperiment = GeologyLabExperiments.None;

            if (node.HasValue("experimentTimes"))
            {
                string experimentTimes = node.GetValue("experimentTimes");
                string[] times = experimentTimes.Split(new char[] { ';' });
                for (int index = 0; index < times.Length; index++)
                    elapsedTimes[index] = float.Parse(times[index]);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            string times = "";

            for (int index = 0; index < elapsedTimes.Length; index++)
                times += elapsedTimes[index].ToString() + ";";
            times = times.Substring(0, times.Length - 1);

            node.SetValue("experimentTimes", times, true);
            node.SetValue("currentExperiment", currentExperiment.ToString(), true);
        }

        public override void OnStart(StartState state)
        {
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            showGUI = false;
            base.OnStart(state);
            SetGuiVisible(false);
            showResults = false;

            resourceList = ResourceMap.Instance.GetResourceItemList(HarvestTypes.Planetary, this.part.vessel.mainBody);

            //Create swizzler
            swizzler = new WBIResultsDialogSwizzler();
            swizzler.onTransmit = transmitData;

            //Elapsed time for current experiment
            if (ModuleIsActive())
            {
                //Get the new elapsed time.
                int elapsedTimeIndex = (int)currentExperiment;
                elapsedTime = elapsedTimes[elapsedTimeIndex];

                //Reset the research start time.
                cycleStartTime = Planetarium.GetUniversalTime() - elapsedTime;
            }

            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
            biomeName = biome.name;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
                harvestType = (HarvestTypes)harvestID;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            setupPartModules();

            //Hack!
            if (Events["StartResourceConverter"].guiActive || Events["StopResourceConverter"].guiActive)
                SetGuiVisible(false);

            if (impactSeismometer != null)
            {
                BaseEvent reviewEvent = impactSeismometer.Events["reviewEvent"];
                if (reviewEvent != null)
                {
                    reviewEvent.guiActive = false;
                    reviewEvent.guiActiveUnfocused = false;
                }
            }
        }

        protected override void OnGUI()
        {
            if (terrainUplinkView.IsVisible())
                terrainUplinkView.DrawWindow();
        }
        #endregion

        #region Helpers
        protected void perfomBiomeAnalysys()
        {
            //We need at least one crewmember in the lab.
            if (this.part.protoModuleCrew.Count == 0)
            {
                ScreenMessages.PostScreenMessage(kNoCrew, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //We need at least one scientist in the lab, or one TERRAIN satellite in orbit.
            float scienceBonus = getBiomeAnalysisBonus();

            //We can run the analysis, add the science bonus
            if (scienceBonus > 0.0f)
            {
                //Generate lab data
                ScienceData data = WBIBiomeAnalysis.CreateData(this.part, scienceBonus);
                scienceContainer.AddData(data);
                scienceContainer.ReviewDataItem(data);
                swizzler.SwizzleResultsDialog();
            }

            //Ok, do we at least have a TERRAIN satellite in orbit?
            else if (planetHasTerrainSat())
            {
                ScreenMessages.PostScreenMessage(kAnalysisUplink, kMessageDuration * 1.5f, ScreenMessageStyle.UPPER_CENTER);
            }

            else
            {
                ScreenMessages.PostScreenMessage(kNoScientistsOrTerrain, kMessageDuration * 1.5f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //Run the analysis
            biomeScanner.RunAnalysis();
            resourceList = ResourceMap.Instance.GetResourceItemList(HarvestTypes.Planetary, this.part.vessel.mainBody);
        }

        protected bool transmitData(ScienceData data)
        {
            //If user is transmitting a biome analysis,
            //ask user to Transmit, Publish, or Sell data.
            if (data.subjectID.Contains(WBIBiomeAnalysis.kBiomeAnalysisID))
                WBIBiomeAnalysis.ResetScienceGains(this.part);

            return true;
        }

        public override void TransmitResults()
        {
            WBIBiomeAnalysis.ResetScienceGains(this.part);
        }

        protected bool planetHasTerrainSat()
        {
            foreach (Vessel vessel in FlightGlobals.VesselsUnloaded)
            {
                if (vessel == this.part.vessel)
                    continue;
                if (vessel.mainBody != this.part.vessel.mainBody)
                    continue;
                if (vessel.situation != Vessel.Situations.ORBITING)
                    continue;

                ProtoVessel protoVessel = vessel.protoVessel;
                foreach (ProtoPartSnapshot partSnapshot in protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot moduleSnapshot in partSnapshot.modules)
                    {
                        if (moduleSnapshot.moduleName == "GeoSurveyCamera")
                            return true;
                    }
                }
            }

            return false;
        }

        protected float getBiomeAnalysisBonus()
        {
            float bonus = 0f;

            foreach (ProtoCrewMember crewMember in this.part.protoModuleCrew)
                if (crewMember.HasEffect(ExperienceEffect))
                {
                    //One point for being a scientist.
                    bonus += 1.0f;

                    //One point for each experience level.
                    bonus += crewMember.experienceLevel;
                }

            return bonus * kBiomeAnalysisFactor;
        }

        #endregion

        protected void setupPartModules()
        {
            //GPS
            if (gps == null)
            {
                gps = this.part.FindModuleImplementing<ModuleGPS>();
                gps.Fields["bioName"].guiActive = false;
                gps.Fields["body"].guiActive = false;
                gps.Fields["lat"].guiActive = false;
                gps.Fields["lon"].guiActive = false;
            }

            //Biome Scanner
            if (biomeScanner == null)
            {
                biomeScanner = this.part.FindModuleImplementing<ModuleBiomeScanner>();
                biomeScanner.Events["RunAnalysis"].guiActive = false;
                biomeScanner.Events["RunAnalysis"].guiActiveUnfocused = false;
            }

            //Setup the science container
            if (scienceContainer == null)
            {
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();
                scienceContainer.Events["ReviewDataEvent"].guiActiveUnfocused = false;
                scienceContainer.Events["ReviewDataEvent"].guiActive = false;

                //Terrain uplink
                terrainUplinkView.part = this.part;
                terrainUplinkView.scienceContainer = scienceContainer;
            }

            //Grab the seismometer (if any)
            foreach (PartModule mod in this.part.Modules)
                if (mod.moduleName == "Seismometer")
                {
                    impactSeismometer = mod;
                    impactSensor = (IScienceDataContainer)impactSeismometer;
                    ScienceData[] impactData = impactSensor.GetData();

                    foreach (ScienceData data in impactData)
                        scienceContainer.AddData(data);
                    foreach (ScienceData doomed in impactData)
                        impactSensor.DumpData(doomed);
                    break;
                }

            //Resource list
            if (resourceList == null)
                resourceList = ResourceMap.Instance.GetResourceItemList(HarvestTypes.Planetary, this.part.vessel.mainBody);
            else if (resourceList.Count == 0)
                resourceList = ResourceMap.Instance.GetResourceItemList(HarvestTypes.Planetary, this.part.vessel.mainBody);
        }

        protected Vessel terrainVesselTarget;

        public void SwitchVessel(Vessel targetVessel)
        {
            terrainUplinkView.SetVisible(false);
            terrainVesselTarget = targetVessel;
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            SetGuiVisible(isVisible);
        }

        public void SetParentView(IParentView parentView)
        {
            this.parentView = parentView;
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("GeoLab");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            if (!HighLogic.LoadedSceneIsFlight)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("Geology Lab");
                GUILayout.Label("This configuration is working, but the contents can only be accessed in flight.");
                GUILayout.EndVertical();
                return;
            }

            bool biomeUnlocked = Utils.IsBiomeUnlocked(this.part.vessel);
            GUILayout.BeginHorizontal();
            drawAbundanceGUI(biomeUnlocked);

            //C&C buttons
            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Width(300));

            //Biome analysis button
            if (biomeUnlocked == false)
                if (GUILayout.Button("Perform biome analysis"))
                    perfomBiomeAnalysys();

            //Review Data button
            int totalData = scienceContainer.GetStoredDataCount();
            if (totalData > 0)
            {
                if (GUILayout.Button("Review [" + totalData + "] Data"))
                {
                    scienceContainer.ReviewData();
                    swizzler.SwizzleResultsDialog();
                }
            }

            //Research projects/Terrain
            if (canImproveEfficiency)
                drawResearchProjectsGUI(biomeUnlocked);
            else
                GUILayout.Label("<color=white>Biome unlocked</color>");

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            //Update elapsed time
            if (ModuleIsActive() && currentExperiment != GeologyLabExperiments.None)
            {
                int elapsedTimeIndex = (int)currentExperiment;
                elapsedTimes[elapsedTimeIndex] = elapsedTime;
            }
        }

        protected void drawAbundanceGUI(bool biomeUnlocked)
        {
            float habitationEfficiency = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestType, EfficiencyData.kHabitationMod);
            float scienceEfficiency = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestType, EfficiencyData.kScienceMod);
            float industryEfficiency = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestType, EfficiencyData.kIndustryMod);
            string abundance = string.Empty;

            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(100));
            GUILayout.Label("<color=white><b>Location:</b> " + gps.body + " " + gps.bioName + "</color>");
            GUILayout.Label("<color=white><b>Lon:</b> " + gps.lon + "</color>");
            GUILayout.Label("<color=white><b>Lat:</b> " + gps.lat + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(100));
            GUILayout.Label("<color=white><b>Habitation Efficiency:</b> " + string.Format("{0:f1}%", habitationEfficiency * 100f) + "</color>");
            GUILayout.Label("<color=white><b>Science Efficiency:</b> " + string.Format("{0:f1}%", scienceEfficiency * 100f) + "</color>");
            GUILayout.Label("<color=white><b>Industry Efficiency:</b> " + string.Format("{0:f1}%", industryEfficiency * 100f) + "</color>");
            GUILayout.EndScrollView();

            if (biomeUnlocked)
            {
                IEnumerable<ResourceCache.AbundanceSummary> abundanceCache = ResourceCache.Instance.AbundanceCache.
                    Where(a => a.HarvestType == HarvestTypes.Planetary && a.BodyId == this.part.vessel.mainBody.flightGlobalsIndex && a.BiomeName == biomeName);

                if (abundanceCache.Count<ResourceCache.AbundanceSummary>() > 0)
                {
                    scrollPosResources = GUILayout.BeginScrollView(scrollPosResources, new GUIStyle(GUI.skin.textArea));

                    foreach (ResourceCache.AbundanceSummary summary in abundanceCache)
                    {
                        abundance = string.Format("{0:f2}%", (summary.Abundance * 100f));
                        GUILayout.Label("<color=white>" + summary.ResourceName + " abundance: " + abundance + "</color>");
                    }
                }
                else
                {
                    scrollPosResources = GUILayout.BeginScrollView(scrollPosResources, new GUIStyle(GUI.skin.textArea));
                    GUILayout.Label("<color=yellow>No detectable resources in this area.</color>");
                }
            }

            else
            {
                scrollPosResources = GUILayout.BeginScrollView(scrollPosResources, new GUIStyle(GUI.skin.textArea));
                GUILayout.Label("<color=yellow>Unlock the biome to get the resource composition.</color>");
            }


            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected string getExperimentName(GeologyLabExperiments experiment)
        {
            switch (experiment)
            {
                default:
                case GeologyLabExperiments.None:
                    return "None";

                case GeologyLabExperiments.SoilAnalysis:
                    return "Soil Analysis";

                case GeologyLabExperiments.MetallurgyAnalysis:
                    return "Metallurgic Analysis";

                case GeologyLabExperiments.ChemicalAnalysis:
                    return "Chemical Analysis";

                case GeologyLabExperiments.BiomeAnalysis:
                    return "Biome Analysis";
            }
        }

        protected void drawResearchProjectsGUI(bool biomeUnlocked)
        {
            string researchButtonTitle;
            int coreSampleCount = countCoreSamples();

            //Research projects (soil analysis, metallurgy, chemical analysis)
            if (biomeUnlocked)
            {
                GUILayout.Label("<color=white>Core samples available: " + coreSampleCount + "</color>");
                if (ModuleIsActive())
                {
                    GUILayout.Label("<color=white>Current research: " + getExperimentName(currentExperiment) + "</color>");
                    GUILayout.Label("<color=white>Progress: " + progress + "</color>");
                    researchButtonTitle = "Stop Research";
                }
                else
                {
                    GUILayout.Label("<color=white>Current research: None</color>");
                    GUILayout.Label("<color=white>Progress: None</color>");
                    researchButtonTitle = "Start Research";
                }

                //Start/stop research button
                drawResearchButtonGUI(researchButtonTitle, coreSampleCount);

                //Geology experiment to research
                drawGeoExperimentGUI();

                if (GUILayout.Button("T.E.R.R.A.I.N. Uplink"))
                {
                    terrainUplinkView.parentView = this.parentView;
                    terrainUplinkView.SetVisible(true);
                }
            }
        }

        protected void drawResearchButtonGUI(string researchButtonTitle, int coreSampleCount)
        {
            if (GUILayout.Button(researchButtonTitle))
            {
                IsActivated = !IsActivated;

                if (ModuleIsActive())
                {
                    //Make sure we have enough scientists
                    if (getBiomeAnalysisBonus() == 0f)
                    {
                        ScreenMessages.PostScreenMessage(kNoScientists, kMessageDuration * 1.5f, ScreenMessageStyle.UPPER_CENTER);
                        StopConverter();
                        return;
                    }

                    //Make sure we have enough core samples
                    if (coreSampleCount == 0 && currentExperiment != GeologyLabExperiments.BiomeAnalysis)
                    {
                        ScreenMessages.PostScreenMessage(kOutOfSamples, 6.0f, ScreenMessageStyle.UPPER_CENTER);
                        StopConverter();
                        return;
                    }

                    //Get the new elapsed time.
                    int elapsedTimeIndex = (int)currentExperiment;
                    elapsedTime = elapsedTimes[elapsedTimeIndex];

                    //Reset the research start time.
                    cycleStartTime = Planetarium.GetUniversalTime() - elapsedTime;

                    //Show tooltip
                    checkAndShowToolTip();

                    StartConverter();
                }

                else
                {
                    int elapsedTimeIndex = (int)currentExperiment;
                    elapsedTimes[elapsedTimeIndex] = elapsedTime;

                    StopConverter();
                }
            }
        }

        protected void drawGeoExperimentGUI()
        {
            //If user has switched the experiment to research, then we must record the elapsed time for that experiment.
            GeologyLabExperiments experiment = (GeologyLabExperiments)GUILayout.SelectionGrid((int)currentExperiment, experimentTypes, 2);
            if (experiment != currentExperiment && ModuleIsActive())
            {
                int elapsedTimeIndex;

                //Record current elapsed time.
                if (currentExperiment != GeologyLabExperiments.None)
                {
                    elapsedTimeIndex = (int)currentExperiment;
                    elapsedTimes[elapsedTimeIndex] = elapsedTime;
                }

                //Get the new elapsed time.
                elapsedTimeIndex = (int)experiment;
                elapsedTime = elapsedTimes[elapsedTimeIndex];

                //Reset the research start time.
                cycleStartTime = Planetarium.GetUniversalTime() - elapsedTime;

                //Show tooltip
                checkAndShowToolTip();
            }

            //Record the new experiment
            currentExperiment = experiment;
        }

        protected int countCoreSamples()
        {
            int coreSamples = 0;
            ScienceData[] dataList = scienceContainer.GetData();

            foreach (ScienceData data in dataList)
            {
                if (data.subjectID.Contains("WBICoreSampleAnalysis") || data.subjectID.Contains(WBIBiomeAnalysis.kBiomeAnalysisID))
                    coreSamples += 1;
            }

            return coreSamples;
        }

        protected void checkAndShowToolTip()
        {
            //Check tooltips
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            string experimentName = getExperimentName(currentExperiment);
            string experimentTip;

            //Add first time for redecoration
            if (scenario.HasShownToolTip(experimentName) == false)
            {
                scenario.SetToolTipShown(experimentName);

                switch (currentExperiment)
                {
                    default:
                    case GeologyLabExperiments.SoilAnalysis:
                        experimentTip = kTTSoilAnalysis;
                        break;

                    case GeologyLabExperiments.MetallurgyAnalysis:
                        experimentTip = kTTMetalAnalysis;
                        break;

                    case GeologyLabExperiments.ChemicalAnalysis:
                        experimentTip = kTTChemAnalysis;
                        break;

                    case GeologyLabExperiments.BiomeAnalysis:
                        experimentTip = kTTBiomeAnalysis;
                        break;

                    case GeologyLabExperiments.Drilling:
                        experimentTip = kTTDrilling;
                        break;
                }

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kTTTitle + experimentName, experimentTip);
                toolTipWindow.SetVisible(true);
            }
        }

        protected bool scientistIsAboard()
        {
            foreach (ProtoCrewMember crewMember in this.part.protoModuleCrew)
                if (crewMember.HasEffect(ExperienceEffect))
                    return true;

            return false;
        }

        protected string getAbundance(string resourceName)
        {
            AbundanceRequest request = new AbundanceRequest();
            double lattitude = ResourceUtilities.Deg2Rad(this.part.vessel.latitude);
            double longitude = ResourceUtilities.Deg2Rad(this.part.vessel.longitude);

            request.BiomeName = Utils.GetCurrentBiome(this.part.vessel).name;
            request.BodyId = this.part.vessel.mainBody.flightGlobalsIndex;
            request.Longitude = longitude;
            request.Latitude = lattitude;
            request.CheckForLock = true;
            request.ResourceName = resourceName;

            float abundance = ResourceMap.Instance.GetAbundance(request) * 100.0f;

            if (abundance > 0.001)
                return string.Format("{0:f2}%", abundance);
            else
                return "Unknown";
        }
        #endregion

        #region BasicScienceLab
        protected override void addCurrency()
        {
            //Stub this out.
        }

        protected override void onSuccess()
        {
            if (scienceContainer == null)
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            //Time to pay the piper
            if (currentExperiment == GeologyLabExperiments.BiomeAnalysis)
            {
                if (ResearchAndDevelopment.Instance.Science >= kBiomeResearchCost)
                {
                    ResearchAndDevelopment.Instance.AddScience(-kBiomeResearchCost, TransactionReasons.RnDTechResearch);
                }

                //Not enough science. Efforts wasted.
                else
                {
                    ScreenMessages.PostScreenMessage(kNotEnoughScience, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    StopConverter();
                    return;
                }

                //Generate biome analysis
                ScienceData data = WBIBiomeAnalysis.CreateData(this.part, kBiomeResearchCost);
                scienceContainer.AddData(data);
            }

            else
            {
                float efficiencyModifier = 0.1f + (totalCrewSkill / 200f);

                applyResults(efficiencyModifier, kBetterEfficiency);

                //Reduce core sample count
                dumpFirstCoreSample();
            }
        }

        protected override void onFailure()
        {
            if (scienceContainer == null)
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            //Reduce core sample count
            dumpFirstCoreSample();
        }

        protected override void onCriticalSuccess()
        {
            if (scienceContainer == null)
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            //Time to pay the piper
            if (currentExperiment == GeologyLabExperiments.BiomeAnalysis)
            {
                if (ResearchAndDevelopment.Instance.Science >= kBiomeResearchCost)
                {
                    ResearchAndDevelopment.Instance.AddScience(-kBiomeResearchCost, TransactionReasons.RnDTechResearch);
                }

                //Not enough science. Efforts wasted.
                else
                {
                    ScreenMessages.PostScreenMessage(kNotEnoughScience, 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    StopConverter();
                    return;
                }

                //Generate biome analysis
                ScienceData data = WBIBiomeAnalysis.CreateData(this.part, kBiomeResearchCost * (1.0f + getBiomeAnalysisBonus() / 100f));
                scienceContainer.AddData(data);
            }

            else
            {
                float efficiencyModifier = 0.1f + (totalCrewSkill / 100f);

                applyResults(efficiencyModifier, kBetterEfficiency);

                //Reduce core sample count
                dumpFirstCoreSample();
            }
        }

        protected override void onCriticalFailure()
        {
            if (scienceContainer == null)
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            applyResults(-0.1f, kWorseEfficiency);

            //Time to pay the piper
            if (currentExperiment == GeologyLabExperiments.BiomeAnalysis)
            {
                if (ResearchAndDevelopment.Instance.Science >= kBiomeResearchCost)
                    ResearchAndDevelopment.Instance.AddScience(-kBiomeResearchCost, TransactionReasons.RnDTechResearch);
            }

            //Reduce core sample count
            dumpFirstCoreSample();
        }

        protected void dumpFirstCoreSample()
        {
            if (scienceContainer == null)
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();

            ScienceData[] dataList = scienceContainer.GetData();

            foreach (ScienceData data in dataList)
            {
                if (data.subjectID.Contains("WBICoreSampleAnalysis") || data.subjectID.Contains(WBIBiomeAnalysis.kBiomeAnalysisID))
                {
                    scienceContainer.DumpData(data);
                    return;
                }
            }
        }

        protected void applyResults(float efficiencyModifier, string resultsPreamble)
        {
            int planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            HarvestTypes harvestType = HarvestTypes.Planetary;
            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
            string biomeName = biome.name;
            string modifierName = "";
            string processChanged = "";

            switch (currentExperiment)
            {
                case GeologyLabExperiments.SoilAnalysis:
                    modifierName = EfficiencyData.kHabitationMod;
                    processChanged = kLifeSupport;
                    break;

                case GeologyLabExperiments.MetallurgyAnalysis:
                    modifierName = EfficiencyData.kIndustryMod;
                    processChanged = kManufacturing;
                    break;

                case GeologyLabExperiments.Drilling:
                    modifierName = EfficiencyData.kExtractionMod;
                    processChanged = kExtraction;
                    efficiencyModifier /= 2.0f;
                    break;

                default:
                case GeologyLabExperiments.ChemicalAnalysis:
                    modifierName = EfficiencyData.kScienceMod;
                    processChanged = kChemicalProducts;
                    break;
            }

            //Get existing modifier
            float currentModifer = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biome.name, harvestType, modifierName);

            //Add improvement
            currentModifer += efficiencyModifier;
            WBIPathfinderScenario.Instance.SetEfficiencyData(planetID, biome.name, harvestType, modifierName, currentModifer);

            //Inform user
            string message = string.Format(resultsPreamble, efficiencyModifier * 100f) + processChanged + "</color>";
            ScreenMessages.PostScreenMessage(message, 8.0f, ScreenMessageStyle.UPPER_CENTER);
        }

        #endregion
    }
}
