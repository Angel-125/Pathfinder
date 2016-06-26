using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2015, by Michael Billard (Angel-125)
License: CC BY-NC-SA 4.0
License URL: https://creativecommons.org/licenses/by-nc-sa/4.0/
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
    public class PathfinderScenario : ScenarioModule
    {
        private const int kMaxCoreSamples = 8;
        private const string kEfficiencyData = "EfficiencyData";

        public static PathfinderScenario Instance;

        public int reputationIndex;
        public bool drillToolTipShown;

        private Dictionary<string, EfficiencyData> efficiencyDataMap = new Dictionary<string, EfficiencyData>();

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode[] efficiencyNodes = node.GetNodes(kEfficiencyData);
            string value = node.GetValue("reputationIndex");

            if (string.IsNullOrEmpty(value) == false)
                reputationIndex = int.Parse(value);

            value = node.GetValue("drillToolTipShown");
            if (string.IsNullOrEmpty(value) == false)
                drillToolTipShown = bool.Parse(value);

            foreach (ConfigNode efficiencyNode in efficiencyNodes)
            {
                EfficiencyData efficiencyData = new EfficiencyData();
                efficiencyData.Load(efficiencyNode);
                efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
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

            if (node.HasValue("drillToolTipShown"))
                node.SetValue("drillToolTipShown", drillToolTipShown.ToString());
            else
                node.AddValue("drillToolTipShown", drillToolTipShown.ToString());

            foreach (EfficiencyData data in efficiencyDataMap.Values)
            {
                efficiencyNode = new ConfigNode(kEfficiencyData);
                data.Save(efficiencyNode);
                node.AddNode(efficiencyNode);
            }
        }

        public void ResetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                efficiencyData.efficiencyModifier = 1.0f;
                efficiencyData.attemptsRemaining = kMaxCoreSamples;
            }

            else
            {
                //Create a new entry.
                efficiencyData = new EfficiencyData();
                efficiencyData.planetID = planetID;
                efficiencyData.biomeName = biomeName;
                efficiencyData.harvestType = harvestType;
                efficiencyData.efficiencyModifier = 1.0f;
                efficiencyData.attemptsRemaining = kMaxCoreSamples;

                efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
            }
        }

        public void SetEfficiencyData(int planetID, string biomeName, HarvestTypes harvestType, float efficiencyModifier)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            //If we already have the efficiency data then just update the value
            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                efficiencyData.efficiencyModifier = efficiencyModifier;
                return;
            }

            //Create a new entry.
            efficiencyData = new EfficiencyData();
            efficiencyData.planetID = planetID;
            efficiencyData.biomeName = biomeName;
            efficiencyData.harvestType = harvestType;
            efficiencyData.efficiencyModifier = efficiencyModifier;
            efficiencyData.attemptsRemaining = kMaxCoreSamples;

            efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
        }

        public float GetEfficiencyModifier(int planetID, string biomeName, HarvestTypes harvestType)
        {
            string key = planetID.ToString() + biomeName + harvestType.ToString();
            EfficiencyData efficiencyData = null;

            if (efficiencyDataMap.ContainsKey(key))
            {
                efficiencyData = efficiencyDataMap[key];
                return efficiencyData.efficiencyModifier;
            }

            else
            {
                //Create a new entry.
                efficiencyData = new EfficiencyData();
                efficiencyData.planetID = planetID;
                efficiencyData.biomeName = biomeName;
                efficiencyData.harvestType = harvestType;
                efficiencyData.efficiencyModifier = 1.0f;
                efficiencyData.attemptsRemaining = kMaxCoreSamples;

                efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
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
                efficiencyData = new EfficiencyData();
                efficiencyData.planetID = planetID;
                efficiencyData.biomeName = biomeName;
                efficiencyData.harvestType = harvestType;
                efficiencyData.efficiencyModifier = 1.0f;
                efficiencyData.attemptsRemaining = kMaxCoreSamples;

                efficiencyDataMap.Add(efficiencyData.Key, efficiencyData);
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

    }
}
