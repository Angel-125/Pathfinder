using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;

/*
Source code copyrighgt 2019, by Michael Billard (Angel-125)
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
    public class WBIGoldStrikeScenario : ScenarioModule
    {
        #region Constants
        public const double kMaxProspectSearchDistance = 0.1f; //km
        public const string kLodeIcon = "WildBlueIndustries/000WildBlueTools/Icons/LodeIcon";

        public const string kGoldStrikeDataNode = "GOLDSTRIKE";
        public const string kGoldStrikeLodeNode = "GoldStrikeLode";
        public const string kAsteroidsSearched = "AsteroidsSearched";
        public const string kAsteroidNodeName = "ProspectedAsteroid";
        private const string kName = "name";
        #endregion

        #region Housekeeping
        public static WBIGoldStrikeScenario Instance;

        //Gold strike
        public Dictionary<string, GoldStrikeData> goldStrikeResources = new Dictionary<string, GoldStrikeData>();
        public Dictionary<string, Dictionary<string, GoldStrikeLode>> goldStrikeLodes = new Dictionary<string, Dictionary<string, GoldStrikeLode>>();
        public List<String> prospectedAsteroids = new List<string>();
        public static bool showDebugLog = false;
        public static bool debugGoldStrike = false;
        public static bool debugProspectAlwaysSuccessful = false;

        protected void debugLog(string message)
        {
            if (showDebugLog)
                Debug.Log("[WBIGoldStrikeScenario] - " + message);
        }
        #endregion

        #region Overrides
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
            ConfigNode[] goldStrikeNodes = GameDatabase.Instance.GetConfigNodes(kGoldStrikeDataNode);
            ConfigNode[] goldStrikeLodeNodes = node.GetNodes(kGoldStrikeLodeNode);
            ConfigNode[] asteroidsSearched = node.GetNodes(kAsteroidsSearched);

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

            debugLog("OnLoad: there are " + asteroidsSearched.Length + " asteroids that have been prospected.");
            foreach (ConfigNode asteroidNode in asteroidsSearched)
                prospectedAsteroids.Add(asteroidNode.GetValue(kName));
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            debugLog("OnSave: there are " + goldStrikeLodes.Values.Count + " GoldStrikeLode items to save.");
            foreach (Dictionary<string, GoldStrikeLode> lodeMap in goldStrikeLodes.Values)
            {
                foreach (GoldStrikeLode lode in lodeMap.Values)
                {
                    ConfigNode lodeNode = lode.Save();
                    node.AddNode(lodeNode);
                }
            }

            debugLog("OnSave: there are " + prospectedAsteroids.Count + " prospected asteroids to save.");
            foreach (string asteroidName in prospectedAsteroids)
            {
                ConfigNode asteroidNode = new ConfigNode(kAsteroidNodeName);
                asteroidNode.AddValue(kName, asteroidName);
                node.AddNode(asteroidNode);
            }
        }
        #endregion

        #region GoldStrike
        public void ClearProspects()
        {
            goldStrikeLodes.Clear();
            prospectedAsteroids.Clear();
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

        public void UpdateDrillLodes(ModuleAsteroid asteroid)
        {

            //Find all loaded vessels that have drills, and tell them to update their lode data.
            //If we prospected an asteroid then just inform the asteroid drills.
            //Otherwise just inform the ground drills.
            if (asteroid == null)
            {
                debugLog("Updating ground drills");
                List<WBIGoldStrikeDrill> groundDrills = null;
                foreach (Vessel loadedVessel in FlightGlobals.VesselsLoaded)
                {
                    groundDrills = loadedVessel.FindPartModulesImplementing<WBIGoldStrikeDrill>();
                    if (groundDrills != null)
                    {
                        foreach (WBIGoldStrikeDrill groundDrill in groundDrills)
                            groundDrill.UpdateLode();
                    }
                }
            }

            else
            {
                debugLog("Updating asteroid drills");
                List<WBIGoldStrikeAsteroidDrill> asteroidDrills = null;
                foreach (Vessel loadedVessel in FlightGlobals.VesselsLoaded)
                {
                    asteroidDrills = loadedVessel.FindPartModulesImplementing<WBIGoldStrikeAsteroidDrill>();
                    if (asteroidDrills != null)
                    {
                        debugLog("Found " + asteroidDrills.Count + " asteroid drills");
                        foreach (WBIGoldStrikeAsteroidDrill asteroidDrill in asteroidDrills)
                            asteroidDrill.UpdateLode();
                    }
                }
            }
        }

        public double GetDistanceFromLastLocation(Vessel vessel)
        {
            double distance = 0;

            //Pull last prospect location from the vessel module.
            GoldStrikeVesselModule vesselModule = null;
            foreach (VesselModule module in vessel.vesselModules)
            {
                if (module is GoldStrikeVesselModule)
                {
                    vesselModule = (GoldStrikeVesselModule)module;
                    break;
                }
            }
            if (vesselModule == null)
                return double.MaxValue;

            //If we've never set a prospect location then we're automatically ok.
            if (!vesselModule.HasLastProspectLocation())
                return double.MaxValue;

            //Calculate the distance
            distance = Utils.HaversineDistance(vesselModule.lastProspectLongitude, vesselModule.lastProspectLatitude,
                vessel.longitude, vessel.latitude, vessel.mainBody);

            return distance;
        }

        public bool IsAsteroidProspected(ModuleAsteroid asteroid)
        {
            if (asteroid == null)
                return false;

            if (this.prospectedAsteroids.Contains(asteroid.AsteroidName))
                return true;

            return false;
        }

        public bool VesselSituationValid(Vessel vessel)
        {
            if (vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.SPLASHED || vessel.situation == Vessel.Situations.PRELAUNCH)
                return true;
            else
                return false;
        }

        public ProspectSituations GetProspectSituation(Vessel vessel, out GoldStrikeLode lode, out List<GoldStrikeData> prospectResources)
        {
            ModuleAsteroid asteroid = null;
            CBAttributeMapSO.MapAttribute biome = null;
            string biomeName = string.Empty;
            int planetID = int.MaxValue;
            bool vesselSituationIsValid = false;
            double longitude = 0f;
            double latitude = 0f;
            double altitude = 0f;

            //Initialize the list of resources to prospect.
            prospectResources = null;

            //If we're landed then we're ok to check prospect situation.
            if (VesselSituationValid(vessel))
            {
                biome = Utils.GetCurrentBiome(vessel);
                biomeName = biome.name;
                planetID = vessel.mainBody.flightGlobalsIndex;
                longitude = vessel.longitude;
                latitude = vessel.latitude;
                altitude = vessel.altitude;
                vesselSituationIsValid = true;
                debugLog("Vessel is landed or prelaunch");
            }

            //If we have an asteroid, then we're ok to check prospect situation.
            asteroid = vessel.FindPartModuleImplementing<ModuleAsteroid>();
            if (asteroid != null)
            {
                biomeName = asteroid.AsteroidName;
                vesselSituationIsValid = true;
                debugLog("Vessel has an asteroid");
            }

            //Is there a lode in the area?
            if (asteroid != null)
                lode = FindNearestLode(asteroid);
            else
                lode = FindNearestLode(planetID, biomeName, vessel.longitude, vessel.latitude, vessel.altitude);
            if (lode != null)
                return ProspectSituations.LodeAlreadyExists;

            //If the flight situation is bad then we're done.
            if (vesselSituationIsValid == false)
                return ProspectSituations.InvalidVesselSituation;

            //Is the prospect site an asteroid, and has it been prospected?
            if (asteroid != null)
            {
                if (prospectedAsteroids.Contains(asteroid.AsteroidName))
                    return ProspectSituations.AsteroidProspected;

                else //Record the fact that we prospected the asteroid.
                    prospectedAsteroids.Add(asteroid.AsteroidName);
            }

            //Do we have resources that can be prospected?
            else
            {
                double travelDistance = GetDistanceFromLastLocation(vessel);
                List<GoldStrikeData> resources = new List<GoldStrikeData>();
                GoldStrikeData strikeData;
                int count = goldStrikeResources.Keys.Count;
                string[] keys = goldStrikeResources.Keys.ToArray();
                string resourceName;

                for (int index = 0; index < count; index++)
                {
                    resourceName = keys[index];
                    strikeData = goldStrikeResources[resourceName];

                    if (strikeData.MatchesProspectCriteria(vessel.situation, vessel.mainBody.name, biomeName, altitude, travelDistance))
                        resources.Add(strikeData);
                }

                //If we have no resources that meet the criteria then we're done.
                if (resources.Count == 0)
                    return ProspectSituations.NoResourcesToProspect;

                //Record the possible prospect resources.
                else
                    prospectResources = resources;
            }

            return ProspectSituations.Valid;
        }

        public GoldStrikeLode FindNearestLode(ModuleAsteroid asteroid)
        {
            int planetID;
            string biomeName;
            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, null, asteroid);

            string planetBiomeKey = planetID.ToString() + biomeName;
            Dictionary<string, GoldStrikeLode> lodeMap = null;
            debugLog("planetBiomeKey: " + planetBiomeKey);

            //Get the lode map. If there is none then we're done.
            if (goldStrikeLodes.ContainsKey(planetBiomeKey) == false)
            {
                debugLog("goldStrikeLodes has no lodeMap for key: " + planetBiomeKey);
                return null;
            }
            lodeMap = goldStrikeLodes[planetBiomeKey];
            debugLog("lodeMap has " + lodeMap.Count + " entries");
            if (showDebugLog)
            {
                foreach (string key in lodeMap.Keys)
                {
                    debugLog("Key: " + key + "\r\n lode: " + lodeMap[key].ToString());
                }
            }

            //Asteroids only have one lode
            if (lodeMap.ContainsKey(asteroid.AsteroidName) == false)
            {
                debugLog("No GoldStrikeLode found in lodeMap for " + asteroid.AsteroidName);
                return null;
            }

            return lodeMap[asteroid.AsteroidName];
        }

        public GoldStrikeLode FindNearestLode(int planetID, string biome, double longitude, double latitude, double altitude, double searchDistance = kMaxProspectSearchDistance)
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

                distance = Utils.HaversineDistance(longitude, latitude, lode.longitude, lode.lattitude, FlightGlobals.Bodies[lode.planetID]);
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

        public GoldStrikeLode AddLode(ModuleAsteroid asteroid, string resourceName, float abundance)
        {
            int planetID;
            string biomeName;
            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, null, asteroid);
            GoldStrikeLode lode = new GoldStrikeLode();
            Dictionary<string, GoldStrikeLode> lodeMap = null;
            string planetBiomeKey = planetID.ToString() + biomeName;

            //Setup the new lode
            lode.resourceName = resourceName;
            lode.longitude = 0;
            lode.lattitude = 0;
            lode.biome = biomeName;
            lode.abundance = abundance;
            lode.planetID = planetID;

            //Get the lode map
            if (goldStrikeLodes.ContainsKey(planetBiomeKey) == false)
            {
                lodeMap = new Dictionary<string, GoldStrikeLode>();
                goldStrikeLodes.Add(planetBiomeKey, lodeMap);
                debugLog("Added new goldStrikeLode with planetBiomeKey: " + planetBiomeKey);
            }
            lodeMap = goldStrikeLodes[planetBiomeKey];

            //Add the new lode
            lodeMap.Add(asteroid.AsteroidName, lode);
            goldStrikeLodes[planetBiomeKey] = lodeMap;
            debugLog("Added new lode: " + lode.ToString());

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);

            return lode;
        }

        public GoldStrikeLode AddLode(int planetID, string biome, double longitude, double lattitude, double altitude, string resourceName, double amountRemaining)
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
            goldStrikeLodes[planetBiomeKey] = lodeMap;
            debugLog("Added new lode: " + lode.ToString());

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);

            return lode;
        }
        #endregion
    }
}
