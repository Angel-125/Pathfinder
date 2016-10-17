using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

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
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WBIDrillManager : MonoBehaviour
    {
        public static WBIDrillManager Instance;

        static Vessel previousVessel;
        static Dictionary<ModuleResourceHarvester, float> originalHarvesterEfficiencies = new Dictionary<ModuleResourceHarvester, float>();

        public void Start()
        {
            Instance = this;

            previousVessel = null;

            GameEvents.onVesselGoOffRails.Add(VesselWentOffRails);
            GameEvents.onVesselLoaded.Add(VesselWasLoaded);
            GameEvents.onVesselChange.Add(VesselWasChanged);
//            GameEvents.onPartUnpack.Add(PartUnpacked);
        }

        /*
        public void PartUnpacked(Part part)
        {
            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            WBIExtractionMonitor extractionMonitor = null;

            if (part.FindModuleImplementing<ModuleResourceHarvester>() == null)
                return;

            //Add an extraction monitor if needed.
            extractionMonitor = part.FindModuleImplementing<WBIExtractionMonitor>();
            if (extractionMonitor == null)
            {
                extractionMonitor = (WBIExtractionMonitor)part.AddModule("WBIExtractionMonitor");
                extractionMonitor.OnActive();
                extractionMonitor.OnStart(PartModule.StartState.Landed);
                extractionMonitor = null;
            }
        }
         */

        public void VesselWasChanged(Vessel vessel)
        {
            previousVessel = null;
            originalHarvesterEfficiencies.Clear();
        }

        public void VesselWasLoaded(Vessel vessel)
        {
            previousVessel = null;
            originalHarvesterEfficiencies.Clear();
        }

        public void VesselWentOffRails(Vessel vessel)
        {
            //Prevent re-entrant calculations. KSP can call this routine several times.
            if (vessel == previousVessel)
                return;
            previousVessel = vessel;
            originalHarvesterEfficiencies.Clear();

            //Now update harvester efficiencies
            UpdateHarvesterEfficiencies(vessel);
        }

        public void UpdateHarvesterEfficiencies(Vessel vessel)
        {
            CBAttributeMapSO.MapAttribute biome = null;
            List<ModuleResourceHarvester> harvesters = null;
            Dictionary<string, EfficiencyData> efficiencyMap = null;
            WBIExtractionMonitor extractionMonitor = null;
            string key;
            int harvestID;

            //If the vessel has landed, find out what biome it is located in.
            if (vessel.situation == Vessel.Situations.LANDED || vessel.situation == Vessel.Situations.PRELAUNCH || vessel.situation == Vessel.Situations.SPLASHED)
                biome = Utils.GetCurrentBiome(vessel);

            //TODO: No biome? We need to figure out what to do about asteroids.
            //For now, we're done
            if (biome == null)
                return;

            //If our Scenario has efficiency data for the biome, then retrieve it.
            efficiencyMap = WBIPathfinderScenario.Instance.GetEfficiencyDataForBiome(vessel.mainBody.flightGlobalsIndex, biome.name);
            if (efficiencyMap == null)
                return;

            //Go through the vessel's parts and find all the parts that implement ModuleResourceHarvester.
            foreach (Part part in vessel.parts)
            {
                harvesters = part.FindModulesImplementing<ModuleResourceHarvester>();
                if (harvesters.Count == 0)
                    continue;

                //Add an extraction monitor if needed.
                extractionMonitor = part.FindModuleImplementing<WBIExtractionMonitor>();
                if (extractionMonitor == null)
                {
                    extractionMonitor = (WBIExtractionMonitor)part.AddModule("WBIExtractionMonitor");
                    extractionMonitor.OnActive();
                    extractionMonitor.OnStart(PartModule.StartState.Landed);
                    extractionMonitor = null;
                }

                //For each ModuleResourceHarvester, multiply the harvester's Efficiency by the biome's EfficiencyModifier.
                //Note that the efficiency modifier applies to all drills of the specified harvest type regardless of resource.
                //Mostly because I don't see a need to modify individual resources and just knowing that your drills are ###% better or worse
                //is all a player really cares about.
                foreach (ModuleResourceHarvester harvester in harvesters)
                {
                    harvestID = (int)harvester.HarvesterType;
                    key = biome.name + harvestID.ToString();

                    if (efficiencyMap.ContainsKey(key))
                    {
                        //It's possible that the player may repeatedly take core samples until getting a different result.
                        //We want the modified result to always be based on the original efficiency.
                        if (originalHarvesterEfficiencies.ContainsKey(harvester))
                        {
                            harvester.Efficiency = originalHarvesterEfficiencies[harvester] * efficiencyMap[key].modifiers[EfficiencyData.kExtractionMod];
                        }

                        //Multiply the harvester's efficiency by the modifier, and add the orignal value to the map.
                        else
                        {
                            originalHarvesterEfficiencies.Add(harvester, harvester.Efficiency);
                            harvester.Efficiency *= efficiencyMap[key].modifiers[EfficiencyData.kExtractionMod];
                        }

                        //Dirty the harvester
                        harvester.DirtyFlag = true;
                    }
                }
            }

        }
    }
}
