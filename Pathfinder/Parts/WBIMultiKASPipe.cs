using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyright 2016, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    [KSPModule("Multi KAS Pipe")]
    public class WBIMultiKASPipe : PartModule
    {
        [KSPField]
        public string portName;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Rename the KAS ports
            string portID;
            foreach (PartModule mod in this.part.Modules)
                if (mod.moduleName == "KASModuleStrut")
                {
                    //Get the ID number
                    portID = (string)Utils.GetField("nodeTransform", mod);
                    portID = portID.Replace(portName, "");

                    //Rename the event
                    mod.Events["ContextMenuLink"].guiName = "Link Port " + portID;
                    mod.Events["ContextMenuUnlink"].guiName = "Unlink Port " + portID;
                }
        }
    }
}
