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
    public class TalliedResource
    {
        public List<WBIResourceDistributor> distributors = new List<WBIResourceDistributor>();
        public double grandTotal;
        public double grandCapacity;
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    class WBIDistributionManager : MonoBehaviour
    {
        public static WBIDistributionManager Instance;
        public static double secondsPerCycle = 10.0f;

        protected Dictionary<string, TalliedResource> requiredResourceTally = new Dictionary<string, TalliedResource>();
        protected Dictionary<string, TalliedResource> sharedResourceTally = new Dictionary<string, TalliedResource>();
        protected double elapsedTime;
        protected double cycleStartTime;
        protected bool distributionInProgress;

        public void Start()
        {
            Instance = this;
            cycleStartTime = Planetarium.GetUniversalTime();
            elapsedTime = 0f;
        }

        public void FixedUpdate()
        {
            //If we've waited long enough to distribute resources, then distribute them.
            elapsedTime = Planetarium.GetUniversalTime() - cycleStartTime;

            if (elapsedTime / secondsPerCycle > 1.0f)
            {
                cycleStartTime = Planetarium.GetUniversalTime();
                elapsedTime = 0f;
                DistributeResources();
            }
        }

        public void OptInActiveVessel()
        {
            List<WBIResourceDistributor> distributors = null;
            WBIResourceDistributor distributor;
            int totalCount;

            distributors = FlightGlobals.ActiveVessel.FindPartModulesImplementing<WBIResourceDistributor>();
            totalCount = distributors.Count;

            for (int index = 0; index < totalCount; index++)
            {
                distributor = distributors[index];
                distributor.SetDistributionMode(EDistributionModes.DistributionModeShare);
            }
        }

        public void OptInConsumerActiveVessel()
        {
            List<WBIResourceDistributor> distributors = null;
            WBIResourceDistributor distributor;
            int totalCount;

            distributors = FlightGlobals.ActiveVessel.FindPartModulesImplementing<WBIResourceDistributor>();
            totalCount = distributors.Count;

            for (int index = 0; index < totalCount; index++)
            {
                distributor = distributors[index];
                distributor.SetDistributionMode(EDistributionModes.DistributionModeConsume);
            }
        }

        public void OptOutActiveVessel()
        {
            List<WBIResourceDistributor> distributors = null;
            WBIResourceDistributor distributor;
            int totalCount;

            distributors = FlightGlobals.ActiveVessel.FindPartModulesImplementing<WBIResourceDistributor>();
            totalCount = distributors.Count;

            for (int index = 0; index < totalCount; index++)
            {
                distributor = distributors[index];
                distributor.SetDistributionMode(EDistributionModes.DistributionModeOff);
            }
        }

        public double GetDistributedAmount(string resourceName)
        {
            double distributedAmount = 0f;

            getDistributors();
            if (sharedResourceTally.ContainsKey(resourceName) == false)
            {
                Debug.Log(resourceName + " does not appear to be a shared resource");
                sharedResourceTally.Clear();
                requiredResourceTally.Clear();
                return 0f;
            }

            //Get the amount that we have.
            distributedAmount = sharedResourceTally[resourceName].grandTotal;

            //Cleanup
            sharedResourceTally.Clear();
            requiredResourceTally.Clear();
            return distributedAmount;
        }

        public double RequestDistributedResource(string resourceName, double amountRequest, bool acceptPartial = true)
        {
            double amountRemaining = 0f;
            double grandCapacity;
            double amountObtained = 0f;
            double sharePercent;
            TalliedResource talliedResource;
            WBIResourceDistributor distributor;
            int totalDistributors, distributorIndex;

            distributionInProgress = true;
            getDistributors();
            if (sharedResourceTally.ContainsKey(resourceName) == false)
            {
                distributionInProgress = false;
                sharedResourceTally.Clear();
                requiredResourceTally.Clear();
                return 0f;
            }

            //Get the amount remaining and the grand capacity.
            amountRemaining = sharedResourceTally[resourceName].grandTotal;
            grandCapacity = sharedResourceTally[resourceName].grandCapacity;
            if (amountRemaining < 0.001f && grandCapacity < 0.001f)
            {
                distributionInProgress = false;
                sharedResourceTally.Clear();
                requiredResourceTally.Clear();
                return 0f;
            }

            //If we have more than we need then just provide the amount requested and update.
            if (amountRemaining > amountRequest)
            {
                amountObtained = amountRequest;
                amountRemaining = amountRemaining - amountRequest;
            }

            //If the caller accepts partial requests then give what we have.
            else if (acceptPartial)
            {
                amountObtained = amountRemaining;
                amountRemaining = 0;
            }

            //Caller doesn't accept partial requests, then we're done.
            else
            {
                distributionInProgress = false;
                sharedResourceTally.Clear();
                requiredResourceTally.Clear();
                return 0;
            }

            //If we have nothing left over then we're not done, we need to clean out the distributors.
            if (amountRemaining < 0.001f)
                amountRemaining = 0f;

            //Now distribute the leftovers to the shared distributors
            sharePercent = amountRemaining / grandCapacity;
            talliedResource = sharedResourceTally[resourceName];
            totalDistributors = talliedResource.distributors.Count;
            for (distributorIndex = 0; distributorIndex < totalDistributors; distributorIndex++)
            {
                distributor = talliedResource.distributors[distributorIndex];
                distributor.TakeShare(resourceName, sharePercent);
            }

            //Cleanup
            sharedResourceTally.Clear();
            requiredResourceTally.Clear();
            distributionInProgress = false;
            return amountObtained;
        }


        public void DistributeResources()
        {
            if (distributionInProgress)
                return;
            distributionInProgress = true;

            List<WBIResourceDistributor> distributors = getDistributors();
            TalliedResource talliedResource;
            double amountRemaining;
            double grandCapacity;
            double sharePercent;
            int totalDistributors, distributorIndex;
            WBIResourceDistributor distributor;
            string[] tallyKeys;
            int totalTallyKeys, keyIndex;
            string resourceName;

            //Now go through each resource in the dictionary of shared resources.
            //Take the grand total and distribute it amongst the dictionary of required resources (if an entry exists) first.
            //Take whatever is left and divide it amongst the shared distributors.
            tallyKeys = sharedResourceTally.Keys.ToArray<string>();
            totalTallyKeys = tallyKeys.Length;
            for (keyIndex = 0; keyIndex < totalTallyKeys; keyIndex++)
            {
                resourceName = tallyKeys[keyIndex];
                amountRemaining = sharedResourceTally[resourceName].grandTotal;
                grandCapacity = sharedResourceTally[resourceName].grandCapacity;

                if (amountRemaining < 0.001f && grandCapacity < 0.001f)
                    continue;

                //If the required resources dictionary has the resource, then distribute the available amount to the
                //required resource distributors first.
                if (requiredResourceTally.ContainsKey(resourceName))
                {
                    talliedResource = requiredResourceTally[resourceName];
                    totalDistributors = talliedResource.distributors.Count;
                    for (distributorIndex = 0; distributorIndex < totalDistributors; distributorIndex++)
                    {
                        distributor = talliedResource.distributors[distributorIndex];
                        amountRemaining = distributor.FillRequiredResource(resourceName, amountRemaining);
                        if (amountRemaining < 0.001f)
                            break;
                    }
                }

                //If we have nothing left over then we're not done, we need to clean out the distributors.
                if (amountRemaining < 0.001f)
                    amountRemaining = 0f;

                //Now distribute the leftovers to the shared distributors
                sharePercent = amountRemaining / grandCapacity;
                talliedResource = sharedResourceTally[resourceName];
                totalDistributors = talliedResource.distributors.Count;
                for (distributorIndex = 0; distributorIndex < totalDistributors; distributorIndex++)
                {
                    distributor = talliedResource.distributors[distributorIndex];
                    distributor.TakeShare(resourceName, sharePercent);
                }
            }

            //Cleanup
            sharedResourceTally.Clear();
            requiredResourceTally.Clear();
            distributionInProgress = false;
        }

        protected List<WBIResourceDistributor> getDistributors()
        {
            List<WBIResourceDistributor> distributors = null;
            WBIResourceDistributor distributor;
            Vessel vessel;
            int totalVessels, totalDistributors, vesselIndex, distributorIndex;

            //Get the list of all the vessels within physics range (they're loaded)
            totalVessels = FlightGlobals.Vessels.Count;
            for (vesselIndex = 0; vesselIndex < totalVessels; vesselIndex++)
            {
                vessel = FlightGlobals.Vessels[vesselIndex];
                if (vessel.loaded)
                {
                    if (vessel.situation != Vessel.Situations.PRELAUNCH && vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.SPLASHED)
                    {
                        //Debug.Log("Skipping vessel due to situation: " + vessel.situation);
                        continue;
                    }

                    distributors = vessel.FindPartModulesImplementing<WBIResourceDistributor>();
                    totalDistributors = distributors.Count;

                    for (distributorIndex = 0; distributorIndex < totalDistributors; distributorIndex++)
                    {
                        distributor = distributors[distributorIndex];
                        //If the distributor is actively participating then tally its resources.
                        if (distributor.isParticipating)
                            tallyResources(distributor);
                    }
                }
            } 
            
            return distributors;
        }

        protected void tallyResources(WBIResourceDistributor distributor)
        {
            List<PartResource> sharedResources = new List<PartResource>();
            List<PartResource> requiredResources = new List<PartResource>();
            TalliedResource talliedResource;
            PartResource resource;
            int totalCount;

            //Get the list of shared and required resources
            distributor.GetResourcesToDistribute(sharedResources, requiredResources);

            //For each required resource, add the distributor to the required resources map.
            totalCount = requiredResources.Count;
            for (int index = 0; index < totalCount; index++)
            {
                resource = requiredResources[index];

                //Add the resource to the dictionary if needed.
                if (requiredResourceTally.ContainsKey(resource.resourceName) == false)
                {
                    talliedResource = new TalliedResource();
                    requiredResourceTally.Add(resource.resourceName, talliedResource);
                }

                //Add the distributor to the tallied resource.
                talliedResource = requiredResourceTally[resource.resourceName];
                talliedResource.distributors.Add(distributor);
            }

            //Now add all non-required resources
            totalCount = sharedResources.Count;
            for (int index = 0; index < totalCount; index++)
            {
                resource = sharedResources[index];

                //Add the resource to the dictionary if needed.
                if (sharedResourceTally.ContainsKey(resource.resourceName) == false)
                {
                    talliedResource = new TalliedResource();
                    sharedResourceTally.Add(resource.resourceName, talliedResource);
                }

                //Add the distributor to the tallied resource
                talliedResource = sharedResourceTally[resource.resourceName];
                talliedResource.distributors.Add(distributor);

                //Tally up the current and max amounts
                talliedResource.grandTotal += resource.amount;
                talliedResource.grandCapacity += resource.maxAmount;
            }
        }
    }
}
