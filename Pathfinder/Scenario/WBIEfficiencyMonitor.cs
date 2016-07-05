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
    public class WBIEfficiencyMonitor : ExtendedPartModule
    {
        [KSPField]
        public string efficiencyType;

        [KSPField]
        public int harvestType;

        private string biomeName;
        private int planetID = -1;
        float efficiency = -1f;
        List<ModuleResourceConverter> converters;
        HarvestTypes harvestID;
        WBIResourceSwitcher switcher;
        string originalEfficiencyType;
        int originalHarvestType;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

                biomeName = biome.name;
                harvestID = (HarvestTypes)harvestType;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            }

            converters = this.part.FindModulesImplementing<ModuleResourceConverter>();

            originalEfficiencyType = efficiencyType;
            originalHarvestType = harvestType;

            switcher = this.part.FindModuleImplementing<WBIResourceSwitcher>();
            if (switcher != null)
                switcher.onModuleRedecorated += new ModuleRedecoratedEvent(switcher_onModuleRedecorated);
        }

        void switcher_onModuleRedecorated(ConfigNode templateNode)
        {
            if (templateNode.HasValue("efficiencyType"))
                efficiencyType = templateNode.GetValue("efficiencyType");
            else
                efficiencyType = originalEfficiencyType;

            if (templateNode.HasValue("harvestType"))
                harvestID = (HarvestTypes)int.Parse(templateNode.GetValue("harvestType"));
            else
                harvestID = (HarvestTypes)originalHarvestType;
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            if (planetID == -1 || string.IsNullOrEmpty(biomeName))
                return;

            if (converters == null)
                converters = this.part.FindModulesImplementing<ModuleResourceConverter>();

            if (converters == null)
                return;

            if (converters.Count == 0)
            {
                converters = null;
                return;
            }

            float efficiencyModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestID, efficiencyType);
            if (efficiency != efficiencyModifier)
            {
                efficiency = efficiencyModifier;

                if (converters.Count == 0)
                    converters = this.part.FindModulesImplementing<ModuleResourceConverter>();
                foreach (ModuleResourceConverter converter in converters)
                {
                    converter.EfficiencyBonus = efficiency;
                }
            }
        }
    }
}
