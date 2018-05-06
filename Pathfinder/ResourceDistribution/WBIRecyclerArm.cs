using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;
using KSP.UI.Screens;
using KSP.Localization;


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
    public class WBIRecyclerArm : ModuleAnimateGeneric
    {
        [KSPField]
        public string primaryRecycleResource = "Equipment";

        [KSPField]
        public string secondaryRecyceResource = "MaterialKits";

        [KSPField(guiActive = true, guiActiveEditor = true, guiName = "Resource Mode")]
        public string recycleMode = "Equipment";

        [KSPField(isPersistant = true)]
        public bool usePrimaryResource = true;

        [KSPField]
        public float recyclePercent = 0.5f;

        public bool IsActivated;

        protected bool emittersEnabled;
        protected PartResourceDefinition def;
        protected BaseEvent toggleEvent;
        protected string recycleResource = "Equipment";

        protected void Log(string message)
        {
            Debug.Log("[WBIRecylerArm] - " + message);
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIRecylerArm] - " + message);
            }
        }

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Toggle Recycle Mode")]
        public void ToggleRecycleMode()
        {
            usePrimaryResource = !usePrimaryResource;

            setupRecycleResource();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //Setup recycle resource
            setupRecycleResource();

            //Get the toggle event
            toggleEvent = Events["Toggle"];
        }

        public override void OnUpdate()
        {
            base.OnUpdate();
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            if (aniState != animationStates.LOCKED)
                return;

            //Get activation state
            if (toggleEvent.guiName == startEventGUIName)
                IsActivated = false;
            else
                IsActivated = true;

            //Determine emitter state
            if (IsActivated && !emittersEnabled)
            {
                emittersEnabled = true;
                setupEmitters();
            }
            else if (!IsActivated && emittersEnabled)
            {
                emittersEnabled = false;
                setupEmitters();
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            if (collider.attachedRigidbody == null || !collider.CompareTag("Untagged"))
                return;
            if (!IsActivated)
                return;

            //Get the part that collided with the trigger
            Part collidedPart = collider.attachedRigidbody.GetComponent<Part>();
            if (collidedPart == null)
                return;

            //Recycle the part
            recyclePart(collidedPart);
        }

        protected void setupEmitters()
        {
            KSPParticleEmitter emitter;
            KSPParticleEmitter[] emitters = part.GetComponentsInChildren<KSPParticleEmitter>();
            if (emitters == null)
                return;

            for (int index = 0; index < emitters.Length; index++)
            {
                emitter = emitters[index];

                emitter.emit = emittersEnabled;
                emitter.enabled = emittersEnabled;
            }
        }

        protected void setupRecycleResource()
        {
            if (usePrimaryResource)
                recycleResource = primaryRecycleResource;
            else
                recycleResource = secondaryRecyceResource;

            def = ResourceHelper.DefinitionForResource(recycleResource);

            recycleMode = def.displayName;
        }

        protected void recyclePart(Part doomed)
        {
            //Get the total units of the recycle resource
            double totalRecycleUnits = (doomed.mass / def.density) * recyclePercent;

            //Shut off any resource distribution
            WBIResourceDistributor distributor = doomed.FindModuleImplementing<WBIResourceDistributor>();
            if (distributor != null)
            {
                distributor.isParticipating = false;
                distributor.distribution = EDistributionModes.DistributionModeOff;
            }

            //Find any resources that the part has.
            List<DistributedResource> scrappedResources = new List<DistributedResource>();
            PartResource[] doomedResources = doomed.Resources.ToArray();
            string recycleMessage;
            for (int index = 0; index < doomedResources.Length; index++)
            {
                if (doomedResources[index].amount > 0.0001f)
                {
                    recycleMessage = string.Format(WBIPartScrapper.kResourceRecycled, doomedResources[index].amount, doomedResources[index].resourceName);
                    scrappedResources.Add(new DistributedResource(doomedResources[index].resourceName, doomedResources[index].amount, recycleMessage));
                }
            }

            //Now distribute the recycled resources
            recycleMessage = string.Format(WBIPartScrapper.kResourceRecycled, totalRecycleUnits, recycleResource);
            scrappedResources.Add(new DistributedResource(recycleResource, totalRecycleUnits, recycleMessage));
            int totalResources = scrappedResources.Count;
            double amountRecycled = 0;
            double totalAmountRecycled = 0f;
            DistributedResource scrappedResource;
            int totalVessels = FlightGlobals.VesselsLoaded.Count;
            Vessel currentVessel;
            for (int index = 0; index < totalResources; index++)
            {
                //Setup
                totalAmountRecycled = 0;
                scrappedResource = scrappedResources[index];

                //Skip ElectricCharge
                if (scrappedResource.resourceName == "ElectricCharge")
                    continue;

                //Priority goes to the vessel that owns the recycler arm.
                currentVessel = this.part.vessel;
                amountRecycled = currentVessel.rootPart.RequestResource(scrappedResource.resourceName, -scrappedResource.amount);
                scrappedResource.amount += amountRecycled;
                Log("Arm owner recycled " + Math.Abs(amountRecycled) + " units of " + scrappedResource.resourceName);
                Log("Units remaining: " + scrappedResource.amount);

                //Zero out the resource if needed.
                if (scrappedResource.amount < 0.001f)
                    scrappedResource.amount = 0f;

                //Record what we recycled.
                totalAmountRecycled += amountRecycled;

                //Now go through every loaded vessel and hand out the resource.
                for (int vesselIndex = 0; vesselIndex < totalVessels; vesselIndex++)
                {
                    //Skip the distribution if we're out of the resource
                    if (scrappedResource.amount == 0f)
                        break;

                    //Get the vessel that we'll distribute the resource to.
                    currentVessel = FlightGlobals.VesselsLoaded[vesselIndex];

                    //Skip the arm's vessel
                    if (currentVessel == this.part.vessel)
                        continue;

                    //Skip the vessel if it's not in range
                    if ((Vector3.Distance(vessel.GetWorldPos3D(), FlightGlobals.ActiveVessel.GetWorldPos3D()) / WBIDistributionManager.kDefaultDistributionRange) > 1.0f)
                    {
                        Log("Vessel " + currentVessel.vesselName + " not in range");
                        continue;
                    }

                    //Hand out the resource.
                    amountRecycled = currentVessel.rootPart.RequestResource(scrappedResource.resourceName, -scrappedResource.amount);
                    scrappedResource.amount += amountRecycled;
                    Log("Nearby vessel recycled " + Math.Abs(amountRecycled) + " units of " + scrappedResource.resourceName);
                    Log("Units remaining: " + scrappedResource.amount);

                    //Zero out the resource if needed.
                    if (scrappedResource.amount < 0.001f)
                        scrappedResource.amount = 0f;

                    //Record what we recycled.
                    totalAmountRecycled += amountRecycled;
                }

                //Report what we recycled.
                if (!string.IsNullOrEmpty(scrappedResource.message))
                    ScreenMessages.PostScreenMessage(scrappedResource.message, 3.0f, ScreenMessageStyle.UPPER_LEFT);
            }

            //Finally, poof the part.
            ScreenMessages.PostScreenMessage(string.Format(WBIPartScrapper.kPartRecycled, doomed.partInfo.title), WBIPartScrapper.kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
            doomed.explosionPotential = WBIPartScrapper.kExplosionPotential;
            doomed.explode();
        }
    }
}
