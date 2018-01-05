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
    public class WBIPowerGenerator : ModuleBreakableConverter, IOpsView
    {
        [KSPField(guiName = "Power Output", guiActive = true)]
        public string powerOutputDisplay = string.Empty;

        [KSPField()]
        public string efficiencyType;

        [KSPField()]
        public int harvestType;

        protected double ecBaseOutput;
        private string biomeName;
        private int planetID = -1;
        HarvestTypes harvestID;
        protected string efficiencyString = "Efficiency";

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            ResourceRatio[] outputs = outputList.ToArray();
            ResourceRatio output;
            for (int index = 0; index < outputs.Length; index++)
            {
                output = outputs[index];
                if (output.ResourceName == "ElectricCharge")
                {
                    ecBaseOutput = output.Ratio;
                    break;
                }
            }

            CBAttributeMapSO.MapAttribute biome = Utils.GetCurrentBiome(this.part.vessel);

            biomeName = biome.name;

            if (this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.SPLASHED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH)
            {
                harvestID = (HarvestTypes)harvestType;
                planetID = this.part.vessel.mainBody.flightGlobalsIndex;
            }

            float efficiencyModifier = WBIPathfinderScenario.Instance.GetEfficiencyModifier(planetID, biomeName, harvestID, efficiencyType);
            if (this.EfficiencyBonus != efficiencyModifier)
                this.EfficiencyBonus = efficiencyModifier;
        }

        protected override void PostProcess(ConverterResults result, double deltaTime)
        {
            base.PostProcess(result, deltaTime);

            if (!IsActivated)
            {
                powerOutputDisplay = "n/a";
                return;
            }

            //Power output
            if (this.status == null)
                return;

            if (this.status.Contains("load"))
            {
                //Get the numerical value (*somebody* didn't seem to make this convenient to obtain :( )
                powerOutputDisplay = this.status.Substring(0, this.status.IndexOf("%"));
                double load;
                if (double.TryParse(powerOutputDisplay, out load))
                {
                    load = load / 100f;
                    load = load * ecBaseOutput;
                    powerOutputDisplay = string.Format("{0:f2}/sec", load);
                }

                else
                {
                    powerOutputDisplay = "n/a";
                }
            }
            else
            {
                powerOutputDisplay = "n/a";
            }
        }

        public override void StopConverter()
        {
            base.StopConverter();
            powerOutputDisplay = "n/a";
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public virtual List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add(ConverterName);
            return buttonLabels;
        }

        public virtual void SetContextGUIVisible(bool isVisible)
        {
            Fields["powerOutputDisplay"].guiActive = true;
            
        }

        public virtual void DrawOpsWindow(string buttonLabel)
        {
            string absentResource = GetMissingRequiredResource();
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(170));
            if (string.IsNullOrEmpty(absentResource))
                GUILayout.Label("<color=white><b>Status: </b>" + this.status + "</color>");
            else
                GUILayout.Label("<color=white><b>Status: </b>Requires " + absentResource + "</color>");
            GUILayout.Label("<color=white><b>Power Generation: </b>" + powerOutputDisplay + "</color>");
            GUILayout.Label(string.Format("<color=white><b>" + efficiencyString + ": </b>{0:f2}%</color>", this.EfficiencyBonus * 100f));
            GUILayout.EndScrollView();

            if (ModuleIsActive())
            {
                if (GUILayout.Button(StopActionName))
                    StopConverter();
            }

            else if (GUILayout.Button(StartActionName))
            {
                StartConverter();
            }

            GUILayout.EndVertical();
        }
        #endregion

        public string GetMissingRequiredResource()
        {
            PartResourceDefinition definition;
            Dictionary<string, PartResource> resourceMap = new Dictionary<string, PartResource>();

            foreach (PartResource res in this.part.Resources)
            {
                resourceMap.Add(res.resourceName, res);
            }

            //If we have required resources, make sure we have them.
            if (reqList.Count > 0)
            {
                foreach (ResourceRatio resRatio in reqList)
                {
                    //Do we have a definition?
                    definition = ResourceHelper.DefinitionForResource(resRatio.ResourceName);
                    if (definition == null)
                    {
                        return resRatio.ResourceName;
                    }

                    //Do we have the resource aboard?
                    if (resourceMap.ContainsKey(resRatio.ResourceName) == false)
                    {
                        return resRatio.ResourceName;
                    }

                    //Do we have enough?
                    if (resourceMap[resRatio.ResourceName].amount < resRatio.Ratio)
                    {
                        return resRatio.ResourceName;
                    }
                }
            }

            return null;
        }
    }
}
