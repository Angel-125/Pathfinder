using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

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
    public class GoldStrikeData
    {
        public string resourceName;
        public float minAmount;
        public float maxAmount;
        public float motherlodeMultiplier;
        public int anomalyChance;
        public float minProspectDistance;
        public string planetsFound;
        public string biomesFound;
        public float minAltitude;
        public float maxAltitude;
        public string resourceTypes;

        public GoldStrikeData()
        {
            minProspectDistance = 3.0f;
            planetsFound = "Any";
            biomesFound = "Any";
            resourceTypes = "Planetary";
            minAltitude = float.MinValue;
            maxAltitude = float.MaxValue;
        }

        public void Load(ConfigNode node)
        {
            try
            {
                if (node.HasValue("resourceName"))
                    resourceName = node.GetValue("resourceName");

                if (node.HasValue("minAmount"))
                    minAmount = float.Parse(node.GetValue("minAmount"));

                if (node.HasValue("maxAmount"))
                    maxAmount = float.Parse(node.GetValue("maxAmount"));

                if (node.HasValue("motherlodeMultiplier"))
                    motherlodeMultiplier = float.Parse(node.GetValue("motherlodeMultiplier"));

                if (node.HasValue("anomalyChance"))
                    anomalyChance = int.Parse(node.GetValue("anomalyChance"));

                if (node.HasValue("minProspectDistance"))
                    minProspectDistance = float.Parse(node.GetValue("minProspectDistance"));

                if (node.HasValue("minAltitude"))
                    minAltitude = float.Parse(node.GetValue("minAltitude"));

                if (node.HasValue("maxAltitude"))
                    maxAltitude = float.Parse(node.GetValue("maxAltitude"));

                if (node.HasValue("resourceTypes"))
                    resourceTypes = node.GetValue("resourceTypes");

                if (node.HasValue("planetsFound"))
                    planetsFound = node.GetValue("planetsFound");

                if (node.HasValue("biomesFound"))
                    biomesFound = node.GetValue("biomesFound");
            }
            catch (Exception ex)
            {
                Debug.Log("[GoldStrikeData] - encountered an exception during Load: " + ex);
            }
        }

        public ConfigNode Save()
        {
            ConfigNode node = new ConfigNode("GoldStrikeData");

            node.AddValue("resourceName", resourceName);
            node.AddValue("minAmount", minAmount);
            node.AddValue("maxAmount", maxAmount);
            node.AddValue("motherlodeMultiplier", motherlodeMultiplier);
            node.AddValue("anomalyChance", anomalyChance);
            node.AddValue("minProspectDistance", minProspectDistance);
            node.AddValue("minAltitude", minAltitude);
            node.AddValue("maxAltitude", maxAltitude);
            node.AddValue("resourceTypes", resourceTypes);
            node.AddValue("planetsFound", planetsFound);
            node.AddValue("biomesFound", biomesFound);

            return node;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("resourceName: " + resourceName);
            sb.AppendLine("resourceTypes: " + resourceTypes);
            sb.AppendLine("minProspectDistance: " + minProspectDistance);
            sb.AppendLine("planetsFound: " + planetsFound);
            sb.AppendLine("biomesFound: " + biomesFound);
            sb.AppendLine("minAltitude: " + minAltitude);
            sb.AppendLine("maxAltitude: " + maxAltitude);
            sb.AppendLine("minAmount: " + minAmount);
            sb.AppendLine("maxAmount: " + maxAmount);
            sb.AppendLine("motherlodeMultiplier: " + motherlodeMultiplier);
            sb.AppendLine("anomalyChance: " + anomalyChance);

            return sb.ToString();
        }

        public bool MatchesProspectCriteria(Vessel.Situations situation, string planetName, string biomeName, double altitude, double travelDistance)
        {
            //Check planet
            if (planetsFound != "Any")
            {
                if (!planetsFound.Contains(planetName))
                    return false;
            }

            //Check biome
            if (biomesFound != "Any")
            {
                if (!biomesFound.Contains(biomeName))
                    return false;
            }

            //Check situation
            if ((situation == Vessel.Situations.LANDED || situation == Vessel.Situations.PRELAUNCH) && !resourceTypes.Contains("Planetary"))
                return false;
            else if (situation == Vessel.Situations.SPLASHED && !resourceTypes.Contains("Oceanic"))
                return false;
            else if (situation == Vessel.Situations.FLYING && !resourceTypes.Contains("Atmospheric"))
                return false;
            else if ((situation == Vessel.Situations.ORBITING || situation == Vessel.Situations.SUB_ORBITAL) && !resourceTypes.Contains("Exospheric"))
                return false;

            //Check altitude
            if (altitude > maxAltitude)
                return false;
            else if (altitude < minAltitude)
                return false;

            //Check travel distance
            if (travelDistance < minProspectDistance)
                return false;
            return true;
        }
    }
}
