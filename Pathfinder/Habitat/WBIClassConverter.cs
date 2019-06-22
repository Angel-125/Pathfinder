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

Portions of this software use code from the Firespitter plugin by Snjo, used with permission. Thanks Snjo for sharing how to switch meshes. :)

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    [KSPModule("Class Converter")]
    public class WBIClassConverter : PartModule
    {
        /// <summary>
        /// List of traits that may be converted. Don't set this if any trait can be converted. Separate traits by semicolon.
        /// Example traitsAllowedToConvert = Tourist
        /// </summary>
        [KSPField]
        public string traitsAllowedToConvert = string.Empty;

        /// <summary>
        /// Kerbals restricted from converting to these traits. Separate traits by semicolon.
        /// Example: blacklistedTraits = Scout;Botanist
        /// </summary>
        [KSPField]
        public string blacklistedTraits = string.Empty;

        /// <summary>
        /// If true, resets the kerbal's experience after retraining.
        /// </summary>
        [KSPField]
        public bool resetExperience = true;

        WBIClassConverterView converterView;

        [KSPEvent(guiActive = true, guiName = "Retrain Kerbals")]
        public void retrainKerbals()
        {
            //Make sure we have a connection back to KSC.
            if (CommNet.CommNetScenario.CommNetEnabled && (this.part.vessel.connection == null || !this.part.vessel.connection.IsConnectedHome))
            {
                ScreenMessages.PostScreenMessage("Retraining kerbals requires a connection back to Kerbal Space Center, which is currently unavailable.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            //Show the view
            converterView = new WBIClassConverterView();
            converterView.part = this.part;
            converterView.traitsAllowedToConvert = traitsAllowedToConvert;
            converterView.blacklistedTraits = blacklistedTraits;
            converterView.resetExperience = resetExperience;
            converterView.SetVisible(true);
        }
    }
}
