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
    [KSPModule("Science Converter")]
    public class WBIScienceConverter : ModuleScienceConverter
    {
        [KSPField]
        public float fundsPerData;

        [KSPField]
        public float reputationPerData;

        protected ModuleScienceLab sciLab = null;
        protected TransmitHelper transmitHelper = new TransmitHelper();

        [KSPEvent(guiName = "Transmit Science", active = true, guiActive = true)]
        public void TransmitResearch()
        {
            //Nothing to do here except call the science lab's transmit function.
            sciLab.TransmitScience();
        }

        [KSPEvent(guiName = "Publish Research", active = true, guiActive = true)]
        public void PublishResearch()
        {
            float storedScience = sciLab.storedScience;
            float value = storedScience * reputationPerData * (1.0f + (scientistBonus * GetTotalCrewSkill()));

            //Transmit data for publishing (Reputation gain)
            transmitHelper.TransmitToKSC(0, value, 0, storedScience);
        }

        [KSPEvent(guiName = "Sell Research", active = true, guiActive = true)]
        public void SellResearch()
        {
            float storedScience = sciLab.storedScience;
            float value = storedScience * fundsPerData * (1.0f + (scientistBonus * GetTotalCrewSkill()));

            //Transmit data for sale (Funds gain)
            transmitHelper.TransmitToKSC(0, 0, value, storedScience);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Get the science lab and hide its transmit button. We'll use our own buttons.
            sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            sciLab.Events["TransmitScience"].guiActive = false;

            transmitHelper.part = this.part;
            transmitHelper.transmitCompleteDelegate = DataTransmitted;

            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                Events["PublishResearch"].guiActive = false;
                Events["SellResearch"].guiActive = false;
            }
        }

        protected bool transmitData(float dataAmount, bool transmitForSale = false)
        {
            float amount;
            bool dataTransmitted = false;

            if (transmitForSale)
            {
                amount = dataAmount * fundsPerData * (1.0f + (scientistBonus * GetTotalCrewSkill()));
                dataTransmitted = transmitHelper.TransmitToKSC(0, 0, amount, dataAmount);
            }
            else
            {
                amount = dataAmount * reputationPerData * (1.0f + (scientistBonus * GetTotalCrewSkill()));
                dataTransmitted = transmitHelper.TransmitToKSC(0, amount, 0, dataAmount);
            }

            return dataTransmitted;
        }

        public void DataTransmitted()
        {
            sciLab.storedScience = 0f;
        }

        public void SetGuiVisible(bool isVisible)
        {
            Events["TransmitResearch"].guiActive = isVisible;
            Events["PublishResearch"].guiActive = isVisible;
            Events["SellResearch"].guiActive = isVisible;
            Events["StartResourceConverter"].guiActive = isVisible;
            Events["StopResourceConverter"].guiActive = isVisible;
//            Fields["datString"].guiActive = isVisible;
//            Fields["rateString"].guiActive = isVisible;
//            Fields["sciString"].guiActive = isVisible;
//            Fields["status"].guiActive = isVisible;

            //Hide publish and sell buttons when we aren't in Career mode.
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
            {
                Events["PublishResearch"].guiActive = false;
                Events["SellResearch"].guiActive = false;
            }

            //Don't forget about the science lab!
//            sciLab.Fields["statusText"].guiActive = isVisible;
            sciLab.Events["CleanModulesEvent"].guiActive = isVisible;
        }

        public virtual void SetModuleEnabled(bool isEnabled)
        {
            Debug.Log("[WBIScienceConverter] SetModuleEnabled called. isEnabled: " + isEnabled);

            SetGuiVisible(isEnabled);

            //Enable/disable our self
            if (isEnabled)
                EnableModule();
            else
                DisableModule();
        }

        public virtual float GetTotalCrewSkill()
        {
            float totalSkillPoints = 0f;
            int totalScientists = 0;

            if (this.part.CrewCapacity == 0)
                return 0f;

            foreach (ProtoCrewMember crewMember in this.part.protoModuleCrew)
            {
                if (crewMember.HasEffect(ExperienceEffect))
                {
                    totalSkillPoints += crewMember.experienceTrait.CrewMemberExperienceLevel();
                    totalScientists += 1;
                }
            }

            return totalSkillPoints;
        }
    }
}
