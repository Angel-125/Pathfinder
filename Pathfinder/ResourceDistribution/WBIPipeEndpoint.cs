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
    [KSPModule("Pipe Endpoint")]
    public class WBIPipeEndpoint : PartModule, ICanBreak
    {
        #region Constants
        public const string kNoIdentifier = "kNoIdentifier";
        public const string kNominalStatus = "A-OK";
        public const string kNoECStatus = "Not enough ElectricCharge";
        public const string kOffStatus = "Deactivated";
        public const string kToolTipSend = "Mass drivers let you send resources to and receive resources from other mass drivers. Each delivery costs trajectory data to calculate the trajectory, ElectricCharge to launch the projectile, and LiquidFuel and Oxidizer to make course corrections. This part can generate trajectory data, but you can also obtain data from the Ranch House as well as the Doc Science Lab.";
        public const string kTookTipReceive = "Mass catchers can receive resources launched from mass drivers. Each shipment's resources will be evenly distributed to all parts on the vessel that can hold the resource. Any excess will be lost. Make sure to turn the power on or you won't receive your shipments.";
        public const string kPowerOn = "Power On";
        public const string kPowerOff = "Power Off";
        #endregion

        #region Transfer capability
        /// <summary>
        /// Toggle to indicate whether or not the endpoint is activated.
        /// </summary>
        [UI_Toggle(enabledText = "On", disabledText = "Off")]
        public bool IsActivated = false;

        /// <summary>
        /// Can the pipe endpoint send payloads? Example: mass catchers cannot send payloads. Marked persistent so that we can access them from an unloaded vessel.
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool canSendPayloads = true;

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

        #endregion

        #region Payload capability and costs
        /// <summary>
        /// Maximum metric tons of payload per delivery. This assumes a total vacuum.
        /// </summary>
        [KSPField]
        public float payloadCapacity = 10.0f;

        /// <summary>
        /// Multiplied by payloadCapacity to get total fuel mass. The required amount of LiquidFuel and Oxidizer is derived from the total fuel mass.
        /// </summary>
        [KSPField]
        public float fuelMassFraction = 1.0f;

        /// <summary>
        /// The dry mass fraction of the delivery vehicle. Multiply the payloadCapacity and fuelMassFraction by this value to get the delivery vehicle's dry mass.
        /// </summary>
        [KSPField]
        public float dryMassFraction = 0.05f;

        /// <summary>
        /// How much payload capacity to lose per increment of atmospheric pressure.
        /// </summary>
        [KSPField]
        public float payloadAtmosphericLoss = 1.0f;

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
        #endregion

        #region Trajectory data accumulation
        /// <summary>
        /// Toggle to indicate whether or not to accumulate trajectory data
        /// </summary>
        [KSPField(guiName = "Collect Trajectory Data", isPersistant = true, guiActiveEditor = true, guiActive = true)]
        [UI_Toggle(enabledText = "Yes", disabledText = "No")]
        public bool accumulateData = true;

        /// <summary>
        /// How much trajectory data we have available to spend. Marked persistent so that we can access them from an unloaded vessel.
        /// </summary>
        [KSPField(isPersistant = true, guiActive = true, guiName = "Trajectory Data", guiUnits = "Mits", guiFormat = "f2")]
        public float trajectoryDataAmount = 0f;

        /// <summary>
        /// How much trajectory data to generate per second while the endpoint is active. Requires canSendPayloads = true.
        /// </summary>
        [KSPField]
        public float dataGenerationRate = 0.000069446f;

        //Max amount of trajectory data that the computer can hold.
        [KSPField]
        public float maxTrajectoryData = 1000.0f;

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

        protected BaseQualityControl qualityControl;
        PipelineWindow pipelineWidow = new PipelineWindow();
        PartResourceDefinition resourceDef = null;

        protected void Log(string message)
        {
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIPipeEndpoint] - " + message);
            }
        }

        #endregion
        [KSPEvent(guiActive = true)]
        public void ToggleActivation()
        {
            IsActivated = !IsActivated;

            //Check for shipments.
            if (IsActivated)
            {
                Fields["ToggleActivation"].guiName = kPowerOff;
            }

            else
            {
                Fields["ToggleActivation"].guiName = kPowerOn;
            }
        }

        [KSPEvent(guiActive = true, guiName = "Schedule A Delivery")]
        public void ToggleSendGUI()
        {
            pipelineWidow.ToggleVisible();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Get resource definition for electric charge
            if (activationCostEC > 0f)
            {
                PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
                if (definitions.Contains("ElectricCharge"))
                    resourceDef = definitions["ElectricCharge"];
                else
                    return;
            }

            //Setup GUI
            this.Events["ToggleSendGUI"].active = canSendPayloads;
            if (IsActivated)
                Fields["ToggleActivation"].guiName = kPowerOff;
            else
                Fields["ToggleActivation"].guiName = kPowerOn;

            //Generate an identifier if needed. This is used for payload transactions.
            if (uniqueIdentifier == kNoIdentifier)
                uniqueIdentifier = Guid.NewGuid().ToString();

            //If we have any deliveries then grab them and distribute the resources.
        }

        public override string GetInfo()
        {
            StringBuilder infoBuilder = new StringBuilder();

            infoBuilder.AppendLine(base.GetInfo());

            //Payload capacity
            infoBuilder.AppendLine(string.Format("<b>Max Payload:</b> {0:f1} t (vac)", payloadCapacity));

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

            //LFO
            float fuelAmount = (payloadCapacity * fuelMassFraction * 0.45f) / 0.005f;
            infoBuilder.AppendLine(string.Format("LiquidFuel: {0:f1} t (Max)", fuelAmount));
            fuelAmount = (payloadCapacity * fuelMassFraction * 0.55f) / 0.005f;
            infoBuilder.AppendLine(string.Format("Oxidizer: {0:f1} t (Max)", fuelAmount));

            //Trajectory data
            infoBuilder.AppendLine(string.Format("Trajectory data: {0:f3} Mits/km", dataCostPerKm));
            infoBuilder.AppendLine("x10 for orbital shots");

            return infoBuilder.ToString();
        }

        public void AddData(float amount)
        {
            if (!accumulateData)
                return;

            trajectoryDataAmount += amount;
            if (trajectoryDataAmount > maxTrajectoryData)
                trajectoryDataAmount = maxTrajectoryData;

            //Dirty the GUI
            MonoUtilities.RefreshContextWindows(this.part);
        }

        public virtual void Destroy()
        {
            qualityControl.onPartBroken -= OnPartBroken;
            qualityControl.onPartFixed -= OnPartFixed;
            qualityControl.onUpdateSettings -= onUpdateSettings;
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
                if (canSendPayloads)
                {
                    trajectoryDataAmount += dataGenerationRate * TimeWarp.deltaTime;
                    if (trajectoryDataAmount > maxTrajectoryData)
                        trajectoryDataAmount = maxTrajectoryData;
                }                
            }

            //Not enough EC to run the pipe endpoint
            else
            {
                status = Localizer.Format(kNoECStatus);
            }
        }

        protected void checkAndShowToolTip()
        {
            //Now we can check to see if the tooltip for the current template has been shown.
            WBIPathfinderScenario scenario = WBIPathfinderScenario.Instance;
            if (scenario.HasShownToolTip(this.part.partInfo.title))
                return;

            string toolTip = kToolTipSend;
            if (!canSendPayloads)
                toolTip = kTookTipReceive;
            WBIToolTipWindow toolTipWindow = new WBIToolTipWindow(this.part.partInfo.title, toolTip);
            toolTipWindow.SetVisible(true);

            //Cleanup
            scenario.SetToolTipShown(this.part.partInfo.title);
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
    }
}
