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
    [KSPModule("Gaslight")]
    class WBIGaslight : WBILight, IOpsView
    {
        WBIAnimation anim;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            
            //Get the telescopic animation
            anim = this.part.FindModuleImplementing<WBIAnimation>();
        }

        public override void ToggleAnimation()
        {
            base.ToggleAnimation();

            //Make sure the lamppost has been deployed.
            if (anim.isDeployed == false)
                anim.ToggleAnimation();
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Gaslight");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });

            if (GUILayout.Button(Events["ToggleAnimation"].guiName))
                ToggleAnimation();

            GUILayout.Label("<color=red>red</color>");
            red = GUILayout.HorizontalSlider(red, 0f, 1f);

            GUILayout.Label("<color=green>green</color>");
            green = GUILayout.HorizontalSlider(green, 0f, 1f);

            GUILayout.Label("<color=blue>blue</color>");
            blue = GUILayout.HorizontalSlider(blue, 0f, 1f);

            GUILayout.Label("<color=white>level</color>");
            level = GUILayout.HorizontalSlider(level, 0f, 1f);

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            /*
            showGui(isVisible);
            Fields["red"].guiActive = isVisible;
            Fields["red"].guiActiveEditor = isVisible;
            Fields["green"].guiActive = isVisible;
            Fields["green"].guiActiveEditor = isVisible;
            Fields["blue"].guiActive = isVisible;
            Fields["blue"].guiActiveEditor = isVisible;
            Fields["level"].guiActive = isVisible;
            Fields["level"].guiActiveEditor = isVisible;
             */
        }
        #endregion

    }
}
