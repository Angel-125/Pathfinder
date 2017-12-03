using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Contracts;
using KSP;
using KSPAchievements;

//Courtesy of MrHappyFace
namespace ContractsPlus.Contracts
{
    public class WBIFerryKerbalParam : ContractParameter
    {
        const string ParameterTitle = "Ferry {0} to {1}";

        public string vesselID = string.Empty;
        public string vesselName = string.Empty;
        public string kerbalName = string.Empty;

        public WBIFerryKerbalParam()
        {
        }

        public WBIFerryKerbalParam(string vesselID, string vesselName, string kerbalName)
        {
            this.vesselID = vesselID;
            this.vesselName = vesselName;
            this.kerbalName = kerbalName;
        }

        protected void Log(string message)
        {
            if (WildBlueIndustries.PathfinderSettings.LoggingEnabled)
            {
                Debug.Log("[WBIFerryKerbalParam] - " + message);
            }
        }

        protected override string GetHashString()
        {
            return Guid.NewGuid().ToString();
        }

        protected override string GetTitle()
        {
            return string.Format(ParameterTitle, kerbalName, vesselName);
        }

        protected override void OnRegister()
        {
            GameEvents.onVesselCrewWasModified.Add(onVesselCrewWasModified);
        }

        protected override void OnUnregister()
        {
            GameEvents.onVesselCrewWasModified.Remove(onVesselCrewWasModified);
        }

        protected override void OnSave(ConfigNode node)
        {
            node.AddValue("vesselID", vesselID);
            node.AddValue("vesselName", vesselName);
            node.AddValue("kerbalName", kerbalName);
        }

        protected override void OnLoad(ConfigNode node)
        {
            vesselID = node.GetValue("vesselID");
            vesselName = node.GetValue("vesselName");
            kerbalName = node.GetValue("kerbalName");
        }

        /*
        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (HighLogic.LoadedSceneIsFlight == false)
                return;
        }
         */

        private void onVesselCrewWasModified(Vessel vessel)
        {
            if (vessel.id.ToString() == vesselID)
            {
                List<ProtoCrewMember> vesselCrew = vessel.GetVesselCrew();
                foreach (ProtoCrewMember crewMember in vesselCrew)
                {
                    Log("Checking " + crewMember.name);
                    if (crewMember.name == kerbalName)
                    {
                        Log(crewMember.name + " delivered to " + vesselName);
                        base.SetComplete();
                        return;
                    }
                }
            }
            else
            {
                base.SetIncomplete();
            }
        }
    }
}
