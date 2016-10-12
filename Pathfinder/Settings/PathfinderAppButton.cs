using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

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
    [KSPAddon(KSPAddon.Startup.Flight | KSPAddon.Startup.EveryScene, false)]
    class PathfinderConfigMenu : MonoBehaviour
    {
        static protected Texture2D appIcon = null;
        static protected ApplicationLauncherButton appLauncherButton = null;
        static protected PathfinderAppView appView = new PathfinderAppView();
        static public LocalOpsManager localOpsManager = new LocalOpsManager();

        public void Awake()
        {
            appIcon = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/PathfinderApp", false);
            GameEvents.onGUIApplicationLauncherReady.Add(SetupGUI);
            appView.localOpsManager = localOpsManager;
        }

        public void OnGUI()
        {
            if (appView.IsVisible())
                appView.DrawWindow();
            if (appView.localOpsManager.IsVisible())
                appView.localOpsManager.DrawWindow();
        }

        private void SetupGUI()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                if (appLauncherButton == null)
                    appLauncherButton = ApplicationLauncher.Instance.AddModApplication(ToggleGUI, ToggleGUI, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, appIcon);
            }
            else if (appLauncherButton != null)
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
        }

        private void ToggleGUI()
        {
            appView.SetVisible(!appView.IsVisible());
        }

    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class WBIPathfinderSettingsMonitor : MonoBehaviour
    {
        public void Awake()
        {
            GameEvents.OnGameSettingsApplied.Add(UpdateSettings);
        }

        public void Destroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(UpdateSettings);
        }

        public void UpdateSettings()
        {
            WBITemplateConverter.payForReconfigure = WBIMainSettings.PayToReconfigure;
            WBITemplateConverter.checkForSkill = WBIMainSettings.RequiresSkillCheck;
            WBIBreakableResourceConverter.repairsRequireResources = WBIMainSettings.RepairsRequireResources;
        }
    }
}
