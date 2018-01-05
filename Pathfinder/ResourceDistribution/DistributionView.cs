using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

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
    public delegate void RebuildCacheDelegate();
    public delegate void SetParticipationDelegate(bool isEnabled);

    public class DistributionView : Dialog<DistributionView>
    {
        public RebuildCacheDelegate rebuildCache;
        public SetParticipationDelegate setParticipation;

        public bool isParticipating;
        public Dictionary<string, EDistributionModes> distributionMap = null;
        Vector2 scrollPos = new Vector2();
        public Part part;
        string[] distributeOptions = new string[] {"Ignore", "Share", "Consume" };
        GUILayoutOption[] gridOptions = new GUILayoutOption[] { GUILayout.Width(250) };
        GUILayoutOption[] scrollOptions = new GUILayoutOption[] { GUILayout.Height(480) };
        Vector2 scrollPanePos = new Vector2();
        GUIStyle scrollStyle;

        public DistributionView(string title = "Distribute Resources") :
            base(title, 800, 600)
        {
            WindowTitle = title;
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);
            if (!newValue && rebuildCache != null)
            {
                WBIDistributionManager.Log("Calling rebuildCache");
                rebuildCache();
            }
            if (!newValue)
            {
                WBIDistributionManager.Log("Calling distribute resources");
                WBIDistributionManager.Instance.DistributeResourcesImmediately();
            }
        }

        public void DrawView()
        {
            int buttonIndex;
            PartResourceDefinition definition;
            GUILayout.BeginVertical();

            if (scrollStyle == null)
                scrollStyle = new GUIStyle(GUI.skin.textArea);
            GUILayout.BeginScrollView(scrollPanePos, scrollStyle, scrollOptions);

            //Do we participate?
            isParticipating = GUILayout.Toggle(isParticipating, "Participate in resource distribution");

            //Participation level of individual resources
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            string[] resourceKeys = distributionMap.Keys.ToArray<string>();
            string key;
            for (int index = 0; index < resourceKeys.Length; index++)
            {
                key = resourceKeys[index];
                definition = ResourceHelper.DefinitionForResource(key);
                if (distributionMap[key] != EDistributionModes.DistributionModeRequired)
                {
                    GUILayout.BeginHorizontal();
                    buttonIndex = (int)distributionMap[key];
                    buttonIndex = GUILayout.SelectionGrid(buttonIndex, distributeOptions, distributeOptions.Length, gridOptions);
                    distributionMap[key] = (EDistributionModes)buttonIndex;
                    if (!string.IsNullOrEmpty(definition.displayName))
                        GUILayout.Label("<color=white>" + definition.displayName + "</color>");
                    else
                        GUILayout.Label("<color=white>" + key + "</color>");
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        protected override void DrawWindowContents(int windowId)
        {
            DrawView();
            if (setParticipation != null)
                setParticipation(isParticipating);
        }
    }
}
