using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;
using KSP.Localization;

/*
Source code copyrighgt 2017, by Michael Billard (Angel-125)
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
    //Generates science data, the type you typically get from experiments when they're stored in a Mobile Processing Lab.
    //This data is then distributed to not only said labs, but to Gold Strike bonus modules and to pipe endpoint modules.
    [KSPModule("Science Data Generator")]
    public class WBIScienceDataGenerator : WBIResourceConverter
    {
        //Toggle to indicate whether or not to distribute data to science labs
        [KSPField(guiName = "Distribute data to labs", isPersistant = true, guiActiveEditor = true, guiActive = true)]
        [UI_Toggle(enabledText = "Yes", disabledText = "No")]
        public bool distributeDataToLabs = true;

        [KSPField]
        public float dataPerCycle = 50f;

        [KSPField]
        public float criticalSuccessMultiplier = 1.1f;

        [KSPField]
        public bool requiresFullCrew = true;

        protected void Log(string message)
        {
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIScienceDataGenerator] - " + message);
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
        }

        public override string GetInfo()
        {
            StringBuilder infoBuilder = new StringBuilder();

            infoBuilder.AppendLine(base.GetInfo());
            infoBuilder.AppendLine(" ");
            infoBuilder.AppendLine(string.Format("Hours Per Cycle: {0:f2}", hoursPerCycle));
            infoBuilder.AppendLine(string.Format("Mits Per Cycle: {0:f2}", dataPerCycle));
            if (requiresFullCrew)
                infoBuilder.AppendLine("Data yield reduced if the part isn't fully staffed.");

            return infoBuilder.ToString();
        }

        protected override void onSuccess()
        {
            base.onSuccess();

            float amount = dataPerCycle;
            if (requiresFullCrew)
                amount *= this.part.protoModuleCrew.Count / this.part.CrewCapacity;

            WBIPathfinderScenario.Instance.DistributeData(amount, this.part.vessel, distributeDataToLabs);
        }

        protected override void onCriticalSuccess()
        {
            base.onCriticalSuccess();

            float amount = dataPerCycle * criticalSuccessMultiplier;
            if (requiresFullCrew)
                amount *= this.part.protoModuleCrew.Count / this.part.CrewCapacity;

            WBIPathfinderScenario.Instance.DistributeData(amount, this.part.vessel, distributeDataToLabs);
        }
    }
}
