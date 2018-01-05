using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class WBISpyglassOpsView : PartModule, IOpsView
    {
        PartModule exSurveyStation;
        bool isGUIVisible;
        WBILight light;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            foreach (PartModule module in this.part.Modules)
            {
                if (module.moduleName == "ExSurveyStation")
                {
                    exSurveyStation = module;
                    break;
                }
            }

            light = this.part.FindModuleImplementing<WBILight>();
        }

        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Launchpad");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Toggle GUI"))
            {
                isGUIVisible = !isGUIVisible;

                if (isGUIVisible)
                    exSurveyStation.Events["ShowUI"].Invoke();
                else
                    exSurveyStation.Events["HideUI"].Invoke();
            }

            if (light == null)
                return;

            if (GUILayout.Button(light.Events["ToggleAnimation"].guiName))
                light.ToggleAnimation();

            light.red = GUILayout.HorizontalSlider(light.red, 0f, 1f);
            light.green = GUILayout.HorizontalSlider(light.green, 0f, 1f);
            light.blue = GUILayout.HorizontalSlider(light.blue, 0f, 1f);
            light.level = GUILayout.HorizontalSlider(light.level, 0f, 1f);

            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
        }
    }
}
