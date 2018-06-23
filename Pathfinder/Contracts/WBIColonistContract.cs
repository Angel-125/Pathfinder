using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Contracts;
using Contracts.Parameters;
using KSP;
using KSPAchievements;
using ContractsPlus.Contracts;
using FinePrint.Contracts.Parameters;

namespace ContractsPlus.Contracts
{
    public class WBIColonistContract : Contract
    {
        public const int CurrentContractVersion = 1;

        public const string ContractType = "WBIColonistContract";
        public const string ContractTitle = "Ferry {0} potential colonists to {1}";
        public const string ContractTitleSingle = "Ferry a potential colonist to {0}";
        public const string SynopsisMany = "Ferry {0} potential colonists to {1}";
        public const string SynopsisSingle = "Ferry a potential colonist to {0}";
        public const string ContractCompleteMsg = "{0} potential colonist successfully delivered to {1}";
        public const string DescSingle = "{0} would like to experience life as a colonist and after extensive research, {1} wants to visit {2} for several days. Strangely, the proposed contract doesn't have provisions to return {3} home. Perhaps {4} wants to make the visit permanent?";
        public const string DescMany = "A group of tourists would like to experience life as a colonist and after extensive research, they want to visit {0} for several days. Strangely, the proposed contract doesn't have provisions for them to return home. Perhaps they want to make the visit permanent?";
        public const string DescNote = "\r\n\r\nNOTE: Tourists cannot EVA, so you'll need a way to connect the transport craft to {0}.";

        const float fundsAdvance = 15000f;
        const float fundsCompleteBase = 15000f;
        const float fundsFerryComplete = 5000f;
        const float fundsStayComplete = 1000f;
        const float fundsFailure = 35000f;
        const float repComplete = 10f;
        const float repFailure = 10f;
        const float rewardAdjustmentFactor = 0.4f;
        const int MaxTourists = 6;
        const int minimumDays = 2;
        const int maximumDays = 20;

        CelestialBody targetBody = null;
        public int versionNumber = 1;
        string contractID = string.Empty;
        int totalTourists = 0;
        string vesselName = "None";
        List<Vessel> destinationCandidates = new List<Vessel>();
        Dictionary<Vessel, string> partGUIDs = new Dictionary<Vessel, string>();
        int totalDays;
        ProtoCrewMember tourist;
        List<string> kerbalNames = new List<string>();

        protected void Log(string message)
        {
            if (WildBlueIndustries.PathfinderSettings.LoggingEnabled)
            {
                Debug.Log("[WBIColonistContract] - " + message);
            }
        }

        protected override bool Generate()
        {
            int contractCount = WBIContractScenario.Instance.GetContractCount(ContractType);
            Log("Trying to generate a WBIColonistContract, count: " + contractCount + "/" + WBIContractScenario.maxContracts);
            if (contractCount == WBIContractScenario.maxContracts)
                return false;

            //Find destination candidates
            if (destinationCandidates.Count == 0)
                getDestinationCandidates();
            if (destinationCandidates.Count == 0) 
                return false;

            //Determine which candidate to use
            int candidateID = UnityEngine.Random.Range(0, destinationCandidates.Count);
            Vessel targetVessel = destinationCandidates[candidateID];
            vesselName = targetVessel.vesselName;
            targetBody = targetVessel.mainBody;
            Log("Target vessel: " + vesselName);
            Log("Target body: " + targetBody);

            bool isOrbiting = false;
            if (targetVessel.situation == Vessel.Situations.ORBITING)
                isOrbiting = true;

            //Generate number of tourists
            totalTourists = UnityEngine.Random.Range(1, MaxTourists) * ((int)prestige + 1);
            Log("totalTourists: " + totalTourists);

            //Generate total days
            totalDays = UnityEngine.Random.Range(minimumDays, maximumDays) * ((int)prestige + 1);
            Log("totalDays: " + totalDays);

            //Calculate completion funds
            float deliveryFunds;
            float stayFunds;
            float totalFunds;
            if (!isOrbiting)
            {
                deliveryFunds = fundsFerryComplete * targetBody.scienceValues.LandedDataValue;
                stayFunds = fundsStayComplete * (float)totalDays * targetBody.scienceValues.LandedDataValue;
                totalFunds = fundsCompleteBase * targetBody.scienceValues.LandedDataValue;
            }
            else
            {
                deliveryFunds = fundsFerryComplete * targetBody.scienceValues.InSpaceLowDataValue;
                stayFunds = fundsStayComplete * (float)totalDays * targetBody.scienceValues.InSpaceLowDataValue;
                totalFunds = fundsCompleteBase * targetBody.scienceValues.InSpaceLowDataValue;
            }
            stayFunds *= ((float)prestige + 1.0f);
            totalFunds *= ((float)prestige + 1.0f);

            //Be in command of <targetVessel> parameter
            SpecificVesselParameter specificVesselParam = new SpecificVesselParameter(targetVessel);
            this.AddParameter(specificVesselParam, null);

            //Generate kerbals
            WBIFerryKerbalParam ferryParameter;
            WBIKerbalStayParam stayParameter;
            KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
            kerbalNames.Clear();
            for (int index = 0; index < totalTourists; index++)
            {
                tourist = createTourist();

                //Stay at vessel parameter
                stayParameter = new WBIKerbalStayParam(vesselName, tourist.name, totalDays);
                this.AddParameter(stayParameter, null); //Do this before setting other things in the parameter
                stayParameter.SetFunds(stayFunds, targetBody);

                //Ferry to vessel parameter
                ferryParameter = new WBIFerryKerbalParam(vesselName, tourist.name);
                stayParameter.AddParameter(ferryParameter, null);
                ferryParameter.SetFunds(deliveryFunds, targetBody);

                //Record funds
                totalFunds += stayFunds + deliveryFunds;

                //Clean up the roster- we only generate tourists when the contract is accepted.
                kerbalNames.Add(tourist.name);
                roster.Remove(tourist.name);
            }

            //Set rewards
            base.SetExpiry();
            base.SetDeadlineYears(10f, targetBody);
            base.SetReputation(repComplete, repFailure, targetBody);
            base.SetFunds(fundsAdvance, totalFunds, totalFunds * 0.25f, targetBody);

            //Record contract
            contractCount += 1;
            if (contractCount > WBIContractScenario.maxContracts)
                contractCount = WBIContractScenario.maxContracts;
            WBIContractScenario.Instance.SetContractCount(ContractType, contractCount);

            //Done
            if (string.IsNullOrEmpty(contractID))
                contractID = Guid.NewGuid().ToString();
            return true;
        }

        public override bool CanBeCancelled()
        {
            return true;
        }

        public override bool CanBeDeclined()
        {
            return true;
        }

        protected override string GetHashString()
        {
            if (string.IsNullOrEmpty(contractID))
                contractID = Guid.NewGuid().ToString();

            return contractID;
        }

        protected override string GetTitle()
        {
            if (totalTourists > 1)
                return string.Format(ContractTitle, totalTourists, vesselName);
            else
                return string.Format(ContractTitleSingle, vesselName);
        }

        protected override string GetDescription()
        {
            if (totalTourists > 1)
            {
                return string.Format(DescMany, vesselName) + string.Format(DescNote, vesselName);
            }
            else
            {
                string himHer = tourist.gender == ProtoCrewMember.Gender.Male ? "him" : "her";
                string heShe = tourist.gender == ProtoCrewMember.Gender.Male ? "he" : "she";
                return string.Format(DescSingle, tourist.name, heShe, vesselName, himHer, heShe) + string.Format(DescNote, vesselName);
            }
        }

        protected override string GetSynopsys()
        {
            if (totalTourists > 1)
                return string.Format(SynopsisMany, totalTourists, vesselName);
            else
                return string.Format(SynopsisSingle, vesselName);
        }

        protected override string MessageCompleted()
        {
            return string.Format(ContractCompleteMsg, totalTourists, vesselName);
        }

        protected override void OnLoad(ConfigNode node)
        {
            contractID = node.GetValue("contractID");
            if (int.TryParse("versionNumber", out versionNumber) == false)
                versionNumber = CurrentContractVersion;

            int bodyID = int.Parse(node.GetValue("targetBody"));
            foreach (var body in FlightGlobals.Bodies)
            {
                if (body.flightGlobalsIndex == bodyID)
                    targetBody = body;
            }
            totalTourists = int.Parse(node.GetValue("totalTourists"));
            vesselName = node.GetValue("vesselName");
            totalDays = int.Parse(node.GetValue("totalDays"));

            ConfigNode[] touristNodes = node.GetNodes("TOURIST");
            kerbalNames.Clear();
            foreach (ConfigNode touristNode in touristNodes)
            {
                kerbalNames.Add(touristNode.GetValue("name"));
            }
        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractID", contractID);
            node.AddValue("versionNumber", CurrentContractVersion);

            int bodyID = targetBody.flightGlobalsIndex;
            node.AddValue("targetBody", bodyID);
            node.AddValue("totalTourists", totalTourists);
            node.AddValue("vesselName", vesselName);
            node.AddValue("totalDays", totalDays);

            ConfigNode touristNode;
            foreach (string kerbalName in kerbalNames)
            {
                touristNode = new ConfigNode("TOURIST");
                touristNode.AddValue("name", kerbalName);
                node.AddNode(touristNode);
            }
        }

        protected override void OnParameterStateChange(ContractParameter p)
        {
            base.OnParameterStateChange(p);

            foreach (ContractParameter parameter in AllParameters)
            {
                if (parameter.State == ParameterState.Incomplete || parameter.State == ParameterState.Failed)
                {
                    return;
                }
            }

            //All parameters are complete
            SetState(State.Completed);
        }

        protected void decrementContractCount()
        {
            int contractCount = WBIContractScenario.Instance.GetContractCount(ContractType) - 1;
            if (contractCount < 0)
                contractCount = 0;
            WBIContractScenario.Instance.SetContractCount(ContractType, contractCount);
        }

        protected override void OnAccepted()
        {
            base.OnAccepted();

            foreach (string kerbalName in kerbalNames)
            {
                createTourist(kerbalName);
            }
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();
            decrementContractCount();
            removeTourists();
        }

        protected override void OnFailed()
        {
            base.OnFailed();
            decrementContractCount();
            removeTourists();
        }

        protected override void OnFinished()
        {
            base.OnFinished();
            decrementContractCount();
            removeTourists();
        }

        protected override void OnDeclined()
        {
            base.OnDeclined();
            decrementContractCount();
            removeTourists();
        }

        protected override void OnOfferExpired()
        {
            base.OnOfferExpired();
            decrementContractCount();
            removeTourists();
        }

        protected override void OnCancelled()
        {
            base.OnCancelled();
            decrementContractCount();
            removeTourists();
        }

        public override bool MeetRequirements()
        {
            int contractCount = WBIContractScenario.Instance.GetContractCount(ContractType);
            if (contractCount == WBIContractScenario.maxContracts)
                return false;
            else
                return true;
            /*
            Log("Checking for requirements...");
            //Is there a vessel that has one of the required parts?
            getDestinationCandidates();
            if (destinationCandidates.Count > 0)
                return true;
            else
                return false;
             */
        }

        protected void removeTourists()
        {
            WBIKerbalStayParam stayParam;
            KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;

            foreach (ContractParameter parameter in AllParameters)
            {
                if (parameter is WBIKerbalStayParam)
                {
                    stayParam = (WBIKerbalStayParam)parameter;
                    if (roster[stayParam.kerbalName] != null)
                    {
                        //Remove them if they haven't flown yet.
                        if (roster[stayParam.kerbalName].rosterStatus == ProtoCrewMember.RosterStatus.Available)
                            roster.Remove(stayParam.kerbalName);

                        //Remove them when they recover
                        else
                            WBIContractScenario.Instance.registerKerbal(stayParam.kerbalName);
                    }
                }
            }
        }

        protected void getDestinationCandidates()
        {
            Log("Looking for destination candidates for colonist contracts");
            destinationCandidates.Clear();
            partGUIDs.Clear();

            //Loaded vessels
            //Only parts with a WBITouristTrap will be considered for the contract.
            int vesselCount = FlightGlobals.VesselsLoaded.Count;
            int partCount;
            Vessel vessel;
            int totalModules;
            WBITouristTrap touristTrap;
            if (vesselCount > 0)
            {
                Log("There are " + vesselCount + " loaded vessels.");
                for (int index = 0; index < vesselCount; index++)
                {
                    vessel = FlightGlobals.VesselsLoaded[index];
                    if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject || vessel.vesselType == VesselType.Unknown || vessel.vesselType == VesselType.EVA)
                    {
                        Log("Skipping vessel " + vessel.vesselName + " of type " + vessel.vesselType);
                        continue;
                    }
                    if (vessel.mainBody.isHomeWorld && prestige != ContractPrestige.Trivial)
                        continue;
                    touristTrap = vessel.FindPartModuleImplementing<WBITouristTrap>();
                    if (touristTrap != null)
                    {
                        destinationCandidates.Add(vessel);
                    }
                }
            }

            //Unloaded vessels
            //Only parts with a WBITouristTrap will be considered for the contract.
            vesselCount = FlightGlobals.VesselsUnloaded.Count;
            if (vesselCount > 0)
            {
                ProtoPartSnapshot partSnapshot;
                ProtoPartModuleSnapshot moduleSnapshot;
                Log("There are " + vesselCount + " unloaded vessels.");
                for (int index = 0; index < vesselCount; index++)
                {
                    vessel = FlightGlobals.VesselsUnloaded[index];
                    if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject || vessel.vesselType == VesselType.Unknown || vessel.vesselType == VesselType.EVA)
                    {
                        Log("Skipping vessel " + vessel.vesselName + " of type " + vessel.vesselType);
                        continue;
                    }
                    if (vessel.mainBody.isHomeWorld && prestige != ContractPrestige.Trivial)
                        continue;
                    partCount = vessel.protoVessel.protoPartSnapshots.Count;
                    for (int partIndex = 0; partIndex < partCount; partIndex++)
                    {
                        partSnapshot = vessel.protoVessel.protoPartSnapshots[partIndex];
                        totalModules = partSnapshot.modules.Count;
                        for (int moduleIndex = 0; moduleIndex < totalModules; moduleIndex++)
                        {
                            moduleSnapshot = partSnapshot.modules[moduleIndex];

                            //Find the tourist trap
                            if (moduleSnapshot.moduleName == "WBITouristTrap")
                            {
                                destinationCandidates.Add(vessel);
                                break;
                            }
                        }
                    }
                }
            }

            //Did we find any?
            if (destinationCandidates.Count > 0)
            {
                if (WildBlueIndustries.PathfinderSettings.LoggingEnabled)
                {
                    vesselCount = destinationCandidates.Count;
                    Log("Found " + vesselCount + " destination candidates");
                    for (int index = 0; index < vesselCount; index++)
                        Log("Destination candidate: " + destinationCandidates[index].vesselName);
                }
            }
            else
            {
                Log("No candidates found");
            }

        }

        protected ProtoCrewMember createTourist(string kerbalName = null)
        {
            KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
            string message = string.Empty;
            ProtoCrewMember newRecruit = roster.GetNewKerbal(ProtoCrewMember.KerbalType.Tourist);
            if (!string.IsNullOrEmpty(kerbalName))
                newRecruit.ChangeName(kerbalName);
            Log("Created new tourist: " + newRecruit.name);

            newRecruit.rosterStatus = ProtoCrewMember.RosterStatus.Available;

            //Game events
            newRecruit.UpdateExperience();
            roster.Update(Planetarium.GetUniversalTime());
            GameEvents.onKerbalAdded.Fire(newRecruit);

            return newRecruit;
        }
    }
}
