using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
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
    [KSPModule("OSE Workshop Helper")]
    public class WBIOSEWorkshop : ExtendedPartModule, IOpsView
    {
        PartModule oseWorkshop;
        PartModule oseRecycler;
        MethodInfo methodOpenWorkshop = null;
        MethodInfo methodOpenRecycler = null;
        WBIInflatablePartModule inflatableModule = null;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            inflatableModule = this.part.FindModuleImplementing<WBIInflatablePartModule>();

            if (this.part.Modules == null)
                return;
            foreach (PartModule mod in this.part.Modules)
            {
                if (mod.moduleName == "OseModuleWorkshop")
                    oseWorkshop = mod;
                else if (mod.moduleName == "OseModuleRecycler")
                    oseRecycler = mod;
            }

            //Use reflection to find the methods we need.
            findWorkshopMethods();
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetContextGUIVisible(bool isVisible)
        {
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Workshop");
            return buttonLabels;
        }


        public void DrawOpsWindow(string buttonLabel)
        {
            string workshopStatus;
            string recyclerStatus;

            GUILayout.BeginVertical();
            //This can happen when KIS gets updated and OSE Workshop hasn't been updated yet.
            if (oseWorkshop == null && oseRecycler == null)
            {
                GUILayout.Label("Unable to render the OSE Workshop GUI, please install the latest version of OSE Workshop.");
            }

            else
            {
                if (inflatableModule != null)
                {
                    if (inflatableModule.isInflatable && inflatableModule.isDeployed == false)
                    {
                        GUILayout.Label("In order to use the workshop, you need to inflate it first.");
                        GUILayout.EndVertical();
                        return;
                    }
                }

                //Draw status fields
                if (oseWorkshop != null)
                {
                    workshopStatus = (string)Utils.GetField("Status", oseWorkshop);
                    GUILayout.Label("<b>Workshop Status:</b> " + workshopStatus);
                }

                if (oseRecycler != null)
                {
                    recyclerStatus = (string)Utils.GetField("Status", oseRecycler);
                    GUILayout.Label("<b>Recycler Status:</b> " + recyclerStatus);
                }

                //Draw buttons
                if (oseWorkshop != null)
                {
                    if (GUILayout.Button("Open Workshop"))
                    {
                        methodOpenWorkshop.Invoke(oseWorkshop, null);
                    }
                }

                if (oseRecycler != null)
                {
                    if (GUILayout.Button("Open Recycler"))
                        methodOpenRecycler.Invoke(oseRecycler, null);
                }
            }
            GUILayout.EndVertical();
        }
        #endregion

        protected void findWorkshopMethods()
        {
            if (oseWorkshop == null && oseRecycler == null)
                return;

            //Find the methods we need.
            foreach (AssemblyLoader.LoadedAssembly assembly in AssemblyLoader.loadedAssemblies)
            {
                if (assembly.name == "Workshop")
                {
                    Type[] classes = assembly.assembly.GetTypes();
                    foreach (Type OseModuleWorkshop in classes)
                    {
                        if (OseModuleWorkshop.Name == "OseModuleWorkshop" && oseWorkshop != null)
                            methodOpenWorkshop = OseModuleWorkshop.GetMethod("ContextMenuOpenWorkbench");

                        else if (OseModuleWorkshop.Name == "OseModuleRecycler" && oseRecycler != null)
                            methodOpenRecycler = OseModuleWorkshop.GetMethod("ContextMenuOnOpenRecycler");
                    }
                }
            }

        }
    }
}
