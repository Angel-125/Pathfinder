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
    internal class SciLabOpsWindow : Window<SciLabOpsWindow>
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
            GUILayout.BeginVertical();

            if (HighLogic.LoadedSceneIsEditor)
            {
                GUILayout.Label("<color=yellow>Your Mobile Processing Lab is working. However, there's nothing to do in the editor.</color>");
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
            if (GUILayout.Button(scienceIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
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
                if (GUILayout.Button(publishIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
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
                if (GUILayout.Button(sellIcon, new GUILayoutOption[] { GUILayout.Width(64), GUILayout.Height(64) }))
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
            GUILayout.Label("<color=white><b>" + converter.Fields["status"].guiName + "</b>: " + converter.status + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>Data: </b>" + converter.datString + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label("<color=white><b>Rate: </b>" + converter.rateString + "</color>");
            GUILayout.EndScrollView();

            GUILayout.BeginScrollView(new Vector2(0, 0));
            GUILayout.Label(new GUIContent("<color=lightBlue><b> Science: </b>" + sciLab.storedScience * converter.reputationPerData + "</color>", scienceIconWhite),
                new GUILayoutOption[] { GUILayout.Height(24) });
            GUILayout.EndScrollView();

            if (HighLogic.CurrentGame.Mode == Game.Modes.CAREER)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0));
                GUILayout.Label(new GUIContent("<color=yellow><b> Reputation: </b>" + sciLab.storedScience * converter.reputationPerData + "</color>", publishIconWhite),
                    new GUILayoutOption[] { GUILayout.Height(24) });
                GUILayout.EndScrollView();

                GUILayout.BeginScrollView(new Vector2(0, 0));
                GUILayout.Label(new GUIContent("<b> Funds: </b>" + sciLab.storedScience * converter.fundsPerData, sellIconWhite), new GUILayoutOption[] { GUILayout.Height(24) });
                GUILayout.EndScrollView();
            }

            GUILayout.EndVertical();
        }
    }
}
