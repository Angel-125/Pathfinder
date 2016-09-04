using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2016, by Michael Billard (Angel-125)
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
    public class WBISciLabOpsView : ExtendedPartModule, IOpsView
    {
        [KSPField]
        public bool showOpsView;

        SciLabOpsWindow opsWindow = new SciLabOpsWindow("Science Lab");

        [KSPEvent(guiName = "Show Lab GUI", active = true, guiActive = false)]
        public void ShowOpsView()
        {
            if (opsWindow == null)
            {
                WBISciLabOpsView opsView = this.part.FindModuleImplementing<WBISciLabOpsView>();

                if (opsView == null)
                {
                    ScreenMessages.PostScreenMessage("WBISciLabOpsView required in config file to show the window.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                    opsWindow.converter.SetGuiVisible(true);
                    Events["ShowOpsView"].guiActive = false;
                    return;
                }
            }

            opsWindow.SetVisible(true);
        }

        public void OnGUI()
        {
            if (opsWindow.IsVisible())
                opsWindow.DrawWindow();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Set up the ops window
            opsWindow.part = this.part;
            opsWindow.WindowTitle = this.part.partInfo.title;
            opsWindow.FindPartModules();

            //If we want to show the ops view dialog instead of the context buttons,
            //then hide the context buttons.
            if (showOpsView)
            {
                Events["ShowOpsView"].guiActive = true;

                opsWindow.converter.Events["TransmitResearch"].guiActive = false;
                opsWindow.converter.Events["PublishResearch"].guiActive = false;
                opsWindow.converter.Events["SellResearch"].guiActive = false;
                opsWindow.converter.Events["StartResourceConverter"].guiActive = false;
                opsWindow.converter.Events["StopResourceConverter"].guiActive = false;

            }
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            Events["ShowOpsView"].guiActive = isVisible;
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Mobile Process");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            opsWindow.DrawOpsWindow();
        }
        #endregion
    }
}
