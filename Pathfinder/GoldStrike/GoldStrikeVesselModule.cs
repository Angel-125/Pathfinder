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
    public class GoldStrikeVesselModule : VesselModule
    {
        public int lastProspectPlanet = -1;
        public string lastProspectBiome = string.Empty;
        public double lastProspectLongitude;
        public double lastProspectLatitude;
        public double lastProspectAltitude;
        public bool hasLastProspectLocation = false;

        protected void debugLog(string message)
        {
            if (WBIPathfinderScenario.showDebugLog == true)
                Debug.Log("[WBIGoldStrike] - " + message);
        }

        protected override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasValue("lastProspectLongitude"))
                lastProspectLongitude = double.Parse(node.GetValue("lastProspectLongitude"));
            if (node.HasValue("lastProspectLatitude"))
                lastProspectLatitude = double.Parse(node.GetValue("lastProspectLatitude"));
            if (node.HasValue("lastProspectAltitude"))
                lastProspectAltitude = double.Parse(node.GetValue("lastProspectAltitude"));
            if (node.HasValue("lastProspectPlanet"))
                lastProspectPlanet = int.Parse(node.GetValue("lastProspectPlanet"));
            if (node.HasValue("lastProspectBiome"))
                lastProspectBiome = node.GetValue("lastProspectBiome");

            //Set last prospect location flag.
            hasLastProspectLocation = true;
       }

        protected override void OnSave(ConfigNode node)
        {
            base.OnSave(node);
            node.AddValue("lastProspectLongitude", lastProspectLongitude);
            node.AddValue("lastProspectLatitude", lastProspectLatitude);
            node.AddValue("lastProspectAltitude", lastProspectAltitude);
            node.AddValue("lastProspectPlanet", lastProspectPlanet);
            node.AddValue("lastProspectBiome", lastProspectBiome);
        }

        public void UpdateLastProspectLocation()
        {
            ModuleAsteroid asteroid = this.vessel.FindPartModuleImplementing<ModuleAsteroid>();
            GoldStrikeUtils.GetBiomeAndPlanet(out lastProspectBiome, out lastProspectPlanet, this.vessel, asteroid);

            //If we don't have an asteroid then we care about where we are.
            if (asteroid == null)
            {
                lastProspectLongitude = this.vessel.longitude;
                lastProspectLatitude = this.vessel.latitude;
                lastProspectAltitude = this.vessel.altitude;
            }

            //If we have an asteroid then we don't care about our location because we'll be prospecting the asteroid.
            else
            {
                lastProspectLongitude = 0;
                lastProspectLatitude = 0;
                lastProspectAltitude = 0;
            }

            debugLog("Last lode location: " + lastProspectPlanet +
                " " + lastProspectBiome +
                " lon: " + lastProspectLongitude +
                " lat: " + lastProspectLatitude +
                " altitude: " + lastProspectAltitude);

            //Set last prospect location flag.
            hasLastProspectLocation = true;
        }

        public bool HasLastProspectLocation()
        {
            return hasLastProspectLocation;
        }
    }
}
