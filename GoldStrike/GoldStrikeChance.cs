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
    public class GoldStrikeChance
    {
        public int planetID;
        public string biome;
        public int chancesRemaining;
        public Vector3d lastProspectLocation = Vector3d.zero;

        public GoldStrikeChance()
        {
            chancesRemaining = GoldStrikeSettings.ProspectsPerBiome;
        }

        public bool IsAsteroid()
        {
            if (planetID == int.MaxValue)
                return true;
            else
                return false;
        }

        public double GetDistanceFromLastLocation(double longitude, double latitude, double altitude)
        {
            //If the prospect was on an asteroid then we're done.
            if (IsAsteroid())
                return 0f;

            //If we've never set a prospect location then we're automatically ok.
            if (lastProspectLocation == Vector2d.zero)
                return double.MaxValue;

            double distance = GoldStrikeUtils.HaversineDistance(lastProspectLocation.x, lastProspectLocation.y,
                longitude, latitude, FlightGlobals.Bodies[planetID]);

            return distance;
        }

        public void Load(ConfigNode node)
        {
            try
            {
                if (node.HasValue("planetID"))
                    planetID = int.Parse(node.GetValue("planetID"));

                if (node.HasValue("biome"))
                    biome = node.GetValue("biome");

                if (node.HasValue("chancesRemaining"))
                    chancesRemaining = int.Parse(node.GetValue("chancesRemaining"));

                if (node.HasValue("lastProspectLongitude"))
                    lastProspectLocation.x = double.Parse(node.GetValue("lastProspectLongitude"));

                if (node.HasValue("lastProspectLatitude"))
                    lastProspectLocation.y = double.Parse(node.GetValue("lastProspectLatitude"));

                if (node.HasValue("lastProspectAltitude"))
                    lastProspectLocation.z = double.Parse(node.GetValue("lastProspectAltitude"));
            }
            catch (Exception ex)
            {
                Debug.Log("[GoldStrikeChance] - encountered an exception during Load: " + ex);
            }
        }

        public ConfigNode Save()
        {
            ConfigNode node = new ConfigNode("GoldStrikeChance");

            node.AddValue("planetID", planetID);
            node.AddValue("biome", biome);
            node.AddValue("chancesRemaining", chancesRemaining);
            node.AddValue("lastProspectLongitude", lastProspectLocation.x);
            node.AddValue("lastProspectLatitude", lastProspectLocation.y);
            node.AddValue("lastProspectAltitude", lastProspectLocation.z);

            return node;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("planetID: " + planetID);
            sb.AppendLine("biome: " + biome);
            sb.AppendLine("chancesRemaining: " + chancesRemaining);
            sb.AppendLine(string.Format("Last Prospect Location (lon/lat/alt): {0:f2}/{1:f2}/{2:f2}", lastProspectLocation.x, lastProspectLocation.y, lastProspectLocation.z)); 

            return sb.ToString();
        }
    }
}
