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
    public class WBIExtractionMonitor : PartModule
    {
        private string biomeName;
        private int planetID = -1;
        HarvestTypes harvestType;

        [KSPField(guiActive = true, guiName = "Extraction Rate At")]
        public string extractionRateChange;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

                biomeName = biome.name;
                harvestType = HarvestTypes.Planetary;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            }

        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            if (planetID == -1 || string.IsNullOrEmpty(biomeName))
                return;
            
            float efficiencyModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestType, EfficiencyData.kExtractionMod);

            extractionRateChange = string.Format("{0:f2}%", efficiencyModifier * 100f);
        }
    }
}
