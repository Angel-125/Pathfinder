using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public enum CoreSampleStates
    {
        Ready,
        Deploying,
        TakingSample,
        Retracting
    }

    public class WBIModuleCoreSample : ModuleScienceExperiment
    {
        private const float kLabBonusModifier = 0.6f;
        private const float kBaseReputationLoss = 10f;
        private const float maxWorsenRoll = 40f;
        private const float minImprovementRoll = 60f;
        private const float kBaseEfficiencyModifier = 0.1f;
        private const float kExperiencePercentModifier = 3.0f;
        private const float kMessageDuration = 5.0f;
        private const float kWarningMsgDuration = 10.0f;
        private const string kNeedVesselLanded = "Vessel needs to be landed in order to peform analysis.";
        private const string kNeedVesselSplashed = "Vessel needs to be splashed in order to peform analysis.";
        private const string kUnlockBiome = "Perform a surface scan before performing the analysis.";
        private const string kResourceExtractionImproved = "Extraction rates for all drills increased by {0:#.##}% for ";
        private const string kResourceExtractionWorsened = "Extraction rates for all drills reduced by {0:#.##}% for ";
        private const string kResourceExtractionUnchanged = "Extraction rates for all drills unchanged for ";
        private const string kAnalysisStatus = "Analyzing results, please wait...";
        private const string kNoMoreAttempts = "Analysis cycle completed. Either move to a new biome or declare your results invalid.";
        private const string kUnknown = "Unknown";
        private const string kConfirmInvalidate = "Declaring core sample results invalid will adversely affect your reputation. Click again to confirm.";
        private const string kToolTip = "DrillToolTip";
        private const string kFirstCoreSampleTitle = "Your First Core Sample!";
        private const string kFirstCoreSampleMsg = "Congratulations, you've taken your very first core sample! Core samples provide a detailed look at the biome's resources and their results will affect the efficiency of your drills. A good core sample result will improve the extraction efficiency of your drill; the converse is also true. You'll only get a few attempts to run the core samples...watch your drill extraction rates and should you decide to invalidate your test results in order to gain more core sample attempts, choose wisely...";

        [KSPField]
        public int resourceType;

        [KSPField]
        public string analysisGUIName;

        [KSPField]
        public string analysisActionName;

        [KSPField]
        public string analysisSkill;

        [KSPField]
        public float analysisTime;

        [KSPField]
        public string attemptStatus;

        [KSPField(guiActive = true, guiName = "Core Samples Left")]
        public string coreSampleStatus;

        public CoreSampleStates coreSampleState;
        private ModuleAnimationGroup drillAnimation;
        private Dictionary<string, ResourceData> resourceDataMap = new Dictionary<string, ResourceData>();
        private ModuleResourceHarvester harvester;
        private float analysisTimeRemaining;
        private ScreenMessage analysisStatusMsg;
        private bool invalidateConfirm;
        private WBIGoldStrike goldStrike;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight == false)
            {
                setupExperimentGUI();
                return;
            }

            //Get drill animation
            drillAnimation = this.part.FindModuleImplementing<ModuleAnimationGroup>();

            //Harvester
            harvester = this.part.FindModuleImplementing<ModuleResourceHarvester>();

            //Core sample state
            coreSampleState = CoreSampleStates.Ready;

            //If the biome has been unlocked yet then get the samples left
            if (situationIsValid() && Utils.IsBiomeUnlocked(this.part.vessel))
                coreSampleStatus = getSamplesLeft().ToString();
            else
                coreSampleStatus = kUnknown;

            //GoldStrike
            goldStrike = this.part.FindModuleImplementing<WBIGoldStrike>();

            //Setup the gui
            setupGUI();
        }

        [KSPAction("Take Core Sample")]
        public void TakeSampleAction(KSPActionParam param)
        {
            TakeSample();
        }

        [KSPEvent(guiActive = true, guiName = "Run Analysis", active = true, externalToEVAOnly = false, unfocusedRange = 3.0f, guiActiveUnfocused = true)]
        public void TakeSample()
        {
            //If we aren't in the right situation for the resource type then we're done.
            if (situationIsValid() == false)
                return;

            //If the biome hasn't been unlocked yet then we're done.
            if (Utils.IsBiomeUnlocked(this.part.vessel) == false)
            {
                ScreenMessages.PostScreenMessage(kUnlockBiome, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //If we're out of attempts then we're done.
            if (getSamplesLeft() == 0)
            {
                ScreenMessages.PostScreenMessage(kNoMoreAttempts, kWarningMsgDuration, ScreenMessageStyle.UPPER_CENTER);
                Events["InvalidateResults"].guiActive = true;
                Events["InvalidateResults"].guiActiveUnfocused = true;
                return;
            }

            //If the drill isn't deployed then deploy it before performing the analysis.
            if (drillAnimation != null)
            {
                if (drillAnimation.isDeployed == false)
                {
                    drillAnimation.DeployModule();
                    coreSampleState = CoreSampleStates.Deploying;
                    Events["TakeSample"].guiActive = false;
                    Events["TakeSample"].guiActiveUnfocused = false;
                    return;
                }
            }

            //If we're aren't drilling then take a sample
            if (harvester != null)
            {
                if (harvester.isActiveAndEnabled)
                {
                    performAnalysis();
                }
                else
                {
                    Events["TakeSample"].guiActive = false;
                    Events["TakeSample"].guiActiveUnfocused = false;
                    coreSampleState = CoreSampleStates.TakingSample;
                    harvester.StartResourceConverter();
                    analysisTimeRemaining = analysisTime;
                }
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = false, guiName = "Invalidate Results", active = true, externalToEVAOnly = false, unfocusedRange = 3.0f, guiActiveUnfocused = true)]
        public void InvalidateResults()
        {
            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER | HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)
            {
                if (invalidateConfirm == false)
                {
                    ScreenMessages.PostScreenMessage(kConfirmInvalidate, kWarningMsgDuration, ScreenMessageStyle.UPPER_CENTER);
                    invalidateConfirm = true;
                    return;
                }

                if (Reputation.CurrentRep > 0)
                {
                    float fibbo1 = WBIPathfinderScenario.Instance.reputationIndex - 1.0f;
                    float fibbo2 = WBIPathfinderScenario.Instance.reputationIndex - 2.0f;

                    if (fibbo2 < 0.001f)
                        fibbo2 = 0.0f;

                    float reputationHit = kBaseReputationLoss + (fibbo1 + fibbo2);

                    //Take the reputation hit
                    Reputation.Instance.AddReputation(-reputationHit, TransactionReasons.Any);

                    //Increase the reputation hit by increasing the index.
                    WBIPathfinderScenario.Instance.reputationIndex += 1;

                    //Reset efficiency data for the biome
                    WBIPathfinderScenario.Instance.ResetEfficiencyData(this.part.vessel.mainBody.flightGlobalsIndex, biome.name, (HarvestTypes)resourceType);
                    coreSampleStatus = getSamplesLeft().ToString();

                    //Hide the button
                    Events["InvalidateResults"].guiActive = false;
                    Events["InvalidateResults"].guiActiveUnfocused = false;
                }
            }
            
            //Just rest the efficiency data.
            else
            {
                //Reset efficiency data for the biome
                WBIPathfinderScenario.Instance.ResetEfficiencyData(this.part.vessel.mainBody.flightGlobalsIndex, biome.name, (HarvestTypes)resourceType);
                coreSampleStatus = getSamplesLeft().ToString();

                //Hide the button
                Events["InvalidateResults"].guiActive = false;
                Events["InvalidateResults"].guiActiveUnfocused = false;
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //If the biome has been unlocked yet then get the samples left
            if (situationIsValid() && Utils.IsBiomeUnlocked(this.part.vessel))
                coreSampleStatus = getSamplesLeft().ToString();
            else
                coreSampleStatus = kUnknown;

            //Update the core sample state
            updateCoreSampleState();
        }

        #region Helpers
        protected int getSamplesLeft()
        {
            if (!situationIsValid())
                return 0;

            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
            HarvestTypes harvestType = (HarvestTypes)resourceType;
            int samplesLeft = WBIPathfinderScenario.Instance.GetCoreSamplesRemaining(this.part.vessel.mainBody.flightGlobalsIndex, biome.name, harvestType);

            return samplesLeft;
        }

        protected void updateCoreSampleState()
        {
            string message;
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            switch (coreSampleState)
            {
                //If we're done deploying then start taking a core sample.
                case CoreSampleStates.Deploying:
                    if (drillAnimation != null)
                    {
                        if (drillAnimation.DeployAnimation.isPlaying == false)
                        {
                            //Get a small sample
                            if (harvester != null)
                                harvester.StartResourceConverter();

                            //Set analysis time
                            analysisTimeRemaining = analysisTime;

                            //Update state
                            coreSampleState = CoreSampleStates.TakingSample;
                        }
                    }
                    break;

                case CoreSampleStates.TakingSample:

                    //Use regular time to prevent timewarp cheating
                    analysisTimeRemaining -= Time.fixedDeltaTime;

                    if (analysisTimeRemaining <= 0.001f)
                    {
                        //Reset state
                        coreSampleState = CoreSampleStates.Retracting;
                        analysisStatusMsg = null;

                        //Stop harvesting
                        if (harvester != null)
                            harvester.StopResourceConverter();

                        //Retract harvester
                        drillAnimation.RetractModule();

                        //Perform analysis
                        performAnalysis();
                    }

                    else
                    {
                        message = kAnalysisStatus;

                        if (analysisStatusMsg == null)
                            analysisStatusMsg = ScreenMessages.PostScreenMessage(message, analysisTime, ScreenMessageStyle.UPPER_LEFT);
                    }
                    break;

                case CoreSampleStates.Retracting:
                    if (drillAnimation != null)
                    {
                        if (drillAnimation.Fields["animationStatus"].guiActive == false)
                        {
                            //Reset GUI
                            coreSampleState = CoreSampleStates.Ready;
                            Events["TakeSample"].guiActive = true;
                            Events["TakeSample"].guiActiveUnfocused = true;
                        }
                    }
                    break;
            }
        }

        protected virtual float getGeologyLabBonus()
        {
            float labBonus = 0f;
            int researcherCount = 1;

            foreach (Vessel vessel in FlightGlobals.VesselsUnloaded)
            {
                if (vessel.mainBody != this.part.vessel.mainBody)
                    continue;
                if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.SPLASHED && vessel.situation != Vessel.Situations.PRELAUNCH)
                    continue;

                ProtoVessel protoVessel = vessel.protoVessel;
                foreach (ProtoPartSnapshot partSnapshot in protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot moduleSnapshot in partSnapshot.modules)
                    {
                        if (moduleSnapshot.moduleName == "WBIGeologyLab" && Utils.IsExperienceEnabled())
                        {
                            //Now go through the crew list for the part and find scientists.
                            foreach (ProtoCrewMember crewSnapshot in partSnapshot.protoModuleCrew)
                            {
                                if (crewSnapshot.HasEffect(analysisSkill))
                                {
                                    //experience bonus giving is based upon diminishing returns.
                                    labBonus += crewSnapshot.experienceLevel / researcherCount;

                                    //Keep track of the number of researchers we've checked.
                                    researcherCount += 1;
                                }
                            }
                        }
                    }
                }
            }

            labBonus *= kLabBonusModifier;
            return labBonus;
        }

        protected virtual void performAnalysis()
        {
            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
            float experienceLevel = 0f;
            float experienceModifier = 0f;
            float analysisRoll = 0f;
            string analysisResultMessage;
            float efficiencyModifier = 0f;
            float currentModifier = 0f;

            //Decrement the attempts remaining count
            int samplesLeft = getSamplesLeft() - 1;
            if (samplesLeft <= 0)
                samplesLeft = 0;
            WBIPathfinderScenario.Instance.SetCoreSamplesRemaining(this.part.vessel.mainBody.flightGlobalsIndex, biome.name, (HarvestTypes)resourceType, samplesLeft);
            coreSampleStatus = samplesLeft.ToString();

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);

            //If an experienced scientist is taking the core sample, then the scientist's experience will
            //affect the analysis.
            if (FlightGlobals.ActiveVessel.isEVA && Utils.IsExperienceEnabled())
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                if (astronaut.HasEffect(analysisSkill))
                    experienceLevel = astronaut.experienceTrait.CrewMemberExperienceLevel();
            }

            //Add in the science lab bonus
            experienceLevel += getGeologyLabBonus();

            //Seed the random number generator
            UnityEngine.Random.InitState(System.Environment.TickCount);

            //Roll 3d6 to approximate a bell curve, then convert it to a value between 1 and 100.
            analysisRoll = UnityEngine.Random.Range(1, 6);
            analysisRoll += UnityEngine.Random.Range(1, 6);
            analysisRoll += UnityEngine.Random.Range(1, 6);
            analysisRoll *= 5.5556f;

            //Now add the experience modifier
            experienceModifier = experienceLevel * kExperiencePercentModifier;
            analysisRoll += experienceModifier;

            //Did we strike gold?
            if (goldStrike != null)
                goldStrike.CheckGoldStrike();

            //Since we're using a bell curve, anything below maxWorsenRoll worsens the biome's extraction rates.
            //Anything above minImprovementRoll improves the biome's extraction rates.
            //A skilled scientist can affect the modifier by as much as 5%.
            if (analysisRoll <= maxWorsenRoll)
            {
                //Calculate the modifier
                efficiencyModifier = -kBaseEfficiencyModifier * (1.0f - (experienceLevel / 100f));

                //Format the result message
                analysisResultMessage = string.Format(kResourceExtractionWorsened, Math.Abs((efficiencyModifier * 100.0f))) + biome.name;

                //Save the modifier
                currentModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(this.part.vessel.mainBody.flightGlobalsIndex, 
                    biome.name, (HarvestTypes)resourceType, EfficiencyData.kExtractionMod);
                WBIPathfinderScenario.Instance.SetEfficiencyData(this.part.vessel.mainBody.flightGlobalsIndex,
                    biome.name, (HarvestTypes)resourceType, EfficiencyData.kExtractionMod, currentModifier + efficiencyModifier);
            }

            //Good result!
            else if (analysisRoll >= minImprovementRoll)
            {
                //Calculate the modifier
                efficiencyModifier = kBaseEfficiencyModifier * (1.0f + (experienceLevel / 100f));

                //Format the result message
                analysisResultMessage = string.Format(kResourceExtractionImproved, Math.Abs((efficiencyModifier * 100.0f))) + biome.name;

                //Save the modifier
                currentModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(this.part.vessel.mainBody.flightGlobalsIndex, 
                    biome.name, (HarvestTypes)resourceType, EfficiencyData.kExtractionMod);
                WBIPathfinderScenario.Instance.SetEfficiencyData(this.part.vessel.mainBody.flightGlobalsIndex,
                    biome.name, (HarvestTypes)resourceType, EfficiencyData.kExtractionMod, currentModifier + efficiencyModifier);
            }

            else
            {
                analysisResultMessage = kResourceExtractionUnchanged + biome.name;
            }

            //Inform the player of the result.
            ScreenMessages.PostScreenMessage(analysisResultMessage, 5.0f, ScreenMessageStyle.UPPER_CENTER);
            DeployExperiment();


            //First timers: show the tooltip.
            if (WBIPathfinderScenario.Instance.HasShownToolTip(kToolTip) == false)
            {
                WBIPathfinderScenario.Instance.SetToolTipShown(kToolTip);

                WBIToolTipWindow introWindow = new WBIToolTipWindow(kFirstCoreSampleTitle, kFirstCoreSampleMsg);
                introWindow.SetVisible(true);
            }
        }

        protected virtual ResourceData getBiomeData(string bodyName, string biomeName, string resourceName, int resourceType)
        {
            ResourceData data, mapData;
            string resourceTypeStr = resourceType.ToString();
            string key;

            //If the data exists in our biome map, then retrieve it.
            key = bodyName + biomeName + resourceName + resourceTypeStr;
            if (resourceDataMap.ContainsKey(key))
            {
                data = resourceDataMap[key];
                return data;
            }

            //No biome data. Look in the planetary map.
            key = bodyName + resourceName + resourceTypeStr;
            if (resourceDataMap.ContainsKey(key))
            {
                mapData = resourceDataMap[key];

                data = new ResourceData();
                data.BiomeName = biomeName;
                data.PlanetName = mapData.PlanetName;
                data.ResourceName = mapData.ResourceName;
                data.ResourceType = mapData.ResourceType;
                data.Distribution = mapData.Distribution;
                /*
                data.Distribution = new DistributionData();
                data.Distribution.Dispersal = mapData.Distribution.Dispersal;
                data.Distribution.MaxAbundance = mapData.Distribution.MaxAbundance;
                data.Distribution.MaxAbundance = mapData.Distribution.MaxAltitude;
                data.Distribution.MaxRange = mapData.Distribution.MaxRange;
                data.Distribution.MinAbundance = mapData.Distribution.MinAbundance;
                data.Distribution.MinAltitude = mapData.Distribution.MinAltitude;
                data.Distribution.MinRange = mapData.Distribution.MinRange;
                data.Distribution.PresenceChance = mapData.Distribution.PresenceChance;
                data.Distribution.Variance = mapData.Distribution.Variance;
                 */

                ResourceCache.Instance.BiomeResources.Add(data);
                resourceDataMap.Add(bodyName + biomeName + resourceName + resourceTypeStr, data);
                return data;
            }

            //No planetary data. Look in the global map.
            key = resourceName + resourceTypeStr;
            if (resourceDataMap.ContainsKey(key))
            {
                mapData = resourceDataMap[key];

                data = new ResourceData();
                data.BiomeName = biomeName;
                data.PlanetName = bodyName;
                data.ResourceName = mapData.ResourceName;
                data.ResourceType = mapData.ResourceType;
                data.Distribution = mapData.Distribution;
                /*
                data.Distribution = new DistributionData();
                data.Distribution.Dispersal = mapData.Distribution.Dispersal;
                data.Distribution.MaxAbundance = mapData.Distribution.MaxAbundance;
                data.Distribution.MaxAbundance = mapData.Distribution.MaxAltitude;
                data.Distribution.MaxRange = mapData.Distribution.MaxRange;
                data.Distribution.MinAbundance = mapData.Distribution.MinAbundance;
                data.Distribution.MinAltitude = mapData.Distribution.MinAltitude;
                data.Distribution.MinRange = mapData.Distribution.MinRange;
                data.Distribution.PresenceChance = mapData.Distribution.PresenceChance;
                data.Distribution.Variance = mapData.Distribution.Variance;
                 */

                ResourceCache.Instance.BiomeResources.Add(data);
                resourceDataMap.Add(bodyName + biomeName + resourceName + resourceTypeStr, data);
                return data;
            }

            //Uh oh...
            return null;
        }

        protected virtual void generateMaps()
        {
            //We do this once so that we don't have to keep looking through the lists of resource data each time we 
            //alter a different resource.
            string key;

            //Generate biome map.
            resourceDataMap.Clear();
            foreach (ResourceData biomeData in ResourceCache.Instance.BiomeResources)
            {
                key = biomeData.PlanetName + biomeData.BiomeName + biomeData.ResourceName + biomeData.ResourceType.ToString();
                if (resourceDataMap.ContainsKey(key) == false)
                    resourceDataMap.Add(key, biomeData);
            }

            //Planetary map. We use this if there is no biome data
            resourceDataMap.Clear();
            foreach (ResourceData planetData in ResourceCache.Instance.PlanetaryResources)
            {
                key = planetData.PlanetName + planetData.ResourceName + planetData.ResourceType.ToString();
                if (resourceDataMap.ContainsKey(key) == false)
                    resourceDataMap.Add(key, planetData);
            }

            //Global map. We use this if there is no biome or planet data.
            resourceDataMap.Clear();
            foreach (ResourceData globalData in ResourceCache.Instance.GlobalResources)
            {
                key = globalData.ResourceName + globalData.ResourceType.ToString();
                if (resourceDataMap.ContainsKey(key) == false)
                    resourceDataMap.Add(key, globalData);
            }
        }

        protected virtual void setupGUI()
        {
            //If we've made all our attempts to modify the biome resources then hide the event.
            //Otherwise, set the GUI name.
            if (getSamplesLeft() > 0)
            {
                Events["InvalidateResults"].guiActive = false;
                Events["InvalidateResults"].guiActiveUnfocused = false;
            }
            else
            {
                Events["InvalidateResults"].guiActive = true;
                Events["InvalidateResults"].guiActiveUnfocused = true;
            }

            //Setup the gui names for taking samples.
            if (string.IsNullOrEmpty(analysisGUIName) == false)
                Events["TakeSample"].guiName = analysisGUIName;

            if (string.IsNullOrEmpty(analysisActionName) == false)
                Actions["TakeSampleAction"].guiName = analysisActionName;

            //The tech node for using the drills might not be unlocked yet.
            //Setup the drill GUI accordingly.
            setupDrillGUI();

            setupExperimentGUI();
        }

        protected void setupExperimentGUI()
        {
            Events["DeployExperiment"].guiActive = false;
            Events["DeployExperiment"].guiActiveUnfocused = false;
            Events["DeployExperimentExternal"].guiActive = false;
            Events["DeployExperimentExternal"].guiActiveUnfocused = false;

            foreach (BaseAction action in this.Actions)
            {
                if (action.name == "TakeSampleAction")
                    continue;

                action.actionGroup = KSPActionGroup.None;
                action.defaultActionGroup = KSPActionGroup.None;
            }
        }

        protected void setupDrillGUI()
        {
            if (Utils.HasResearchedNode(PathfinderAppView.drillTechNode) == false)
            {
                ModuleResourceHarvester harvester = this.part.FindModuleImplementing<ModuleResourceHarvester>();
                foreach (BaseEvent baseEvent in harvester.Events)
                {
                    baseEvent.guiActive = false;
                    baseEvent.active = false;
                }
                foreach (BaseField field in harvester.Fields)
                    field.guiActive = false;
                foreach (BaseAction action in harvester.Actions)
                {
                    action.actionGroup = KSPActionGroup.None;
                    action.defaultActionGroup = KSPActionGroup.None;
                }

                ModuleAsteroidDrill astroDrill = this.part.FindModuleImplementing<ModuleAsteroidDrill>();
                foreach (BaseEvent baseEvent in astroDrill.Events)
                {
                    baseEvent.guiActive = false;
                    baseEvent.active = false;
                }
                foreach (BaseField field in astroDrill.Fields)
                    field.guiActive = false;
                foreach (BaseAction action in astroDrill.Actions)
                {
                    action.actionGroup = KSPActionGroup.None;
                    action.defaultActionGroup = KSPActionGroup.None;
                }
            }
        }

        protected bool situationIsValid()
        {
            HarvestTypes harvestType = (HarvestTypes)resourceType;
            bool isValid = true;
            string message = null;

            if (HighLogic.LoadedSceneIsFlight == false)
                return true;

            switch (this.part.vessel.situation)
            {
                case Vessel.Situations.LANDED:
                case Vessel.Situations.PRELAUNCH:
                    if (harvestType != HarvestTypes.Planetary && harvestType != HarvestTypes.Atmospheric)
                    {
                        isValid = false;
                        message = kNeedVesselLanded;
                    }
                    break;

                case Vessel.Situations.SPLASHED:
                    if (harvestType != HarvestTypes.Oceanic)
                    {
                        isValid = false;
                        message = kNeedVesselSplashed;
                    }
                    break;

                default:
                    return false;
            }

            //Inform the player if needed
            if (message != null)
                ScreenMessages.PostScreenMessage(message, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);

            return isValid;
        }
        #endregion
    }
}
