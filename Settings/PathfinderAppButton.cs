using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using WBIResources;
using WBIScience;

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
        static protected PathfinderAppView appView = null;
        static public LocalOpsManager localOpsManager = null;

        /// <summary>
        /// Registers the Pathfinder app launcher hook and prepares shared view state.
        /// The app launcher can be created before GameDatabase texture lookup is safe,
        /// so icon loading is isolated and guarded.
        /// </summary>
        public void Awake()
        {
            if (appView == null)
                appView = new PathfinderAppView();
            if (localOpsManager == null)
                localOpsManager = new LocalOpsManager();

            loadAppIcon();
            GameEvents.onGUIApplicationLauncherReady.Add(SetupGUI);
            appView.localOpsManager = localOpsManager;
        }

        /// <summary>
        /// Removes event hooks installed by Awake.
        /// </summary>
        public void OnDestroy()
        {
            GameEvents.onGUIApplicationLauncherReady.Remove(SetupGUI);
        }

        public void OnGUI()
        {
            if (appView.IsVisible())
                appView.DrawWindow();
            if (appView.localOpsManager.IsVisible())
                appView.localOpsManager.DrawWindow();
        }

        /// <summary>
        /// Adds or removes the Pathfinder application launcher button for supported scenes.
        /// </summary>
        private void SetupGUI()
        {
            if (HighLogic.LoadedScene == GameScenes.FLIGHT || HighLogic.LoadedScene == GameScenes.SPACECENTER)
            {
                if (appIcon == null)
                    loadAppIcon();
                if (appLauncherButton == null)
                    appLauncherButton = ApplicationLauncher.Instance.AddModApplication(ToggleGUI, ToggleGUI, null, null, null, null, ApplicationLauncher.AppScenes.ALWAYS, appIcon);
            }
            else if (appLauncherButton != null)
                ApplicationLauncher.Instance.RemoveModApplication(appLauncherButton);
        }

        /// <summary>
        /// Toggles the Pathfinder configuration window.
        /// </summary>
        private void ToggleGUI()
        {
            appView.SetVisible(!appView.IsVisible());
        }

        /// <summary>
        /// Safely loads the app launcher icon. Some heavily patched installs can call
        /// this addon before texture lookup is fully ready, so failures are logged and
        /// a harmless built-in texture is used until the icon can be resolved.
        /// </summary>
        private void loadAppIcon()
        {
            try
            {
                if (GameDatabase.Instance != null)
                    appIcon = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/PathfinderApp", false);
            }
            catch (Exception ex)
            {
                Debug.LogWarning("[Pathfinder] - Unable to load Pathfinder app icon: " + ex.Message);
            }

            if (appIcon == null)
                appIcon = Texture2D.whiteTexture;
        }

    }

    [KSPAddon(KSPAddon.Startup.EveryScene, false)]
    public class WBIPathfinderSettingsMonitor : MonoBehaviour
    {
        public void Awake()
        {
            GameEvents.OnGameSettingsApplied.Add(UpdateSettings);
        }

        public void OnDestroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(UpdateSettings);
        }

        public void UpdateSettings()
        {
        }
    }
}
