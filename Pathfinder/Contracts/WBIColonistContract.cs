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

namespace ContractsPlus.Contracts
{
    public class WBIColonistContract : Contract
    {
        public const int CurrentContractVersion = 1;

        public const string ContractType = "WBIColonistContract";
        public const string ContractTitle = "Ferry {0} tourists to {1}";
        public const string ContractTitleSingle = "Ferry a tourist to {0}";
        public const string SynopsisMany = "Ferry {0} tourists to {1}";
        public const string SynopsisSingle = "Ferry a tourist to {0}";
        public const string ContractCompleteMsg = "{0} tourists successfully delivered to {1}";
        public const string DescSingle = "{0} would like to experience life as a colonist and after extensive research, and wants to visit {1} for several days. Strangely, the proposed contract doesn't have provisions to return {2} home. Perhaps {3} wants to make the visit permanent?";
        public const string DescMany = "A group of tourists would like to experience life as a colonist and after extensive research, they want to visit {0} for several days. Strangely, the proposed contract doesn't have provisions for them to return home. Perhaps they want to make the visit permanent?";

        //Any one of these modules qualifies
        const string requiredModules = "WBITouristTrap";

        const float fundsAdvance = 15000f;
        const float fundsCompleteBase = 15000f;
        const float fundsFerryComplete = 5000f;
        const float fundsStayComplete = 1000f;
        const float fundsFailure = 35000f;
        const float repComplete = 10f;
        const float repFailure = 10f;
        const float rewardAdjustmentFactor = 0.4f;
        const int MaxTourists = 10;
        const int minimumDays = 2;
        const int maximumDays = 20;

        CelestialBody targetBody = null;
        public int versionNumber = 1;
        string contractID = string.Empty;
        int totalTourists = 0;
        string vesselName = "None";
        string vesselID = string.Empty;
        List<Vessel> destinationCandidates = new List<Vessel>();
        int totalDays;
        ProtoCrewMember tourist;

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
            vesselID = targetVessel.id.ToString();
            targetBody = targetVessel.mainBody;
            Log("Target vessel: " + vesselName);
            Log("Target body: " + targetBody);

            bool isOrbiting = false;
            if (targetVessel.situation == Vessel.Situations.ORBITING)
                isOrbiting = true;

            //Generate number of tourists
            totalTourists = UnityEngine.Random.Range(1, MaxTourists);
            Log("totalTourists: " + totalTourists);

            //Generate total days
            totalDays = UnityEngine.Random.Range(minimumDays, maximumDays);
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

            //Generate kerbals
            WBIFerryKerbalParam ferryParameter;
            WBIKerbalStayParam stayParameter;
            for (int index = 0; index < totalTourists; index++)
            {
                tourist = createTourist();

                //Ferry to vessel parameter
                ferryParameter = new WBIFerryKerbalParam(vesselID, vesselName, tourist.name);
                this.AddParameter(ferryParameter, null); //Do this before setting other things in the parameter
                ferryParameter.SetFunds(deliveryFunds, targetBody);

                //Stay at vessel parameter (added to Ferry parameter)
                stayParameter = new WBIKerbalStayParam(vesselID, vesselName, tourist.name, totalDays);
                ferryParameter.AddParameter(stayParameter);
                stayParameter.SetFunds(stayFunds, targetBody);

                totalFunds += stayFunds + deliveryFunds;
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
//            GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.APPEND);
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
                return string.Format(DescMany, vesselName);
            }
            else
            {
                string himHer = tourist.gender == ProtoCrewMember.Gender.Male ? "him" : "her";
                string heShe = tourist.gender == ProtoCrewMember.Gender.Male ? "he" : "she";
                return string.Format(DescSingle, tourist.name, vesselName, himHer, heShe);
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
            vesselID = node.GetValue("vesselID");
            vesselName = node.GetValue("vesselName");
            totalDays = int.Parse(node.GetValue("totalDays"));
        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("contractID", contractID);
            node.AddValue("versionNumber", CurrentContractVersion);

            int bodyID = targetBody.flightGlobalsIndex;
            node.AddValue("targetBody", bodyID);
            node.AddValue("totalTourists", totalTourists);
            node.AddValue("vesselID", vesselID);
            node.AddValue("vesselName", vesselName);
            node.AddValue("totalDays", totalDays);
        }

        protected override void OnParameterStateChange(ContractParameter p)
        {
            base.OnParameterStateChange(p);

            foreach (ContractParameter parameter in AllParameters)
            {
                if (parameter.State == ParameterState.Incomplete || parameter.State == ParameterState.Failed)
                    return;
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

        protected override void OnOffered()
        {
            base.OnOffered();
        }

        protected override void OnAccepted()
        {
            base.OnAccepted();

            //Generate the specific kerbals and add the destination parameter
        }

        protected override void OnCompleted()
        {
            base.OnCompleted();
            decrementContractCount();
        }

        protected override void OnFailed()
        {
            base.OnFailed();
            decrementContractCount();
        }

        protected override void OnFinished()
        {
            base.OnFinished();
            decrementContractCount();
        }

        protected override void OnDeclined()
        {
            base.OnDeclined();
            decrementContractCount();
        }

        protected override void OnOfferExpired()
        {
            base.OnOfferExpired();
            decrementContractCount();
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

        protected void getDestinationCandidates()
        {
            Log("Looking for destination candidates");
            destinationCandidates.Clear();

            //Loaded vessels
            int vesselCount = FlightGlobals.VesselsLoaded.Count;
            int partCount;
            Vessel vessel;
            Part part;
            PartModule partModule;
            int totalModules;
            for (int index = 0; index < vesselCount; index++)
            {
                vessel = FlightGlobals.VesselsLoaded[index];
                if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject || vessel.vesselType == VesselType.Unknown)
                    continue;
                partCount = vessel.parts.Count;
                for (int partIndex = 0; partIndex < partCount; partIndex++)
                {
                    part = vessel.parts[partIndex];
                    totalModules = part.Modules.Count;
                    for (int moduleIndex = 0; moduleIndex < totalModules; moduleIndex++)
                    {
                        partModule = part.Modules[moduleIndex];
                        if (requiredModules.Contains(partModule.moduleName))
                            destinationCandidates.Add(vessel);
                    }
                }
            }

            //Unloaded vessels
            vesselCount = FlightGlobals.VesselsUnloaded.Count;
            ProtoPartSnapshot partSnapshot;
            ProtoPartModuleSnapshot moduleSnapshot;
            for (int index = 0; index < vesselCount; index++)
            {
                vessel = FlightGlobals.VesselsUnloaded[index];
                if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject || vessel.vesselType == VesselType.Unknown)
                    continue;
                partCount = vessel.protoVessel.protoPartSnapshots.Count;
                for (int partIndex = 0; partIndex < partCount; partIndex++)
                {
                    partSnapshot = vessel.protoVessel.protoPartSnapshots[partIndex];
                    totalModules = partSnapshot.modules.Count;
                    for (int moduleIndex = 0; moduleIndex < totalModules; moduleIndex++)
                    {
                        moduleSnapshot = partSnapshot.modules[moduleIndex];
                        if (requiredModules.Contains(moduleSnapshot.moduleName))
                            destinationCandidates.Add(vessel);
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

        protected ProtoCrewMember createTourist()
        {
            KerbalRoster roster = HighLogic.CurrentGame.CrewRoster;
            string message = string.Empty;
            ProtoCrewMember newRecruit = roster.GetNewKerbal(ProtoCrewMember.KerbalType.Tourist);
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
