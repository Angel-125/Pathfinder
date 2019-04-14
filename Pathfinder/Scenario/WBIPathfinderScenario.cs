using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;

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
    public delegate void EfficiencyUpdateDelegate(int planetID, string biomeName, HarvestTypes harvestID, string efficiencyType, float modifier);

    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class WBIPathfinderScenario : ScenarioModule
    {
        public const int kMaxCoreSamples = 8;

        private const string kEfficiencyData = "EfficiencyData";
        private const string kToolTip = "ToolTip";
        private const string kReputationIndex = "reputationIndex";
        private const string kLastPromotion = "lastPromotion";
        private const string kName = "name";

        public static WBIPathfinderScenario Instance;

        //Debug stuff
        public static bool showDebugLog = false;

        //Events
        public event EfficiencyUpdateDelegate onEfficiencyUpdate;

        //Core sample reputation
        public int reputationIndex;

        private Dictionary<string, EfficiencyData> efficiencyDataMap = new Dictionary<string, EfficiencyData>();
        private static Dictionary<string, ConfigNode> toolTips = new Dictionary<string, ConfigNode>();

        #region Housekeeping
        protected void debugLog(string message)
        {
            if (showDebugLog)
                Debug.Log("[WBIPathfinderScenario] - " + message);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
            GameEvents.OnGameSettingsApplied.Add(onGameSettingsApplied);
            showDebugLog = PathfinderSettings.LoggingEnabled;
        }

        public void Destroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(onGameSettingsApplied);
        }

        public void onGameSettingsApplied()
        {
            showDebugLog = PathfinderSettings.LoggingEnabled;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode[] efficiencyNodes = node.GetNodes(kEfficiencyData);
            ConfigNode[] toolTipsShown = node.GetNodes(kToolTip);
            string value = node.GetValue(kReputationIndex);

            if (string.IsNullOrEmpty(value) == false)
                reputationIndex = int.Parse(value);

            foreach (ConfigNode efficiencyNode in efficiencyNodes)
            {
                EfficiencyData efficiencyData = new EfficiencyData();
                efficiencyData.Load(efficiencyNode);
                efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
            }

            foreach (ConfigNode toolTipNode in toolTipsShown)
            {
                if (toolTipNode.HasValue(kName) == false)
                    continue;
                value = toolTipNode.GetValue(kName);

                if (toolTips.ContainsKey(value))
                    toolTips[value] = toolTipNode;
                else
                    toolTips.Add(value, toolTipNode);
            }

            //Backwards compatibility for GoldStrike
            Dictionary<string, ConfigNode[]> goldStrikeNodes = new Dictionary<string, ConfigNode[]>();
            if (node.HasNode(WBIGoldStrikeScenario.kGoldStrikeLodeNode))
                goldStrikeNodes.Add(WBIGoldStrikeScenario.kGoldStrikeLodeNode, node.GetNodes(WBIGoldStrikeScenario.kGoldStrikeLodeNode));
            if (node.HasNode(WBIGoldStrikeScenario.kAsteroidsSearched))
                goldStrikeNodes.Add(WBIGoldStrikeScenario.kAsteroidsSearched, node.GetNodes(WBIGoldStrikeScenario.kAsteroidsSearched));
            if (node.HasNode(WBIGoldStrikeScenario.kAsteroidNodeName))
                goldStrikeNodes.Add(WBIGoldStrikeScenario.kAsteroidNodeName, node.GetNodes(WBIGoldStrikeScenario.kAsteroidNodeName));
            if (goldStrikeNodes.Keys.Count > 0)
                WBIGoldStrikeScenario.goldStrikeNodes = goldStrikeNodes;
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            ConfigNode efficiencyNode;

            if (node.HasValue(kReputationIndex))
                node.SetValue(kReputationIndex, reputationIndex.ToString());
            else
                node.AddValue(kReputationIndex, reputationIndex.ToString());

            foreach (EfficiencyData data in efficiencyDataMap.Values)
            {
                efficiencyNode = new ConfigNode(kEfficiencyData);
                data.Save(efficiencyNode);
                node.AddNode(efficiencyNode);
            }

            foreach (ConfigNode toolTipNode in toolTips.Values)
                node.AddNode(toolTipNode);
        }
        #endregion

        #region Data science
        public float DistributeData(float amount, Vessel vessel, bool addToLabs, bool addToPipeline, bool addToGoldStrike)
        {
            debugLog("DistributeData: " + amount + " to distribute.");
            float amountPerModule;
            float amountDistributed = 0f;
            ModuleScienceLab lab;
            WBIPipeEndpoint pipeEndpoint;
            WBIGoldStrikeBonus goldStrikeBonus;
            int totalCount;

            //List of labs, bonus modules, and pipeline endpoints that are actively processing data.
            List<WBIGoldStrikeBonus> activeBonuses = new List<WBIGoldStrikeBonus>();
            List<WBIPipeEndpoint> activeEndpoints = new List<WBIPipeEndpoint>();
            List<ModuleScienceLab> activeLabs = new List<ModuleScienceLab>();

            //List of labs, bonus modules, and pipeline endpoint to process
            List<ModuleScienceLab> scienceLabs = vessel.FindPartModulesImplementing<ModuleScienceLab>();
            List<WBIGoldStrikeBonus> goldStrikeBonuses = vessel.FindPartModulesImplementing<WBIGoldStrikeBonus>();
            List<WBIPipeEndpoint> pipeEndpoints = vessel.FindPartModulesImplementing<WBIPipeEndpoint>();


            //If we're allowed to distribute data to labs, then get the active labs.
            if (scienceLabs != null && addToLabs)
            {
                totalCount = scienceLabs.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    lab = scienceLabs[index];
                    if (lab.processingData)
                        activeLabs.Add(lab);
                }
                debugLog("Active Labs: " + activeLabs.Count);
            }

            //Get the active pipe endpoints
            if (pipeEndpoints != null && addToPipeline)
            {
                totalCount = pipeEndpoints.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    pipeEndpoint = pipeEndpoints[index];
                    if (pipeEndpoint.accumulateData)
                        activeEndpoints.Add(pipeEndpoint);
                }
                debugLog("Active Endpoints: " + activeEndpoints.Count);
            }

            //Get the active gold strike bonuses
            if (goldStrikeBonuses != null && addToGoldStrike)
            {
                totalCount = goldStrikeBonuses.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    goldStrikeBonus = goldStrikeBonuses[index];
                    if (goldStrikeBonus.accumulateData)
                        activeBonuses.Add(goldStrikeBonus);
                }
                debugLog("Active Bonuses: " + activeBonuses.Count);
            }

            //Divide up the data between the active modules.
            if (activeLabs.Count == 0 && activeBonuses.Count == 0 && activeEndpoints.Count == 0)
                return 0f;
            amountPerModule = amount / (activeBonuses.Count + activeEndpoints.Count + activeLabs.Count);
            debugLog("amountPerModule: " + amountPerModule);

            //Labs
            if (addToLabs)
            {
                totalCount = activeLabs.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    lab = activeLabs[index];

                    //Add data to the lab
                    if (lab.dataStored + amountPerModule <= lab.dataStorage)
                    {
                        lab.dataStored += amountPerModule;
                        amountDistributed += amountDistributed;
                    }

                    else
                    {
                        amountDistributed += lab.dataStorage - lab.dataStored;
                        lab.dataStored = lab.dataStorage;
                    }
                }
            }

            //Endpoints
            if (addToPipeline)
            {
                totalCount = activeEndpoints.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    pipeEndpoint = pipeEndpoints[index];
                    pipeEndpoint.AddData(amountPerModule);
                    amountDistributed += amountDistributed;
                }
            }

            //GoldStrike
            if (addToGoldStrike)
            {
                totalCount = activeBonuses.Count;
                for (int index = 0; index < totalCount; index++)
                {
                    goldStrikeBonus = goldStrikeBonuses[index];
                    goldStrikeBonus.AddData(amountPerModule);
                    amountDistributed += amountDistributed;
                }
            }

            return amountDistributed;
        }
        #endregion

        #region Tool Tips
        public void SetToolTipShown(string toolTipName)
        {
            //If we've already set the tool tip then we're done.
            if (toolTips.ContainsKey(toolTipName))
                return;

            //Node does not exist, then add it.
            ConfigNode nodeTip = new ConfigNode(kToolTip);
            nodeTip.AddValue("name", toolTipName);
            //toolTipsList.Add(nodeTip);
            toolTips.Add(toolTipName, nodeTip);
        }

        public bool HasShownToolTip(string toolTipName)
        {
            if (toolTips.ContainsKey(toolTipName))
                return true;

            return false;
        }
        #endregion

        #region Efficiencies
        public void ResetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            //Create a new entry if needed.
            if (!efficiencyDataMap.ContainsKey(key))
                createNewEfficencyEntry(planetID, biomeName, harvestType);

            efficiencyData = efficiencyDataMap[key];
            foreach (string modifierKey in efficiencyData.modifiers.Keys)
                efficiencyData.modifiers[modifierKey] = 1.0f;
            efficiencyData.attemptsRemaining = kMaxCoreSamples;
        }

        public void SetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType, string modifierName, float modifierValue)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            //Create a new entry if needed.
            if (!efficiencyDataMap.ContainsKey(key))
                createNewEfficencyEntry(planetID, biomeName, harvestType);

            //If we already have the efficiency data then just update the value
            efficiencyData = efficiencyDataMap[key];
            if (efficiencyData.modifiers.ContainsKey(modifierName))
                efficiencyData.modifiers[modifierName] = modifierValue;
            else
                efficiencyData.modifiers.Add(modifierName, modifierValue);

            //Update interested parties
            if (onEfficiencyUpdate != null)
                onEfficiencyUpdate(planetID, biomeName, harvestType, modifierName, modifierValue);
        }

        public float GetEfficiencyModifier(int planetID, string biomeName, HarvestTypes harvestType, string modifierName)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            //Create a new entry if needed.
            if (!efficiencyDataMap.ContainsKey(key))
                createNewEfficencyEntry(planetID, biomeName, harvestType);

            efficiencyData = efficiencyDataMap[key];
            if (efficiencyData.modifiers.ContainsKey(modifierName))
                return efficiencyData.modifiers[modifierName];
            else
                return 1.0f;
        }

        public void SetCoreSamplesRemaining(int planetID, string biomeName, HarvestTypes harvestType, int attemptsRemaining)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                efficiencyData.attemptsRemaining = attemptsRemaining;
            }
        }

        public int GetCoreSamplesRemaining(int planetID, string biomeName, HarvestTypes harvestType)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                return efficiencyData.attemptsRemaining;
            }

            else
            {
                //Create a new entry.
                createNewEfficencyEntry(planetID, biomeName, harvestType);
            }

            return kMaxCoreSamples;
        }

        public Dictionary<string, EfficiencyData> GetEfficiencyDataForBiome(int planetID, string biomeName)
        {
            //We use a dictionary here to help speed up applying the modifier to all harvesters on the vessel.
            Dictionary<string, EfficiencyData> efficiencyMap = new Dictionary<string, EfficiencyData>();
            string key;
            int harvestID;

            foreach (EfficiencyData efficiencyData in efficiencyDataMap.Values)
            {
                if (efficiencyData.planetID == planetID && efficiencyData.biomeName == biomeName)
                {
                    harvestID = (int)efficiencyData.harvestType;
                    key = biomeName + harvestID.ToString();
                    efficiencyMap.Add(key, efficiencyData);
                }
            }

            return efficiencyMap;
        }

        protected void createNewEfficencyEntry(int planetID, string biomeName, HarvestTypes harvestType)
        {
            EfficiencyData efficiencyData = new EfficiencyData();
            efficiencyData.planetID = planetID;
            efficiencyData.biomeName = biomeName;
            efficiencyData.harvestType = harvestType;
            efficiencyData.attemptsRemaining = kMaxCoreSamples;

            //Standard modifiers
            efficiencyData.modifiers.Add(EfficiencyData.kExtractionMod, 1.0f);
            efficiencyData.modifiers.Add(EfficiencyData.kIndustryMod, 1.0f);
            efficiencyData.modifiers.Add(EfficiencyData.kHabitationMod, 1.0f);
            efficiencyData.modifiers.Add(EfficiencyData.kScienceMod, 1.0f);

            efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
        }
        #endregion
    }
}
