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
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class WBIPathfinderScenario : ScenarioModule
    {
        public const int kMaxCoreSamples = 8;
        private const string kEfficiencyData = "EfficiencyData";
        private const string kToolTip = "ToolTip";

        public static WBIPathfinderScenario Instance;

        public int reputationIndex;

        private Dictionary<string, EfficiencyData> efficiencyDataMap = new Dictionary<string, EfficiencyData>();
        private static Dictionary<string, ConfigNode> toolTips = new Dictionary<string, ConfigNode>();

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode[] efficiencyNodes = node.GetNodes(kEfficiencyData);
            ConfigNode[] toolTipsShown = node.GetNodes(kToolTip);
            string value = node.GetValue("reputationIndex");

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
                if (toolTipNode.HasValue("name") == false)
                    continue;
                value = toolTipNode.GetValue("name");

                if (toolTips.ContainsKey(value))
                    toolTips[value] = toolTipNode;
                else
                    toolTips.Add(value, toolTipNode);
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            ConfigNode efficiencyNode;

            if (node.HasValue("reputationIndex"))
                node.SetValue("reputationIndex", reputationIndex.ToString());
            else
                node.AddValue("reputationIndex", reputationIndex.ToString());

            foreach (EfficiencyData data in efficiencyDataMap.Values)
            {
                efficiencyNode = new ConfigNode(kEfficiencyData);
                data.Save(efficiencyNode);
                node.AddNode(efficiencyNode);
            }

            node.RemoveNodes(kToolTip);
            foreach (ConfigNode toolTipNode in toolTips.Values)
                node.AddNode(toolTipNode);
        }

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

        public void ResetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                foreach (string modifierKey in efficiencyData.modifiers.Keys)
                    efficiencyData.modifiers[modifierKey] = 1.0f;
                efficiencyData.attemptsRemaining = kMaxCoreSamples;
            }

            else
            {
                //Create a new entry.
                createNewEfficencyEntry(planetID, biomeName, harvestType);
            }
        }

        public void SetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType, string modifierName, float modifierValue)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            //If we already have the efficiency data then just update the value
            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                if (efficiencyData.modifiers.ContainsKey(modifierName))
                    efficiencyData.modifiers[modifierName] = modifierValue;
                else
                    efficiencyData.modifiers.Add(modifierName, modifierValue);
                return;
            }

            //Create a new entry.
            createNewEfficencyEntry(planetID, biomeName, harvestType);
        }

        public float GetEfficiencyModifier(int planetID, string biomeName, HarvestTypes harvestType, string modifierName)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                if (efficiencyData.modifiers.ContainsKey(modifierName))
                    return efficiencyData.modifiers[modifierName];
                else
                    return 1.0f;
            }

            else
            {
                //Create a new entry.
                createNewEfficencyEntry(planetID, biomeName, harvestType);
            }

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
    }
}
