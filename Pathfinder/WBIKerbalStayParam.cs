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
    public class WBIKerbalStayParam : ContractParameter
    {
        const string ParameterTitle = "Stay at {0}, for {1:f1} days.";

        public string vesselID = string.Empty;
        public string vesselName = string.Empty;
        public string kerbalName = string.Empty;
        public double totalStayTime;
        public double lastUpdate;
        public bool isAtLocation;

        public WBIKerbalStayParam()
        {
        }

        public WBIKerbalStayParam(string vesselID, string vesselName, string kerbalName, int totalDays)
        {
            double secondsPerDay = GameSettings.KERBIN_TIME ? 21600 : 86400;
            this.vesselID = vesselID;
            this.vesselName = vesselName;
            this.kerbalName = kerbalName;
            this.totalStayTime = (double)totalDays * secondsPerDay;
            lastUpdate = Planetarium.GetUniversalTime();
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
            double secondsPerDay = GameSettings.KERBIN_TIME ? 21600 : 86400;
            double totalDays = totalStayTime / secondsPerDay;
            return string.Format(ParameterTitle, vesselName, totalDays);
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
            node.AddValue("totalStayTime", totalStayTime);
            node.AddValue("lastUpdate", lastUpdate);
            node.AddValue("isAtLocation", isAtLocation);
        }

        protected override void OnLoad(ConfigNode node)
        {
            vesselID = node.GetValue("vesselID");
            vesselName = node.GetValue("vesselName");
            kerbalName = node.GetValue("kerbalName");
            totalStayTime = double.Parse(node.GetValue("totalStayTime"));
            lastUpdate = double.Parse(node.GetValue("lastUpdate"));
            isAtLocation = bool.Parse(node.GetValue("isAtLocation"));
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //If the kerbal is't at the desired vessel then we're done.
            if (!isAtLocation)
                return;

            //Calculate elapsed time
            double elapsedTime = Planetarium.GetUniversalTime() - lastUpdate;
            lastUpdate = Planetarium.GetUniversalTime();

            //Update the stay time
            totalStayTime -= elapsedTime;
            if (totalStayTime <= 0.0001)
                base.SetComplete();
            else
                base.SetIncomplete();
        }

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
                        isAtLocation = true;
                        return;
                    }
                }
            }
        }
    }
}
