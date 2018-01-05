using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2015-2017, by Michael Billard (Angel-125)
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
    public class WBIEfficiencyBonus : PartModule, IOpsView
    {
        [KSPField()]
        public float bonusValue = 1.0f;

        [KSPField()]
        public bool requiresFullCrew = true;

        //Used for unloaded vessels
        [KSPField(isPersistant = true)]
        public float calculatedBonus = 1.0f;

        string biomeName;
        int planetID;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            bonusValue = Bonus;

            //Get our biome, planet, and harvest ID.
            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

                biomeName = biome.name;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            }
        }

        public float Bonus
        {
            get
            {
                //If we have no crew capacity then we're done.
                if (this.part.CrewCapacity == 0)
                {
                    calculatedBonus = 0.0f;
                    return bonusValue;
                }

                //If we don't require the full crew then just return the modifier.
                if (!requiresFullCrew)
                    calculatedBonus = bonusValue;

                //The benefit depends upon how many crew we have.
                else
                    calculatedBonus = bonusValue * ((float)this.part.protoModuleCrew.Count / (float)this.part.CrewCapacity);

                return calculatedBonus;
            }
        }

        #region IOpsView
        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Efficiency Bonus");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });

            GUILayout.Label("<color=white><b>Efficiency Bonuses</b></color>");
            float modifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, HarvestTypes.Planetary, EfficiencyData.kHabitationMod);
            modifier = (modifier + calculatedBonus) * 100.0f;
            GUILayout.Label(string.Format("<color=white>Habitation: +{0:f2}</color>", modifier));

            modifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, HarvestTypes.Planetary, EfficiencyData.kScienceMod);
            modifier = (modifier + calculatedBonus) * 100.0f;
            GUILayout.Label(string.Format("<color=white>Science: +{0:f2}</color>", modifier));

            modifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, HarvestTypes.Planetary, EfficiencyData.kIndustryMod);
            modifier = (modifier + calculatedBonus) * 100.0f;
            GUILayout.Label(string.Format("<color=white>Industry: +{0:f2}</color>", modifier));

            modifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, HarvestTypes.Planetary, EfficiencyData.kExtractionMod);
            modifier = (modifier + calculatedBonus) * 100.0f;
            GUILayout.Label(string.Format("<color=white>Drills: +{0:f2}</color>", modifier));

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
        }

        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }
        #endregion
    }
}
