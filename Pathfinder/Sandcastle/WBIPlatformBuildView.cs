using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

/*
Source code copyright 2018, by Michael Billard (Angel-125)
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
    public delegate void OnStaticSelectedDelegate(string staticName, ConfigNode staticNode);
    public delegate void OnCancelBuildDelegate();
    public delegate void OnSpawnStaticDelegate();

    public class WBIPlatformBuildView : Dialog<WBIPlatformBuildView>
    {
        const int DialogWidth = 350;
        const int DialogHeight = 365;

        public List<ConfigNode> staticList = new List<ConfigNode>();
        public List<WBIBuildResource> requiredResources;
        public ConfigNode currentStaticNode;
        public string currentStaticName;
        public bool isBuilding;
        public bool buildCompleted;
        public double crewBonus;

        public OnStaticSelectedDelegate OnStaticSelected;
        public OnCancelBuildDelegate OnCancelBuild;
        public OnSpawnStaticDelegate OnSpawnStatic;

        Vector2 scrollPos;

        public WBIPlatformBuildView() : base("Project Manager", DialogWidth, DialogHeight)
        {
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
            {
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            //Are we currently building something? If so then show its status.
            if (isBuilding || buildCompleted)
                drawBuildStatus();

            //If we're not currently building something then show the different statics that we can build.
            else
                drawBuildOptions();

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Close"))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected void drawBuildStatus()
        {
            GUILayout.BeginVertical();

            //Static name
            string staticName;
            if (currentStaticNode.HasValue("title"))
                staticName = currentStaticNode.GetValue("title");
            else
                staticName = currentStaticName;
            GUILayout.Label("<color=white><b>Project: </b>" + staticName + "</color>");

            if (isBuilding)
            {
                //Build/Cancel flag: we won't actually cancel the build until user presses the OK button.
                if (GUILayout.Button("Cancel Build") && OnCancelBuild != null)
                {
                    isBuilding = false;
                    OnCancelBuild();
                }
            }

            else if (buildCompleted)
            {
                //Spawn static button
                if (GUILayout.Button("Finalize Build") && OnSpawnStatic != null)
                {
                    isBuilding = false;
                    buildCompleted = false;
                    OnSpawnStatic();
                    SetVisible(false);
                }
            }

            //Resource status
            GUILayout.Label("<color=white><b>Resource Status</b></color>");
            int count = requiredResources.Count;
            for (int index = 0; index < count; index ++)
            {
                GUILayout.Label("<color=white>" + requiredResources[index].statusString(crewBonus) + "</color>");
            }

            GUILayout.EndVertical();
        }

        protected void drawBuildOptions()
        {
            GUILayout.BeginVertical();
            int count = staticList.Count;
            ConfigNode staticNode;
            ConfigNode[] buildResources;
            ConfigNode resourceNode;
            GUIStyle style = new GUIStyle(HighLogic.Skin.textArea);
            style.active = style.hover = style.normal;
            style.padding = new RectOffset(0, 0, 0, 0);


            for (int index = 0; index < count; index++)
            {
                staticNode = staticList[index];

                GUILayout.BeginVertical(style);

                //Title
                GUILayout.Label("<color=lightblue><b>" + staticNode.GetValue("title") + "</b></color>");

                //Resources required
                if (staticNode.HasNode(WBIBuildResource.kBuildNode))
                {
                    buildResources = staticNode.GetNodes(WBIBuildResource.kBuildNode);

                    GUILayout.Label("<color=white><b>Required Resources</b></color>");

                    for (int resourceIndex = 0; resourceIndex < buildResources.Length; resourceIndex++)
                    {
                        resourceNode = buildResources[resourceIndex];
                        GUILayout.Label("<color=white>" + WBIBuildResource.GetRequirements(resourceNode) + "</color>");
                    }
                }

                //Select project button
                if (GUILayout.Button("Select Project") && OnStaticSelected != null)
                {
                    string newStaticName = staticNode.GetValue("name");
                    currentStaticName = newStaticName;
                    currentStaticNode = staticNode;
                    OnStaticSelected(newStaticName, staticNode);
                }

                GUILayout.EndVertical();
            }

            GUILayout.EndVertical();
        }
    }
}
