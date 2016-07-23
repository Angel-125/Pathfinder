using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    public class PathfinderSettingsAppHelper : MonoBehaviour
    {
        public PathfinderSettings settingsWindow = new PathfinderSettings();
        public LocalOpsManager localOpsManager = new LocalOpsManager();

        public void Awake()
        {
            settingsWindow.localOpsManager = localOpsManager;
        }

        public void OnGUI()
        {
            if (settingsWindow.IsVisible())
                settingsWindow.DrawWindow();
            if (settingsWindow.localOpsManager.IsVisible())
                settingsWindow.localOpsManager.DrawWindow();
        }
    }

    public class PathfinderSettings : Window<PathfinderSettings>
    {
        public const string kDefaultDrillTechNode = "specializedConstruction";

        public static string drillTechNode;
        public static bool payToRemodel;
        public static bool requireSkillCheck;
        public static bool repairsRequireResources;
        public static bool partsCanBreak;
        public static double secondsBetweenDistribution;

        public LocalOpsManager localOpsManager;
        string settingsPath;
        string playModeName;
        string playModePath;
        PlayModesWindow playModesView = new PlayModesWindow();
        string[] configOptions = { "Resources", "Settings" };
        int configIndex;
        string distributionSeconds;
        GUIStyle normalStyle;
        GUIStyle redStyle;
        bool validSeconds;

        public PathfinderSettings() :
        base("Pathfinder Settings", 320, 100)
        {
            Resizable = false;
            playModesView.changePlayModeDelegate = changePlayMode;

            settingsPath = AssemblyLoader.loadedAssemblies.GetPathByType(typeof(PathfinderSettings)) + "/Settings.cfg";
            loadSettings();

            normalStyle = HighLogic.Skin.textField;

            redStyle = new GUIStyle(normalStyle);
            redStyle.normal.textColor = Color.red;
            redStyle.focused.textColor = Color.red;
            HideCloseButton = true;
        }

        public void changePlayMode()
        {
            payToRemodel = playModesView.payToRemodel;
            requireSkillCheck = playModesView.requireSkillCheck;
            repairsRequireResources = playModesView.repairsRequireResources;
            partsCanBreak = playModesView.partsCanBreak;
            playModeName = playModesView.currentPlayMode;
            playModePath = playModesView.currentPlayModeFile;

            saveSettings();
        }

        public override void DrawWindow()
        {
            base.DrawWindow();
            if (playModesView.IsVisible())
                playModesView.DrawWindow();
        }

        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            //At the space center, draw the play modes
            if (HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                GUILayout.BeginHorizontal();
                if (string.IsNullOrEmpty(playModeName))
                    GUILayout.Label("Play Mode: Default");
                else
                    GUILayout.Label("Play Mode: " + playModeName);
                if (GUILayout.Button("Change...", new GUILayoutOption[] { GUILayout.Width(64) }))
                    playModesView.SetVisible(true);
                GUILayout.EndHorizontal();
            }

            //In flight, switch between resources and settings
            if (HighLogic.LoadedSceneIsFlight)
            {
                if (GUILayout.Button("Manage Operations"))
                    localOpsManager.SetVisible(true);

                configIndex = GUILayout.SelectionGrid(configIndex, configOptions, 2);

                switch (configIndex)
                {
                    case 0: //Resources
                        drawDistributionGUI();
                        break;

                    case 1: //Settings
                        drawSettingsGUI();
                        break;
                }
            }

            //Draw settings panel
            else
            {
                drawSettingsGUI();
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
                WBIDistributionManager.Instance.DistributeResources();

            if (GUILayout.Button("Active Vessel: Opt-In (Distributor)"))
                WBIDistributionManager.Instance.OptInActiveVessel();

            if (GUILayout.Button("Active Vessel: Opt-In (Consumer)"))
                WBIDistributionManager.Instance.OptInConsumerActiveVessel();

            if (GUILayout.Button("Active Vessel: Opt-Out"))
                WBIDistributionManager.Instance.OptOutActiveVessel();
        }

        protected void drawSettingsGUI()
        {
            payToRemodel = GUILayout.Toggle(payToRemodel, "Require resources to reconfigure modules.");
            requireSkillCheck = GUILayout.Toggle(requireSkillCheck, "Require skill check to reconfigure modules.");
            GUILayout.FlexibleSpace();
            repairsRequireResources = GUILayout.Toggle(repairsRequireResources, "Repairs require resources.");
            partsCanBreak = GUILayout.Toggle(partsCanBreak, "Parts can break.");

            WBIAffordableSwitcher.payForReconfigure = payToRemodel;
            WBIAffordableSwitcher.checkForSkill = requireSkillCheck;
            WBITemplateConverter.payForReconfigure = payToRemodel;
            WBITemplateConverter.checkForSkill = requireSkillCheck;
            WBIBreakableResourceConverter.repairsRequireResources = repairsRequireResources;
            WBIResourceConverter.repairsRequireResources = repairsRequireResources;
            WBIResourceConverter.partsCanBreak = partsCanBreak;
            WBIResourceConverter.requireSkillCheck = requireSkillCheck;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
                loadSettings();

            else
                saveSettings();
        }

        protected void saveSettings()
        {
            ConfigNode nodeSettings = new ConfigNode();

            nodeSettings.name = "SETTINGS";
            nodeSettings.AddValue("payToRemodel", payToRemodel.ToString());
            nodeSettings.AddValue("requireSkillCheck", requireSkillCheck.ToString());
            nodeSettings.AddValue("repairsRequireResources", repairsRequireResources.ToString());
            nodeSettings.AddValue("partsCanBreak;", partsCanBreak.ToString());
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
            string value;

            //Now load settings
            nodeSettings = ConfigNode.Load(settingsPath);
            if (nodeSettings != null)
            {
                value = nodeSettings.GetValue("payToRemodel");
                if (string.IsNullOrEmpty(value) == false)
                    payToRemodel = bool.Parse(value);
                else
                    payToRemodel = WBIAffordableSwitcher.payForReconfigure;

                value = nodeSettings.GetValue("requireSkillCheck");
                if (string.IsNullOrEmpty(value) == false)
                    requireSkillCheck = bool.Parse(value);
                else
                    requireSkillCheck = WBIAffordableSwitcher.checkForSkill;

                value = nodeSettings.GetValue("repairsRequireResources");
                if (string.IsNullOrEmpty(value) == false)
                    repairsRequireResources = bool.Parse(value);
                else
                    repairsRequireResources = GeoSurveyCamera.repairsRequireResources;

                value = nodeSettings.GetValue("partsCanBreak");
                if (string.IsNullOrEmpty(value) == false)
                    partsCanBreak = bool.Parse(value);
                else
                    partsCanBreak = WBIResourceConverter.partsCanBreak;

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
                payToRemodel = WBIAffordableSwitcher.payForReconfigure;
                requireSkillCheck = WBIAffordableSwitcher.checkForSkill;
                repairsRequireResources = GeoSurveyCamera.repairsRequireResources;
                partsCanBreak = WBIResourceConverter.partsCanBreak;
                drillTechNode = kDefaultDrillTechNode;
            }

            WBIAffordableSwitcher.payForReconfigure = payToRemodel;
            WBIAffordableSwitcher.checkForSkill = requireSkillCheck;
            WBITemplateConverter.payForReconfigure = payToRemodel;
            WBITemplateConverter.checkForSkill = requireSkillCheck;
            WBIDistributionManager.secondsPerCycle = secondsBetweenDistribution;
            WBIResourceConverter.repairsRequireResources = repairsRequireResources;
            WBIResourceConverter.partsCanBreak = partsCanBreak;
            WBIResourceConverter.requireSkillCheck = requireSkillCheck;

            if (string.IsNullOrEmpty(drillTechNode))
                drillTechNode = kDefaultDrillTechNode;
        }

    }
}
