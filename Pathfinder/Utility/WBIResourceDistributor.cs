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
    public enum EDistributionModes
    {
        DistributionModeOff,
        DistributionModeDistributor,
        DistributionModeConsumer
    }

    [KSPModule("Resource Distributor")]
    public class WBIResourceDistributor : PartModule, IOpsView
    {
        [KSPField()]
        public string resourceBlacklist = string.Empty;

        [KSPField(isPersistant = true)]
        public int distributionMode;

        public EDistributionModes distribution;

        [KSPEvent(guiActive = true, guiActiveEditor = true, guiName = "Mode")]
        public void ToggleDistributionMode()
        {
            switch (distribution)
            {
                case EDistributionModes.DistributionModeOff:
                    distribution = EDistributionModes.DistributionModeDistributor;
                    break;

                case EDistributionModes.DistributionModeDistributor:
                    distribution = EDistributionModes.DistributionModeConsumer;
                    break;

                case EDistributionModes.DistributionModeConsumer:
                    distribution = EDistributionModes.DistributionModeOff;
                    break;
            }
            distributionMode = (int)distribution;
            setDistributionModeGUI();
        }

        public void SetDistributionMode(EDistributionModes mode)
        {
            distribution = mode;
            distributionMode = (int)distribution;
            setDistributionModeGUI();
        }

        protected void setDistributionModeGUI()
        {
            switch (distribution)
            {
                case EDistributionModes.DistributionModeDistributor:
                    Events["ToggleDistributionMode"].guiName = "Dist. Mode: Distributor";
                    break;

                case EDistributionModes.DistributionModeConsumer:
                    Events["ToggleDistributionMode"].guiName = "Dist. Mode: Consumer";
                    break;

                case EDistributionModes.DistributionModeOff:
                default:
                    Events["ToggleDistributionMode"].guiName = "Distribution Off";
                    break;
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            distribution = (EDistributionModes)distributionMode;
            setDistributionModeGUI();
        }
        
        public override string GetInfo()
        {
            return "Distributes resources to nearby vessels. Lock individual resources to prevent distribution, or disable the distributor to exclude the part.";
        }

        public virtual void GetResourcesToDistribute(List<PartResource> sharedResources, List<PartResource> requiredResources)
        {
            List<string> requiredResourceNames = new List<string>();
            List<BaseConverter> converters = null;
            int index, totalCount, totalRequired, reqIndex;
            BaseConverter converter;
            PartResource resource;
            ResourceRatio ratio;

            //Log info
            Debug.Log(this.part.partInfo.title + " is gathering resources to distribute.");
            Debug.Log("Current Mode: " + distribution);

            //Clear the lists passed in.
            sharedResources.Clear();
            requiredResources.Clear();

            //If the part does not particupate in resource distribution then we're done.
            if (distribution == EDistributionModes.DistributionModeOff)
            {
                Debug.Log("This part is not participating in resource distribution");
                return;
            }

            //Find all the required resources (if any)
            converters = this.part.FindModulesImplementing<BaseConverter>();
            totalCount = converters.Count;
            for (index = 0; index < totalCount; index++)
            {
                converter = converters[index];
                totalRequired = converter.reqList.Count;
                for (reqIndex = 0; reqIndex < totalRequired; reqIndex++)
                {
                    //Get the resource
                    ratio = converter.reqList[reqIndex];
                    
                    //Add the required resource to the list.
                    requiredResourceNames.Add(ratio.ResourceName);
                    Debug.Log("Added " + ratio.ResourceName + " to required resouce names list");
                }
            }

            //Now go through our resource list and divide them up between the two lists
            totalCount = this.part.Resources.Count;
            for (index = 0; index < totalCount; index++)
            {
                resource = this.part.Resources[index];

                //See if the resource is on the ignore list
                if (resourceBlacklist.Contains(resource.resourceName))
                {
                    Debug.Log("Skipping " + resource.resourceName + ", it is blacklisted.");
                    continue;
                }

                //Add to the appropriate list
                if (requiredResourceNames.Contains(resource.resourceName) == false)
                {
                    //If the resource is locked then move to the next resource.
                    if (resource.flowState == false)
                    {
                        Debug.Log("Skipping " + resource.resourceName + ", it is locked.");
                        continue;
                    }

                    //Share the resource if we're a distributor
                    if (distribution == EDistributionModes.DistributionModeDistributor)
                    {
                        Debug.Log("Added " + resource.resourceName + " to shared resources");
                        sharedResources.Add(resource);
                    }

                    //Part is a consumer. Only add resource to required resource list if it's not full.
                    else if (resource.amount < resource.maxAmount)
                    {
                        Debug.Log("Added " + resource.resourceName + " to required resources");
                        requiredResources.Add(resource);
                    }
                }

                //Only add resource to required resource list if it's not full.
                else if (resource.amount < resource.maxAmount)
                {
                    Debug.Log("Added " + resource.resourceName + " to required resources");
                    requiredResources.Add(resource);
                }
            }
        }

        public virtual double FillRequiredResource(string resourceName, double grandTotal)
        {
            double amountRemaining = 0f;
            PartResource resource;

            //Make sure we have the resource in question
            if (string.IsNullOrEmpty(resourceName))
                return grandTotal;
            if (this.part.Resources.Contains(resourceName) == false)
                return grandTotal;

            //Get resource
            resource = this.part.Resources[resourceName];
            if (resource.amount >= resource.maxAmount)
                return grandTotal;

            //Now fill up the resource. If the grand total plus current amount is <= max amount
            //then there'll be nothing left.
            if (resource.amount + grandTotal <= resource.maxAmount)
            {
                resource.amount += grandTotal;
                return 0f;
            }

            //We'll have leftovers
            amountRemaining = grandTotal - (resource.maxAmount - resource.amount);

            //Set amount to max.
            resource.amount = resource.maxAmount;

            return amountRemaining;
        }

        public virtual void TakeShare(string resourceName, double sharePercent)
        {
            //Make sure we have the resource in question
            if (string.IsNullOrEmpty(resourceName))
                return;
            if (this.part.Resources.Contains(resourceName) == false)
                return;

            //Set our share amount
            this.part.Resources[resourceName].amount = this.part.Resources[resourceName].maxAmount * sharePercent;
        }

        public void SetGUIVisible(bool isVisible)
        {
            Events["ToggleDistributionMode"].guiActive = false;
            Events["ToggleDistributionMode"].guiActiveEditor = false;
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public virtual void SetContextGUIVisible(bool isVisible)
        {
            SetGUIVisible(isVisible);
        }

        public virtual void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });
            if (GUILayout.Button(Events["ToggleDistributionMode"].guiName))
                ToggleDistributionMode();
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        public List<string> GetButtonLabels()
        {
            List<string> buttonLabels = new List<string>();
            buttonLabels.Add("Distributor");
            return buttonLabels;
        }

        public void SetParentView(IParentView parentView)
        {
        }
        #endregion
    }
}
