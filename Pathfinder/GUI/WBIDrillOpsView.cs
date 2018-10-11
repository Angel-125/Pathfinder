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
    public class WBIDrillOpsView : PartModule, IOpsView
    {
        WBIGoldStrikeDrill harvester;
        WBIDrillSwitcher drillSwitcher;
        WBIEfficiencyMonitor efficiencyMonitor;
        ModuleOverheatDisplay overheatDisplay;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            harvester = this.part.FindModuleImplementing<WBIGoldStrikeDrill>();

            drillSwitcher = this.part.FindModuleImplementing<WBIDrillSwitcher>();
            if (drillSwitcher != null)
            {
                drillSwitcher.Events["ShowDrillSwitchWindow"].guiActive = false;
                drillSwitcher.Events["ShowDrillSwitchWindow"].guiActiveUnfocused = false;
            }

            efficiencyMonitor = this.part.FindModuleImplementing<WBIEfficiencyMonitor>();
            if (efficiencyMonitor != null)
                efficiencyMonitor.Fields["efficiencyDisplayString"].guiActive = false;

            overheatDisplay = this.part.FindModuleImplementing<ModuleOverheatDisplay>();
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetContextGUIVisible(bool isVisible)
        {
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Drilling");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            if (harvester == null)
                return;

            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });

            //Status
            GUILayout.Label("<color=white>Status: " + harvester.ResourceStatus + "</color>");

            //Efficiency Monitor
            if (efficiencyMonitor != null)
            {
                string extractionRate = "100%";

                if (efficiencyMonitor.efficiencyModifier > 0f)
                    extractionRate = string.Format("{0:f2}%", efficiencyMonitor.efficiencyModifier * 100.0f);

                GUILayout.Label("<color=white>Extraction Rate At " + extractionRate + "</color>");
            }

            //Overheat
            if (overheatDisplay != null)
            {
                GUILayout.Label("<color=white>Core Temp: " + overheatDisplay.coreTempDisplay + "</color>");
                GUILayout.Label("<color=white>Thermal Efficiency: " + overheatDisplay.heatDisplay + "</color>");
            }

            //Ops buttons
            if (harvester.IsActivated)
            {
                if (GUILayout.Button(harvester.StopActionName))
                {
                    harvester.StopResourceConverter();
                }
            }
            else
            {
                if (GUILayout.Button(harvester.StartActionName))
                {
                    harvester.StartResourceConverter();
                }
            }

            //Mined resources
            GUILayout.Label("<color=white><b>Mined resources</b></color>");
            GUILayout.Label(harvester.GetMinedResources());

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
        #endregion
    }
}
