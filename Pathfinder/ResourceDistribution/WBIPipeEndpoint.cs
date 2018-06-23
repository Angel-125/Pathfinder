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
    [KSPModule("Pipe Endpoint")]
    public class WBIPipeEndpoint : PartModule, ICanBreak//, IOpsView
    {
        #region Constants
        public const string kNoIdentifier = "kNoIdentifier";
        public const string kNominalStatus = "A-OK";
        public const string kNoECStatus = "Not enough ElectricCharge";
        public const string kOffStatus = "Deactivated";
        public const string kToolTipSend = "Mass drivers let you send resources to and receive resources from other mass drivers. Each delivery costs trajectory data to calculate the trajectory, ElectricCharge to launch the projectile, and LiquidFuel and Oxidizer to make course corrections. This part can generate trajectory data, but you can also obtain data from the Ranch House as well as the Doc Science Lab.";
        public const string kTookTipReceive = "Mass catchers can receive resources launched from mass drivers. Each shipment's resources will be evenly distributed to all parts on the vessel that can hold the resource. Any excess will be lost. Make sure to turn the power on or you won't receive your shipments.";
        public const string kPowerOn = "Turn Power On";
        public const string kPowerOff = "Turn Power Off";
        public const string kItemSkippedMsg = "One or more inventory items could not be delivered due to insufficient storage space.";
        public const float kMessageDuration = 5.0f;
        #endregion

        #region Transfer capability
        /// <summary>
        /// Toggle to indicate whether or not the endpoint is activated.
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool IsActivated = false;

        /// <summary>
        /// Can the pipeline send payloads from orbit to ground?
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool allowOrbitToGround = false;

        /// <summary>
        /// Unique identifier for resource transfer transactions. Marked persistent so that we can access them from an unloaded vessel.
        /// </summary>
        [KSPField(isPersistant = true)]
        public string uniqueIdentifier = kNoIdentifier;

        /// <summary>
        /// Amount of ec/sec required to maintain activation state.
        /// </summary>
        [KSPField]
        public float activationCostEC = 100.0f;

        /// <summary>
        /// These resources cannot be transferred.
        /// </summary>
        [KSPField]
        public string blackListedResources = "ElectricCharge;GeoEnergy;ChargedParticles;LabTime;ScopeTime;ExposureTime;SolarReport;Megajoules;ResourceLode;StoredCharge;SolarWind;VacuumPlasma;WasteHeat";
        #endregion

        #region Payload capability and costs
        /// <summary>
        /// Maximum amount of kinetic energy that the mass driver can generate
        /// K.E. = (0.5 * projectile mass) * (velocity^2)
        /// Calibrated for a 14 metric ton projectile accelerated to 650 m/sec (~Mun orbital velocity)
        /// Factoring in fuel and container mass, and you can put 10 metric tonnes into munar orbit.
        /// Marked persistent so that we can access this from an unloaded vessel.
        /// </summary>
        [KSPField(isPersistant = true)]
        public double maxKineticEnergy = 2958978750;

        /// <summary>
        /// Multiplied by payloadCapacity to get total fuel mass. The required amount of LiquidFuel and Oxidizer is derived from the total fuel mass.
        /// </summary>
        [KSPField]
        public float fuelMassFraction = 0.334f;

        /// <summary>
        /// The dry mass fraction of the delivery vehicle. Multiply the payloadCapacity and fuelMassFraction by this value to get the delivery vehicle's dry mass.
        /// </summary>
        [KSPField]
        public float dryMassFraction = 0.05f;

        /// <summary>
        /// How much trajectory data does it cost per kilometer of distance to the destination.
        /// Example: A destination that is 500km away requires 500 * 0.01 = 5 units of trajectory data.
        /// </summary>
        [KSPField]
        public float dataCostPerKm = 0.01f;

        /// <summary>
        /// Multiplier to the data cost for orbital shots.
        /// Example: A station orbiting at an altitude of 100km requires 100 * .01 * 10 = 10 units of trajectory data.
        /// </summary>
        [KSPField]
        public float orbitalCostMultiplier = 10.0f;

        /// <summary>
        /// How much electric charge to cost per metric ton of the delivery vehicle. Includes the cost of fuel for rocket assist and structural mass.
        /// Example: A vehicle with a 10-tonne capacity and 10 tonnes of fuel masses 21 tonnes. EC cost: 21 * 1000 = 21000.
        /// </summary>
        [KSPField]
        public float electricityCostPerTonne = 1000.0f;

        /// <summary>
        /// Name of the MAC trigger
        /// </summary>
        [KSPField]
        public string macTriggerName = "MACTrigger";
        #endregion

        #region Guidance data accumulation
        /// <summary>
        /// Toggle to indicate whether or not to accumulate trajectory data
        /// </summary>
        [KSPField(guiName = "Collect Guidance Data", isPersistant = true, guiActiveEditor = true, guiActive = true)]
        [UI_Toggle(enabledText = "Yes", disabledText = "No")]
        public bool accumulateData = true;

        /// <summary>
        /// How much trajectory data we have available to spend. Marked persistent so that we can access them from an unloaded vessel.
        /// </summary>
        [KSPField(isPersistant = true, guiActive = true, guiName = "Guidance Data", guiUnits = "Mits", guiFormat = "f2")]
        public float totalGuidanceData = 0f;

        /// <summary>
        /// How much trajectory data to generate per second while the endpoint is active. Requires canSendPayloads = true.
        /// </summary>
        [KSPField]
        public float dataGenerationRate = 0.000069446f;

        //Max amount of trajectory data that the computer can hold.
        [KSPField]
        public float maxGuidanceData = 1000.0f;

        #endregion

        #region ICanBreak fields
        /// <summary>
        /// What skill to use when performing the quality check. This is not always the same skill required to repair or maintain the part.
        /// </summary>
        [KSPField()]
        public string qualityCheckSkill = "RepairSkill";

        /// <summary>
        /// Flag to indicate that the part module is broken. If broken, then it can't be declared broken again by the ModuleQualityControl.
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool isBroken;

        /// <summary>
        /// Message to indicate that the part has broken
        /// </summary>
        [KSPField]
        public string partBrokenMessage = " has failed!";

        /// <summary>
        /// Label for the part to identify what done broke
        /// </summary>
        [KSPField]
        public string partBrokenLabel = "Mass Driver";
        #endregion

        #region Housekeeping
        [KSPField(guiActive = true, guiName = "Status")]
        public string status = "A-OK";

        [KSPField(isPersistant = true)]
        public double lastUpdateTime = 0f;

        protected BaseQualityControl qualityControl;
        PipelineWindow pipelineWidow;
        PartResourceDefinition resourceDef = null;
        WBIPackingBox packingBox;
        static GUIStyle opsWindowStyle = null;
        GUILayoutOption[] opsWindowOptions = new GUILayoutOption[] { GUILayout.Height(480) };
        Vector2 opsWindowPos = new Vector2();
        Transform macTriggerTransform = null;

        protected void Log(string message)
        {
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIPipeEndpoint] - " + message);
            }
        }


        public void onPackingStateChanged(bool isDeployed)
        {
            Fields["status"].guiActive = isDeployed;
            Events["ToggleActivation"].active = isDeployed;
            this.Events["ToggleSendGUI"].active = isDeployed;

            if (!isDeployed)
            {
                IsActivated = false;
            }
        }

        #endregion

        [KSPEvent(guiActive = true, guiName = "Power On")]
        public void ToggleActivation()
        {
            IsActivated = !IsActivated;

            //Check for shipments.
            if (IsActivated)
            {
                Events["ToggleActivation"].guiName = kPowerOff;
            }

            else
            {
                Events["ToggleActivation"].guiName = kPowerOn;
            }

            checkAndShowToolTip();
        }

        [KSPEvent(guiActive = true, guiName = "Schedule A Delivery")]
        public void ToggleSendGUI()
        {
            pipelineWidow.ToggleVisible();
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            if (node.HasValue("maxKineticEnergy"))
                maxKineticEnergy = double.Parse(node.GetValue("maxKineticEnergy"));
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            if (!string.IsNullOrEmpty(macTriggerName))
                macTriggerTransform = this.part.FindModelTransform(macTriggerName);

            //Generate an identifier if needed. This is used for payload transactions.
            if (uniqueIdentifier == kNoIdentifier)
                uniqueIdentifier = Guid.NewGuid().ToString();

            //Setup pipeline window
            pipelineWidow = new PipelineWindow();
            pipelineWidow.part = this.part;
            pipelineWidow.blackListedResources = this.blackListedResources;
            pipelineWidow.maxKineticEnergy = this.maxKineticEnergy;
            pipelineWidow.fuelMassFraction = this.fuelMassFraction;
            pipelineWidow.dryMassFraction = this.dryMassFraction;
            pipelineWidow.dataCostPerKm = this.dataCostPerKm;
            pipelineWidow.orbitalCostMultiplier = this.orbitalCostMultiplier;
            pipelineWidow.electricityCostPerTonne = this.electricityCostPerTonne;
            pipelineWidow.allowOrbitToGround = this.allowOrbitToGround;
            pipelineWidow.totalGuidanceData = this.totalGuidanceData;
            pipelineWidow.setGuidanceDataAmount = setGuidanceDataAmount;
             
            //Get resource definition for electric charge
            if (activationCostEC > 0f)
            {
                PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
                if (definitions.Contains("ElectricCharge"))
                    resourceDef = definitions["ElectricCharge"];
                else
                    return;
            }

            //Setup events
            packingBox = this.part.FindModuleImplementing<WBIPackingBox>();
            if (packingBox != null)
            {
                packingBox.onPackingStateChanged += onPackingStateChanged;
                onPackingStateChanged(packingBox.isDeployed);
            }

            //Setup GUI
            this.Events["ToggleSendGUI"].active = true;
            if (IsActivated)
                Events["ToggleActivation"].guiName = kPowerOff;
            else
                Events["ToggleActivation"].guiName = kPowerOn;

            //If we have any deliveries then grab them and distribute the resources.
            processDeliveries();
        }

        public override string GetInfo()
        {
            StringBuilder infoBuilder = new StringBuilder();

            infoBuilder.AppendLine(base.GetInfo());

            //Data generation rate
            infoBuilder.AppendLine(" ");
            infoBuilder.AppendLine("<b>Activated:</b>");
            infoBuilder.AppendLine(string.Format("<color=white>Requires {0:f1} E.C./sec</color>", activationCostEC));
            infoBuilder.AppendLine(string.Format("<color=white>Generates {0:f1} Mits/hr</color>", (dataGenerationRate * 3600)));

            //Requirements
            infoBuilder.AppendLine(" ");
            infoBuilder.AppendLine("<b>Payload Delivery:</b>");

            //ElectricCharge
            infoBuilder.AppendLine(string.Format("ElectricCharge: {0:f1} E.C./t", electricityCostPerTonne));

            //Trajectory data
            infoBuilder.AppendLine(string.Format("Trajectory data: {0:f3} Mits/km", dataCostPerKm));
            infoBuilder.AppendLine("x10 for orbital shots");

            return infoBuilder.ToString();
        }

        public void AddData(float amount)
        {
            if (!accumulateData)
                return;

            totalGuidanceData += amount;
            if (totalGuidanceData > maxGuidanceData)
                totalGuidanceData = maxGuidanceData;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
        }

        public virtual void Destroy()
        {
            qualityControl.onPartBroken -= OnPartBroken;
            qualityControl.onPartFixed -= OnPartFixed;
            qualityControl.onUpdateSettings -= onUpdateSettings;

            if (packingBox != null)
                packingBox.onPackingStateChanged -= onPackingStateChanged;
        }

        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            if (activationCostEC <= 0.001f)
                return;
            if (!IsActivated)
            {
                status = Localizer.Format(kOffStatus);
                return;
            }

            //Catchup on data generation
            if (lastUpdateTime > 0f)
            {
                double elapsedTime = Planetarium.GetUniversalTime() - lastUpdateTime;
                if (elapsedTime > 1.0f)
                {
                    totalGuidanceData += dataGenerationRate * (float)elapsedTime;
                    if (totalGuidanceData > maxGuidanceData)
                        totalGuidanceData = maxGuidanceData;
                    lastUpdateTime = Planetarium.GetUniversalTime();
                }
            }

            //Request the required electric charge
            double currentAmount = 0f;
            double maxAmount = 0;
            this.part.vessel.resourcePartSet.GetConnectedResourceTotals(resourceDef.id, out currentAmount, out maxAmount, true);
            currentAmount *= TimeWarp.deltaTime;
            double ecPerUpdate = activationCostEC * TimeWarp.deltaTime;
            if (currentAmount > ecPerUpdate)
            {
                //Pay the EC cost
                status = Localizer.Format(kNominalStatus);
                this.part.RequestResource(resourceDef.id, ecPerUpdate, ResourceFlowMode.ALL_VESSEL);

                //Generate trajectory data
                totalGuidanceData += dataGenerationRate * TimeWarp.deltaTime;
                if (totalGuidanceData > maxGuidanceData)
                    totalGuidanceData = maxGuidanceData;
                pipelineWidow.totalGuidanceData = this.totalGuidanceData;
                lastUpdateTime = Planetarium.GetUniversalTime();
            }

            //Not enough EC to run the pipe endpoint
            else
            {
                status = Localizer.Format(kNoECStatus);
            }
        }

        public void OnTriggerEnter(Collider collider)
        {
            if (collider.attachedRigidbody == null || !collider.CompareTag("Untagged"))
                return;
            if (!IsActivated)
                return;
            if (macTriggerTransform == null)
                return;

            //Get the part that collided with the trigger
            Part collidedPart = collider.attachedRigidbody.GetComponent<Part>();
            if (collidedPart == null)
                return;

            //Calculate the projectile velocity
            double projectileVelocity = Math.Sqrt((maxKineticEnergy * 2) / (1000 * collidedPart.vessel.totalMass));

            //Get velocity
            CelestialBody celestialBody = collidedPart.vessel.mainBody;
            double orbitalVelocity = Math.Sqrt(celestialBody.gravParameter / celestialBody.minOrbitalDistance);
            if (projectileVelocity > orbitalVelocity)
                projectileVelocity = orbitalVelocity;

            //Get accelearation.
            double time = 1.0f;
            double acceleration = projectileVelocity / time;

            //Calculate force: f = mass * acceleration
            double force = (1000 * collidedPart.vessel.totalMass) * acceleration;

            //Account for render frames
            force *= TimeWarp.fixedDeltaTime;

            //Apply force
            collidedPart.vessel.IgnoreGForces(250);
            collidedPart.AddForceAtPosition(macTriggerTransform.forward * (float)force, collidedPart.vessel.CoM);

            //Add recoil
            if (!this.part.vessel.permanentGroundContact)
            {
                force = (1000 * this.part.vessel.totalMass) * acceleration;
                force *= TimeWarp.fixedDeltaTime * -1.0f;
                this.part.vessel.IgnoreGForces(250);
                this.part.AddForceAtPosition(macTriggerTransform.forward * (float)force, this.part.vessel.CoM);
            }
        }

        protected void setGuidanceDataAmount(float amount)
        {
            totalGuidanceData = amount;
        }

        protected void checkAndShowToolTip()
        {
            //Now we can check to see if the tooltip for the current template has been shown.
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (scenario.HasShownToolTip(this.part.partInfo.title))
                return;

            string toolTip = kToolTipSend;
            WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(this.part.partInfo.title, toolTip);
            toolTipWindow.SetVisible(true);

            //Cleanup
            scenario.SetToolTipShown(this.part.partInfo.title);
        }

        protected void processDeliveries()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            //We're interested in resource and inventory deliveries...
            List<WBIResourceManifest> resourceManifests = WBIResourceManifest.GetManifestsForDestination(this.uniqueIdentifier);
            Log("Resource manifests count: " + resourceManifests.Count);

            //Distribute the resources throughout the vessel
            WBIResourceManifest resourceManifest;
            int totalManifests = resourceManifests.Count;
            string[] resourceKeys;
            string resourceName;
            double amount;
            for (int index = 0; index < totalManifests; index++)
            {
                //Get the manifest
                resourceManifest = resourceManifests[index];

                //Go through all the resources and distribute them
                resourceKeys = resourceManifest.resourceAmounts.Keys.ToArray();
                for (int keyIndex = 0; keyIndex < resourceKeys.Length; keyIndex++)
                {
                    //Get the name
                    resourceName = resourceKeys[keyIndex];

                    //Get the amount
                    amount = resourceManifest.resourceAmounts[resourceName];

                    //Distribute the resource
                    this.part.RequestResource(resourceName, -amount, ResourceFlowMode.ALL_VESSEL);
                    Log("Added " + amount + " units of " + resourceName);
                }
            }

            //Deliver inventory items
            if (WBIKISWrapper.IsKISInstalled())
            {
                //Get the inventory and available volume
                WBIKISInventoryWrapper inventory = WBIKISInventoryWrapper.GetInventory(this.part);
                List<WBIKISInventoryWrapper> inventories = WBIKISInventoryWrapper.GetInventories(this.part.vessel);
                int currentIndex = 0;
                float contentVolume = 0;
                float availableVolume = 0;

                //Grab the first available inventory
                if (inventory == null)
                {
                    inventory = inventories[0];
                    if (inventory == null)
                        return;
                }

                //Get available volume
                inventory.RefreshMassAndVolume();
                contentVolume = inventory.GetContentVolume();
                availableVolume = inventory.maxVolume - contentVolume;

                //Get all the manifests
                List<WBIKISInventoryManifest> inventoryManifests = WBIKISInventoryManifest.GetManifestsForDestination(this.uniqueIdentifier);
                Log("Inventory manifest count: " + inventoryManifests.Count);
                WBIKISInventoryManifest inventoryManifest;
                WBIInventoryManifestItem inventoryItem;
                totalManifests = inventoryManifests.Count;
                int totalItems;
                AvailablePart availablePart = null;
                int skippedItems = 0;
                for (int index = 0; index < totalManifests; index++)
                {
                    //Get the manifest
                    inventoryManifest = inventoryManifests[index];

                    //Get the total items in the manifest
                    totalItems = inventoryManifest.inventoryItems.Count;
                    for (int itemIndex = 0; itemIndex < totalItems; itemIndex++)
                    {
                        //Get the item
                        inventoryItem = inventoryManifest.inventoryItems[itemIndex];
                        Log("Looking for enough room for " + inventoryItem.partName);

                        //If the inventory has room, then add it.
                        if (inventoryItem.volume < availableVolume)
                        {
                            Log("Current inventory has room.");
                            //Decrease the available volume
                            availableVolume -= inventoryItem.volume;

                            //Get the part info
                            availablePart = PartLoader.getPartInfoByName(inventoryItem.partName);

                            //Add the part
                            inventory.AddItem(availablePart, inventoryItem.partConfigNode, inventoryItem.quantity);
                            Log("Added " + inventoryItem.partName);
                        }

                        //See if another inventory has room
                        else
                        {
                            while (currentIndex < inventories.Count)
                            {
                                currentIndex += 1;
                                inventory = inventories[currentIndex];
                                Log("New inventory found");

                                inventory.RefreshMassAndVolume();
                                contentVolume = inventory.GetContentVolume();
                                availableVolume = inventory.maxVolume - contentVolume;

                                //If the inventory has room, then add it.
                                if (inventoryItem.volume < availableVolume)
                                {
                                    //Decrease the available volume
                                    availableVolume -= inventoryItem.volume;

                                    //Get the part info
                                    availablePart = PartLoader.getPartInfoByName(inventoryItem.partName);

                                    //Add the part
                                    inventory.AddItem(availablePart, inventoryItem.partConfigNode, inventoryItem.quantity);
                                    Log("Added " + inventoryItem.partName);
                                    break;
                                }

                                else
                                {
                                    skippedItems += 1;
                                }
                            }
                        }
                    }
                }

                //Inform the player if we skipped some items
                if (skippedItems > 0)
                    ScreenMessages.PostScreenMessage(kItemSkippedMsg, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
            }
        }

        #region ICanBreak
        protected void onUpdateSettings(BaseQualityControl moduleQualityControl)
        {
            if (!BARISBridge.ConvertersCanFail)
                isBroken = false;
        }

        public string GetCheckSkill()
        {
            return qualityCheckSkill;
        }

        public bool ModuleIsActivated()
        {
            if (!BARISBridge.CrewedPartsCanFail && this.part.CrewCapacity > 0)
                return false;
            if (!BARISBridge.CommandPodsCanFail && this.part.FindModuleImplementing<ModuleCommand>() != null)
                return false;

            if (!BARISBridge.PartsCanBreak || !BARISBridge.ConvertersCanFail)
                return false;

            return IsActivated;
        }

        public void SubscribeToEvents(BaseQualityControl moduleQualityControl)
        {
            qualityControl = moduleQualityControl;
            qualityControl.onPartBroken += OnPartBroken;
            qualityControl.onPartFixed += OnPartFixed;
            qualityControl.onUpdateSettings += onUpdateSettings;

            //Make sure we're broken
            if (isBroken)
                OnPartBroken(qualityControl);
        }

        public virtual void OnPartFixed(BaseQualityControl moduleQualityControl)
        {
            isBroken = false;
        }

        public virtual void OnPartBroken(BaseQualityControl moduleQualityControl)
        {
            if (!BARISBridge.ConvertersCanFail)
                return;

            isBroken = true;
            IsActivated = false;

            //Generate a failure mode

            if (this.part.vessel == FlightGlobals.ActiveVessel)
            {
                string message = Localizer.Format(this.part.partInfo.title + partBrokenMessage);
                BARISBridge.LogPlayerMessage(message);
            }
            qualityControl.UpdateQualityDisplay(qualityControl.qualityDisplay + Localizer.Format(partBrokenLabel));
        }
        #endregion

        #region IOpsView
        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();

            buttonLabels.Add("Pipeline");

            return buttonLabels;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            if (opsWindowStyle == null)
                opsWindowStyle = new GUIStyle(GUI.skin.textArea);

            GUILayout.BeginVertical();
            GUILayout.BeginScrollView(opsWindowPos, opsWindowStyle, opsWindowOptions);

            pipelineWidow.DrawView();

            GUILayout.EndScrollView();
            GUILayout.EndVertical();
        }

        public void SetParentView(IParentView parentView)
        {
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            if (packingBox == null)
                return;

            Fields["status"].guiActive = isVisible & packingBox.isDeployed;
            Events["ToggleActivation"].active = isVisible & packingBox.isDeployed;
            this.Events["ToggleSendGUI"].active = isVisible & packingBox.isDeployed;
        }

        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }
        #endregion
    }
}
