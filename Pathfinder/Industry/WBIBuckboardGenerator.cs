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
    [KSPModule("Buckboard Generator")]
    public class WBIBuckboardGenerator : WBIBreakableResourceConverter
    {
        [KSPField]
        public string smokeTransform;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (string.IsNullOrEmpty(smokeTransform) == false)
                ShowParticleEffect(IsActivated);

            PartModule inventory = this.part.Modules["ModuleKISInventory"];
            if (inventory != null)
                Utils.SetField("maxVolume", 0.001f, inventory);
        }

        public void ShowParticleEffect(bool isVisible)
        {
            KSPParticleEmitter emitter = this.part.GetComponentInChildren<KSPParticleEmitter>();
            if (emitter != null)
            {
                emitter.emit = isVisible;
                emitter.enabled = isVisible;
            }
        }

        public override void StartConverter()
        {
            base.StartConverter();
            ShowParticleEffect(IsActivated);
        }

        public override void StopConverter()
        {
            base.StopConverter();
            ShowParticleEffect(IsActivated);
        }
    }
}
