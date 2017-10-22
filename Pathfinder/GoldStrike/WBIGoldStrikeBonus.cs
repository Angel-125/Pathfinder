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
    //Provides a bonus to prospecting for Gold Strike nodes. It costs a bit of science data to receive the bonus.
    //Science data comes from a science data source, such as the Ranch House's data generator.
    [KSPModule("Gold Strike Bonus")]
    public class WBIGoldStrikeBonus : PartModule, IOpsView
    {
        //Toggle to indicate whether or not to accumulate prospecting data
        [KSPField(guiName = "Collect Prospecting Data", isPersistant = true, guiActiveEditor = true, guiActive = true)]
        [UI_Toggle(enabledText = "Yes", disabledText = "No")]
        public bool accumulateData = true;

        //Geology data accumulated that can be spent to increase the chances of finding a Gold Strike node.
        [KSPField(isPersistant = true, guiActive = true, guiName = "Prospecting Data", guiUnits = "Mits")]
        public float prospectingDataAmount = 0f;

        [KSPField(isPersistant = true)]
        public float dataCostPerBonus = 25;

        [KSPField()]
        public float bonusValue = 1.0f;

        [KSPField()]
        public bool requiresFullCrew = true;

        //Used for unloaded vessels
        [KSPField(isPersistant = true)]
        public float calculatedBonus = 1.0f;

        static GUIStyle opsWindowStyle = null;
        GUILayoutOption[] opsWindowOptions = new GUILayoutOption[] { GUILayout.Height(480) };
        Vector2 opsWindowPos = new Vector2();

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            calculatedBonus = Bonus;
        }

        public override string GetInfo()
        {
            StringBuilder infoBuilder = new StringBuilder();

            infoBuilder.AppendLine(string.Format("Gives {0:f1} prospecting bonus per {1:f1} Mits.", bonusValue, dataCostPerBonus));
            if (requiresFullCrew)
                infoBuilder.AppendLine("Bonus reduced if the part isn't fully staffed.");

            return infoBuilder.ToString();
        }

        public void AddData(float amount)
        {
            if (!accumulateData)
                return;

            prospectingDataAmount += amount;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
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

        #region IOpsView
        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Gold Strike");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            if (opsWindowStyle == null)
                opsWindowStyle = new GUIStyle(GUI.skin.textArea);

            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(opsWindowPos, opsWindowStyle, opsWindowOptions);

            accumulateData = GUILayout.Toggle(accumulateData, "Accumulate Prospecting Data");
            GUILayout.Label(string.Format("<color=white>Next prospect bonus: +{0:f2}</color>", (prospectingDataAmount / dataCostPerBonus) * bonusValue));

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            Fields["accumulateData"].guiActive = isVisible;
        }

        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }
        #endregion
    }
}
