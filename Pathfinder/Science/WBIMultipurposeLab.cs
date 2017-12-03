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
    public class WBIMultipurposeLab : WBIMultipurposeHab, IModuleInfo
    {
        protected WBIScienceConverter scienceConverter;
        protected float originalCrewsRequired;

        public override void OnStart(StartState state)
        {
            ModuleScienceLab sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            if (sciLab != null)
                originalCrewsRequired = sciLab.crewsRequired;
            scienceConverter = this.part.FindModuleImplementing<WBIScienceConverter>();
            if (scienceConverter != null)
                scienceConverter.SetGuiVisible(false);
            base.OnStart(state);
            if (string.IsNullOrEmpty(resourcesToKeep))
                resourcesToKeep = "ElectricCharge";
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
                if (HighLogic.LoadedSceneIsEditor)
                {
                    sciLab.isEnabled = false;
                    sciLab.enabled = false;
                }

                else if (enableMPLModules)
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
                if (HighLogic.LoadedSceneIsEditor)
                {
                    converter.isEnabled = enableMPLModules;
                    converter.enabled = enableMPLModules;
                }

                else
                {
                    converter.isEnabled = enableMPLModules;
                    converter.enabled = enableMPLModules;
                }
            }
        }

        public override string GetInfo()
        {
            return "Click the Manage Operations button to change the configuration.";
        }
    }
}
