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
    [KSPModule("Geo Thermal Plant")]
    public class WBIGeoThermalPlant : WBIBreakableResourceConverter
    {
        ModuleResourceHarvester harvester;
        WBIDrillSwitcher drillSwitcher;
        WBIExtractionMonitor extractionMonitor;
        ModuleOverheatDisplay overheatDisplay;
        
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            harvester = this.part.FindModuleImplementing<ModuleResourceHarvester>();

            drillSwitcher = this.part.FindModuleImplementing<WBIDrillSwitcher>();
            if (drillSwitcher != null)
            {
                drillSwitcher.Events["ShowDrillSwitchWindow"].guiActive = false;
                drillSwitcher.Events["ShowDrillSwitchWindow"].guiActiveUnfocused = false;
            }

            extractionMonitor = this.part.FindModuleImplementing<WBIExtractionMonitor>();
            if (extractionMonitor != null)
            {
                extractionMonitor.Fields["extractionRateChange"].guiActive = false;
            }

            overheatDisplay = this.part.FindModuleImplementing<ModuleOverheatDisplay>();
        }

        public override List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Geothermal");
            return buttonLabels;
        }

        public override void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            base.DrawOpsWindow(buttonLabel);

            if (harvester == null)
            {
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(340));
            GUILayout.Label("<color=white>Geothermal Tap</color>");

            //Status
            GUILayout.Label("<color=white>Status: " + harvester.ResourceStatus + "</color>");

            //Extraction Monitor
            if (extractionMonitor != null)
                GUILayout.Label("<color=white>Extraction Rate At " + extractionMonitor.extractionRateChange + "</color>");

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
                    harvester.StopResourceConverter();
            }
            else
            {
                if (GUILayout.Button(harvester.StartActionName))
                    harvester.StartResourceConverter();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

    }
}
