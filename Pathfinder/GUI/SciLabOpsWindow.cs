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
    internal class SciLabOpsWindow : Dialog<SciLabOpsWindow>
    {
        const string kTransmitResearch = "<color=lightBlue>Transmit research (Science)</color>";
        const string kPublishResearch = "<color=yellow>Publish research (Reputation)</color>";
        const string kSellResearch = "Sell research (Funds)";
        const string kDistributeResearch = "<color=white>Distribute data to science labs, prospecting data collectors, and mass driver trajectory computers.</color>";

        public Part part;
        bool scienceHighlighted = false;
        bool publishHighlighted = false;
        bool sellHighlighted = false;
        bool distributeHighlighted = false;
        Texture publishIconWhite;
        Texture sellIconWhite;
        Texture scienceIconWhite;
        Texture distributeIconWhite;
        Texture publishIconBlack;
        Texture sellIconBlack;
        Texture scienceIconBlack;
        Texture distributeIconBlack;
        Texture publishIcon;
        Texture sellIcon;
        Texture scienceIcon;
        Texture distributeIcon;
        GUILayoutOption[] buttonOptions = new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) };
        GUILayoutOption[] heightOptions = new GUILayoutOption[] { GUILayout.Height(24)};
        Vector2 panelPosition = new Vector2(0, 0);
        public WBIScienceConverter converter = null;
        protected ModuleScienceLab sciLab = null;
        public ModuleScienceContainer scienceContainer = null;

        public SciLabOpsWindow(string title) :
            base(title, 600, 330)
        {
            Resizable = false;

            publishIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIPublishWhite", false);
            sellIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBISellWhite", false);
            scienceIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIScienceWhite", false);
            distributeIconWhite = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/PathfinderApp", false);

            publishIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIPublish", false);
            sellIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBISell", false);
            scienceIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/WBIScience", false);
            distributeIconBlack = GameDatabase.Instance.GetTexture("WildBlueIndustries/Pathfinder/Icons/PathfinderAppBlack", false);

            publishIcon = publishIconBlack;
            sellIcon = sellIconBlack;
            scienceIcon = scienceIconBlack;
            distributeIcon = distributeIconWhite;
        }

        public void FindPartModules()
        {
            if (converter == null)
            {
                converter = this.part.FindModuleImplementing<WBIScienceConverter>();
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
            if (WBIPathfinderScenario.showDebugLog)
            {
                if (GUILayout.Button("+50 data"))
                    converter.Lab.dataStored += 50.0f;
            }
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
            GUILayout.BeginScrollView(panelPosition);

            if (dataCount > 0)
            {
                if (GUILayout.Button("Review [" + dataCount.ToString() + "] Data"))
                {
                }
            }

            if (GUILayout.Button("Clean Experiments"))
                sciLab.CleanModulesEvent();

            if (converter.ModuleIsActive())
            {
                if (GUILayout.Button(converter.StopActionName))
                    converter.StopResourceConverter();
            }
            else
            {
                if (GUILayout.Button(converter.StartActionName))
                    converter.StartResourceConverter();
            }

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        protected void drawTransmitButtons()
        {
            string message = "";

            GUILayout.BeginHorizontal();

            //Transmit button
            if (GUILayout.Button(scienceIcon, buttonOptions))
                converter.TransmitResearch();

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
                if (GUILayout.Button(publishIcon, buttonOptions))
                    converter.PublishResearch();

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
                if (GUILayout.Button(sellIcon, buttonOptions))
                    converter.SellResearch();

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

            //Distribute button
            if (sciLab != null)
            {
                if (GUILayout.Button(distributeIcon, buttonOptions))
                {
                    float amount = converter.Lab.dataStored;
                    converter.Lab.dataStored = 0f;
                    float amountDistributed = WBIPathfinderScenario.Instance.DistributeData(amount, this.part.vessel, true, true, true);
                    if (amountDistributed < amount)
                        converter.Lab.dataStored += (amount - amountDistributed);
                }

                if (Event.current.type == EventType.Repaint && GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                {
                    distributeIcon = distributeIconWhite;
                    distributeHighlighted = true;
                    message = kDistributeResearch;
                }
                else if (distributeHighlighted)
                {
                    distributeIcon = distributeIconWhite;
                    distributeHighlighted = false;
                    message = kDistributeResearch;
                }
                else
                {
                    distributeIcon = distributeIconBlack;
                }
            }

            GUILayout.BeginScrollView(panelPosition);
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

            GUILayout.BeginScrollView(panelPosition);
            GUILayout.Label("<color=white><b>Status: </b>" + sciLab.statusText + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(panelPosition);
            GUILayout.Label("<color=white><b>" + converter.Fields["status"].guiName + "</b>: " + converter.status + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(panelPosition);
            GUILayout.Label(string.Format("<color=white><b>Data: </b>{0:f2}/{1:f2}</color>", converter.Lab.dataStored, converter.Lab.dataStorage));
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(panelPosition);
            GUILayout.Label("<color=white><b>Summary:</b> " + converter.DataExpectationSummary(converter.Lab.dataStored) + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(panelPosition);
            GUILayout.Label(new GUIContent("<color=lightBlue><b> Science: </b>" + sciLab.storedScience + "</color>", scienceIconWhite),
                heightOptions);
            GUILayout.EndScrollView();

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                GUILayout.BeginScrollView(panelPosition);
                GUILayout.Label(new GUIContent("<color=yellow><b> Reputation: </b>" + sciLab.storedScience * converter.reputationPerData + "</color>", publishIconWhite),
                    heightOptions);
                GUILayout.EndScrollView();

                GUILayout.BeginScrollView(panelPosition);
                GUILayout.Label(new GUIContent("<b> Funds: </b>" + sciLab.storedScience * converter.fundsPerData, sellIconWhite), heightOptions);
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }
    }
}
