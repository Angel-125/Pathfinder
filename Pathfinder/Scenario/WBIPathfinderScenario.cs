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
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class WBILodeIconHelper : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);
            GameEvents.onCustomWaypointLoad.Add(OnCustomWaypointLoad);

            SetupLodeIcons();
        }

        public void OnCustomWaypointLoad(GameEvents.FromToAction<Waypoint, ConfigNode> fta)
        {
            Waypoint waypoint = fta.from;
            ConfigNode node = fta.to;
            string location = string.Format("Lon: {0:f2} Lat: {1:f2}", waypoint.longitude, waypoint.latitude);

            if (WBIPathfinderScenario.Instance.IsLodeWaypoint(waypoint.navigationId.ToString()))
            {
                waypoint.id = WBIPathfinderScenario.kLodeIcon;
                waypoint.nodeCaption1 = location;
            }
        }

        public void SetupLodeIcons()
        {
            try
            {
                Dictionary<string, Dictionary<string, GoldStrikeLode>> goldStrikeLodes = WBIPathfinderScenario.Instance.goldStrikeLodes;
                Dictionary<string, GoldStrikeLode>[] lodeMaps = null;
                Dictionary<string, GoldStrikeLode> lodeMap = null;
                GoldStrikeLode[] lodes = null;
                GoldStrikeLode lode = null;
                Waypoint waypoint = null;
                string location = string.Empty;

                lodeMaps = goldStrikeLodes.Values.ToArray();
                for (int index = 0; index < lodeMaps.Length; index++)
                {
                    lodeMap = lodeMaps[index];
                    lodes = lodeMap.Values.ToArray();
                    for (int lodeIndex = 0; lodeIndex < lodes.Length; lodeIndex++)
                    {
                        lode = lodes[lodeIndex];
                        if (string.IsNullOrEmpty(lode.navigationID))
                            continue;

                        waypoint = WaypointManager.FindWaypoint(new Guid(lode.navigationID));
                        location = string.Format("Lon: {0:f2} Lat: {1:f2}", waypoint.longitude, waypoint.latitude);
                        if (waypoint != null)
                        {
                            WaypointManager.RemoveWaypoint(waypoint);
                            waypoint.id = WBIPathfinderScenario.kLodeIcon;
                            waypoint.nodeCaption1 = location;
                            WaypointManager.AddWaypoint(waypoint);
                        }
                    }
                }
            }
            catch { }
        }
    }

    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class WBIPathfinderScenario : ScenarioModule
    {
        public const string kLodeIcon = "WildBlueIndustries/Pathfinder/Icons/LodeIcon";
        public const int kMaxCoreSamples = 8;
        public const double kMaxProspectSearchDistance = 0.1f; //km

        private const string kEfficiencyData = "EfficiencyData";
        private const string kToolTip = "ToolTip";
        private const string kReputationIndex = "reputationIndex";
        private const string kLastPromotion = "lastPromotion";
        private const string kName = "name";
        private const string kGoldStrikeDataNode = "GOLDSTRIKE";
        private const string kGoldStrikeLodeNode = "GoldStrikeLode";
        private const string kGoldStrikeChance = "GoldStrikeChance";

        public static WBIPathfinderScenario Instance;

        //Debug stuff
        public static bool showDebugLog = true;

        //Core sample reputation
        public int reputationIndex;

        //Gold strike
        public Dictionary<string, GoldStrikeData> goldStrikeResources = new Dictionary<string, GoldStrikeData>();
        public Dictionary<string, Dictionary<string, GoldStrikeLode>> goldStrikeLodes = new Dictionary<string, Dictionary<string, GoldStrikeLode>>();
        public Dictionary<string, GoldStrikeChance> goldStrikeChances = new Dictionary<string, GoldStrikeChance>();

        private Dictionary<string, EfficiencyData> efficiencyDataMap = new Dictionary<string, EfficiencyData>();
        private static Dictionary<string, ConfigNode> toolTips = new Dictionary<string, ConfigNode>();

        protected void debugLog(string message)
        {
            if (showDebugLog == true)
                Debug.Log("[WBIPathfinderScenario] - " + message);
        }

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
            GameEvents.OnGameSettingsApplied.Add(onGameSettingsApplied);
        }

        public void Destroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(onGameSettingsApplied);
        }

        public void onGameSettingsApplied()
        {
//            showDebugLog = GoldStrikeSettings.LoggingEnabled;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            ConfigNode[] efficiencyNodes = node.GetNodes(kEfficiencyData);
            ConfigNode[] toolTipsShown = node.GetNodes(kToolTip);
            ConfigNode[] goldStrikeNodes = GameDatabase.Instance.GetConfigNodes(kGoldStrikeDataNode);
            ConfigNode[] goldStrikeLodeNodes = node.GetNodes(kGoldStrikeLodeNode);
            ConfigNode[] strikeChances = node.GetNodes(kGoldStrikeChance);
            string value = node.GetValue(kReputationIndex);
            GoldStrikeChance chance = null;

            if (string.IsNullOrEmpty(value) == false)
                reputationIndex = int.Parse(value);

            debugLog("OnLoad: there are " + goldStrikeNodes.Length + " GOLDSTRIKE items to load.");
            foreach (ConfigNode goldStrikeDataNode in goldStrikeNodes)
            {
                GoldStrikeData strikeData = new GoldStrikeData();
                strikeData.Load(goldStrikeDataNode);
                if (string.IsNullOrEmpty(strikeData.resourceName) == false)
                    goldStrikeResources.Add(strikeData.resourceName, strikeData);
            }

            debugLog("OnLoad: there are " + goldStrikeLodeNodes.Length + " GoldStrikeLode items to load.");
            foreach (ConfigNode goldStrikeLodeNode in goldStrikeLodeNodes)
            {
                GoldStrikeLode lode = new GoldStrikeLode();
                Dictionary<string, GoldStrikeLode> lodeMap = null;
                string planetBiomeKey, lodeKey;

                lode.Load(goldStrikeLodeNode);
                planetBiomeKey = lode.planetID.ToString() + lode.biome;

                if (goldStrikeLodes.ContainsKey(planetBiomeKey) == false)
                {
                    lodeMap = new Dictionary<string, GoldStrikeLode>();
                    goldStrikeLodes.Add(planetBiomeKey, lodeMap);
                }
                lodeMap = goldStrikeLodes[planetBiomeKey];

                //Add the new lode
                lodeKey = lode.longitude.ToString() + lode.lattitude.ToString() + lode.resourceName;
                lodeMap.Add(lodeKey, lode);
            }

            debugLog("OnLoad: there are " + strikeChances.Length + " GoldStrikeChance items to load.");
            foreach (ConfigNode chanceNode in strikeChances)
            {
                chance = new GoldStrikeChance();
                chance.Load(chanceNode);
                string planetBiomeKey = chance.planetID.ToString() + chance.biome;
                goldStrikeChances.Add(planetBiomeKey, chance);
            }

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

            debugLog("OnSave: there are " + goldStrikeLodes.Values.Count + " GoldStrikeLode items to save.");
            foreach (Dictionary<string, GoldStrikeLode> lodeMap in goldStrikeLodes.Values)
            {
                foreach (GoldStrikeLode lode in lodeMap.Values)
                {
                    ConfigNode lodeNode = lode.Save();
                    node.AddNode(lodeNode);
                }
            }

            debugLog("OnSave: there are " + goldStrikeChances.Values.Count + " GoldStrikeChance items to save.");
            foreach (GoldStrikeChance chance in goldStrikeChances.Values)
            {
                ConfigNode chanceNode = chance.Save();
                node.AddNode(chanceNode);
            }

            node.RemoveNodes(kToolTip);
            foreach (ConfigNode toolTipNode in toolTips.Values)
                node.AddNode(toolTipNode);
        }

        public Vector3d GetLastProspectLocation(int planetID, string biome)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            GoldStrikeChance goldStrikeChance = null;
            Vector3d lastLocation = Vector3d.zero;

            if (goldStrikeChances.ContainsKey(planetBiomeKey) == false)
                return Vector3d.zero;

            goldStrikeChance = goldStrikeChances[planetBiomeKey];

            lastLocation.x = goldStrikeChance.lastProspectLocation.x;
            lastLocation.y = goldStrikeChance.lastProspectLocation.y;
            lastLocation.z = goldStrikeChance.lastProspectLocation.z;

            return lastLocation;
        }

        public void SetLastProspectLocation(int planetID, string biome, double longitude, double latitude, double altitude)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            GoldStrikeChance goldStrikeChance = null;

            if (goldStrikeChances.ContainsKey(planetBiomeKey) == false)
            {
                goldStrikeChance = new GoldStrikeChance();
                goldStrikeChance.planetID = planetID;
                goldStrikeChance.biome = biome;
                goldStrikeChances.Add(planetBiomeKey, goldStrikeChance);
            }

            goldStrikeChance = goldStrikeChances[planetBiomeKey];
            goldStrikeChance.lastProspectLocation.x = longitude;
            goldStrikeChance.lastProspectLocation.y = latitude;
            goldStrikeChance.lastProspectLocation.z = altitude;

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);
        }

        public int DecrementProspectAttempts(int planetID, string biome)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            GoldStrikeChance goldStrikeChance = null;

            if (goldStrikeChances.ContainsKey(planetBiomeKey) == false)
            {
                goldStrikeChance = new GoldStrikeChance();
                goldStrikeChance.planetID = planetID;
                goldStrikeChance.biome = biome;
                goldStrikeChances.Add(planetBiomeKey, goldStrikeChance);
            }

            goldStrikeChance = goldStrikeChances[planetBiomeKey];
            goldStrikeChance.chancesRemaining -= 1;

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);

            return goldStrikeChance.chancesRemaining;
        }

        public bool IsLodeWaypoint(string navigationID)
        {
            Dictionary<string, GoldStrikeLode>[] lodeMaps = null;
            Dictionary<string, GoldStrikeLode> lodeMap = null;
            GoldStrikeLode[] lodes = null;
            GoldStrikeLode lode = null;

            lodeMaps = goldStrikeLodes.Values.ToArray();
            for (int index = 0; index < lodeMaps.Length; index++)
            {
                lodeMap = lodeMaps[index];
                lodes = lodeMap.Values.ToArray();
                for (int lodeIndex = 0; lodeIndex < lodes.Length; lodeIndex++)
                {
                    lode = lodes[lodeIndex];
                    if (string.IsNullOrEmpty(lode.navigationID))
                        continue;
                    if (lode.navigationID == navigationID)
                        return true;
                }
            }

            return false;
        }

        public GoldStrikeChance GetGoldStrikeChance(int planetID, string biome)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            GoldStrikeChance goldStrikeChance = null;

            if (goldStrikeChances.ContainsKey(planetBiomeKey) == false)
            {
                goldStrikeChance = new GoldStrikeChance();
                goldStrikeChance.planetID = planetID;
                goldStrikeChance.biome = biome;
                goldStrikeChances.Add(planetBiomeKey, goldStrikeChance);
            }

            goldStrikeChance = goldStrikeChances[planetBiomeKey];
            debugLog("GetGoldStrikeChance retrieved " + goldStrikeChance.ToString());
            return goldStrikeChance;
        }

        public int GetProspectAttemptsRemaining(int planetID, string biome)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            GoldStrikeChance goldStrikeChance = null;

            if (goldStrikeChances.ContainsKey(planetBiomeKey) == false)
            {
                goldStrikeChance = new GoldStrikeChance();
                goldStrikeChances.Add(planetBiomeKey, goldStrikeChance);
            }

            goldStrikeChance = goldStrikeChances[planetBiomeKey];
            return goldStrikeChance.chancesRemaining;
        }

        public ProspectSituations GetProspectSituation(int planetID, string biome, double longitude, double lattitude, double altitude, out GoldStrikeLode lode)
        {
            GoldStrikeChance chance = GetGoldStrikeChance(planetID, biome);
            double travelDistance = chance.GetDistanceFromLastLocation(longitude, lattitude, altitude);

            //Set the lode object
            lode = FindNearestLode(planetID, biome, longitude, lattitude);

            //Are we out of chances?
            if (chance.chancesRemaining <= 0)
                return ProspectSituations.OutOfChances;

            //Is there a lode in the area?
            if (lode != null)
                return ProspectSituations.LodeAlreadyExists;
            
            //Have we traveled far enough?
            if (travelDistance < GoldStrikeSettings.DistanceBetweenProspects)
            {
                debugLog("Calculated distance between current location and last prospect location: " + travelDistance);
                return ProspectSituations.NotEnoughDistance;
            }

            return ProspectSituations.Valid;
        }

        public GoldStrikeLode FindNearestLode(int planetID, string biome, double longitude, double latitude, double searchDistance = kMaxProspectSearchDistance)
        {
            string planetBiomeKey = planetID.ToString() + biome;
            Dictionary<string, GoldStrikeLode> lodeMap = null;

            //Get the lode map. If there is none then we're done.
            if (goldStrikeLodes.ContainsKey(planetBiomeKey) == false)
            {
                debugLog("no lodeMap found for key: " + planetBiomeKey);
                return null;
            }
            lodeMap = goldStrikeLodes[planetBiomeKey];

            //Now iterate through the dictionary to find the nearest lode.
            //We have a minimum cutoff distance
            GoldStrikeLode[] lodes = lodeMap.Values.ToArray<GoldStrikeLode>();
            GoldStrikeLode lode, closestProspect = null;
            double distance, prevDistance;
            debugLog("lodes length: " + lodes.Length);
            prevDistance = double.MaxValue;
            for (int index = 0; index < lodes.Length; index++)
            {
                lode = lodes[index];
                debugLog("checking lode: " + lode.ToString());

                distance = GoldStrikeUtils.HaversineDistance(longitude, latitude, lode.longitude, lode.lattitude, FlightGlobals.Bodies[lode.planetID]);
                debugLog("distance between current location and lode location: " + distance);
                if ((distance <= searchDistance) && (distance < prevDistance))
                {
                    debugLog("new closest lode: " + lode);
                    closestProspect = lode;
                    prevDistance = distance;
                }
            }

            if (closestProspect != null)
            {
                debugLog("closest lode found");
                return closestProspect;
            }
            else
            {
                debugLog("closest lode not found");
                return null;
            }
        }

        public GoldStrikeLode AddLode(int planetID, string biome, double longitude, double lattitude, string resourceName, double amountRemaining)
        {
            GoldStrikeLode lode = new GoldStrikeLode();
            Dictionary<string, GoldStrikeLode> lodeMap = null;
            string planetBiomeKey = planetID.ToString() + biome;
            string lodeKey = longitude.ToString() + lattitude.ToString() + resourceName;

            //Setup the new lode
            lode.resourceName = resourceName;
            lode.longitude = longitude;
            lode.lattitude = lattitude;
            lode.biome = biome;
            lode.amountRemaining = amountRemaining;
            lode.planetID = planetID;

            //Get the lode map
            if (goldStrikeLodes.ContainsKey(planetBiomeKey) == false)
            {
                lodeMap = new Dictionary<string, GoldStrikeLode>();
                goldStrikeLodes.Add(planetBiomeKey, lodeMap);
            }
            lodeMap = goldStrikeLodes[planetBiomeKey];

            //Add the new lode
            lodeMap.Add(lodeKey, lode);
            debugLog("Added new lode: " + lode.ToString());

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);

            return lode;
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
