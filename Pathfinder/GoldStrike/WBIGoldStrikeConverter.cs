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
    public class WBIGoldStrikeConverter : ModuleResourceConverter
    {
        private const float kMessageDisplayTime = 10.0f;
        private const float searchTriggerDistance = 10.0f;

        [KSPField(guiName = "Lode Units Remaining", guiFormat = "f2", guiActive = true)]
        public double lodeUnitsRemaining;

        [KSPField()]
        public string inputResource = "Ore";

        [KSPField()]
        public float maxHarvestRange = 200.0f;

        protected GoldStrikeLode nearestLode = null;
        protected ModuleAsteroid asteroid = null;
        protected Vector3d lastLocation = Vector3d.zero;
        protected double inputResourceDensity = 0f;
        protected double inputResourceUnits = 0f;
        ModuleResourceHarvester harvester = null;
        ModuleAsteroidDrill asteroidDrill = null;

        protected void debugLog(string message)
        {
            if (WBIPathfinderScenario.showDebugLog == true)
                Debug.Log("[WBIGoldStrikeConverter] - " + message);
        }

        public override void OnStart(StartState state)
        {
 	        base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //Get the density of the input resource
            PartResourceDefinition inputDef = ResourceHelper.DefinitionForResource(inputResource);
            if (inputDef != null)
                inputResourceDensity = inputDef.density;

            //Get the units of the input resource
            foreach (ResourceRatio ratio in inputList)
            {
                if (ratio.ResourceName == inputResource)
                {
                    inputResourceUnits = ratio.Ratio;
                    break;
                }
            }
            debugLog("inputResourceDensity: " + inputResourceDensity);
            debugLog("inputResourceUnits: " + inputResourceUnits);

            //Get the asteroid if any
            asteroid = this.part.vessel.FindPartModuleImplementing<ModuleAsteroid>();

            updateNearestLode();
            updateLastLocation();
        }

        protected void updateNearestLode()
        {
            int planetID;
            string biomeName;
            double longitude = 0f;
            double latitude = 0f;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                longitude = this.part.vessel.longitude;
                latitude = this.part.vessel.latitude;
            }

            //Find the nearest lode (if any)
            GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, this.part.vessel, asteroid);
            nearestLode = WBIPathfinderScenario.Instance.FindNearestLode(planetID, biomeName, longitude, latitude);

            if (nearestLode != null)
                debugLog("nearestLode: " + nearestLode.ToString());
            else
                debugLog("No lode found nearby.");
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
            
            //If we have an asteroid then we don't need to do anything.
            if (asteroid != null)
                return;

            //If we aren't landed then we're done.
            if (this.part.vessel.situation != Vessel.Situations.PRELAUNCH && this.part.vessel.situation != Vessel.Situations.LANDED)
                return;
            
            //Have we moved beyond our travel threshold? If so, look for the nearest node.
            double travelDistance = GoldStrikeUtils.HaversineDistance(this.part.vessel.longitude, this.part.vessel.latitude,
                lastLocation.x, lastLocation.y, this.part.vessel.mainBody);
            if (travelDistance > searchTriggerDistance)
            {
                updateNearestLode();
                updateLastLocation();
            }

            if (nearestLode != null)
                lodeUnitsRemaining = nearestLode.amountRemaining;
            else
                lodeUnitsRemaining = double.NaN;
        }

        double outputUnits = 0f;
        protected override ConversionRecipe PrepareRecipe(double deltatime)
        {
            PartResourceDefinition outputDef = null;
            double flowRate = 1.0f;
            string flowRateString = string.Empty;

            //If the converter isn't running then we're done.
            if (IsActivated == false)
            {
                return base.PrepareRecipe(deltatime);
            }

            //If our situation isn't valid then we're done.
            if (!situationIsValid())
                return base.PrepareRecipe(deltatime);
           
            //Ok, we've got a valid lode, we're in harvest range, and there are units remaining.
            //Find the resource definition for the output resource
            outputDef = ResourceHelper.DefinitionForResource(nearestLode.resourceName);
            if (outputDef == null)
            {
                debugLog("No definition for " + nearestLode.resourceName);
                ScreenMessages.PostScreenMessage("Can't find a definition for " + nearestLode.resourceName, kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                StopResourceConverter();
                return base.PrepareRecipe(deltatime);
            }

            //Get the input resource flow rate.
            if (asteroid == null) //Use planetary drill
            {
                //Good going SQUAD, how about making _resFlow public???
                if (string.IsNullOrEmpty(harvester.ResourceStatus) == false)
                {
                    flowRateString = harvester.ResourceStatus.Substring(0, harvester.ResourceStatus.IndexOf("/ sec"));
                    double.TryParse(flowRateString, out flowRate);
                }
            }

            //Output units per second is the ratio of the input to output density, multiplied by the drill's flow rate.
            outputUnits = (inputResourceDensity / outputDef.density) * flowRate;
            //The output list is in terms of units per second. We need units per time tick for the hack to work.
            outputUnits *= deltatime; 

            //Keep track of how many units we've dug up.
            //If the amount remaining < outputUnits then adjust accordingly.
            double unitsAvailable = 0;
            if (nearestLode.amountRemaining > outputUnits)
            {
                unitsAvailable = outputUnits;
            }

            else
            {
                unitsAvailable = nearestLode.amountRemaining;
            }

//            debugLog("nearestLode.amountRemaining: " + nearestLode.amountRemaining);
//            debugLog("status: " + status);
//            debugLog("statusPercent: " + statusPercent);
//            debugLog("outputUnits: " + outputUnits);

            /*
             *HACK! Technically, I can add the output source to the output list and the converter will do its thing- unless the output is pretty small.
             *Thanks to the crappy documentation by SQUAD, I can't figure out why the small numbers of the output ratio don't seen to actually output the desired resource.
             *So, what we'll do is simply let the converter consume the Ore per time tick, and manually add the desired resource throughout the vessel.
            ResourceRatio outputSource = new ResourceRatio { ResourceName = nearestLode.resourceName, Ratio = outputUnits, FlowMode = ResourceFlowMode.ALL_VESSEL_BALANCE, DumpExcess = false };
            outputList.Clear();
            outputList.Add(outputSource);
            outputUnits = 0f;
             */

            //HACK! The converter is set up to just consume Ore. Manually create the output resource, with the units already accounting for conservation of mass.
            double unitsAdded = this.part.RequestResource(nearestLode.resourceName, -unitsAvailable, ResourceFlowMode.ALL_VESSEL_BALANCE);
            if (unitsAdded > 0.001)
            {
                nearestLode.amountRemaining -= unitsAdded;
                if (nearestLode.amountRemaining < 0.001)
                    nearestLode.amountRemaining = 0;
            }

            //No room on the ship, stop converter.
            else
            {
                status = nearestLode.resourceName + " full";
                StopResourceConverter();
            }

            //Update GUI
            lodeUnitsRemaining = nearestLode.amountRemaining;

            return base.PrepareRecipe(deltatime);
        }

        protected bool situationIsValid()
        {
            //If we don't have input units and density, then we're in trouble.
            if (inputResourceDensity == 0f || inputResourceUnits == 0f)
            {
                ScreenMessages.PostScreenMessage("Converter stopped. No input resource density or input units found.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                StopResourceConverter();
                return false;
            }

            //If we don't have a harvesting node then see if we can find one.
            if (nearestLode == null)
            {
                updateLastLocation();
                updateNearestLode();

                //If we don't have a nearby node then we're done.
                if (nearestLode == null)
                {
                    debugLog("Converter stopped. Vessel isn't near a lode (null).");
                    ScreenMessages.PostScreenMessage("Converter stopped. This vessel isn't in range of a lode.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                    StopResourceConverter();
                    return false;
                }

                else
                {
                    lodeUnitsRemaining = nearestLode.amountRemaining;
                }
            }

            //If we're out of resource or are full then we're done.
            if (status.ToLower().Contains("missing") || status.ToLower().Contains("full"))
            {
                StopResourceConverter();
                lodeUnitsRemaining = nearestLode.amountRemaining;
                return false;
            }

            //If we aren't within harvesting range of the nearest lode, then we're done.
            double harvestDistance = GoldStrikeUtils.HaversineDistance(this.part.vessel.longitude, this.part.vessel.latitude,
                nearestLode.longitude, nearestLode.lattitude, this.part.vessel.mainBody);
            if (harvestDistance > maxHarvestRange)
            {
                debugLog("Converter stopped. Vessel isn't near a lode (out of range).");
                ScreenMessages.PostScreenMessage("Converter stopped. This vessel isn't in range of a lode.", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                StopResourceConverter();
                return false;
            }

            //If we don't have any more resources to harvest then we're done.
            if (nearestLode.amountRemaining == 0f)
            {
                debugLog("Converter stopped. Lode is depleted.");
                ScreenMessages.PostScreenMessage("This vein of " + nearestLode.resourceName + " has been depleted. Time to move on...", kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                StopResourceConverter();
                return false;
            }

            //Grab the drills if needed. We need at least one planetary or one asteroid drill.
            if (harvester == null)
                harvester = this.part.FindModuleImplementing<ModuleResourceHarvester>();
            if (asteroidDrill == null)
                asteroidDrill = this.part.FindModuleImplementing<ModuleAsteroidDrill>();
            if (harvester == null || asteroidDrill == null)
            {
                debugLog("No drill found!!!");
                return false;
            }

            //Situation is valid
            return true;
        }

        protected override void PostProcess(ConverterResults result, double deltaTime)
        {
            base.PostProcess(result, deltaTime);

            debugLog("Status: " + result.Status);
            debugLog("TimeFactor: " + result.TimeFactor);
            debugLog("deltaTime: " + deltaTime);

            if (result.Status.ToLower().Contains("missing") || result.Status.ToLower().Contains("full"))
            {
                StopResourceConverter();
                return;
            }
        }

        protected void updateLastLocation()
        {
            lastLocation.x = this.part.vessel.longitude;
            lastLocation.y = this.part.vessel.latitude;
            lastLocation.z = this.part.vessel.altitude;
        }

    }
}
