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

            distributors = FlightGlobals.ActiveVessel.FindPartModulesImplementing<WBIResourceDistributor>();

            foreach (WBIResourceDistributor distributor in distributors)
                distributor.distributeResources = true;
        }

        public void OptOutActiveVessel()
        {
            List<WBIResourceDistributor> distributors = null;

            distributors = FlightGlobals.ActiveVessel.FindPartModulesImplementing<WBIResourceDistributor>();

            foreach (WBIResourceDistributor distributor in distributors)
                distributor.distributeResources = false;
        }

        public void DistributeResources()
        {
            if (distributionInProgress)
                return;
            distributionInProgress = true;

            List<WBIResourceDistributor> distributors = null;
            TalliedResource talliedResource;
            double amountRemaining;
            double grandCapacity;
            double sharePercent;

            //Get the list of all the vessels within physics range (they're loaded)
            foreach (Vessel vessel in FlightGlobals.Vessels)
            {
                if (vessel.loaded)
                {
                    distributors = vessel.FindPartModulesImplementing<WBIResourceDistributor>();

                    foreach (WBIResourceDistributor distributor in distributors)
                    {
                        //If the distributor is actively participating then tally its resources.
                        if (distributor.distributeResources)
                            tallyResources(distributor);
                    }
                }
            }

            //Now go through each resource in the dictionary of shared resources.
            //Take the grand total and distribute it amongst the dictionary of required resources (if an entry exists) first.
            //Take whatever is left and divide it amongst the shared distributors.
            foreach (string resourceName in sharedResourceTally.Keys)
            {
                amountRemaining = sharedResourceTally[resourceName].grandTotal;
                grandCapacity = sharedResourceTally[resourceName].grandCapacity;

                if (amountRemaining < 0.001f && grandCapacity < 0.001f)
                    continue;

                //If the required resources dictionary has the resource, then distribute the available amount to the
                //required resource distributors first.
                if (requiredResourceTally.ContainsKey(resourceName))
                {
                    talliedResource = requiredResourceTally[resourceName];
                    foreach (WBIResourceDistributor distributor in talliedResource.distributors)
                    {
                        amountRemaining = distributor.FillRequiredResource(resourceName, amountRemaining);
                        if (amountRemaining < 0.001f)
                            break;
                    }
                }

                //If we have nothing left over then we're done.
                if (amountRemaining < 0.001f)
                    break;

                //Now distribute the leftovers to the shared distributors
                sharePercent = amountRemaining / grandCapacity;
                talliedResource = sharedResourceTally[resourceName];
                foreach (WBIResourceDistributor distibutor in talliedResource.distributors)
                    distibutor.TakeShare(resourceName, sharePercent);
            }

            //Cleanup
            sharedResourceTally.Clear();
            requiredResourceTally.Clear();
            distributionInProgress = false;
        }

        protected void tallyResources(WBIResourceDistributor distributor)
        {
            List<PartResource> sharedResources = new List<PartResource>();
            List<PartResource> requiredResources = new List<PartResource>();
            TalliedResource talliedResource;

            //Get the list of shared and required resources
            distributor.GetResourcesToDistribute(sharedResources, requiredResources);

            //For each required resource, add the distributor to the required resources map.
            foreach (PartResource resource in requiredResources)
            {
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
            foreach (PartResource resource in sharedResources)
            {
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
