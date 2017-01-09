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

            return node;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("resourceName: " + resourceName);
            sb.AppendLine("minAmount: " + minAmount);
            sb.AppendLine("maxAmount: " + maxAmount);
            sb.AppendLine("motherlodeMultiplier: " + motherlodeMultiplier);

            return sb.ToString();
        }
    }
}
