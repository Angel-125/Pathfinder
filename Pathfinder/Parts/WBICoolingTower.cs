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
    [KSPModule("Cooling Tower")]
    class WBICoolingTower : WBIInflatablePartModule, IOpsView
    {
        ModuleActiveRadiator radiator;
        KSPParticleEmitter emitter;

        [KSPField]
        public string coolantResource = "Water";

        [KSPField]
        public float dumpRate = 1.0f;

        [KSPField(guiActive = true)]
        public string Status;

        [KSPField(isPersistant = true)]
        public bool isCooling = true;

        [KSPField(isPersistant = true)]
        public bool isOpenCycle;

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 3.0f, guiName = "Toggle Open Cycle")]
        public void ToggleOpenCycle()
        {
            isOpenCycle = !isOpenCycle;
            if (isOpenCycle)
                this.Events["ToggleOpenCycle"].guiName = "Open Cycle Off";
            else
                this.Events["ToggleOpenCycle"].guiName = "Open Cycle On";
        }

        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 3.0f)]
        public void ToggleCooling()
        {
            isCooling = !isCooling;

            radiator.isEnabled = isDeployed && isCooling;
            radiator.enabled = isDeployed && isCooling;
            emitter.emit = isDeployed && isCooling;
            emitter.enabled = isDeployed && isCooling;

            UpdateStatus();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            radiator = this.part.FindModuleImplementing<ModuleActiveRadiator>();
            radiator.isEnabled = isDeployed && isCooling;
            radiator.enabled = isDeployed && isCooling;

            emitter = this.part.GetComponentInChildren<KSPParticleEmitter>();
            if (emitter != null)
            {
                emitter.emit = isDeployed && isCooling;
                emitter.enabled = isDeployed && isCooling;
            }

            UpdateStatus();

            if (isOpenCycle)
                this.Events["ToggleOpenCycle"].guiName = "Open Cycle Off";
            else
                this.Events["ToggleOpenCycle"].guiName = "Open Cycle On";
        }

        public override void OnFixedUpdate()
        {
            base.OnFixedUpdate();

            if (isOpenCycle)
                dumpCoolant();
        }

        protected void dumpCoolant()
        {
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            PartResourceDefinition resourceDef;
            double coolantToDump = dumpRate * TimeWarp.fixedDeltaTime;
            double coolantDumped = 0;
            double thermalEnergyCoolant = 0;

            //The the resource definition
            resourceDef = definitions[coolantResource];

            //Now calculate the resource amount dumped and the thermal energy of that slug of resource.
            coolantDumped = this.part.RequestResource(resourceDef.id, coolantToDump * dumpRate, ResourceFlowMode.ALL_VESSEL);
            thermalEnergyCoolant = this.part.temperature * this.part.resourceThermalMass * coolantDumped;

            //Practice conservation of energy...
            if (coolantDumped > 0.001)
                this.part.AddThermalFlux(-thermalEnergyCoolant);
        }

        public void UpdateStatus()
        {
            if (isCooling)
            {
                Status = "Cooling";
                Events["ToggleCooling"].guiName = "Turn cooling off";
            }

            else
            {
                Status = "Off";
                Events["ToggleCooling"].guiName = "Turn cooling on";
            }
        }

        public override void ToggleInflation()
        {
            base.ToggleInflation();

            radiator.isEnabled = isDeployed;
            radiator.enabled = isDeployed;

            if (emitter != null)
            {
                emitter.emit = isDeployed;
                emitter.enabled = isDeployed;
            }

            isCooling = isDeployed;
            UpdateStatus();
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Cooling");
            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("<color=white>Status: " + Status + "</color>");
            
            if (GUILayout.Button(Events["ToggleCooling"].guiName))
                ToggleCooling();

            if (GUILayout.Button(Events["ToggleOpenCycle"].guiName))
                ToggleOpenCycle();

            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            /*
            this.Events["ToggleOpenCycle"].guiActive = isVisible;
            this.Events["ToggleOpenCycle"].guiActiveEditor = isVisible;
            this.Events["ToggleCooling"].guiActive = isVisible;
            this.Events["ToggleCooling"].guiActiveEditor = isVisible;
            this.Fields["Status"].guiActive = isVisible;
             */
        }
        #endregion
    }
}
