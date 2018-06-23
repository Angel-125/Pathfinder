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
       
        [KSPField]
        public float efficiencyModifier;

        [KSPField]
        public string efficiencyGUIName = "Efficiency";

        [KSPField(guiActive = true)]
        public string efficiencyDisplayString;

        private string biomeName;
        private int planetID = -1;
        List<BaseConverter> converters;
        HarvestTypes harvestID;
        WBIResourceSwitcher switcher;
        string originalEfficiencyType;
        int originalHarvestType;

        public void Destroy()
        {
            WBIPathfinderScenario.Instance.onEfficiencyUpdate -= onEfficiencyUpdate;
            if (switcher != null)
                switcher.onModuleRedecorated -= switcher_onModuleRedecorated;
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            Fields["efficiencyDisplayString"].guiName = efficiencyGUIName;

            //Get our biome, planet, and harvest ID.
            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

                if (biome != null)
                {
                    biomeName = biome.name;
                    harvestID = (HarvestTypes)harvestType;
                    planetID = this.part.vessel.mainBody.flightGlobalsIndex;
                }
            }

            //Get our converters
            converters = this.part.FindModulesImplementing<BaseConverter>();

            //Make sure we know the original efficiency and harvest type since these can change.
            originalEfficiencyType = efficiencyType;
            originalHarvestType = harvestType;

            //Hook into the switcher's events
            switcher = this.part.FindModuleImplementing<WBIResourceSwitcher>();
            if (switcher != null)
                switcher.onModuleRedecorated += switcher_onModuleRedecorated;

            //Hook into the scenario's events
            WBIPathfinderScenario.Instance.onEfficiencyUpdate += onEfficiencyUpdate;

            //Make sure that we set up efficiency
            if (!string.IsNullOrEmpty(biomeName))
            {
                onEfficiencyUpdate(planetID, biomeName, harvestID, efficiencyType,
                    WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestID, efficiencyType));
            }
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

        private void onEfficiencyUpdate(int planetID, string biomeName, HarvestTypes harvestID, string efficiencyType, float modifier)
        {
            //Are we interested in this update?
            if (this.planetID != planetID)
                return;
            if (this.biomeName != biomeName)
                return;
            if (this.harvestID != harvestID)
                return;
            if (this.efficiencyType != efficiencyType)
                return;

            //Record the modifier
            efficiencyModifier = modifier;

            //Find all the efficiency modifier parts and add their modifiers.
            List<WBIEfficiencyBonus> bonusModifiers = this.part.vessel.FindPartModulesImplementing<WBIEfficiencyBonus>();
            WBIEfficiencyBonus bonusModifier;
            int totalMultipliers = bonusModifiers.Count;
            for (int index = 0; index < totalMultipliers; index++)
            {
                bonusModifier = bonusModifiers[index];
                efficiencyModifier += bonusModifier.Bonus;
            }

            //Update efficiency
            UpdateEfficiency();
        }

        public void UpdateEfficiency()
        {
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
            if (planetID == -1 || string.IsNullOrEmpty(biomeName))
                return;
            if (converters == null)
                converters = this.part.FindModulesImplementing<BaseConverter>();
            if (converters == null)
                return;
            if (converters.Count == 0)
            {
                converters = null;
                return;
            }

            //Now update efficiency
            BaseConverter converter;
            int converterCount = converters.Count;
            for (int index = 0; index < converterCount; index++)
            {
                converter = converters[index];
                converter.EfficiencyBonus = efficiencyModifier;
            }

            //Update display string
            efficiencyDisplayString = string.Format("{0:f2}%", efficiencyModifier * 100.0f);
        }

    }
}
