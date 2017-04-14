using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2017, by Michael Billard (Angel-125)
License: GPLV3

Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    class PathfinderSettings : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Confirm scrap operation", toolTip = "If enabled, you'll need to click the scrap button again to confirm the scrap operation.", autoPersistance = true)]
        public bool confirmScrap = true;

        #region Properties
        public static bool ConfirmScrap
        {
            get
            {
                PathfinderSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<PathfinderSettings>();
                return settings.confirmScrap;
            }

            set
            {
                PathfinderSettings settings = HighLogic.CurrentGame.Parameters.CustomParams<PathfinderSettings>();
                settings.confirmScrap = value;
            }
        }        
        #endregion

        #region CustomParameterNode
        public override string Section
        {
            get
            {
                return "Pathfinder";
            }
        }

        public override string Title
        {
            get
            {
                return "Settings";
            }
        }

        public override int SectionOrder
        {
            get
            {
                return 1;
            }
        }

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
        #endregion
    }
}
