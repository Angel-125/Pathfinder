using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;

/*
Source code copyrighgt 2017, by Michael Billard (Angel-125)
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
    public class WBIGoldStrike : PartModule, IOpsView
    {
        private const float kMessageDisplayTime = 10.0f;
        private const float kMotherlodeFactor = 0.05f;
        private const float kLabSkillBonus = 0.5f;

        [KSPField()]
        public bool showGUI;

        //prospectChance: base chance to find a prospect. Some parts are better than others.
        [KSPField()]
        public float prospectChance = 10.0f;

        //Skill required to do some prospecting.
        [KSPField()]
        public string prospectSkill = "ScienceSkill";

        //prospectSkillBonus: multiplied by the EVA prospector's skill level. Default is 1.0.
        [KSPField()]
        public float prospectSkillBonus = 1.0f;

        //Default ore to search through is er, Ore.
        [KSPField()]
        public string oreResource = "Ore";

        [KSPField()]
        public float prospectThreshold = 0.01f;

        [KSPField()]
        public string lodeStrikeSound = string.Empty;

        [KSPField(guiName = "Next Prospect (km)", guiFormat = "f2", guiActive = true)]
        public double nextProspectDistance = 0f;

        protected ModuleAsteroid asteroid = null;
        protected ModuleAsteroidInfo asteroidInfo = null;
        protected AudioSource jingle = null;
        protected Vector3d lastProspectLocation = Vector3d.zero;
        double minTravelDistance = 10f;
        bool prospectResetConfirmed;

        protected void debugLog(string message)
        {
            if (WBIPathfinderScenario.showDebugLog == true)
                Debug.Log("[WBIGoldStrike] - " + message);
        }

        public void onGameSettingsApplied()
        {
            minTravelDistance = GoldStrikeSettings.DistanceBetweenProspects;
        }

        public void Destroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(onGameSettingsApplied);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Events["CheckGoldStrike"].guiActive = showGUI;
            Events["CheckGoldStrike"].guiActiveUncommand = showGUI;

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //See if we have an asteroid.
            asteroid = this.part.vessel.FindPartModuleImplementing<ModuleAsteroid>();

            //Minimum travel distance
            minTravelDistance = GoldStrikeSettings.DistanceBetweenProspects;
            GameEvents.OnGameSettingsApplied.Add(onGameSettingsApplied);

            //Get the last prospect location for the local planet and biome
            if (this.part.vessel.situation == Vessel.Situations.PRELAUNCH || this.part.vessel.situation == Vessel.Situations.LANDED)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
                lastProspectLocation = WBIPathfinderScenario.Instance.GetLastProspectLocation(this.part.vessel.mainBody.flightGlobalsIndex, biome.name);
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //If we don't have a last prospect location then the distance is zero.
            if (lastProspectLocation == Vector3d.zero)
            {
                nextProspectDistance = 0f;
                return;
            }

            double distance = getDistanceFromLastLocation();

            //Update distance to next prospect
            nextProspectDistance = (minTravelDistance - distance);
        }

        [KSPEvent(guiName = "Reset Prospect Chances", unfocusedRange = 3.0f)]
        public void ResetProspects()
        {
            //Since it costs science, ask for confirmation.
            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER || HighLogic.CurrentGame.Mode == Game.Modes.SCIENCE_SANDBOX)
            {
                float scienceCost = GoldStrikeSettings.ProspectResetCost;

                //Confirmed, pay the science cost.
                if (prospectResetConfirmed)
                {
                    ResearchAndDevelopment.Instance.AddScience(-scienceCost, TransactionReasons.Any);
                }

                else
                {
                    prospectResetConfirmed = true;
                    string message = string.Format("It will cost {0:f2} Science to renew your prospecting chances in this biome. Click to confirm.", scienceCost);
                    ScreenMessages.PostScreenMessage(message, kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
            }

            //Reset the chances
            string biomeName = string.Empty;
            int planetID = -1;
            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, this.part.vessel, asteroid);
            GoldStrikeChance chance = WBIPathfinderScenario.Instance.GetGoldStrikeChance(planetID, biomeName);
            chance.chancesRemaining = GoldStrikeSettings.ProspectsPerBiome;

            //Hide the reset button
            Events["ResetProspects"].guiActive = false;
            Events["ResetProspects"].guiActiveUnfocused = false;

            //Save the game
            GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);
        }

        [KSPEvent(guiName = "Prospect for resources", unfocusedRange = 3.0f)]
        public void CheckGoldStrike()
        {
            string[] strikeResourceKeys = null;
            int resourceIndex;
            GoldStrikeData strikeData = null;
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            string resourceName = string.Empty;
            double resourceAmount = 0f;
            float analysisRoll = 0f;
            float successTargetNumber = prospectChance;
            int chancesRemaining = 0;
            string navigationID = string.Empty;
            GoldStrikeLode lode = null;
            string biomeName = string.Empty;
            int planetID = -1;

            //Do we have gold strike resources?
            if (scenario.goldStrikeResources.Count() == 0)
            {
                ScreenMessages.PostScreenMessage("There are no Gold Strike resources to prospect!", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                debugLog("No resources to prospect");
                return;
            }

            //Do we have a valid situation?
            if (SituationIsValid() == false)
                return;

            //Ok, we can prospect at this location.
            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, this.part.vessel, asteroid);
            setLastLocation();
            chancesRemaining = updateChancesRemaining();

            //Time to see if we find anything.
            //Tally up the % chance we have to make a successful prospect.
            //prospectChance: base chance to find a prospect. Some parts are better than others.
            //prospectSkillBonus: multiplied by the EVA prospector's skill level. Default is 1.0.
            //labBonus: For each Pathfinder geology lab in the vicinity, give one point per crew member staffing the lab that has the prospectSkill.
            //For each non-Pathfinder geology lab in the vicinity, give half a point per crew member staffing the lab that has the prospectSkill.
            //Chance = prospectChance + prospectSkillBonus + labBonus.
            //Ex: A 3-star scientist on EVA makes a prospect check. skillBonus = 3; prospectSkillBonus = 1.0. Total skill bonus = 3.0.
            //Inside the Bison are two scientists staffing a geology lab (non-pathfinder). labBonus = 2 * 0.5 = 1
            //Gold Digger has a base 10% chance of finding a prospect.
            //successTargetNumber = 10 + 3 + 1 = 14.
            successTargetNumber = prospectChance + GetProspectBonus();
            debugLog("Base chance to succeed: " + prospectChance);
            debugLog("successTargetNumber: " + successTargetNumber);

            //Roll the chance and check it.
            analysisRoll = UnityEngine.Random.Range(1, 6);
            analysisRoll += UnityEngine.Random.Range(1, 6);
            analysisRoll += UnityEngine.Random.Range(1, 6);
            analysisRoll *= 5.5556f;
            debugLog("analysisRoll: " + analysisRoll);

            //If we didn't succeed then we're done.
            if (analysisRoll > successTargetNumber)
            {
                debugLog("Prospect failed; didn't roll low enough.");
                ScreenMessages.PostScreenMessage("Nothing of value here, try another location. " + chancesRemaining + " chances remain in the " + biomeName, kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //Success! Get the resource name and strike data.
            strikeResourceKeys = scenario.goldStrikeResources.Keys.ToArray();
            resourceIndex = UnityEngine.Random.Range(0, strikeResourceKeys.Length - 1);
            resourceName = strikeResourceKeys[resourceIndex];
            strikeData = scenario.goldStrikeResources[resourceName];
            debugLog("strikeResourceKeys count: " + strikeResourceKeys.Length);
            debugLog("resourceIndex: " + resourceIndex);
            debugLog("strikeData: " + strikeData.ToString());

            //Now, generate the resource amount to add to the map
            resourceAmount = UnityEngine.Random.Range(strikeData.minAmount, strikeData.maxAmount);
            debugLog("resourceAmount: " + resourceAmount);

            //If we hit the motherlode then factor that in.
            //The motherloade is 5% of the target number.
            if (analysisRoll <= (successTargetNumber * kMotherlodeFactor))
            {
                resourceAmount *= strikeData.motherlodeMultiplier;
                debugLog("resourceAmount after motherlode: " + resourceAmount);
                ScreenMessages.PostScreenMessage(string.Format("Congratulations! You found a {0:s} motherlode with {1:f2} units available to mine!", resourceName, resourceAmount),
                    kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
            }

            else
            {
                ScreenMessages.PostScreenMessage(string.Format("Congratulations! You found a {0:s} lode with {1:f2} units available to mine!", resourceName, resourceAmount),
                    kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
            }
            ScreenMessages.PostScreenMessage(chancesRemaining + " chances remain to find another lode in the " + biomeName, kMessageDisplayTime, ScreenMessageStyle.UPPER_LEFT);

            //Play the jingle
            playJingle();

            //Now set up the lode
            debugLog("Adding new lode entry to " + FlightGlobals.currentMainBody.name + " with flight global index " + FlightGlobals.currentMainBody.flightGlobalsIndex);
            debugLog("Biome: " + biomeName);
            debugLog("Lon/Lat: " + this.part.vessel.longitude + "/" + this.part.vessel.latitude);
            lode = scenario.AddLode(planetID, biomeName,
                this.part.vessel.longitude, this.part.vessel.latitude, resourceName, resourceAmount);

            //Set waypoint
            if (lode != null)
                setWaypoint(resourceName, lode);

        }

        public bool SituationIsValid()
        {
            debugLog("SituationIsValid: checking...");
            CBAttributeMapSO.MapAttribute biome = null;
            string biomeName = string.Empty;
            int planetID = int.MaxValue;
            bool vesselSituationIsValid = false;
            double longitude = 0f;
            double latitude = 0f;
            double altitude = 0f;
            GoldStrikeLode lode = null;

            //If we're landed then we're ok to check prospect situation.
            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                biome = Utils.GetCurrentBiome(this.part.vessel);
                biomeName = biome.name;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
                longitude = this.part.vessel.longitude;
                latitude = this.part.vessel.latitude;
                altitude = this.part.vessel.altitude;
                vesselSituationIsValid = true;
                debugLog("Vessel is landed or prelaunch");
            }

            //If we're docked or orbiting, and we have an asteroid, then we're ok to check prospect situation.
            if ((this.part.vessel.situation == Vessel.Situations.ORBITING || this.part.vessel.situation == Vessel.Situations.DOCKED) && asteroid != null)
            {
                biomeName = asteroid.AsteroidName;
                vesselSituationIsValid = true;
                debugLog("Vessel has an asteroid");
            }

            //If the flight situation is bad then we're done.
            if (vesselSituationIsValid == false)
            {
                debugLog("Vessel situation not valid");
                ScreenMessages.PostScreenMessage("Vessel must be landed or in orbit/docked with an asteroid attached", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            //If we don't have sufficient ore abundance then we're done.
            if (!HasSufficientAbundance())
                return false;

            //Can we prospect the location?
            switch (WBIPathfinderScenario.Instance.GetProspectSituation(planetID, biomeName, longitude, latitude, altitude, out lode))
            {
                case ProspectSituations.LodeAlreadyExists:
                    if (lode != null)
                    {
                        debugLog("Situation not valid, existing lode found: " + lode.ToString());
                        string message = string.Format("You already found a vein of {0:s} at this location. It has {1:f2} units remaining.", lode.resourceName, lode.amountRemaining);
                        ScreenMessages.PostScreenMessage(message, kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                    }
                    return false;

                case ProspectSituations.NotEnoughDistance:
                    debugLog("Vessel has not traveled enough distance between prospects");
                    ScreenMessages.PostScreenMessage("Vessel must travel further before prospecting again.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                    return false;

                case ProspectSituations.OutOfChances:
                    debugLog("Out of prospecting chances for this area or asteroid");
                    ScreenMessages.PostScreenMessage("Out of prospecting chances in this area.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                    //Enable option to reset chances.
                    Events["ResetProspects"].guiActive = true;
                    Events["ResetProspects"].guiActiveUnfocused = true;
                    return false;

                //A-OK
                default:
                    return true;
            }

        }

        public float GetProspectBonus()
        {
            float skillBonus = GoldStrikeSettings.BonusPerSkillPoint;
            float prospectBonus = 0f;
            float labBonus = 0f;
            Vessel[] vessels;
            Vessel vessel;
            ProtoCrewMember[] crewMembers;
            ProtoCrewMember crewMember;
            WBIGeologyLab[] pathfinderLabs;
            WBIGeologyLab pathfinderLab;
            WBIGeoLab[] geoLabs;
            WBIGeoLab geoLab;
            ProtoVessel protoVessel;

            //If an experienced prospector is taking the core sample, then the prospector's experience will
            //affect the analysis.
            if (FlightGlobals.ActiveVessel.isEVA && Utils.IsExperienceEnabled())
            {
                vessel = FlightGlobals.ActiveVessel;
                ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                if (astronaut.HasEffect(prospectSkill))
                    prospectBonus = astronaut.experienceTrait.CrewMemberExperienceLevel() * skillBonus;
            }

            //Factor in lab bonus
            //Bonus is 0.5 * crew skill (max 2.5) per crewmember
            vessels = FlightGlobals.VesselsLoaded.ToArray();
            for (int index = 0; index < vessels.Length; index++)
            {
                vessel = vessels[index];

                List<WBIGeologyLab> wbiGeologyLabs = vessel.FindPartModulesImplementing<WBIGeologyLab>();
                if (wbiGeologyLabs.Count > 0)
                {
                    pathfinderLabs = wbiGeologyLabs.ToArray();
                    for (int labIndex = 0; labIndex < pathfinderLabs.Length; labIndex++)
                    {
                        pathfinderLab = pathfinderLabs[labIndex];
                        if (pathfinderLab.part.protoModuleCrew.Count > 0)
                        {
                            crewMembers = pathfinderLab.part.protoModuleCrew.ToArray();
                            for (int crewIndex = 0; crewIndex < crewMembers.Length; crewIndex++)
                            {
                                crewMember = crewMembers[crewIndex];
                                if (crewMember.HasEffect(prospectSkill))
                                {
                                    labBonus += kLabSkillBonus * crewMember.experienceTrait.CrewMemberExperienceLevel();
                                }
                            }
                        }
                    }
                }

                List<WBIGeoLab> geologyLabs = vessel.FindPartModulesImplementing<WBIGeoLab>();
                if (geologyLabs.Count > 0)
                {
                    geoLabs = geologyLabs.ToArray();
                    for (int labIndex = 0; labIndex < geoLabs.Length; labIndex++)
                    {
                        geoLab = geoLabs[labIndex];
                        if (geoLab.part.protoModuleCrew.Count > 0)
                        {
                            crewMembers = geoLab.part.protoModuleCrew.ToArray();
                            for (int crewIndex = 0; crewIndex < crewMembers.Length; crewIndex++)
                            {
                                crewMember = crewMembers[crewIndex];
                                if (crewMember.HasEffect(prospectSkill))
                                {
                                    labBonus += kLabSkillBonus * crewMember.experienceTrait.CrewMemberExperienceLevel();
                                }
                            }
                        }
                    }
                }
            }

            //Unloaded vessels
            vessels = FlightGlobals.VesselsUnloaded.ToArray();
            for (int index = 0; index < vessels.Length; index++)
            {
                vessel = vessels[index];
                protoVessel = vessel.protoVessel;

                //Only applies to vessels of the same body that are landed.
                if (vessel.mainBody != this.part.vessel.mainBody)
                    continue;
                if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.PRELAUNCH)
                    continue;

                //Find all the pathfinder labs
                foreach (ProtoPartSnapshot partSnapshot in protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot moduleSnapshot in partSnapshot.modules)
                    {
                        if (moduleSnapshot.moduleName == "WBIGeologyLab" || moduleSnapshot.moduleName == "WBIGeoLab")
                        {
                            foreach (ProtoCrewMember protoCrewMember in partSnapshot.protoModuleCrew)
                            {
                                if (protoCrewMember.HasEffect(prospectSkill))
                                    labBonus += kLabSkillBonus * protoCrewMember.experienceTrait.CrewMemberExperienceLevel();
                            }
                        }
                    }
                }
            }

            debugLog("Prospector bonus: " + prospectBonus + " labBonus: " + labBonus);
            return prospectBonus + labBonus;
        }

        public bool HasSufficientAbundance()
        {
            ModuleAsteroidResource[] asteroidResources;

            if (asteroid == null)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
                string biomeName = biome.name;

                IEnumerable<ResourceCache.AbundanceSummary> abundanceCache = ResourceCache.Instance.AbundanceCache.
                    Where(a => a.HarvestType == HarvestTypes.Planetary && a.BodyId == this.part.vessel.mainBody.flightGlobalsIndex && a.BiomeName == biomeName);

                if (abundanceCache.Count<ResourceCache.AbundanceSummary>() > 0)
                {
                    foreach (ResourceCache.AbundanceSummary summary in abundanceCache)
                    {
                        if (summary.ResourceName != oreResource)
                            continue;

                        if (summary.Abundance < prospectThreshold)
                        {
                            setLastLocation();
                            debugLog("Biome location's ore concentration is too low.");
                            debugLog("Abundance: " + summary.Abundance);
                            debugLog("Threshold: " + prospectThreshold);
                            ScreenMessages.PostScreenMessage("Insufficient concentration of " + oreResource + " in this area. Try another location.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                            return false;
                        }

                        else //We have enough ore in the location
                        {
                            debugLog("Biome location has enough ore concentration");
                            debugLog("Abundance: " + summary.Abundance);
                            break;
                        }
                    }
                }
            }

            //Check asteroid ore concentration
            else
            {
                asteroidResources = asteroid.part.FindModulesImplementing<ModuleAsteroidResource>().ToArray();
                foreach (ModuleAsteroidResource asteroidResource in asteroidResources)
                {
                    if (asteroidResource.abundance < prospectThreshold)
                    {
                        debugLog("asteroid's ore concentration is too low.");
                        debugLog("Abundance: " + asteroidResource.abundance);
                        debugLog("Threshold: " + prospectThreshold);
                        ScreenMessages.PostScreenMessage("Insufficient concentration of " + oreResource + " in the asteroid. Try another asteroid.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                        return false;
                    }
                    else //We have enough ore in the asteroid
                    {
                        debugLog("asteroid has enough ore concentration");
                        debugLog("Abundance: " + asteroidResource.abundance);
                        break;
                    }
                }
            }

            return true;
        }

        protected void playJingle()
        {
            if (!string.IsNullOrEmpty(lodeStrikeSound))
            {
                jingle = gameObject.AddComponent<AudioSource>();
                jingle.clip = GameDatabase.Instance.GetAudioClip(lodeStrikeSound);
                jingle.volume = GameSettings.SHIP_VOLUME;
                jingle.loop = false;
            }

            if (jingle != null && jingle.isPlaying == false)
                jingle.Play();
        }

        protected string setWaypoint(string resourceName, GoldStrikeLode lode)
        {
            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                debugLog("Trying to set waypoint");
                string location = string.Format("Lon: {0:f2} Lat: {1:f2}", this.part.vessel.longitude, this.part.vessel.latitude);

                Waypoint waypoint = new Waypoint();
                waypoint.name = resourceName + " Lode";
                waypoint.isExplored = true;
                waypoint.isNavigatable = true;
                waypoint.isOnSurface = true;
                waypoint.celestialName = this.part.vessel.mainBody.name;
                waypoint.longitude = this.part.vessel.longitude;
                waypoint.latitude = this.part.vessel.latitude;
                waypoint.seed = UnityEngine.Random.Range(0, int.MaxValue);
                waypoint.navigationId = Guid.NewGuid();

                //Add the waypoint to the custom waypoint scenario
                ScenarioCustomWaypoints.AddWaypoint(waypoint);
                
                //Our icon is not correct, do a quick remove, reset the icon, and add
                WaypointManager.RemoveWaypoint(waypoint);
                waypoint.id = WBIPathfinderScenario.kLodeIcon;
                waypoint.nodeCaption1 = location;
                WaypointManager.AddWaypoint(waypoint);

                //Record the waypoint info
                lode.navigationID = waypoint.navigationId.ToString();

                //Save the game
                GamePersistence.SaveGame("quicksave", HighLogic.SaveFolder, SaveMode.BACKUP);

                //Done
                return waypoint.navigationId.ToString();
            }

            return string.Empty;
        }

        protected void setLastLocation()
        {
            CBAttributeMapSO.MapAttribute biome = null;
            string biomeName = string.Empty;
            int planetID = int.MaxValue;
            double longitude = 0f;
            double latitude = 0f;
            double altitude = 0f;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                biome = Utils.GetCurrentBiome(this.part.vessel);
                biomeName = biome.name;

                planetID = this.part.vessel.mainBody.flightGlobalsIndex;

                longitude = this.part.vessel.longitude;
                latitude = this.part.vessel.latitude;
                altitude = this.part.vessel.altitude;
            }

            else if ((this.part.vessel.situation == Vessel.Situations.ORBITING || this.part.vessel.situation == Vessel.Situations.DOCKED) && asteroid != null)
            {
                biomeName = asteroid.AsteroidName;
            }

            //Keep a copy of the location for our purposes
            lastProspectLocation.x = longitude;
            lastProspectLocation.y = latitude;

            debugLog("Last lode location: " + planetID + " " + biomeName + " lon: " + longitude + " lat: " + latitude + " altitude: " + altitude);
            WBIPathfinderScenario.Instance.SetLastProspectLocation(planetID, biomeName, longitude, latitude, altitude);
        }

        protected int updateChancesRemaining()
        {
            string biomeName = string.Empty;
            int planetID = int.MaxValue;
            int chancesRemaining = 0;

            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, this.part.vessel, asteroid);

            chancesRemaining = WBIPathfinderScenario.Instance.DecrementProspectAttempts(planetID, biomeName);
            debugLog("chancesRemaining at " + planetID + " " + biomeName + " = " + chancesRemaining);
            return chancesRemaining;
        }

        protected double getDistanceFromLastLocation()
        {
            double distance = 0f;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                //If we have no last prospect then the distance is zero.
                if (lastProspectLocation == Vector3d.zero)
                    return 0f;

                //In kilometers
                distance = GoldStrikeUtils.HaversineDistance(lastProspectLocation.x, lastProspectLocation.y,
                    this.part.vessel.longitude, this.part.vessel.latitude, this.part.vessel.mainBody);
            }

            return distance;
        }

        public List<string> GetButtonLabels()
        {
            List<string> labels = new List<string>();

            labels.Add("Gold Strike");

            return labels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            GUILayout.Label(string.Format("Next Prospect: {0:f2}km", nextProspectDistance));

            if (GUILayout.Button("Prospect for resources"))
                CheckGoldStrike();

            if (Events["ResetProspects"].guiActive)
            {
                if (GUILayout.Button("Reset prospect chances"))
                    ResetProspects();
            }

            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            Events["CheckGoldStrike"].guiActive = isVisible;
            Events["CheckGoldStrike"].guiActiveUncommand = isVisible;
        }

        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }
    }
}
