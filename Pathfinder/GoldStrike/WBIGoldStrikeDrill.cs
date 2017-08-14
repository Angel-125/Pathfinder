using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;
using KSP.Localization;

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
    public class WBIGoldStrikeDrill : ModuleBreakableHarvester
    {
        private const float kMessageDisplayTime = 10.0f;

        [KSPField()]
        public string depletedMessage = "This vein of {0:s} has been depleted. Time to move on...";

        [KSPField()]
        public string statusOKName = "A-OK";

        [KSPField()]
        public string statusFullName = "Full";

        [KSPField()]
        public string statusDepletedName = "Depleted";

        [KSPField()]
        public string statusNoNearbyName = "None Nearby";

        [KSPField()]
        public string statusNAName = "N/A";

        [KSPField(guiName = "Lode", guiActive = true)]
        public string lodeStatus = "N/A";

        [KSPField(guiName = "Lode Resource", guiActive = true)]
        public string lodeResourceName = "N/A";

        [KSPField(guiName = "Lode Units Remaining", guiFormat = "f2", guiActive = true)]
        public double lodeUnitsRemaining;

        [KSPField()]
        public float maxHarvestRange = 200.0f;

        protected GoldStrikeLode nearestLode = null;
        protected Vector3d lastLocation = Vector3d.zero;
        protected double outputUnits = 0f;
        protected PartResourceDefinition outputDef;

        public override void StartConverter()
        {
            //Update the output units
            UpdateLode();

            base.StartConverter();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Make sure our lode is up to date
            UpdateLode();
        }

        public bool UpdateLode()
        {
            if (HighLogic.LoadedSceneIsFlight == false)
                return false;

            //If we aren't landed then we're done.
            if (this.part.vessel.situation != Vessel.Situations.PRELAUNCH && this.part.vessel.situation != Vessel.Situations.LANDED)
            {
                //Inform player
                lodeStatus = Localizer.Format(statusNoNearbyName);
                return false;
            }

            //Find the nearest node
            findNearestLode();
            if (nearestLode == null)
            {
                //Inform player
                lodeStatus = Localizer.Format(statusNoNearbyName);
                return false;
            }

            //Check units remaining
            if (nearestLode.amountRemaining == 0)
            {
                //Inform player
                debugLog("Converter stopped. Lode is depleted.");
                ScreenMessages.PostScreenMessage(string.Format(depletedMessage, nearestLode.resourceName), kMessageDisplayTime, ScreenMessageStyle.UPPER_CENTER);
                return false;
            }

            //Ok, we've got a valid lode, we're in harvest range, there are units remaining, it's dark, and we're wearing sunglasses.
            //Hit it!
            //Find the resource definition for the output resource
            outputDef = ResourceHelper.DefinitionForResource(nearestLode.resourceName);
            if (outputDef == null)
            {
                debugLog("No definition for " + nearestLode.resourceName);
                StopResourceConverter();
                return false;
            }

            //Output units per second is the ratio of the input to output density, multiplied by the drill's flow rate.
            outputUnits = outputDef.density * Efficiency * EfficiencyBonus;

            return true;
        }

        protected void findNearestLode()
        {
            int planetID;
            string biomeName;
            double longitude = 0f;
            double latitude = 0f;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                longitude = this.part.vessel.longitude;
                latitude = this.part.vessel.latitude;

                //Find the nearest lode (if any)
                GoldStrikeUtils.GetBiomeAndPlanet(out biomeName, out planetID, this.part.vessel);
                nearestLode = WBIPathfinderScenario.Instance.FindNearestLode(planetID, biomeName, longitude, latitude, maxHarvestRange);

                if (nearestLode != null)
                {
                    lodeStatus = Localizer.Format(statusOKName);
                    lodeResourceName = nearestLode.resourceName;
                    debugLog("nearestLode: " + nearestLode.ToString());
                }
                else
                {
                    lodeStatus = Localizer.Format(statusNoNearbyName);
                    lodeResourceName = "N/A";
                    debugLog("No lode found nearby.");
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //Only need to do the stuff below if we're in flight.
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //Make sure our conditions are met.
            if (this.part.vessel.situation != Vessel.Situations.PRELAUNCH && this.part.vessel.situation != Vessel.Situations.LANDED)
                return;

            //If we've traveled too far then trigger a node search.
            double travelDistance = GoldStrikeUtils.HaversineDistance(this.part.vessel.longitude, this.part.vessel.latitude,
                lastLocation.x, lastLocation.y, this.part.vessel.mainBody);
            if (travelDistance > maxHarvestRange)
            {
                findNearestLode();
                updateLastLocation();
            }

            //Update lode units remaining
            if (nearestLode != null)
                lodeUnitsRemaining = nearestLode.amountRemaining;
        }

        protected void updateLastLocation()
        {
            lastLocation.x = this.part.vessel.longitude;
            lastLocation.y = this.part.vessel.latitude;
            lastLocation.z = this.part.vessel.altitude;
        }

        protected override void PostProcess(ConverterResults result, double deltaTime)
        {
            base.PostProcess(result, deltaTime);
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
            if (IsActivated == false)
            {
                lodeStatus = Localizer.Format(statusNAName);
                return;
            }
            if (this.part.vessel.situation != Vessel.Situations.PRELAUNCH && this.part.vessel.situation != Vessel.Situations.LANDED)
            {
                lodeStatus = Localizer.Format(statusNAName);
                return;
            }
            if (isBroken)
            {
                StopResourceConverter();
                return;
            }
            
            //Make sure we have some stuff remaining.
            if (nearestLode == null)
            {
                lodeStatus = Localizer.Format(statusNoNearbyName);
                return;
            }
            if (nearestLode.amountRemaining == 0)
            {
                lodeStatus = Localizer.Format(statusDepletedName);
                return;
            }

            //Get the current and max available space for the lode resource
            double currentAmount;
            double maxAmount;
            this.part.vessel.resourcePartSet.GetConnectedResourceTotals(outputDef.id, out currentAmount, out maxAmount, true);

            //If we have no space then we're done.
            if (maxAmount < 0.0001)
            {
                lodeStatus = Localizer.Format(statusFullName);
                return;
            }
            if (currentAmount / maxAmount > 0.999999)
            {
                lodeStatus = Localizer.Format(statusFullName);
                return;
            }

            //Ok, we have some room. Calculate how much to request from the lode.
            double requestAmount = outputUnits * deltaTime; // this.lastTimeFactor;

            //Adjust to the drill's resource flow rate if it exists.
            if (_resFlow >= 0f)
                requestAmount = _resFlow * deltaTime;

            //Make sure we don't pull more than we need.
            double maxRequestAmount = maxAmount - currentAmount;
            if (requestAmount > maxRequestAmount)
                requestAmount = maxRequestAmount;

            //Make sure the lode has enough
            if (nearestLode.amountRemaining < requestAmount)
                requestAmount = nearestLode.amountRemaining;

            //Now we can do our business. Add the resource to the vessel.
            double amountObtained = Math.Abs(this.part.RequestResource(outputDef.id, -requestAmount));

            //Update the lode.
            nearestLode.amountRemaining -= amountObtained;
            if (nearestLode.amountRemaining < 0.0001)
                nearestLode.amountRemaining = 0f;

            //Status update
            lodeStatus = Localizer.Format(statusOKName);
        }
    }
}
