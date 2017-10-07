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
    public class WBIEfficiencyBonus : PartModule
    {
        [KSPField()]
        public float bonusValue = 1.0f;

        [KSPField()]
        public bool requiresFullCrew = true;

        //Used for unloaded vessels
        [KSPField(isPersistant = true)]
        public float calculatedBonus = 1.0f;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            bonusValue = Bonus;
        }

        public float Bonus
        {
            get
            {
                //If we don't require the full crew then just return the modifier.
                if (!requiresFullCrew)
                    calculatedBonus = bonusValue;

                //The benefit depends upon how many crew we have.
                else
                    calculatedBonus = bonusValue * ((float)this.part.protoModuleCrew.Count / (float)this.part.CrewCapacity);

                return calculatedBonus;
            }
        }
    }
}
