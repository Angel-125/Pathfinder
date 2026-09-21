using System;
using System.Collections.Generic;
using System.Globalization;
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
    [KSPScenario(ScenarioCreationOptions.AddToAllGames, GameScenes.SPACECENTER, GameScenes.EDITOR, GameScenes.FLIGHT, GameScenes.TRACKSTATION)]
    public class WBIManifestScenario : ScenarioModule
    {
        public static WBIManifestScenario Instance;

        public List<ConfigNode> manifestNodes = new List<ConfigNode>();

        #region Housekeeping
        protected void Log(string message)
        {
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIManifestScenario] - " + message);
            }
        }

        public override void OnAwake()
        {
            base.OnAwake();
            Instance = this;
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            manifestNodes.Clear();
            ConfigNode[] manifests = node.GetNodes(WBIManifest.kManifest);
            foreach (ConfigNode manifestNode in manifests)
            {
                double creationDate;
                double deliveryTime;
                if (!string.IsNullOrEmpty(manifestNode.GetValue(WBIManifest.kDestinationID)) &&
                    !string.IsNullOrEmpty(manifestNode.GetValue(WBIManifest.kManifestType)) &&
                    tryParseDouble(manifestNode.GetValue(WBIManifest.kCreationDate), out creationDate) &&
                    tryParseDouble(manifestNode.GetValue(WBIManifest.kDeliveryTime), out deliveryTime))
                    manifestNodes.Add(manifestNode);
                else
                    Log("Discarding malformed manifest while loading the save.");
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            foreach (ConfigNode manifestNode in manifestNodes)
                node.AddNode(manifestNode);
        }
        #endregion

        #region API
        public bool AddManifest(WBIManifest manifest)
        {
            if (manifest == null || string.IsNullOrEmpty(manifest.destinationID) || manifest.manifestType == WBIManifest.kNoType)
                return false;

            //Get the config node
            ConfigNode manifestNode = new ConfigNode(WBIManifest.kManifest);
            manifest.Save(manifestNode);

            //Add the config node to the list.
            manifestNodes.Add(manifestNode);
            return true;
        }

        public bool RemoveManifest(ConfigNode manifestNode)
        {
            return manifestNode != null && manifestNodes.Remove(manifestNode);
        }

        public List<ConfigNode> GetManifestConfigs(string destinationID, string manifestType, bool removeFromList = true)
        {
            List<ConfigNode> manifestConfigs = new List<ConfigNode>();

            //Find all the manifests that match the desired destination and type
            double creationDate;
            double deliveryTime;
            Log("GetManifestConfigs: manifestNodes count: " + manifestNodes.Count);
            foreach (ConfigNode manifestNode in manifestNodes)
            {
                //See if the manifest config matches what we're looking for.
                if (manifestNode.GetValue(WBIManifest.kDestinationID) == destinationID && manifestNode.GetValue(WBIManifest.kManifestType) == manifestType)
                {
                    //Ok, we found a match. Has it completed its flight time?
                    //Get the creation date and delivery time
                    if (!tryParseDouble(manifestNode.GetValue(WBIManifest.kCreationDate), out creationDate) ||
                        !tryParseDouble(manifestNode.GetValue(WBIManifest.kDeliveryTime), out deliveryTime))
                    {
                        Log("Skipping malformed manifest for " + destinationID);
                        continue;
                    }

                    //If there's no delivery time, then we're done.
                    if (deliveryTime < 0.00001)
                        manifestConfigs.Add(manifestNode);

                    //Get elapsed time. If we've met or exceeded the elapsed time then deliver the package.
                    else if (Planetarium.GetUniversalTime() >= (creationDate + deliveryTime))
                        manifestConfigs.Add(manifestNode);
                }
            }

            //Remove the located items if desired
            if (removeFromList)
            {
                foreach (ConfigNode doomed in manifestConfigs)
                    manifestNodes.Remove(doomed);
            }

            return manifestConfigs;
        }

        bool tryParseDouble(string value, out double result)
        {
            if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out result))
                return true;
            return double.TryParse(value, out result);
        }
        #endregion
    }
}
