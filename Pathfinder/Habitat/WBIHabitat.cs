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
    [KSPModule("Habitat")]
    public class WBIHabitat : ExtendedPartModule, IOpsView
    {
        PartModule kosModule;
        int partCount;
        Dictionary<Part, List<PartModule>> actionGroupParts = new Dictionary<Part, List<PartModule>>();
        string[] groupNames = { "Stage", "Gear", "Light", "RCS", "SAS", "Brakes", "Abort", "Custom 1", "Custom 2", "Custom 3", "Custom 4", "Custom 5", "Custom 6", "Custom 7", "Custom 8", "Custom 9", "Custom 10" };
        bool[] selectedGroups = new bool[17];
        int selectedIndex;
        Vector2 scrollPos;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            foreach (PartModule mod in this.part.Modules)
            {
                if (mod.moduleName == "kOSProcessor")
                {
                    kosModule = mod;
                    break;
                }
            }
            if (kosModule != null)
                hideKOSGUI();
        }

        protected void hideKOSGUI()
        {
            foreach (BaseField field in kosModule.Fields)
            {
                field.guiActive = false;
                field.guiActiveEditor = false;
            }

            foreach (BaseEvent evnt in kosModule.Events)
            {
                evnt.guiActive = false;
                evnt.guiActiveEditor = false;
                evnt.guiActiveUnfocused = false;
            }
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
            buttonLabels.Add("Habitat");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            KSPActionGroup selectedGroup;
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();
            drawKOSGUI();
            GUILayout.EndVertical();

            scrollPos = GUILayout.BeginScrollView(scrollPos, new GUIStyle(GUI.skin.textArea), GUILayout.Width(400));
            GUILayout.BeginVertical();

            //Group title
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("<color=white><b>-= Action Groups =-</b></color>");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, groupNames, 1, GUILayout.Width(80));
            selectedGroup = KSPActionGroup.Light;

            //If needed, refresh the action group list
            if (partCount != this.part.vessel.parts.Count)
                refreshPartList();

            bool showLabel;
            GUILayout.BeginVertical();
            List<PartModule> actionGroupModules;
            foreach (Part actionPart in actionGroupParts.Keys)
            {
                showLabel = true;
                actionGroupModules = actionGroupParts[actionPart];

                foreach (PartModule module in actionGroupModules)
                {
                    foreach (BaseAction action in module.Actions)
                        if (action.actionGroup == selectedGroup)
                        {
                            if (showLabel)
                            {
                                showLabel = false;
                                GUILayout.Label("<color=white>" + actionPart.partInfo.title + "</color>");
                            }
                            GUILayout.Label(action.guiName);
                        }
                }
            }
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
        }

        protected void drawKOSGUI()
        {
            //Draw kOS view
            if (kosModule != null)
            {
                int diskSpace = (int)Utils.GetField("diskSpace", kosModule);
                float requiredPower = (float)Utils.GetField("RequiredPower", kosModule);

                GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea));

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label("<color=white><b>-= kOS =-</b></color>");
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Label("<color=white><b>Disk Space: </b>" + diskSpace + "</color>");
                GUILayout.Label("<color=white><b>Required Power: </b>" + string.Format("{0:f1}", requiredPower) + "</color>");

                if (GUILayout.Button("Open Terminal"))
                    kosModule.Events["Activate"].Invoke();
                if (requiredPower > 0f)
                {
                    if (GUILayout.Button("Toggle Power"))
                        kosModule.Events["TogglePower"].Invoke();
                }
                GUILayout.EndScrollView();
            }
        }

        protected void refreshPartList()
        {
            List<PartModule> actionGroupModules;

            actionGroupParts.Clear();
            partCount = this.part.vessel.parts.Count;

            foreach (Part vslPart in this.part.vessel.parts)
            {
                foreach (PartModule module in vslPart.Modules)
                {
                    //If the module has actions, then add it to our dictionary
                    if (module.Actions.Count > 0)
                    {
                        //Create the entry if needed
                        if (actionGroupParts.ContainsKey(vslPart) == false)
                        {
                            actionGroupModules = new List<PartModule>();
                            actionGroupParts.Add(vslPart, actionGroupModules);
                        }

                        //Get the entry
                        actionGroupModules = actionGroupParts[vslPart];

                        //Add the module if needed.
                        if (actionGroupModules.Contains(module) == false)
                            actionGroupModules.Add(module);
                    }
                }
            }
        }

        #endregion
    }
}
