using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using KSP.IO;
using ModuleWheels;
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
    public class PathfinderAppViewAppHelper : MonoBehaviour
    {
        public PathfinderAppView settingsWindow = new PathfinderAppView();
        public LocalOpsManager localOpsManager = new LocalOpsManager();

        public void Awake()
        {
            settingsWindow.localOpsManager = localOpsManager;
        }
    }

    public class PathfinderAppView : Dialog<PathfinderAppView>
    {
        public const string kDefaultDrillTechNode = "advConstruction";

        public static string drillTechNode;
        public static double secondsBetweenDistribution;

        public LocalOpsManager localOpsManager;
        string settingsPath;
        string playModeName;
        string playModePath;
        string[] configOptions = { "Resources", "Cheats" };
        int configIndex;
        string distributionSeconds;
        GUIStyle normalStyle;
        GUIStyle redStyle;
        bool validSeconds;
        public bool debugMode;

        public PathfinderAppView() :
        base("Pathfinder Settings", 320, 100)
        {
            Resizable = false;
            settingsPath = AssemblyLoader.loadedAssemblies.GetPathByType(typeof(PathfinderAppView)) + "/Settings.cfg";
            loadSettings();

            normalStyle = HighLogic.Skin.textField;

            redStyle = new GUIStyle(normalStyle);
            redStyle.normal.textColor = Color.red;
            redStyle.focused.textColor = Color.red;
        }

        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            //In flight, switch between resources and settings
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (WBIGoldStrikeScenario.debugGoldStrike)
                {
                    if (GUILayout.Button("Clear Prospects"))
                    {
                        WBIGoldStrikeScenario.Instance.ClearProspects();
                    }
                }

                if (GUILayout.Button("Manage Operations"))
                    localOpsManager.SetVisible(true);

                configIndex = GUILayout.SelectionGrid(configIndex, configOptions, 2);

                switch (configIndex)
                {
                    case 0: //Resources
                        drawDistributionGUI();
                        break;

                    case 1: //Settings
                        drawCheatsGUI();
                        break;
                }
            }

            GUILayout.EndVertical();
        }

        protected void drawDistributionGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Resource Distribution: ");
            validSeconds = double.TryParse(distributionSeconds, out secondsBetweenDistribution);
            if (validSeconds)
                distributionSeconds = GUILayout.TextField(distributionSeconds, normalStyle);
            else
                distributionSeconds = GUILayout.TextField(distributionSeconds, redStyle);
            GUILayout.Label("seconds");
            GUILayout.EndHorizontal();

            if (GUILayout.Button("Distribute Resources"))
                WBIDistributionManager.Instance.DistributeResourcesImmediately();

            if (GUILayout.Button("Active Vessel: Opt-In (Distributor)"))
                WBIDistributionManager.Instance.OptInActiveVessel();

            if (GUILayout.Button("Active Vessel: Opt-In (Consumer)"))
                WBIDistributionManager.Instance.OptInConsumerActiveVessel();

            if (GUILayout.Button("Active Vessel: Opt-Out"))
                WBIDistributionManager.Instance.OptOutActiveVessel();
        }

        protected void drawCheatsGUI()
        {
            if (GUILayout.Button("Top Off Resources"))
                maxAllResources();
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
                loadSettings();

            else
                saveSettings();
        }

        protected void maxAllResources()
        {
            Vessel activeVessel = FlightGlobals.ActiveVessel;
            List<WBIResourceSwitcher> switchers = activeVessel.FindPartModulesImplementing<WBIResourceSwitcher>();

            foreach (WBIResourceSwitcher switcher in switchers)
                switcher.MaxResources();
        }

        protected void saveSettings()
        {
            ConfigNode nodeSettings = new ConfigNode();

            nodeSettings.name = "SETTINGS";
            nodeSettings.AddValue("drillTechNode", drillTechNode.ToString());
            if (string.IsNullOrEmpty("playModePath") == false)
                nodeSettings.AddValue("playModePath", playModePath);
            if (string.IsNullOrEmpty("playModeName") == false)
                nodeSettings.AddValue("playModeName", playModeName);
            nodeSettings.AddValue("secondsBetweenDistribution", secondsBetweenDistribution.ToString());
            nodeSettings.Save(settingsPath);

            WBIDistributionManager.secondsPerCycle = secondsBetweenDistribution;
        }

        protected void loadSettings()
        {
            ConfigNode nodeSettings = new ConfigNode();

            //Now load settings
            nodeSettings = ConfigNode.Load(settingsPath);
            if (nodeSettings != null)
            {
                drillTechNode = nodeSettings.GetValue("drillTechNode");

                if (nodeSettings.HasValue("playModePath"))
                    playModePath = nodeSettings.GetValue("playModePath");
                if (nodeSettings.HasValue("playModeName"))
                    playModeName = nodeSettings.GetValue("playModeName");

                if (nodeSettings.HasValue("secondsBetweenDistribution"))
                {
                    secondsBetweenDistribution = float.Parse(nodeSettings.GetValue("secondsBetweenDistribution"));
                    int seconds = (int)secondsBetweenDistribution;
                    distributionSeconds = seconds.ToString();
                }

                else
                {
                    secondsBetweenDistribution = 10f;
                    distributionSeconds = "10";
                }
            }
            else
            {
                drillTechNode = kDefaultDrillTechNode;
            }

            if (string.IsNullOrEmpty(drillTechNode))
                drillTechNode = kDefaultDrillTechNode;
        }

    }
}
