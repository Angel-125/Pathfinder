using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2016, by Michael Billard (Angel-125)
License: GPLV3

Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class GoldStrikeSettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomIntParameterUI("Prospects Per Biome", maxValue = 50, minValue = 1, stepSize = 1, toolTip = "How many chances to strike it rich", autoPersistance = true, gameMode = GameParameters.GameMode.ANY)]
        public int prospectsPerBiome = 20;

        [GameParameters.CustomFloatParameterUI("Prospect Reset Cost", maxValue = 500.0f, minValue = 10.0f, toolTip = "How much for extra prospects?", autoPersistance = true, gameMode = GameParameters.GameMode.SCIENCE | GameParameters.GameMode.CAREER)]
        public float prospectResetCost = 100.0f;

        [GameParameters.CustomIntParameterUI("Distance Between Prospects (KM)", maxValue = 100, minValue = 10, stepSize = 1, toolTip = "How far to travel between prospects", autoPersistance = true, gameMode = GameParameters.GameMode.ANY)]
        public int distanceBetweenProspects = 10;

        [GameParameters.CustomIntParameterUI("Bonus Per Prospect Skill", maxValue = 10, minValue = 1, stepSize = 1, toolTip = "What are those skill points worth?", autoPersistance = true, gameMode = GameParameters.GameMode.ANY)]
        public int bonusPerSkillPoint = 1;

//        [GameParameters.CustomParameterUI("Enable debug logging", toolTip = "Help a modder out when you get stuck. Enable logging.", autoPersistance = true, gameMode = GameParameters.GameMode.ANY)]
//        public bool loggingEnabled = true;

        #region Properties
        /*
        public static bool LoggingEnabled
        {
            get
            {
                GoldStrikeSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GoldStrikeSettings>();
                return settings.loggingEnabled;
            }
        }
         */

        public static int ProspectsPerBiome
        {
            get
            {
                GoldStrikeSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GoldStrikeSettings>();
                return settings.prospectsPerBiome;
            }
        }

        public static float ProspectResetCost
        {
            get
            {
                GoldStrikeSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GoldStrikeSettings>();
                return settings.prospectResetCost;
            }
        }

        public static float DistanceBetweenProspects
        {
            get
            {
                GoldStrikeSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GoldStrikeSettings>();
                return settings.distanceBetweenProspects;
            }
        }

        public static int BonusPerSkillPoint
        {
            get
            {
                GoldStrikeSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<GoldStrikeSettings>();
                return settings.bonusPerSkillPoint;
            }
        }

        #endregion

        #region GameParameters.CustomParameterNode
        public override GameParameters.GameMode GameMode
        {
            get
            {
                return GameParameters.GameMode.ANY;
            }
        }

        public override bool HasPresets
        {
            get
            {
                return false;
            }
        }

        public override string Section
        {
            get
            {
                return "Pathfinder";
            }
        }

        public override int SectionOrder
        {
            get
            {
                return 0;
            }
        }

        public override string Title
        {
            get
            {
                return "Gold Strike";
            }
        }
        #endregion
    }
}
