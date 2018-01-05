using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2015, by Michael Billard (Angel-125)
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
    public class EfficiencyData
    {
        static public string kExtractionMod = "extractionModifier";
        static public string kIndustryMod = "industry";
        static public string kHabitationMod = "habitation";
        static public string kScienceMod = "science";

        public int planetID;
        public string biomeName;
        public HarvestTypes harvestType;
        public int attemptsRemaining;
        public Dictionary<string, float> modifiers = new Dictionary<string, float>();

        public void Load(ConfigNode node)
        {
            string value;
            string modifierName;

            value = node.GetValue("planetID");
            if (string.IsNullOrEmpty(value) == false)
                planetID = int.Parse(value);

            biomeName = node.GetValue("biomeName");

            value = node.GetValue("harvestType");
            if (string.IsNullOrEmpty(value) == false)
                harvestType = (HarvestTypes)int.Parse(value);

            value = node.GetValue("attemptsRemaining");
            if (string.IsNullOrEmpty(value) == false)
                attemptsRemaining = int.Parse(value);

            //Modifiers
            modifiers.Clear();
            ConfigNode[] modifierNodes = node.GetNodes("MODIFIER");
            if (modifierNodes != null)
            {
                foreach (ConfigNode modNode in modifierNodes)
                {
                    modifierName = modNode.GetValue("name");
                    value = modNode.GetValue("value");
                    modifiers.Add(modifierName, float.Parse(value));
                }
            }

            //Backwards compatibility
            value = node.GetValue("efficiencyModifier");
            if (string.IsNullOrEmpty(value) == false)
                modifiers.Add("extractionModifier", float.Parse(value));
        }

        public void Save(ConfigNode node)
        {
            node.AddValue("planetID", planetID.ToString());

            if (!string.IsNullOrEmpty(biomeName))
                node.AddValue("biomeName", biomeName);

            int harvestValue = (int)harvestType;
            node.AddValue("harvestType", harvestValue.ToString());

            node.AddValue("attemptsRemaining", attemptsRemaining.ToString());

            //Modifiers
            foreach (string key in modifiers.Keys)
            {
                ConfigNode modifierNode = new ConfigNode("MODIFIER");
                modifierNode.name = "MODIFIER";
                modifierNode.AddValue("name", key);
                modifierNode.AddValue("value", modifiers[key]);
                node.AddNode(modifierNode);
            }
        }

        public string Key
        {
            get
            {
                string key = planetID.ToString() + biomeName + harvestType.ToString();

                return key;
            }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            builder.Append(base.ToString() + "\r\n");
            builder.Append("planetID: " + planetID + "\r\n");
            builder.Append("biomeName: " + biomeName + "\r\n");
            builder.Append("harvestType: " + harvestType + "\r\n");
            foreach (string key in modifiers.Keys)
                builder.Append(key + string.Format(": {0:f2}", modifiers[key]) + "\r\n");
            builder.Append("attemptsRemaining: " + attemptsRemaining + "\r\n");

            return builder.ToString();
        }

    }
}
