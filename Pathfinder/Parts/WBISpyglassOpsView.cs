using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using WBIResources;

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
namespace WBIPathfinder
{
    public class WBISpyglassOpsView : PartModule, IOpsView
    {
        PartModule exSurveyStation;
        bool isGUIVisible;

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
