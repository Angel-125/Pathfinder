using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

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
    public class WBIPathfinderResults: PartModule, IWBIExperimentResults
    {
        const string kBetterEfficiency = "Production efficiency improved by {0:f2} for ";
        const string kWorseEfficiency = "Production efficiency worsened by {0:f2} for ";
        const string kLifeSupport = "life support processors.";
        const string kManufacturing = "fabrication processors.";
        const string kChemicalProducts = "chemical processeors.";
        const string kExtraction = "extraction rates.";

        public void ExperimentRequirementsMet(string experimentID, float chanceOfSuccess, float resultRoll)
        {
            float efficiencyModifier = 0.1f;
            int planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            HarvestTypes harvestType = HarvestTypes.Planetary;
            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);
            string biomeName = biome.name;
            string modifierName = "";
            string processChanged = "";
            string resultsPreamble = string.Empty;

            switch (experimentID)
            {
                case "WBISoilAnalysis":
                    modifierName = EfficiencyData.kHabitationMod;
                    processChanged = kLifeSupport;
                    break;

                case "WBIMetallurgyAnalysis":
                    modifierName = EfficiencyData.kIndustryMod;
                    processChanged = kManufacturing;
                    break;

                case "WBIExtractionAnalysis":
                    modifierName = EfficiencyData.kExtractionMod;
                    processChanged = kExtraction;
                    efficiencyModifier /= 2.0f;
                    break;

                default:
                case "WBIChemicalAnalysis":
                    modifierName = EfficiencyData.kScienceMod;
                    processChanged = kChemicalProducts;
                    break;
            }

            //Factor success/fail
            if (resultRoll >= chanceOfSuccess)
            {
                resultsPreamble = kBetterEfficiency;
            }

            else //Worse results
            {
                resultsPreamble = kWorseEfficiency;
                efficiencyModifier = efficiencyModifier * -1.0f;
            }

            //Get existing modifier
            float currentModifer = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biome.name, harvestType, modifierName);

            //Add improvement
            currentModifer += efficiencyModifier;
            WBIPathfinderScenario.Instance.SetEfficiencyData(planetID, biome.name, harvestType, modifierName, currentModifer);

            //Inform user
            string message = string.Format(resultsPreamble, efficiencyModifier * 100f) + processChanged;
            ScreenMessages.PostScreenMessage(message, 8.0f, ScreenMessageStyle.UPPER_CENTER);

        }
    }
}
