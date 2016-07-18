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
    [KSPModule("Multipurpose Lab")]
    public class WBIMultipurposeLab : WBIMultiConverter, IModuleInfo
    {
        private const string kDocOpsView = "Doc Operations";

        [KSPField]
        public string partToolTip = string.Empty;

        [KSPField]
        public string partToolTipTitle = string.Empty;

        [KSPField]
        public string opsViewTitle = string.Empty;

        Animation anim;
        WBIScienceConverter scienceConverter;
        private float originalCrewsRequired;

        public override void OnStart(StartState state)
        {
            ModuleScienceLab sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            if (sciLab != null)
                originalCrewsRequired = sciLab.crewsRequired;
            scienceConverter = this.part.FindModuleImplementing<WBIScienceConverter>();
            scienceConverter.SetGuiVisible(false);
            base.OnStart(state);

            if (string.IsNullOrEmpty(animationName))
                return;
            anim = this.part.FindModelAnimators(animationName)[0];

            opsManagerView.WindowTitle = this.part.partInfo.title + " Operations";
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (anim == null)
            {
                checkAndShowToolTip();
                return;
            }

            //We're only interested in the act of inflating the module.
            if (isDeployed == false)
            {
                animationStarted = false;
                return;
            }

            //If we've completed the animation then we are done.
            if (animationStarted == false)
                return;

            //Animation may not be done yet.
            if (anim.isPlaying)
                return;

            //At this point we know that the animation was playing but has now stopped.
            //We also know that the animation was started. Now reset the flag.
            animationStarted = false;

            checkAndShowToolTip();
        }

        public override void RedecorateModule(bool loadTemplateResources = true)
        {
            base.RedecorateModule(loadTemplateResources);
            bool enableMPLModules = false;

            if (CurrentTemplate.HasValue("enableMPLModules"))
                enableMPLModules = bool.Parse(CurrentTemplate.GetValue("enableMPLModules"));
            ModuleScienceLab sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            if (sciLab != null)
            {
                if (enableMPLModules)
                {
                    sciLab.isEnabled = true;
                    sciLab.enabled = true;
                    sciLab.crewsRequired = originalCrewsRequired;
                }
                else
                {
                    sciLab.crewsRequired = 2000.0f;
                    sciLab.isEnabled = false;
                    sciLab.enabled = false;
                }

            }

            ModuleScienceConverter converter = this.part.FindModuleImplementing<ModuleScienceConverter>();
            if (converter != null)
            {
                converter.isEnabled = enableMPLModules;
                converter.enabled = enableMPLModules;
            }
        }

        public override void UpdateContentsAndGui(string templateName)
        {
            base.UpdateContentsAndGui(templateName);

            //Check to see if we've displayed the tooltip for the template.
            //First, we're only interested in deployed modules.
            if (isDeployed == false)
                return;

            //Now check
            checkAndShowToolTip();
        }

        protected void checkAndShowToolTip()
        {
            //Now we can check to see if the tooltip for the current template has been shown.
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (scenario.HasShownToolTip(CurrentTemplateName) && scenario.HasShownToolTip(getMyPartName()))
                return;

            //Tooltip for the current template has never been shown. Show it now.
            string toolTipTitle = CurrentTemplate.GetValue("toolTipTitle");
            string toolTip = CurrentTemplate.GetValue("toolTip");

            if (string.IsNullOrEmpty(toolTipTitle))
                toolTipTitle = partToolTipTitle;

            //Add the very first part's tool tip.
            if (scenario.HasShownToolTip(getMyPartName()) == false)
            {
                toolTip = partToolTip + "\r\n\r\n" + toolTip;

                scenario.SetToolTipShown(getMyPartName());
            }

            if (string.IsNullOrEmpty(toolTip) == false)
            {
                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(toolTipTitle, toolTip);
                toolTipWindow.SetVisible(true);

                //Cleanup
                scenario.SetToolTipShown(CurrentTemplateName);
            }
        }

        protected override void hideEditorGUI(PartModule.StartState state)
        {
            base.hideEditorGUI(state);
            Events["ToggleInflation"].guiActiveEditor = false;
        }

        public override string GetInfo()
        {
            return "Click the Manage Operations button to change the configuration.";
        }

        public string GetModuleTitle()
        {
            return "Multipurpose Lab";
        }

        public string GetPrimaryField()
        {
            return "Inflated Crew Capacity: " + inflatedCrewCapacity.ToString();
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }
    }
}
