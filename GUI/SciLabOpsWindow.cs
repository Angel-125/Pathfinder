using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using WBIResources;

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
    internal class SciLabOpsWindow : Dialog<SciLabOpsWindow>
    {
        const string kTransmitResearch = "<color=lightBlue>Transmit research (Science)</color>";
        const string kPublishResearch = "<color=yellow>Publish research (Reputation)</color>";
        const string kSellResearch = "Sell research (Funds)";

        public Part part;
        bool scienceHighlighted = false;
        bool publishHighlighted = false;
        bool sellHighlighted = false;
        Texture publishIconWhite;
        Texture sellIconWhite;
        Texture scienceIconWhite;
        Texture publishIconBlack;
        Texture sellIconBlack;
        Texture scienceIconBlack;
        Texture publishIcon;
        Texture sellIcon;
        Texture scienceIcon;
        public PartModule converter = null;
        protected ModuleScienceLab sciLab = null;
        public ModuleScienceContainer scienceContainer = null;

        public SciLabOpsWindow(string title) :
            base(title, 600, 330)
        {
            Resizable = false;

            publishIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIPublishWhite", false);
            sellIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBISellWhite", false);
            scienceIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIScienceWhite", false);

            publishIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIPublish", false);
            sellIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBISell", false);
            scienceIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIScience", false);

            publishIcon = publishIconBlack;
            sellIcon = sellIconBlack;
            scienceIcon = scienceIconBlack;
        }

        public void FindPartModules()
        {
            if (converter == null)
            {
                converter = findModuleByName("WBIScienceConverter");
                sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();
            }
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);
        }

        public void DrawOpsWindow()
        {
            FindPartModules();

            GUILayout.BeginVertical();

            if (HighLogic.LoadedSceneIsEditor)
            {
                GUILayout.Label("<color=yellow>Your Mobile Processing Lab is working. However, there's nothing to do in the editor.</color>");
                GUILayout.EndVertical();
                return;
            }

            else if (converter == null)
            {
                GUILayout.Label("<color=yellow>Can't seem to find WBIScienceConverter.</color>");
                GUILayout.EndVertical();
                return;
            }

            else if (sciLab == null)
            {
                GUILayout.Label("<color=yellow>Can't seem to find ModuleScienceLab.</color>");
                GUILayout.EndVertical();
                return;
            }

            else if (scienceContainer == null)
            {
                GUILayout.Label("<color=yellow>Can't seem to find ModuleScienceContainer.</color>");
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginHorizontal();
            drawStatus();
            drawCnCButtons();
            GUILayout.EndHorizontal();
            drawTransmitButtons();
            GUILayout.EndVertical();
        }

        protected override void DrawWindowContents(int windowId)
        {
            DrawOpsWindow();
        }

        protected void drawCnCButtons()
        {
            int dataCount = scienceContainer.GetScienceCount();

            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(new Vector2(0, 0));

            if (dataCount > 0)
            {
                if (GUILayout.Button("Review [" + dataCount.ToString() + "] Data"))
                {
                }
            }

            if (GUILayout.Button("Clean Experiments"))
                sciLab.CleanModulesEvent();

            if (moduleIsActive())
            {
                if (GUILayout.Button(getStringMember("StopActionName", "Stop Research")))
                    invokeConverterMethod("StopResourceConverter");
            }
            else
            {
                if (GUILayout.Button(getStringMember("StartActionName", "Start Research")))
                    invokeConverterMethod("StartResourceConverter");
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void drawTransmitButtons()
        {
            string message = "";

            GUILayout.BeginHorizontal();

            //Transmit button
            if (GUILayout.Button(scienceIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
                invokeConverterMethod("TransmitResearch");

            if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
            {
                scienceIcon = scienceIconWhite;
                scienceHighlighted = true;
                message = kTransmitResearch;
            }
            else if (scienceHighlighted)
            {
                scienceIcon = scienceIconWhite;
                scienceHighlighted = false;
                message = kTransmitResearch;
            }
            else
            {
                scienceIcon = scienceIconBlack;
            }

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                //Publish button
                if (GUILayout.Button(publishIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
                    invokeConverterMethod("PublishResearch");

                if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    publishIcon = publishIconWhite;
                    publishHighlighted = true;
                    message = kPublishResearch;
                }
                else if (publishHighlighted)
                {
                    publishIcon = publishIconWhite;
                    publishHighlighted = false;
                    message = kPublishResearch;
                }
                else
                {
                    publishIcon = publishIconBlack;
                }

                //Sell button
                if (GUILayout.Button(sellIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
                    invokeConverterMethod("SellResearch");

                if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    sellIcon = sellIconWhite;
                    sellHighlighted = true;
                    message = kSellResearch;
                }
                else if (sellHighlighted)
                {
                    sellIcon = sellIconWhite;
                    sellHighlighted = false;
                    message = kSellResearch;
                }
                else
                {
                    sellIcon = sellIconBlack;
                }
            }

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            GUILayout.Label(message);
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();

            GUILayout.EndHorizontal();
        }

        protected void drawStatus()
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>Status: </b>" + sciLab.statusText + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>" + getFieldGuiName("status", "Status") + "</b>: " + getStringMember("status", "") + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>Data: </b>" + getStringMember("datString", "") + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>Rate: </b>" + getStringMember("rateString", "") + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label(new GUIContent("<color=lightBlue><b> Science: </b>" + sciLab.storedScience * getFloatMember("reputationPerData", 0f) + "</color>", scienceIconWhite),
                new GUILayoutOption[] { GUILayout.Height(24) });
            GUILayout.EndScrollView();

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0));
                GUILayout.Label(new GUIContent("<color=yellow><b> Reputation: </b>" + sciLab.storedScience * getFloatMember("reputationPerData", 0f) + "</color>", publishIconWhite),
                    new GUILayoutOption[] { GUILayout.Height(24) });
                GUILayout.EndScrollView();

                GUILayout.BeginScrollView(new Vector2(0, 0));
                GUILayout.Label(new GUIContent("<b> Funds: </b>" + sciLab.storedScience * getFloatMember("fundsPerData", 0f), sellIconWhite), new GUILayoutOption[] { GUILayout.Height(24) });
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }

        /// <summary>
        /// Finds a part module by class name without requiring the module's compile-time type.
        /// This keeps Pathfinder buildable when optional science converter sources are absent.
        /// </summary>
        /// <param name="moduleName">The class name of the module to find.</param>
        /// <returns>The matching PartModule, or null if none is installed on the part.</returns>
        private PartModule findModuleByName(string moduleName)
        {
            if (part == null || part.Modules == null)
                return null;

            for (int index = 0; index < part.Modules.Count; index++)
            {
                PartModule module = part.Modules[index];
                if (module != null && module.moduleName == moduleName)
                    return module;
            }

            return null;
        }

        /// <summary>
        /// Invokes a parameterless method on the optional science converter, if present.
        /// </summary>
        /// <param name="methodName">The method to invoke.</param>
        private void invokeConverterMethod(string methodName)
        {
            if (converter == null)
                return;

            System.Reflection.MethodInfo method = converter.GetType().GetMethod(methodName);
            if (method != null)
                method.Invoke(converter, null);
        }

        /// <summary>
        /// Checks whether the optional science converter is active.
        /// </summary>
        /// <returns>True if the converter reports that it is active.</returns>
        private bool moduleIsActive()
        {
            if (converter == null)
                return false;

            System.Reflection.MethodInfo method = converter.GetType().GetMethod("ModuleIsActive");
            if (method == null)
                return false;

            object result = method.Invoke(converter, null);
            return result is bool && (bool)result;
        }

        /// <summary>
        /// Retrieves a string field or property from the optional science converter.
        /// </summary>
        /// <param name="memberName">The field or property name to read.</param>
        /// <param name="defaultValue">The value to return when the member is unavailable.</param>
        /// <returns>The member value as a string.</returns>
        private string getStringMember(string memberName, string defaultValue)
        {
            object value = getMemberValue(memberName);
            return value != null ? value.ToString() : defaultValue;
        }

        /// <summary>
        /// Retrieves a float field or property from the optional science converter.
        /// </summary>
        /// <param name="memberName">The field or property name to read.</param>
        /// <param name="defaultValue">The value to return when the member is unavailable.</param>
        /// <returns>The member value as a float.</returns>
        private float getFloatMember(string memberName, float defaultValue)
        {
            object value = getMemberValue(memberName);
            if (value == null)
                return defaultValue;

            float floatValue;
            if (float.TryParse(value.ToString(), out floatValue))
                return floatValue;

            return defaultValue;
        }

        /// <summary>
        /// Retrieves a PAW field display name from the optional science converter.
        /// </summary>
        /// <param name="fieldName">The field name to query.</param>
        /// <param name="defaultValue">The label to use when the field is unavailable.</param>
        /// <returns>The localized field display name.</returns>
        private string getFieldGuiName(string fieldName, string defaultValue)
        {
            if (converter == null || converter.Fields == null)
                return defaultValue;

            try
            {
                BaseField field = converter.Fields[fieldName];
                return field != null ? field.guiName : defaultValue;
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Retrieves a field or property value from the optional science converter.
        /// </summary>
        /// <param name="memberName">The field or property name to read.</param>
        /// <returns>The member value, or null if it is unavailable.</returns>
        private object getMemberValue(string memberName)
        {
            if (converter == null)
                return null;

            Type converterType = converter.GetType();
            System.Reflection.FieldInfo field = converterType.GetField(memberName);
            if (field != null)
                return field.GetValue(converter);

            System.Reflection.PropertyInfo property = converterType.GetProperty(memberName);
            if (property != null)
                return property.GetValue(converter, null);

            return null;
        }
    }
}
