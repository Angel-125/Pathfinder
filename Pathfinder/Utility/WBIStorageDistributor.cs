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
    public enum EDistributionModes
    {
        DistributionModeOff,
        DistributionModeDistributor,
        DistributionModeConsumer
    }

    public class WBIStorageDistributor : WBIResourceDistributor
    {
        [KSPField(isPersistant = true)]
        public string distributionModeStr = EDistributionModes.DistributionModeDistributor.ToString();

        public EDistributionModes distributionMode = EDistributionModes.DistributionModeDistributor;

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Mode")]
        public void ToggleDistributionMode()
        {
            switch (distributionMode)
            {
                case EDistributionModes.DistributionModeDistributor:
                    distributionMode = EDistributionModes.DistributionModeConsumer;
                    break;

                case EDistributionModes.DistributionModeConsumer:
                    distributionMode = EDistributionModes.DistributionModeOff;
                    break;

                case EDistributionModes.DistributionModeOff:
                default:
                    distributionMode = EDistributionModes.DistributionModeDistributor;
                    break;
            }

            setDistributionModeGUI();
        }

        protected void setDistributionModeGUI()
        {
            switch (distributionMode)
            {
                case EDistributionModes.DistributionModeDistributor:
                    Events["ToggleDistributionMode"].guiName = "Dist. Mode: Consumer";
                    break;

                case EDistributionModes.DistributionModeConsumer:
                    Events["ToggleDistributionMode"].guiName = "Distribution Off";
                    break;

                case EDistributionModes.DistributionModeOff:
                default:
                    Events["ToggleDistributionMode"].guiName = "Dist. Mode: Distributor";
                    break;
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            Fields["distributeResources"].guiActive = false;
            Fields["distributeResources"].guiActiveEditor = false;

            setDistributionModeGUI();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            distributionMode = (EDistributionModes)Enum.Parse(typeof(EDistributionModes), distributionModeStr);
        }

        public override void OnSave(ConfigNode node)
        {
            distributionModeStr = distributionMode.ToString();

            base.OnSave(node);
        }

        public override void GetResourcesToDistribute(List<PartResource> sharedResources, List<PartResource> requiredResources)
        {
            //Clear the lists passed in.
            sharedResources.Clear();
            requiredResources.Clear();

            //Now go through our resource list and divide them up between the two lists
            foreach (PartResource resource in this.part.Resources)
            {
                //If the resource is locked then move to the next resource.
                if (resource.isTweakable == false)
                    continue;

                //Add to the appropriate list
                switch (distributionMode)
                {
                    case EDistributionModes.DistributionModeDistributor:
                        sharedResources.Add(resource);
                        break;

                    case EDistributionModes.DistributionModeConsumer:
                        requiredResources.Add(resource);
                        break;

                    case EDistributionModes.DistributionModeOff:
                    default:
                        break;
                }
            }
        }

        #region IOpsView
        public override void SetContextGUIVisible(bool isVisible)
        {
            base.SetContextGUIVisible(isVisible);

            Events["ToggleDistributionMode"].guiActive = isVisible;
            Events["ToggleDistributionMode"].guiActiveEditor = isVisible;
        }

        public override void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });
            distributeResources = GUILayout.Toggle(distributeResources, "Enable resource distribution");
            if (GUILayout.Button(Events["ToggleDistributionMode"].guiName))
                ToggleDistributionMode();
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        #endregion

    }
}
