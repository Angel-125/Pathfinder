using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2017, by Michael Billard (Angel-125)
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
    public delegate void OnPackingStateChanged(bool isDeployed);

    [KSPModule("Packing Box")]
    public class WBIPackingBox : WBIMultiConverter, IModuleInfo
    {
        private const string kStaticAttachName = "StaticAttachBody";
        private const string kSettingsWindow = "Settings Window";
        private const string kPartsTip = "Don't want to pay to redecorate? No problem. Just press Mod P (the modifier key defaults to the Alt key on Windows) to open the Settings window and uncheck the option.\r\n\r\n";
        private const string kResourceDistributed = "Distributed {0:f2} units of {1:s}";

        [KSPField]
        public string partToolTip;

        [KSPField]
        public string partToolTipTitle;

        [KSPField]
        public string packingBoxTransform = "PackingBox";

        [KSPField]
        public bool isOneShot = true;

        [KSPField]
        public bool showOpsView = true;

        [KSPField]
        public bool staticAttachOnDeploy = true;

        public event OnPackingStateChanged onPackingStateChanged;
        protected bool isMoving = false;
        GameObject staticAttachObject;
        FixedJoint staticAttachJoint;

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Show Resource Requirements")]
        public void showAssemblyRequirements()
        {
            buildInputList(CurrentTemplateName);
            StringBuilder requirementsList = new StringBuilder();
            string templateName = "";

            //Template name
            if (CurrentTemplate.HasValue("title"))
                templateName = CurrentTemplate.GetValue("title");
            else
                templateName = CurrentTemplate.GetValue("name");
            requirementsList.AppendLine("Configuration: " + templateName);

            //Resource Requirements.
            string[] keys = inputList.Keys.ToArray();
            for (int index = 0; index < keys.Length; index++)
            {
                requirementsList.Append(keys[index]);
                requirementsList.Append(": ");
                requirementsList.AppendLine(string.Format("{0:f2}", inputList[keys[index]]));
            }

            if (keys.Length == 0)
                requirementsList.AppendLine("No resource requirements");

            InfoView infoView = new InfoView();
            infoView.ModuleInfo = requirementsList.ToString();
            infoView.SetVisible(true);
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (HighLogic.LoadedSceneIsEditor)
                this.part.CrewCapacity = 0;

            opsManagerView.WindowTitle = this.part.partInfo.title + " Operations";

            if (string.IsNullOrEmpty(resourcesToKeep))
                resourcesToKeep = "ElectricCharge";

            //Hide/show the packing box if needed.
            setPackingBoxVisible();

            //Lights
            setupLightGUI();

            //If this is a one-shot then hide the animation button.
            if (isOneShot && isDeployed && HighLogic.LoadedSceneIsEditor == false)
                Events["ToggleInflation"].active = false;

            setManageOpsButtonVisible();

            if (isDeployed)
                SetStaticAttach();
        }

        public override void ToggleInflation()
        {
            base.ToggleInflation();
            if (!canDeploy)
                return;
            isMoving = true;

            //Tooltip
            checkAndShowToolTip();

            //Manage Ops button
            setManageOpsButtonVisible();

            //If this is a one-shot then hide the animation button.
            if (isOneShot && HighLogic.LoadedSceneIsEditor == false)
                Events["ToggleInflation"].active = false;

            //Hide/show the packing box if needed.
            setPackingBoxVisible();

            //Lights
            setupLightGUI();

            //Fire events
            if (onPackingStateChanged != null)
                onPackingStateChanged(isDeployed);
        }

        public override List<string> GetButtonLabels()
        {
            List<string> buttonLabels = opsManagerView.GetButtonLabels();

            if (!fieldReconfigurable && buttonLabels.Contains("Config"))
                buttonLabels.Remove("Config");

            return buttonLabels;
        }

        public override void SetContextGUIVisible(bool isVisible)
        {
            base.SetContextGUIVisible(isVisible);
            setManageOpsButtonVisible();
        }

        public override void UpdateContentsAndGui(string templateName)
        {
            base.UpdateContentsAndGui(templateName);

            //Check to see if we've displayed the tooltip for the template.
            //First, we're only interested in deployed modules.
            if (isInflatable && isDeployed == false)
                return;

            //Now check
            checkAndShowToolTip();
        }

        public override void OnUpdate()
        {
            base.OnUpdate();

            //Static attach
            if (!staticAttachOnDeploy)
                return;
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
            if (!isMoving)
                return;
            if (anim != null)
            {
                if (!anim.isPlaying && isMoving)
                {
                    isMoving = false;
                    SetStaticAttach();
                }
                else if (!anim.isPlaying && staticAttachOnDeploy && isDeployed && !this.part.PermanentGroundContact)
                {
                    SetStaticAttach();
                }
            }
        }

        public void SetStaticAttach(bool isAttached = true)
        {
            if (!staticAttachOnDeploy)
                return;
            if (this.part.vessel.situation != Vessel.Situations.LANDED &&
                this.part.vessel.situation != Vessel.Situations.PRELAUNCH)
                return;

            //Set ground contact
            this.part.PermanentGroundContact = true;
//            this.part.vessel.permanentGroundContact = true;

            //Zero velocity
            int partCount = this.part.vessel.parts.Count;
            Rigidbody rigidBody;
            for (int index = 0; index < partCount; index++)
            {
                rigidBody = this.part.vessel.parts[index].Rigidbody;
                if (rigidBody == null)
                    continue;

                rigidBody.velocity *= 0;
                rigidBody.angularVelocity *= 0f;
            }

            //Destroy the static attach if needed
            if (staticAttachJoint != null)
                Destroy(staticAttachJoint);
            if (staticAttachObject != null)
                Destroy(staticAttachObject);

            //Setup static attach object
            staticAttachObject = new GameObject(kStaticAttachName);
            Rigidbody staticRigidBody = staticAttachObject.AddComponent<Rigidbody>();
            staticRigidBody.isKinematic = true;
            staticAttachObject.transform.position = this.part.transform.position;
            staticAttachObject.transform.rotation = this.part.transform.rotation;

            //Setup the attachment joint
            staticAttachJoint = staticAttachObject.AddComponent<FixedJoint>();
            staticAttachJoint.connectedBody = this.part.Rigidbody;
            staticAttachJoint.breakForce = float.MaxValue;
            staticAttachJoint.breakTorque = float.MaxValue;
        }

        protected void setupLightGUI()
        {
            WBILight light = this.part.FindModuleImplementing<WBILight>();
            if (light != null)
                light.Events["ToggleAnimation"].active = isDeployed;

            ModuleColorChanger colorChanger = this.part.FindModuleImplementing<ModuleColorChanger>();
            if (colorChanger != null)
                colorChanger.Events["ToggleEvent"].active = isDeployed;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
        }

        protected void setManageOpsButtonVisible()
        {
            if (!showOpsView)
            {
                Events["ReconfigureStorage"].guiActive = false;
                Events["ReconfigureStorage"].guiActiveUnfocused = false;
                Events["ReconfigureStorage"].guiActiveEditor = false;
                return;
            }

            Events["ReconfigureStorage"].guiActive = isDeployed;
            Events["ReconfigureStorage"].guiActiveUnfocused = isDeployed;
            Events["ReconfigureStorage"].guiActiveEditor = isDeployed;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
        }

        protected void setPackingBoxVisible()
        {
            if (!string.IsNullOrEmpty(packingBoxTransform))
            {
                //Get the targets
                Transform[] targets;
                targets = part.FindModelTransforms(packingBoxTransform);
                if (targets == null)
                {
                    Debug.Log("No targets found for " + packingBoxTransform);
                    return;
                }

                foreach (Transform target in targets)
                {
                    target.gameObject.SetActive(!isDeployed);
                    Collider collider = target.gameObject.GetComponent<Collider>();
                    if (collider != null)
                        collider.enabled = !isDeployed;
                }
            }
        }

        protected override void notEnoughParts()
        {
            base.notEnoughParts();

            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;

            //Add first time for redecoration
            if (scenario.HasShownToolTip(kSettingsWindow) == false)
            {
                scenario.SetToolTipShown(kSettingsWindow);

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kSettingsWindow, kPartsTip);
                toolTipWindow.SetVisible(true);
            }

            //Show the unpack button
            Events["ToggleInflation"].active = true;
        }

        protected override bool canAffordReconfigure(string templateName, bool deflatedModulesAutoPass = true)
        {
            bool canAfford = base.canAffordReconfigure(templateName, deflatedModulesAutoPass);

            //If the vessel can't afford to reconfigure the module, then maybe the distribution manager can help.
            if (canAfford == false)
            {
                canAfford = true;
                ScreenMessages.PostScreenMessage("Checking distributors...", 10.0f);

                string[] keys = inputList.Keys.ToArray();
                string resourceName;
                double distributedAmount;
                for (int index = 0; index < keys.Length; index++)
                {
                    resourceName = keys[index];
                    distributedAmount = WBIDistributionManager.Instance.GetDistributedAmount(resourceName);
                    Log("Distributors have " + distributedAmount + " units of " + resourceName);

                    if (distributedAmount < inputList[resourceName])
                    {
                        ScreenMessages.PostScreenMessage("No active distributors have " + resourceName + " to share. Make sure resource distribution is turned on, and a distributor is sharing " + resourceName + ".", 10.0f);
                        canAfford = false;
                        break;
                    }
                }
            }

            //Add first time for redecoration
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (!canAfford && scenario.HasShownToolTip(kSettingsWindow) == false)
            {
                scenario.SetToolTipShown(kSettingsWindow);

                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(kSettingsWindow, kPartsTip);
                toolTipWindow.SetVisible(true);
            }

            return canAfford;
        }

        protected override bool payPartsCost(int templateIndex)
        {
            bool canAffordCost = base.payPartsCost(templateIndex);

            //Maybe the distribution manager can help?
            if (canAffordCost == false)
            {
                string[] keys = inputList.Keys.ToArray();
                string resourceName;
                double amountObtained;

                for (int index = 0; index < keys.Length; index++)
                {
                    resourceName = keys[index];
                    amountObtained = WBIDistributionManager.Instance.RequestDistributedResource(resourceName, inputList[resourceName], false);
                    if (amountObtained <= 0.00001)
                        return false;
                }
            }
            return true;
        }

        protected override void recoverResourceCost(string resourceName, double recycleAmount)
        {
            double availableStorage = ResourceHelper.GetTotalResourceSpaceAvailable(resourceName, this.part.vessel);

            //Do we have sufficient space in the vessel to store the recycled parts?
            //If not, distrubute what we don't have space for.
            if (availableStorage < recycleAmount)
            {
                double amountRemaining = recycleAmount - availableStorage;
                string recycleMessage;

                //We'll only recycle what we have room to store aboard the vessel.
                recycleAmount = availableStorage;

                //Distribute the rest.
                List<DistributedResource> recycledResources = new List<DistributedResource>();
                recycleMessage = string.Format(kResourceDistributed, amountRemaining, resourceName);
                recycledResources.Add(new DistributedResource(resourceName, amountRemaining, recycleMessage));
                WBIDistributionManager.Instance.DistributeResources(recycledResources);
            }

            //Store what we have space for.
            this.part.RequestResource(resourceName, -recycleAmount, ResourceFlowMode.ALL_VESSEL);
        }

        protected void checkAndShowToolTip()
        {
            //Now we can check to see if the tooltip for the current template has been shown.
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (scenario.HasShownToolTip(CurrentTemplateName) && scenario.HasShownToolTip(getMyPartName()))
                return;

            //Tooltip for the current template has never been shown. Show it now.
            string toolTipTitle = CurrentTemplate.GetValue("toolTipTitle");
            string toolTip = CurrentTemplate.GetValue("toolTip");

            if (string.IsNullOrEmpty(toolTipTitle))
                toolTipTitle = partToolTipTitle;

            //Add the very first part's tool tip.
            if (scenario.HasShownToolTip(getMyPartName()) == false)
            {
                toolTip = partToolTip + "\r\n\r\n" + toolTip;

                scenario.SetToolTipShown(getMyPartName());
            }

            if (string.IsNullOrEmpty(toolTip) == false)
            {
                WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(toolTipTitle, toolTip);
                toolTipWindow.SetVisible(true);

                //Cleanup
                scenario.SetToolTipShown(CurrentTemplateName);
            }
        }

        protected override void hideEditorGUI(PartModule.StartState state)
        {
            base.hideEditorGUI(state);
        }

        public override string GetInfo()
        {
            return "Click the Manage Operations button to change the configuration.";
        }

        public string GetModuleTitle()
        {
            return "Packing Box";
        }

        public string GetPrimaryField()
        {
            return "Inflated Crew Capacity: " + inflatedCrewCapacity.ToString();
        }

        public Callback<Rect> GetDrawModulePanelCallback()
        {
            return null;
        }
    }
}
