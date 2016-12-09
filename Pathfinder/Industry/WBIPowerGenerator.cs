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
    public class WBIPowerGenerator : WBIBreakableResourceConverter
    {
        [KSPField(guiName = "Power Output", guiActive = true)]
        public string powerOutputDisplay = string.Empty;

        protected double ecBaseOutput;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            ResourceRatio[] outputs = outputList.ToArray();
            ResourceRatio output;
            for (int index = 0; index < outputs.Length; index++)
            {
                output = outputs[index];
                if (output.ResourceName == "ElectricCharge")
                {
                    ecBaseOutput = output.Ratio;
                    break;
                }
            }
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (!IsActivated)
            {
                powerOutputDisplay = "n/a";
                return;
            }

            //Power output
            if (this.status.Contains("load"))
            {
                //Get the numerical value (*somebody* didn't seem to make this convenient to obtain :( )
                powerOutputDisplay = this.status.Substring(0, this.status.IndexOf("%"));
                double load;
                if (double.TryParse(powerOutputDisplay, out load))
                {
                    load = load / 100f;
                    load = load * ecBaseOutput;
                    powerOutputDisplay = string.Format("{0:f2}/sec", load);
                }

                else
                {
                    powerOutputDisplay = "n/a";
                }
            }
            else
            {
                powerOutputDisplay = "n/a";
            }
        }

        public override void SetContextGUIVisible(bool isVisible)
        {
            base.SetContextGUIVisible(isVisible);
            Fields["powerOutputDisplay"].guiActive = true;
        }

        public override void DrawOpsWindow(string buttonLabel)
        {
            string absentResource = GetMissingRequiredResource();
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(170));
            if (string.IsNullOrEmpty(absentResource))
                GUILayout.Label("<color=white><b>Status: </b>" + this.status + "</color>");
            else
                GUILayout.Label("<color=white><b>Status: </b>Requires " + absentResource + "</color>");
            GUILayout.Label("<color=white><b>Power Generation: </b>" + powerOutputDisplay + "</color>");
            GUILayout.Label(string.Format("<color=white><b>" + efficiencyString + ": </b>{0:f2}%</color>", this.EfficiencyBonus * 100f));
            GUILayout.Label("<color=white><b>Time Until Maintennance: </b>" + getTimeUntilCheck() + "</color>");
            GUILayout.Label("<color=white><b>Failure Probability: </b>" + criticalFail + "%</color>");
            GUILayout.EndScrollView();

            if (ModuleIsActive())
            {
                if (GUILayout.Button(StopActionName))
                    StopConverter();
            }

            else if (GUILayout.Button(StartActionName))
            {
                StartConverter();
            }

            GUILayout.EndVertical();
        }
    }
}
