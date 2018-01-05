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
    public class WBIPackingBox : WBIMultipurposeLab, IModuleInfo
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

            setManageOpsButtonVisible();

            if (isDeployed)
            {
                SetStaticAttach();
                Events["showAssemblyRequirements"].active = false;
            }
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
                    Events["showAssemblyRequirements"].active = false;
                }
                else if (!anim.isPlaying && staticAttachOnDeploy && isDeployed && !this.part.PermanentGroundContact)
                {
                    SetStaticAttach();
                    Events["showAssemblyRequirements"].active = false;
                }
            }
        }

        public void SetStaticAttach(bool isAttached = true)
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
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

            Events["ReconfigureStorage"].guiActive = fieldReconfigurable;
            Events["ReconfigureStorage"].guiActiveUnfocused = fieldReconfigurable;
            Events["ReconfigureStorage"].guiActiveEditor = fieldReconfigurable;

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
    }
}
