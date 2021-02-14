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
        public bool staticAttachOnDeploy = true;

        public string managedPartModules = string.Empty;
        public event OnPackingStateChanged onPackingStateChanged;
        GameObject staticAttachObject;
        FixedJoint staticAttachJoint;
        Dictionary<string, double> resourceMaxAmounts = new Dictionary<string, double>();

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Show Resource Requirements")]
        public void showAssemblyRequirements()
        {
            StringBuilder requirementsList = new StringBuilder();
            string templateName = "";

            //If we have a template then be sure to list its requirements.
            //Template name
            if (CurrentTemplate.HasValue("title"))
                templateName = CurrentTemplate.GetValue("title");
            else
                templateName = CurrentTemplate.GetValue("name");
            if (templateName.ToLower() != "empty")
                requirementsList.AppendLine("Configuration: " + templateName);

            //Resource Requirements.
            buildInputList(templateName);
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

            getManagedModuleNames();
            getPartResources();
            updateManagedResources();

            if (HighLogic.LoadedSceneIsEditor)
                this.part.CrewCapacity = 0;

            opsManagerView.WindowTitle = this.part.partInfo.title + " Operations";

            if (string.IsNullOrEmpty(resourcesToKeep))
                resourcesToKeep = "ElectricCharge";

            //Lights
            setupLightGUI();

            setManageOpsButtonVisible();

            if (isDeployed)
            {
                SetStaticAttach();
                Events["showAssemblyRequirements"].active = false;
            }

            updateManagedModules();
        }

        protected void updateManagedResources()
        {
            if (!this.isInflatable)
                return;
            if (resourceMaxAmounts.Keys.Count == 0)
                return;

            int count = resourceMaxAmounts.Keys.Count;
            string resourceName;
            string[] resourceKeys = resourceMaxAmounts.Keys.ToArray();
            double maxAmount = 0;
            for (int index = 0; index < count; index++)
            {
                resourceName = resourceKeys[index];
                maxAmount = resourceMaxAmounts[resourceName];

                if (!this.part.Resources.Contains(resourceName) && isDeployed)
                {
                    this.part.Resources.Add(resourceName, 0, maxAmount, true, true, false, true, PartResource.FlowMode.Both);
                }
                else if (this.part.Resources.Contains(resourceName) && !isDeployed)
                {
                    this.part.Resources.Remove(resourceName);
                }
            }
        }

        protected void updateManagedModules()
        {
            if (!this.isInflatable)
                return;

            //Update the template modules
            int count = this.addedPartModules.Count;
            for (int index = 0; index < count; index++)
            {
                this.addedPartModules[index].moduleIsEnabled = this.isDeployed;
                this.addedPartModules[index].enabled = this.isDeployed;
                this.addedPartModules[index].isEnabled = this.isDeployed;
            }

            //Now update modules that are permanently part of the part config.
            if (string.IsNullOrEmpty(managedPartModules))
                return;

            count = this.part.Modules.Count;
            PartModule module;
            for (int index = 0; index < count; index ++)
            {
                module = this.part.Modules[index];
                if (managedPartModules.Contains(module.ClassName))
                {
                    module.enabled = this.isDeployed;
                    module.isEnabled = this.isDeployed;
                }
            }
        }

        public override void ToggleInflation()
        {
            base.ToggleInflation();
            if (!canDeploy)
                return;

            //Tooltip
            checkAndShowToolTip();

            //Manage Ops button
            setManageOpsButtonVisible();

            //If this is a one-shot then hide the animation button.
            if (isOneShot && HighLogic.LoadedSceneIsEditor == false)
                Events["ToggleInflation"].active = false;

            //Resources required event
            Events["showAssemblyRequirements"].active = false;

            //Lights
            setupLightGUI();

            updateManagedModules();
            updateManagedResources();

            //Fire events
            if (onPackingStateChanged != null)
                onPackingStateChanged(isDeployed);
        }

        public override void OnToggleStateCompleted()
        {
            base.OnToggleStateCompleted();

            if (staticAttachOnDeploy && isDeployed && !this.part.PermanentGroundContact)
                SetStaticAttach();
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

        protected void getManagedModuleNames()
        {
            if (this.part.partInfo.partConfig == null)
                return;
            ConfigNode[] nodes = this.part.partInfo.partConfig.GetNodes("MODULE");
            ConfigNode node = null;
            string moduleName;
            List<string> optionNamesList = new List<string>();

            //Get the config node.
            for (int index = 0; index < nodes.Length; index++)
            {
                node = nodes[index];
                if (node.HasValue("name"))
                {
                    moduleName = node.GetValue("name");
                    if (moduleName == this.ClassName)
                    {
                        if (node.HasNode("MANAGED_MODULES"))
                        {
                            node = node.GetNode("MANAGED_MODULES");
                            string[] moduleNames = node.GetValues("moduleName");
                            managedPartModules = "";
                            for (int nameIndex = 0; nameIndex < moduleNames.Length; nameIndex++)
                                managedPartModules += moduleNames[nameIndex] + ";";
                        }
                        break;
                    }
                }
            }
        }

        protected void getPartResources()
        {
            if (this.part.partInfo.partConfig == null)
                return;
            if (templateNodes != "EMPTY")
                return;
            ConfigNode[] nodes = this.part.partInfo.partConfig.GetNodes("RESOURCE");
            ConfigNode node = null;
            string resourceName = string.Empty;
            double maxAmount = 0;

            resourceMaxAmounts.Clear();
            for (int index = 0; index < nodes.Length; index++)
            {
                node = nodes[index];
                if (node.HasValue("name"))
                    resourceName = node.GetValue("name");
                maxAmount = 0;
                if (node.HasValue("maxAmount"))
                    double.TryParse(node.GetValue("maxAmount"), out maxAmount);
                if (!resourceMaxAmounts.ContainsKey(resourceName))
                    resourceMaxAmounts.Add(resourceName, maxAmount);
            }
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
            Events["ReconfigureStorage"].guiActive = fieldReconfigurable || ShowGUI;
            Events["ReconfigureStorage"].guiActiveUnfocused = fieldReconfigurable || ShowGUI;
            Events["ReconfigureStorage"].guiActiveEditor = fieldReconfigurable || ShowGUI;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
        }

        public override void SetGUIVisible(bool isVisible)
        {
            base.SetGUIVisible(isVisible);
            setManageOpsButtonVisible();
        }

        public override void SetContextGUIVisible(bool isVisible)
        {
            base.SetContextGUIVisible(isVisible);
            setManageOpsButtonVisible();
        }

        protected override void runHeadless(ModuleResourceConverter converter)
        {
            if (ShowGUI)
                base.runHeadless(converter);
        }
    }
}
