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
    //Asteroid drills work differently than planetary drills.
    //They will drill for every resource that an asteroid has.
    public class WBIGoldStrikeAsteroidDrill : ModuleBreakableAsteroidDrill
    {
        private const float kMessageDisplayTime = 10.0f;

        [KSPField()]
        public string statusOKName = "A-OK";

        [KSPField()]
        public string statusNoNearbyName = "None Nearby";

        [KSPField()]
        public string statusAlreadyProspectedName = "Already prospected";

        [KSPField()]
        public string statusNAName = "N/A";

        [KSPField(guiName = "Lode", guiActive = true)]
        public string lodeStatus = "N/A";

        [KSPField(guiName = "Lode Resource", guiActive = true)]
        public string lodeResourceName = "N/A";

        [KSPField(guiName = "Lode abundance", guiFormat = "f2", guiActive = true, guiUnits = "%")]
        public double lodeAbundance;

        protected GoldStrikeLode nearestLode = null;
        protected ModuleAsteroid asteroid;

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

            //Find the nearest node
            findNearestLode();
            if (nearestLode == null)
            {
                //Inform player
                lodeStatus = Localizer.Format(statusNoNearbyName);
                return false;
            }

            //Get abundance
            lodeAbundance = nearestLode.abundance;

            return true;
        }

        protected void findNearestLode()
        {
            //Do we have an asteroid?
            asteroid = this.part.vessel.FindPartModuleImplementing<ModuleAsteroid>();
            if (asteroid == null)
            {
                nearestLode = null;
                lodeStatus = Localizer.Format(statusNoNearbyName);
                lodeResourceName = "N/A";
                debugLog("No lode found nearby because there's no captured asteroid.");
                return;
            }

            //Find the nearest lode (if any)
            debugLog("Looking for a prospect lode for asteroid " + asteroid.AsteroidName);
            nearestLode = WBIPathfinderScenario.Instance.FindNearestLode(asteroid);

            if (nearestLode != null)
            {
                lodeStatus = Localizer.Format(statusOKName);
                lodeResourceName = nearestLode.resourceName;
                lodeAbundance = nearestLode.abundance * 100.0f;
                debugLog("nearestLode: " + nearestLode.ToString());
            }
            else
            {
                lodeStatus = Localizer.Format(statusNoNearbyName);
                lodeResourceName = "N/A";
                debugLog("No lode found nearby.");
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //Only need to do the stuff below if we're in flight.
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //Make sure our situation is met
            if (asteroid == null)
            {
                lodeStatus = Localizer.Format(statusNAName);
            }

            //Update lode units remaining
            else if (nearestLode != null)
            {
                lodeStatus = Localizer.Format(statusOKName);
                lodeAbundance = nearestLode.abundance * 100.0f;
            }

            //Has asteroid been prospected?
            else if (WBIPathfinderScenario.Instance.IsAsteroidProspected(asteroid))
            {
                lodeStatus = Localizer.Format(statusAlreadyProspectedName);
            }

            //No nearby lode
            else
            {
                lodeStatus = Localizer.Format(statusNoNearbyName);
            }
        }
    }
}
