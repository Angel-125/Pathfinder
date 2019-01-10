using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KerbalActuators;
using KSP.IO;
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
    public struct PipeEndpointNode
    {
        public WBIPipeEndpoint endpoint;
        public string vesselName;
        public ConfigNode moduleValues;
        public ProtoPartModuleSnapshot snapshot;
        public Vessel vessel;
    }

    public struct WBIResourceTotals
    {
        public string resourceName;
        public string displayName;
        public double amount;
        public double maxAmount;
        public float transferPercent;
        public float density;
    }

    public struct WBIPackingItem
    {
        public string partTitle;
        public float mass;
        public float volume;
        public bool isSelected;
        public WBIKISItem kisItem;
        public int quantity;
        public int slot;
    }

    public enum PipelineViewPages
    {
        SelectVessel,
        ChooseResources,
        ConfirmDelivery
    }

    public enum WBITransferTypes
    {
        Unknown,
        GroundToGround,
        GroundToOrbit,
        OrbitToOrbit,
        OrbitToGround
    }

    public delegate void setGuidanceDataAmountDelegate(float amount);

    public class PipelineWindow : Dialog<PipelineWindow>
    {
        #region Constants
        const int windowWidth = 600;
        const int windowHeight = 480;
        const float kLiquidFuelRatio = 0.45f;
        const float kOxidizerRatio = 0.55f;
        const string kNoVesselsInRange = "<color=yellow><b>No Pipelines found within range of this vessel.</b></color>";
        const string kNoResources = "<color=yellow><b>Vessel has no resources to send.</b></color>";
        const string kSelectVessel = "<color=white>Select a destination:</color>";
        const string kBackLabel = "Back";
        const string kNextLabel = "Next";
        const string kAddLabel = "Add";
        const string kEditLabel = "Edit";
        const string kLaunchLabel = "LAUNCH!";
        const string kDeleteLabel = "Remove";
        const string kInventoryLabel = "Inventory";
        const string kResourcesLabel = "Resources";
        const string kLFShortage = "<color=white>Insufficient LiquidFuel, {0:f2}units needed</color>";
        const string kOxShortage = "<color=white>Insufficient Oxidizer, {0:f2}units needed</color>";
        const string kECShortage = "<color=white>Insufficient ElectricCharge, {0:f2}units needed</color>";
        const string kNoPayload = "<color=white>No payload to launch</color>";
        const string kInsufficientDeltaV = "<color=white>Insufficient Delta-V, {0:f2}m/s needed</color>";
        const string kInsufficientDeltaVTarget = "<color=white>Insufficient Delta-V at target, {0:f2}m/s needed</color>";
        const string kInsufficientRange = "<color=white>Insufficient range, {0:f2}m needed</color>";
        const string kInsufficientGuidance = "<color=white>Insufficient guidance data, {0:f2}mits needed</color>";
        const string kNoIssues = "All systems GO!";
        const float kMessageDuration = 5.0f;
        const string kNoOrbitToGroundMsg = "Cannot send to destination, orbit-to-ground transfers aren't possible.";
        const string kLaunchIssuesMsg = "Cannot send to destination, one or more issues remain.";
        const string kShipmentLaunched = "Shipment in flight to ";
        const string kLaunchAzimuthGo = "<color=white>Launch Azimuth: {0:f2} </color><b>GO!</b>";
        const string kLaunchAzimuthNoGo = "<color=white>Launch Azimuth: {0:f2} </color><color=red><b>NO-GO!</b></color>";
        const string kLaunchAzimuthNoGoAlert = "Cannot deliver to target. Wait for better launch window.";
        #endregion

        public Part part;
        public string blackListedResources;
        public double maxKineticEnergy = 0f;
        public float fuelMassFraction = 1.0f;
        public float dryMassFraction = 0.05f;
        public float dataCostPerKm = 0.01f;
        public float orbitalCostMultiplier = 10.0f;
        public float electricityCostPerTonne = 1000.0f;
        public bool allowOrbitToGround = false;
        public float totalGuidanceData = 0f;
        public float maxLaunchAzimuth = 15.0f;
        public setGuidanceDataAmountDelegate setGuidanceDataAmount;

        PipeEndpointNode[] pipeEndpoints;
        PipelineViewPages pageID;
        PipeEndpointNode selectedPipelineNode;
        Vector2 scrollPos = new Vector2();
        Vector2 scrollPosResources = new Vector2();
        Vector2 panelPos = new Vector2();
        Vector2 targetPanel = new Vector2();
        Vector2 targetPane = new Vector2();
        GUILayoutOption[] vesselSelectionLayoutOption = new GUILayoutOption[] { GUILayout.Width(375) };
        GUILayoutOption[] resourcePaneOptions = new GUILayoutOption[] { GUILayout.Height(windowHeight), GUILayout.Width(250) };
        GUILayoutOption[] inventoryButtonOptions = new GUILayoutOption[] { GUILayout.Width(250) };
        GUILayoutOption[] buttonOptions = new GUILayoutOption[] { GUILayout.Width(80) };
        GUILayoutOption[] resourcePanelOptions = new GUILayoutOption[] { GUILayout.Height(75) };
        GUILayoutOption[] targetPanelOptions = new GUILayoutOption[] { GUILayout.Height(windowHeight), GUILayout.Width(250) };
        GUILayoutOption[] targetPaneOptions = new GUILayoutOption[] { GUILayout.Height(windowHeight), GUILayout.Width(150) };
        string[] vesselNames;
        string[] sourceDisplayNames;
        Dictionary<string, WBIResourceTotals> sourceVesselResources = new Dictionary<string, WBIResourceTotals>();
        StringBuilder issues = new StringBuilder();
        double payloadMass = 0f;
        double projectileMass = 0f;
        WBITransferTypes transferType;
        float totalDataCost;
        int selectedIndex = 0;
        double liquidFuelUnits;
        double oxidizerUnits;
        WBIKISInventoryWrapper inventory = null;
        bool showInventory = false;
        List<WBIPackingItem> packingList = new List<WBIPackingItem>();
        int inventoryItemCount = 0;
        float packingListMass = 0f;
        bool enableAzimuthRestriction;

        public PipelineWindow(string title = "Pipelines") :
            base(title, windowWidth, windowHeight)
        {
            WindowTitle = title;
            Resizable = false;
        }

        protected void Log(string message)
        {
            if (WBIPathfinderScenario.showDebugLog)
            {
                Debug.Log("[WBIPipeEndpoint] - " + message);
            }
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);
            WindowTitle = this.part.partInfo.title;

            if (newValue)
            {
                //Set starting page
                pageID = PipelineViewPages.SelectVessel;

                //Find Pipelines
                FindPipeEndpoints();

                //Get vessel resources
                sourceVesselResources.Clear();
                getVesselResources(this.part.vessel, sourceVesselResources);
                if (sourceVesselResources.Keys.Count > 0)
                    sourceDisplayNames = sourceVesselResources.Keys.ToArray();
                else
                    sourceDisplayNames = null;

                //Get inventory if any
                if (WBIKISWrapper.IsKISInstalled())
                    inventory = WBIKISInventoryWrapper.GetInventory(this.part);

                //Launch azimuth restriction
                enableAzimuthRestriction = PathfinderSettings.EnableAzimuthRestriction;
            }
        }

        public void FindPipeEndpoints()
        {
            //We're only interested in vessels outside of physics range.
            Vessel[] vessels = FlightGlobals.VesselsUnloaded.ToArray();
//            Vessel[] vessels = FlightGlobals.VesselsLoaded.ToArray();
            Vessel vessel;
            PipeEndpointNode endpointNode;
            List<PipeEndpointNode> endpoints = new List<PipeEndpointNode>();
            bool foundEndpipe;

            //Find vessels that have a pipe endpoint that are on the active vessel's celestial body.
            pipeEndpoints = null;
            for (int index = 0; index < vessels.Length; index++)
            {
                //Get the vessel
                vessel = vessels[index];

                //If the vessel is not on or orbiting the active vessel's celestial body, then we're done.
                if (vessel.mainBody != FlightGlobals.ActiveVessel.mainBody)
                    continue;
                if (vessel == this.part.vessel)
                    continue;

                //See if the vessel has an active pipeline. If so, add it to the list.
                foundEndpipe = false;
                foreach (ProtoPartSnapshot protoPart in vessel.protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot protoModule in protoPart.modules)
                    {
                        if (protoModule.moduleName == "WBIPipeEndpoint")
                        {
                            if (canReceiveTransfers(vessel, protoModule))
                            {
                                endpointNode = new PipeEndpointNode();
                                endpointNode.moduleValues = protoModule.moduleValues; //Use this to tell who connects to whom
                                endpointNode.snapshot = protoModule;
                                endpointNode.vessel = vessel;
                                endpointNode.vesselName = vessel.vesselName;
                                endpoints.Add(endpointNode);
                                foundEndpipe = true;
                                break;
                            }
                        }
                    }
                    if (foundEndpipe)
                        break;
                }
            }

            //Set up the array
            if (endpoints.Count > 0)
            {
                pipeEndpoints = endpoints.ToArray();

                List<string> vesselNameList = new List<string>();
                for (int index = 0; index < pipeEndpoints.Length; index++)
                    vesselNameList.Add(pipeEndpoints[index].vesselName);

                vesselNames = vesselNameList.ToArray();
            }
        }

        public void DrawView()
        {
            //Draw page view. This is set up in a wizard format.
            //1) Select a vessel from the list of vessels.
            //2) Select resources to transfer from active vessel.
            //3) Confirm delivery.
            switch (pageID)
            {
                default:
                case PipelineViewPages.SelectVessel: //vessel selection
                    drawSelectVesselPage();
                    break;

                case PipelineViewPages.ChooseResources:
                    drawSelectResourcesPage();
                    break;

                case PipelineViewPages.ConfirmDelivery:
                    drawConfirmPage();
                    break;
            }
        }

        protected void drawConfirmPage()
        {
            GUILayout.BeginVertical();
            //Payload Summary
            GUILayout.Label("<color=white><b>Payload:</b></color>");

            scrollPosResources = GUILayout.BeginScrollView(scrollPosResources, resourcePaneOptions);

            //Resources
            int totalResources = sourceDisplayNames.Length;
            WBIResourceTotals resourceTotals;
            for (int index = 0; index < totalResources; index++)
            {
                resourceTotals = sourceVesselResources[sourceDisplayNames[index]];
                if (resourceTotals.transferPercent < 0.0001f)
                    continue;

                GUILayout.BeginScrollView(panelPos, resourcePanelOptions);
                GUILayout.BeginVertical();

                //Resource name
                GUILayout.Label("<color=white><b>" + resourceTotals.displayName + "</b></color>");

                //Amount text field
                GUILayout.Label("<color=white> Units: " + string.Format("{0:f2}", resourceTotals.amount * resourceTotals.transferPercent) + "</color>");

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                sourceVesselResources[sourceDisplayNames[index]] = resourceTotals;
            }

            //Packing list
            if (packingList.Count > 0)
            {
                WBIPackingItem packingItem;
                int totalItems = packingList.Count;
                for (int index = 0; index < totalItems; index++)
                {
                    //Get the packing item
                    packingItem = packingList[index];

                    //If selected list out the item
                    if (packingItem.isSelected)
                    {
                        GUILayout.Label("<color=white>" + packingItem.partTitle + "(" + packingItem.quantity + ")</color>");
                    }
                }
            }

            GUILayout.EndScrollView();

            //Buttons
            GUILayout.BeginHorizontal();
            //Back button
            if (GUILayout.Button(kBackLabel))
                pageID = PipelineViewPages.ChooseResources;

            //Launch button
            //At this time, deduct the required resources and payload resources
            if (GUILayout.Button(kLaunchLabel))
            {
                //Pay EC cost
                double amountToTransfer = electricityCostPerTonne * projectileMass;
                this.part.RequestResource("ElectricCharge", amountToTransfer, ResourceFlowMode.ALL_VESSEL);

                //Pay LFO units
                this.part.RequestResource("LiquidFuel", liquidFuelUnits, ResourceFlowMode.ALL_VESSEL);
                this.part.RequestResource("Oxidizer", oxidizerUnits, ResourceFlowMode.ALL_VESSEL);

                //Pay guidance data cost
                if (setGuidanceDataAmount != null)
                    setGuidanceDataAmount(totalGuidanceData - totalDataCost);

                //Create the resource manifest
                WBIResourceManifest manifest = new WBIResourceManifest();
                manifest.destinationID = selectedPipelineNode.moduleValues.GetValue("uniqueIdentifier");

                //Deduct payload resources and load them into the transfer request.
                for (int index = 0; index < totalResources; index++)
                {
                    //Get the totals, skipping any that has no transfer percent
                    resourceTotals = sourceVesselResources[sourceDisplayNames[index]];
                    if (resourceTotals.transferPercent < 0.0001f)
                        continue;

                    //Deduct the resource amount from the vessel
                    amountToTransfer = resourceTotals.amount * resourceTotals.transferPercent;
                    this.part.RequestResource(resourceTotals.resourceName, amountToTransfer, ResourceFlowMode.ALL_VESSEL);

                    //Add the resource to the transfer manifest.
                    manifest.resourceAmounts.Add(resourceTotals.resourceName, amountToTransfer);
                }
                if (manifest.resourceAmounts.Count > 0)
                    WBIManifestScenario.Instance.AddManifest(manifest);

                //Create the Inventory manifest
                if (packingList.Count > 0)
                {
                    int totalItems = packingList.Count;

                    WBIKISInventoryManifest inventoryManifest = new WBIKISInventoryManifest();
                    inventoryManifest.destinationID = selectedPipelineNode.moduleValues.GetValue("uniqueIdentifier");

                    WBIPackingItem packingItem;
                    WBIInventoryManifestItem manifestItem;
                    for (int index = 0; index < totalItems; index++)
                    {
                        //Get the packing item
                        packingItem = packingList[index];
                        if (!packingItem.isSelected)
                            continue;

                        //Fill out the manifest
                        manifestItem = new WBIInventoryManifestItem();
                        manifestItem.partName = packingItem.kisItem.availablePart.name;
                        manifestItem.quantity = packingItem.quantity;
                        manifestItem.volume = packingItem.volume;
                        ConfigNode partNode = new ConfigNode(WBIKISInventoryManifest.kPartNode);
                        packingItem.kisItem.availablePart.partConfig.CopyTo(partNode);
                        ConfigNode partConfigNode = new ConfigNode(WBIKISInventoryManifest.kPartConfig);
                        partConfigNode.AddNode(partNode);
                        manifestItem.partConfigNode = partConfigNode;
                        inventoryManifest.inventoryItems.Add(manifestItem);

                        //Remove the item from the inventory
                        inventory.DeleteItem(packingItem.slot);
                    }
                    WBIManifestScenario.Instance.AddManifest(inventoryManifest);
                    packingList.Clear();
                }

                //Play launch effects and inform user.
                ScreenMessages.PostScreenMessage(kShipmentLaunched + selectedPipelineNode.vesselName, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);

                //Change page
                pageID = PipelineViewPages.SelectVessel;
            }
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        protected void drawSelectResourcesPage()
        {
            if (sourceDisplayNames == null)
            {
                GUILayout.Label(kNoResources);
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //Left pane
            drawLeftPane();

            //Right pane
            drawRightPane();

            GUILayout.EndHorizontal();

            //Buttons
            GUILayout.BeginHorizontal();

            //Back button
            if (GUILayout.Button(kBackLabel))
                pageID = PipelineViewPages.SelectVessel;

            //Next button
            if (GUILayout.Button(kNextLabel))
            {
                if (issues.Length == 0)
                    pageID = PipelineViewPages.ConfirmDelivery;
                else
                    ScreenMessages.PostScreenMessage(kLaunchIssuesMsg, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
            }
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected void drawRightPane()
        {
            //Launch vehicle requirements (LFO, E.C., trajectory data costs, range)
            //Go/No Go status lights: green/red
            GUILayout.BeginScrollView(panelPos);

            //Destination
            GUILayout.Label("<color=white><b>Destination: </b>" + selectedPipelineNode.vesselName + "</color>");

            //Payload mass
            payloadMass = getPayloadMass();
            projectileMass = (payloadMass * fuelMassFraction) + payloadMass;
            projectileMass *= (1.0f + dryMassFraction);
            GUILayout.Label(string.Format("<color=white><b>Payload Mass: </b>{0:f3}tonnes</color>", payloadMass));

            GUILayout.Label(" ");
            GUILayout.Label("<color=white><b>Preflight:</b></color>");

            issues = new StringBuilder();
            //Payload status
            if (payloadMass > 0.001f)
            {
                GUILayout.Label("<color=white><b>Payload: </b></color>GO");
            }

            else
            {
                GUILayout.Label("<color=white><b>Payload: </b></color><color=red>NO GO</color>");
                issues.AppendLine(kNoPayload);
            }

            //Status check: LiquidFuel Go/No Go
            //Status check: Oxidizer Go/No Go
            drawLFOStatus();

            //Status check: ElectricCharge Go/No Go
            drawECStatus();

            //Status check: Delta V Go/No Go
            drawDeltaVStatus();

            //Status check: Trajectory Data Go/No Go
            drawTrajectoryStatus();

            //Summary: (not enough LFO, not enough EC, not enough trajectory data, max payload exceeded, not enough delta v for the payload)
            string status = issues.ToString();
            GUILayout.Label(" ");
            GUILayout.Label("<color=white><b>Issue Summary:</b></color>");
            if (!string.IsNullOrEmpty(status))
                GUILayout.Label("<color=white>" + status + "</color>");
            else
                GUILayout.Label("<b>" + kNoIssues + "</b>");

            GUILayout.EndScrollView();
        }

        protected void drawLeftPane()
        {
            scrollPosResources = GUILayout.BeginScrollView(scrollPosResources, resourcePaneOptions);

            //Inventory button
            if (inventory != null)
            {
                if (showInventory)
                {
                    if (GUILayout.Button(kResourcesLabel))
                        showInventory = false;
                }
                else if (GUILayout.Button(kInventoryLabel))
                {
                    showInventory = true;
                }
            }

            //Inventory
            if (showInventory)
                drawPackingList();

            //Resources
            else
                drawResourceList();
            GUILayout.EndScrollView();
        }

        protected void drawPackingList()
        {
            inventory.RefreshMassAndVolume();
            Dictionary<int, WBIKISItem> items = inventory.items;

            //Rebuild the packing list if the item count has changed.
            WBIPackingItem packingItem;
            WBIKISItem item;
            if (inventoryItemCount != items.Count)
            {
                inventoryItemCount = items.Count;
                packingList.Clear();
                packingListMass = 0;
                foreach (int key in items.Keys)
                {
                    item = items[key];

                    packingItem = new WBIPackingItem();
                    packingItem.kisItem = item;
                    packingItem.isSelected = false;
                    packingItem.partTitle = item.availablePart.title;
                    packingItem.mass = item.totalMass;
                    packingItem.volume = item.volume;
                    packingItem.quantity = item.quantity;
                    packingItem.slot = key;
                    packingList.Add(packingItem);
                }
            }

            //Inventory mass
            GUILayout.Label(string.Format("<color=white><b>Package Mass: </b>{0:f3}tonnes</color>", packingListMass));

            //Now go through all the inventory items and let the player select the desired
            //items for the shipment.
            packingListMass = 0f;
            int totalItems = packingList.Count;
            string toggleText;
            for (int index = 0; index < totalItems; index++)
            {
                //Get the packing item
                packingItem = packingList[index];

                //Get toggle text
                toggleText = packingItem.partTitle;
                if (packingItem.quantity > 1)
                    toggleText = toggleText + " (" + Mathf.RoundToInt(packingItem.quantity).ToString() + ")";

                //Draw toggle
                packingItem.isSelected = GUILayout.Toggle(packingItem.isSelected, toggleText);
                packingList[index] = packingItem;

                //Update packing list mass
                if (packingItem.isSelected)
                    packingListMass += packingItem.mass;
            }
        }

        protected void drawResourceList()
        {
            int totalResources = sourceDisplayNames.Length;
            WBIResourceTotals resourceTotals;
            for (int index = 0; index < totalResources; index++)
            {
                resourceTotals = sourceVesselResources[sourceDisplayNames[index]];

                GUILayout.BeginScrollView(panelPos, resourcePanelOptions);
                GUILayout.BeginVertical();

                //Resource name
                GUILayout.Label("<color=white><b>" + resourceTotals.displayName + "</b></color>");

                //Amount slider - amount to max amount
                resourceTotals.transferPercent = GUILayout.HorizontalSlider(resourceTotals.transferPercent, 0.0f, 1.0f);

                //Amount text field
                GUILayout.Label("<color=white> Units: " + string.Format("{0:f2}", resourceTotals.amount * resourceTotals.transferPercent) + "</color>");

                GUILayout.EndVertical();
                GUILayout.EndScrollView();

                sourceVesselResources[sourceDisplayNames[index]] = resourceTotals;
            }
        }

        protected void calculateDataGroundToGround()
        {
            CelestialBody celestialBody = selectedPipelineNode.vessel.mainBody;

            //Get the distance to the target.
            double distanceToTarget = Utils.HaversineDistance(this.part.vessel.longitude, this.part.vessel.latitude,
                selectedPipelineNode.vessel.longitude, selectedPipelineNode.vessel.latitude, celestialBody);
            if (distanceToTarget < 1.0f)
                distanceToTarget = 1.0f;

            //Get the data cost
            totalDataCost = (float)(dataCostPerKm * distanceToTarget);
        }

        protected void calculateDataGroundToOrbit()
        {
            totalDataCost = (float)(dataCostPerKm * (selectedPipelineNode.vessel.altitude / 1000));

            //Orbital shots are more expensive
            totalDataCost *= orbitalCostMultiplier;
        }

        protected void calculateDataOrbitToOrbit()
        {
            totalDataCost = (float)(dataCostPerKm * (Math.Abs(selectedPipelineNode.vessel.altitude - FlightGlobals.ActiveVessel.altitude) / 1000));
        }

        protected void drawTrajectoryStatus()
        {
            if (payloadMass < 0.001f)
            {
                GUILayout.Label("<color=white><b>Guidance: </b></color>GO");
                return;
            }

            switch (transferType)
            {
                default:
                case WBITransferTypes.GroundToGround:
                    calculateDataGroundToGround();
                    break;

                case WBITransferTypes.OrbitToGround:
                case WBITransferTypes.GroundToOrbit:
                    calculateDataGroundToOrbit();
                    break;

                case WBITransferTypes.OrbitToOrbit:
                    calculateDataOrbitToOrbit();
                    break;
            }

            if (totalGuidanceData >= totalDataCost)
            {
                GUILayout.Label("<color=white><b>Guidance: </b></color>GO");
            }

            else
            {
                GUILayout.Label("<color=white><b>Guidance: </b></color><color=red>NO GO</color>");
                issues.AppendLine(string.Format(kInsufficientGuidance, (totalDataCost - totalGuidanceData)));
            }
        }

        protected void drawDeltaVStatus()
        {
            if (payloadMass < 0.001f)
            {
                GUILayout.Label("<color=white><b>Delta-V: </b></color>GO");
                return;
            }

            //Determine type of shot
            bool hasEnoughDeltaV = false;
            switch (transferType)
            {
                default:
                case WBITransferTypes.GroundToGround:
                    hasEnoughDeltaV = drawDeltaVStatusGroundToGround();
                    break;

                case WBITransferTypes.OrbitToGround:
                case WBITransferTypes.GroundToOrbit:
                    hasEnoughDeltaV = drawDeltaVStatusGroundToOrbit();
                    break;

                case WBITransferTypes.OrbitToOrbit:
                    hasEnoughDeltaV = drawDeltaVStatusOrbitToOrbit();
                    break;
            }

            if (hasEnoughDeltaV)
                GUILayout.Label("<color=white><b>Delta-V: </b></color>GO");
            else
                GUILayout.Label("<color=white><b>Delta-V: </b></color><color=red>NO GO</color>");
        }

        protected bool drawDeltaVStatusGroundToOrbit()
        {
            CelestialBody celestialBody = selectedPipelineNode.vessel.mainBody;

            //Calculate orbital velocity needed
            double orbitalVelocity = Math.Sqrt(celestialBody.gravParameter / (selectedPipelineNode.vessel.altitude + celestialBody.Radius));

            //Calculate the projectile velocity
            double projectileVelocity = Math.Sqrt((maxKineticEnergy * 2) / (1000 * projectileMass));

            //If the payload velocity meets or exceeds orbital velocity then we're good.
            if (projectileVelocity >= orbitalVelocity)
            {
                return true;
            }

            else
            {
                issues.AppendLine(string.Format(kInsufficientDeltaV, (orbitalVelocity - projectileVelocity)));
                return false;
            }
        }

        protected bool drawDeltaVStatusOrbitToOrbit()
        {
            CelestialBody celestialBody = FlightGlobals.ActiveVessel.mainBody;

            //Calculate the Hohmann transfer deltaV requirements
            double celestialBodyRadius = celestialBody.Radius;
            double gravParameter = celestialBody.gravParameter;

            double startAltitude = FlightGlobals.ActiveVessel.altitude;
            double endAltitude = selectedPipelineNode.vessel.altitude;

            double endpointOrbitalRadius = celestialBodyRadius + startAltitude;
            double targetOrbitalRadius = celestialBodyRadius + endAltitude;
            double semiMajorElipseAxis = (endpointOrbitalRadius + targetOrbitalRadius) / 2.0f;
            double initialVelocity = Math.Sqrt(gravParameter / endpointOrbitalRadius);
            double finalVelocity = Math.Sqrt(gravParameter / targetOrbitalRadius);
            double transferOrbitVelocity = Math.Sqrt(gravParameter * (2 / endpointOrbitalRadius - 1 / semiMajorElipseAxis));
            double circularizationOrbitVelocity = Math.Sqrt(gravParameter * (2 / targetOrbitalRadius - 1 / semiMajorElipseAxis));

            double initialVelocityChange = Math.Abs(transferOrbitVelocity - initialVelocity);
            double finalVelocityChange = Math.Abs(finalVelocity - circularizationOrbitVelocity);

            //Calculate inclination change deltaV requirements
            double angleChange = Math.Abs(FlightGlobals.ActiveVessel.orbit.inclination - selectedPipelineNode.vessel.orbit.inclination);
            angleChange = (Math.PI/180.0f) * angleChange;
            double inclinationDeltaV = (2 * initialVelocity) * Math.Sin(angleChange / 2.0f);

            //Calculate the projectile velocity
            double projectileVelocity = Math.Sqrt((maxKineticEnergy * 2) / (1000 * projectileMass));

            //Can we afford the initial velocity?
            initialVelocityChange += inclinationDeltaV;
            if (projectileVelocity < initialVelocityChange)
            {
                issues.AppendLine(string.Format(kInsufficientDeltaV, (initialVelocityChange - projectileVelocity)));
                return false;
            }

            //Can the target pipeline handle the deceleration?
            double destinationKineticEnergy = 0f;
            if (selectedPipelineNode.moduleValues.HasValue("maxKineticEnergy"))
                destinationKineticEnergy = double.Parse(selectedPipelineNode.moduleValues.GetValue("maxKineticEnergy"));
            double decelerationVelocity = Math.Sqrt((destinationKineticEnergy * 2) / (1000 * projectileMass));
            if (decelerationVelocity < finalVelocityChange)
            {
                issues.AppendLine(string.Format(kInsufficientDeltaVTarget, (finalVelocityChange - decelerationVelocity)));
                return false;
            }

            return true;
        }

        protected bool drawDeltaVStatusGroundToGround()
        {
            CelestialBody celestialBody = selectedPipelineNode.vessel.mainBody;

            //Calculate the projectile velocity
            double projectileVelocity = Math.Sqrt((maxKineticEnergy * 2) / (1000 * projectileMass));

            //We don't meet or exceed orbital velocity, but the target might still be in ballistics range.
            //Get the distance to the target.
            double distanceToTarget = Utils.HaversineDistance(this.part.vessel.longitude, this.part.vessel.latitude, 
                selectedPipelineNode.vessel.longitude, selectedPipelineNode.vessel.latitude, celestialBody);

            //Now we need to compute the ballistic trajectory. We assume a 45 degree angle on the trajectory angle. That gives max distance.
            double distanceTraveled = (projectileVelocity * projectileVelocity) / celestialBody.GeeASL;
            if (distanceTraveled >= distanceToTarget)
            {
                return true;
            }

            else
            {
                issues.AppendLine(string.Format(kInsufficientRange, (distanceToTarget - distanceTraveled)));
                return false;
            }
        }

        protected void drawECStatus()
        {
            if (payloadMass < 0.001f)
            {
                GUILayout.Label("<color=white><b>ElectricCharge: </b></color>GO");
                return;
            }

            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            PartResourceDefinition def;
            double amountAvailable = 0f;
            double maxAmount = 0;
            double amountRequired = electricityCostPerTonne * projectileMass;

            def = definitions["ElectricCharge"];
            FlightGlobals.ActiveVessel.rootPart.GetConnectedResourceTotals(def.id, out amountAvailable, out maxAmount, true);

            if (amountAvailable >= amountRequired)
            {
                GUILayout.Label("<color=white><b>ElectricCharge: </b></color>GO");
            }
            else
            {
                GUILayout.Label("<color=white><b>ElectricCharge: </b></color><color=red>NO GO</color>");
                issues.AppendLine(string.Format(kECShortage, (amountRequired - amountAvailable)));
            }
        }

        protected void drawLFOStatus()
        {
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            PartResourceDefinition def;
            WBIResourceTotals[] resourceTotals = sourceVesselResources.Values.ToArray();
            WBIResourceTotals totals;

            //Calculate the LiquidFuel requirements
            def = definitions["LiquidFuel"];
            liquidFuelUnits = ((fuelMassFraction * payloadMass) * kLiquidFuelRatio) / def.density;

            totals.amount = 0f;
            totals.transferPercent = 0f;
            totals.maxAmount = 0f;
            for (int index = 0; index < resourceTotals.Length; index++)
            {
                totals = resourceTotals[index];
                if (totals.resourceName == "LiquidFuel")
                {
                    break;
                }
            }
            if (totals.maxAmount - (totals.amount * totals.transferPercent) >= liquidFuelUnits)
            {
                GUILayout.Label("<color=white><b>LiquidFuel: </b></color>GO");
            }
            else
            {
                GUILayout.Label("<color=white><b>LiquidFuel: </b></color><color=red>NO GO</color>");
                issues.AppendLine(string.Format(kLFShortage, (liquidFuelUnits - (totals.maxAmount - (totals.amount * totals.transferPercent)))));
            }

            //Calculate the Oxidizer requirements
            def = definitions["Oxidizer"];
            oxidizerUnits = ((fuelMassFraction * payloadMass) * kOxidizerRatio) / def.density;

            totals.amount = 0f;
            totals.transferPercent = 0f;
            totals.maxAmount = 0f;
            for (int index = 0; index < resourceTotals.Length; index++)
            {
                totals = resourceTotals[index];
                if (totals.resourceName == "Oxidizer")
                {
                    break;
                }
            }
            if (totals.maxAmount - (totals.amount * totals.transferPercent) >= oxidizerUnits)
            {
                GUILayout.Label("<color=white><b>Oxidizer: </b></color>GO");
            }
            else
            {
                GUILayout.Label("<color=white><b>Oxidizer: </b></color><color=red>NO GO</color>");
                issues.AppendLine(string.Format(kOxShortage, (oxidizerUnits - (totals.maxAmount - (totals.amount * totals.transferPercent)))));
            }
        }

        protected double calculateKineticEnergy(double payloadMass, double velocity)
        {
            double totalMass = (payloadMass + (payloadMass * fuelMassFraction));
            totalMass += (totalMass * dryMassFraction);
            totalMass *= 1000.0f;
            double kineticEnergy = 0.5f * (totalMass * (velocity * velocity));
            return kineticEnergy;
        }

        protected double getPayloadMass()
        {
            double payloadMass = 0;
            double resourceMass = 0;

            WBIResourceTotals[] resourceTotals = sourceVesselResources.Values.ToArray();
            for (int index = 0; index < resourceTotals.Length; index++)
            {
                resourceMass = resourceTotals[index].amount * resourceTotals[index].transferPercent * resourceTotals[index].density;
                payloadMass += resourceMass;
            }

            return payloadMass + packingListMass;
        }

        protected void getVesselResources(Vessel vessel, Dictionary<string, WBIResourceTotals> resourceMap)
        {
            resourceMap.Clear();

            Part vesselPart;
            int totalParts = vessel.parts.Count;
            PartResource resource;
            int totalResources;
            WBIResourceTotals resourceTotals;
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            PartResourceDefinition def;
            string displayName;

            for (int index = 0; index < totalParts; index++)
            {
                vesselPart = vessel.parts[index];
                if (vesselPart == this.part)
                    continue;

                totalResources = vesselPart.Resources.Count;
                for (int resourceIndex = 0; resourceIndex < totalResources; resourceIndex++)
                {
                    //Get the resource
                    resource = vesselPart.Resources[resourceIndex];
                    if (blackListedResources.Contains(resource.resourceName))
                        continue;

                    //Get the display name
                    def = definitions[resource.resourceName];
                    if (!string.IsNullOrEmpty(def.displayName))
                        displayName = def.displayName;
                    else
                        displayName = resource.resourceName;

                    //Fill out the totals
                    if (!resourceMap.ContainsKey(displayName))
                    {
                        resourceTotals = new WBIResourceTotals();
                        resourceTotals.resourceName = resource.resourceName;
                        resourceTotals.displayName = displayName;
                        resourceMap.Add(displayName, resourceTotals);
                    }
                    resourceTotals = resourceMap[displayName];
                    resourceTotals.amount += resource.amount;
                    resourceTotals.maxAmount += resource.maxAmount;
                    resourceTotals.density = def.density;
                    resourceMap[displayName] = resourceTotals;
                }
            }
        }

        protected WBITransferTypes getTransferType()
        {
            //Ground to ground, ground to orbit
            if ((FlightGlobals.ActiveVessel.situation == Vessel.Situations.LANDED ||
                FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH ||
                FlightGlobals.ActiveVessel.situation == Vessel.Situations.SPLASHED))
            {
                if (selectedPipelineNode.vessel.situation == Vessel.Situations.LANDED ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.PRELAUNCH ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.SPLASHED)
                    return WBITransferTypes.GroundToGround;

                //Ground to orbit
                else if (selectedPipelineNode.vessel.situation == Vessel.Situations.ORBITING ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.FLYING)
                    return WBITransferTypes.GroundToOrbit;

                else
                {
                    Debug.Log("[WBIPipeEndpoint] - Can't determine transfer type: Active Vessel is ground.");
                    return WBITransferTypes.Unknown;
                }
            }

            //Orbit to orbit, orbit to ground
            else if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING ||
                FlightGlobals.ActiveVessel.situation == Vessel.Situations.FLYING)
            {
                //Ground to orbit
                if (selectedPipelineNode.vessel.situation == Vessel.Situations.ORBITING ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.FLYING)
                    return WBITransferTypes.OrbitToOrbit;

                else if (selectedPipelineNode.vessel.situation == Vessel.Situations.LANDED ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.PRELAUNCH ||
                    selectedPipelineNode.vessel.situation == Vessel.Situations.SPLASHED)
                    return WBITransferTypes.OrbitToGround;

                else
                {
                    Debug.Log("[WBIPipeEndpoint] - Can't determine transfer type: Active Vessel is orbiting.");
                    return WBITransferTypes.Unknown;
                }
            }

            return WBITransferTypes.Unknown;
        }

        protected bool canReceiveTransfers(Vessel vessel, WBIPipeEndpoint pipeEndpoint)
        {
            if (!pipeEndpoint.IsActivated)
                return false;

            if (string.IsNullOrEmpty(pipeEndpoint.uniqueIdentifier))
                return false;

            //If this is an orbit-to-ground transfer, then it might not be allowed.
            if ((FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING) &&
                (vessel.situation == Vessel.Situations.LANDED ||
                vessel.situation == Vessel.Situations.PRELAUNCH ||
                vessel.situation == Vessel.Situations.SPLASHED))
                return allowOrbitToGround;

            //We're good
            Log("canReceiveTransfers: Pipeline found on " + vessel.vesselName);
            return true;
        }

        protected bool canReceiveTransfers(Vessel vessel, ProtoPartModuleSnapshot protoModule)
        {
            //Is the pipeline activated?
            if (!protoModule.moduleValues.HasValue("IsActivated"))
            {
                Log("canReceiveTransfers: Pipeline not activated");
                return false;
            }
            if (protoModule.moduleValues.GetValue("IsActivated").ToLower() != "true")
            {
                Log("canReceiveTransfers: Pipeline not activated");
                return false;
            }
            if (!protoModule.moduleValues.HasValue("uniqueIdentifier"))
            {
                Log("canReceiveTransfers: Pipeline UUID not found");
                return false;
            }

            //If this is an orbit-to-ground transfer, then it might not be allowed.
            if ((FlightGlobals.ActiveVessel.situation == Vessel.Situations.ORBITING) &&
                (vessel.situation == Vessel.Situations.LANDED ||
                vessel.situation == Vessel.Situations.PRELAUNCH ||
                vessel.situation == Vessel.Situations.SPLASHED))
                return allowOrbitToGround;

            //We're good
            Log("canReceiveTransfers: Pipeline found on " + vessel.vesselName);
            return true;
        }

        protected void drawSelectVesselPage()
        {
            if (pipeEndpoints == null)
            {
                GUILayout.Label(kNoVesselsInRange);
                return;
            }

            //Directions
            GUILayout.Label(kSelectVessel);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();

            //Selection list
            targetPanel = GUILayout.BeginScrollView(targetPanel);

            selectedIndex = GUILayout.SelectionGrid(selectedIndex, vesselNames, 1);
            selectedPipelineNode = pipeEndpoints[selectedIndex];

            GUILayout.EndScrollView();

            //If the part has a WBITrackingController then point at the target.
            WBITrackingController controller = this.part.FindModuleImplementing<WBITrackingController>();
            if (controller != null && controller.targetVessel != selectedPipelineNode.vessel)
            {
                controller.ClearTarget();
                controller.targetVessel = selectedPipelineNode.vessel;
                controller.UpdateTarget();
            }

            GUILayout.EndVertical();

            //Go/NO-GO
            bool launchAzimuthIsGo = true;
            if (enableAzimuthRestriction)
            {
                GUILayout.BeginVertical();
                targetPane = GUILayout.BeginScrollView(targetPane, targetPaneOptions);

                //Check launch azimuth
                Vector3 inversePosition = this.part.transform.InverseTransformPoint(selectedPipelineNode.vessel.vesselTransform.position);
                float azimuth = Mathf.Atan2(inversePosition.x, inversePosition.z) * Mathf.Rad2Deg;
                if (azimuth <= 0)
                    azimuth = 360 + azimuth;
                if (azimuth >= 0 && azimuth <= maxLaunchAzimuth)
                {
                    GUILayout.Label(string.Format(kLaunchAzimuthGo, Mathf.Abs(azimuth)));
                }
                else if (azimuth <= 180 && azimuth >= 180 - maxLaunchAzimuth)
                {
                    GUILayout.Label(string.Format(kLaunchAzimuthGo, Mathf.Abs(azimuth)));
                }
                else if (azimuth > 180 && azimuth <= 180 + maxLaunchAzimuth)
                {
                    GUILayout.Label(string.Format(kLaunchAzimuthGo, Mathf.Abs(azimuth)));
                }
                else if (azimuth >= 360 - maxLaunchAzimuth)
                {
                    GUILayout.Label(string.Format(kLaunchAzimuthGo, Mathf.Abs(azimuth)));
                }
                else
                {
                    GUILayout.Label(string.Format(kLaunchAzimuthNoGo, Mathf.Abs(azimuth)));
                    launchAzimuthIsGo = false;
                }

                GUILayout.EndScrollView();
                GUILayout.EndVertical();
            }

            GUILayout.EndHorizontal();

            //Next button
            if (GUILayout.Button(kNextLabel))
            {
                if (launchAzimuthIsGo)
                {
                    //Get transfer type
                    transferType = getTransferType();

                    //Make sure that the transfer is allowed
                    if (transferType == WBITransferTypes.OrbitToGround && !allowOrbitToGround)
                    {
                        ScreenMessages.PostScreenMessage(kNoOrbitToGroundMsg, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                    }

                    //Set next page
                    else
                    {
                        pageID = PipelineViewPages.ChooseResources;
                    }
                }
                else
                {
                    ScreenMessages.PostScreenMessage(kLaunchAzimuthNoGoAlert, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                }
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            DrawView();
        }
    }
}
