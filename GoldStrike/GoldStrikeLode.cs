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
    public enum ProspectSituations
    {
        Valid,
        NotEnoughDistance,
        OutOfChances,
        LodeAlreadyExists
    }

    public class GoldStrikeLode
    {
        public double longitude;
        public double lattitude;
        public int planetID;
        public string biome;
        public string resourceName;
        public double amountRemaining;
        public string navigationID;

        public void Load(ConfigNode node)
        {
            try
            {
                if (node.HasValue("resourceName"))
                    resourceName = node.GetValue("resourceName");

                if (node.HasValue("longitude"))
                    longitude = double.Parse(node.GetValue("longitude"));

                if (node.HasValue("lattitude"))
                    lattitude = double.Parse(node.GetValue("lattitude"));

                if (node.HasValue("planetID"))
                    planetID = int.Parse(node.GetValue("planetID"));

                if (node.HasValue("biome"))
                    biome = node.GetValue("biome");

                if (node.HasValue("navigationID"))
                    navigationID = node.GetValue("navigationID");

                if (node.HasValue("amountRemaining"))
                    amountRemaining = double.Parse(node.GetValue("amountRemaining"));
            }
            catch (Exception ex)
            {
                Debug.Log("[GoldStrikeLode] - encountered an exception during Load: " + ex);
            }
        }

        public ConfigNode Save()
        {
            ConfigNode node = new ConfigNode("GoldStrikeLode");

            node.AddValue("longitude", longitude);
            node.AddValue("lattitude", lattitude);
            node.AddValue("planetID", planetID);
            node.AddValue("biome", biome);
            if (string.IsNullOrEmpty(navigationID) == false)
                node.AddValue("navigationID", navigationID);
            node.AddValue("resourceName", resourceName);
            node.AddValue("amountRemaining", amountRemaining);

            return node;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("longitude: " + longitude);
            sb.AppendLine("lattitude: " + lattitude);
            sb.AppendLine("planetID: " + planetID);
            sb.AppendLine("biome: " + biome);
            if (string.IsNullOrEmpty(navigationID) == false)
                sb.AppendLine("navigationID: " + navigationID);
            sb.AppendLine("resourceName: " + resourceName);
            sb.AppendLine("amountRemaining: " + amountRemaining);

            return sb.ToString();
        }
    }
}
