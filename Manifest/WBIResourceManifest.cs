using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;
using KSP.UI.Screens;
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
    /// <summary>
    /// This class is designed to shuttle resources between vessels.
    /// </summary>
    public class WBIResourceManifest : WBIManifest
    {
        public const string kResourceNode = "RESOURCE";
        public const string kResourceName = "resourceName";
        public const string kAmount = "amount";
        public const string kResourceManifestType = "WBIResourceManifest";

        public Dictionary<string, double> resourceAmounts = new Dictionary<string, double>();

        public static List<WBIResourceManifest> GetManifestsForDestination(string destinationID)
        {
            List<WBIResourceManifest> manifestList = new List<WBIResourceManifest>();

            //Find all the resource manifests that match the desired destination.
            WBIResourceManifest resourceManifest;
            List<ConfigNode> nodes = WBIManifestScenario.Instance.GetManifestConfigs(destinationID, WBIResourceManifest.kResourceManifestType);
            foreach (ConfigNode node in nodes)
            {
                resourceManifest = new WBIResourceManifest();
                resourceManifest.Load(node);
                manifestList.Add(resourceManifest);
            }

            return manifestList;
        }

        public WBIResourceManifest()
        {
            manifestType = kResourceManifestType;
        }

        public override void Load(ConfigNode node)
        {
            base.Load(node);

            resourceAmounts.Clear();
            ConfigNode[] resources = node.GetNodes(kResourceNode);
            string resourceName;
            double amount;
            foreach (ConfigNode resource in resources)
            {
                resourceName = resource.GetValue(kResourceName);
                amount = double.Parse(resource.GetValue(kAmount));
                resourceAmounts.Add(resourceName, amount);
            }
        }

        public override void Save(ConfigNode node)
        {
            base.Save(node);

            //Load up the manifest items
            string[] resourceNames = resourceAmounts.Keys.ToArray();
            ConfigNode resourceNode;
            foreach (string resourceName in resourceNames)
            {
                resourceNode = new ConfigNode(kResourceNode);
                resourceNode.AddValue(kResourceName, resourceName);
                resourceNode.AddValue(kAmount, resourceAmounts[resourceName]);
                node.AddNode(resourceNode);
            }
        }
    }
}
