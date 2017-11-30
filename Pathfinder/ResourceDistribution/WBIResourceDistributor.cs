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
        DistributionModeShare,
        DistributionModeConsume,
        DistributionModeRequired
    }

    [KSPModule("Resource Distributor")]
    public class WBIResourceDistributor : PartModule, IOpsView
    {
        [KSPField]
        public bool isConsumer;

        [KSPField()]
        public string resourceBlacklist = string.Empty;

        [KSPField(isPersistant = true, guiName = "Shares resources")]
        [UI_Toggle(enabledText = "Yes", disabledText = "No")]
        public bool isParticipating;

        public EDistributionModes distribution;
        public Dictionary<string, EDistributionModes> distributionMap = new Dictionary<string, EDistributionModes>();
        string templateName;
        WBIResourceSwitcher switcher;
        DistributionView distributionView = new DistributionView();
        List<PartResource> sharedResourcesCache = new List<PartResource>();
        List<PartResource> requiredResourcesCache = new List<PartResource>();

        [KSPEvent(guiActiveEditor = true, guiActive = true, guiName = "Setup Distribution")]
        public void SetupDistribution()
        {
            //Setup view
            distributionView.part = this.part;
            distributionView.isParticipating = this.isParticipating;
            distributionView.distributionMap = this.distributionMap;
            distributionView.rebuildCache = RebuidDistribtuionCache;
            distributionView.setParticipation = setParticipation;
            distributionView.SetVisible(!distributionView.IsVisible());
        }

        public void SetDistributionMode(EDistributionModes mode)
        {
            //Setup our level of participation
            switch (mode)
            {
                case EDistributionModes.DistributionModeConsume:
                case EDistributionModes.DistributionModeShare:
                    isParticipating = true;
                    break;

                default:
                    isParticipating = false;
                    break;
            }

            distributionView.isParticipating = isParticipating;

            //Setting distribution mode in this manner affects all resources except those that are required.
            string[] keys = distributionMap.Keys.ToArray<string>();
            string key;
            for (int index = 0; index < keys.Length; index++ )
            {
                key = keys[index];

                if (distributionMap[key] != EDistributionModes.DistributionModeRequired)
                    distributionMap[key] = mode;
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);

            ConfigNode[] distributionNodes = node.GetNodes("DISTRIBUTION");
            ConfigNode distNode;
            for (int index = 0; index < distributionNodes.Length; index++)
            {
                distNode = distributionNodes[index];
                distributionMap.Add(distNode.GetValue("resourceName"), (EDistributionModes)int.Parse(distNode.GetValue("mode")));
                RebuidDistribtuionCache();
            }
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            ConfigNode distributionNode;
            EDistributionModes mode;
            foreach (string key in distributionMap.Keys)
            {
                mode = distributionMap[key];
                distributionNode = new ConfigNode("DISTRIBUTION");
                distributionNode.AddValue("resourceName", key);
                distributionNode.AddValue("mode", (int)mode);
                node.AddNode(distributionNode);
            }
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            //Tap into the resource switcher's redecoration event
            switcher = this.part.FindModuleImplementing<WBIResourceSwitcher>();
            if (switcher != null)
            {
                switcher.onModuleRedecorated += new ModuleRedecoratedEvent(switcher_onModuleRedecorated);
                templateName = switcher.CurrentTemplateName;
            }

            //if we have an empty list then create a new one.
            if (distributionMap.Keys.Count == 0)
                RebuildDistributionList();

            if (isConsumer)
            {
                Events["SetupDistribution"].guiActive = false;
                Fields["isParticipating"].guiName = "Acquire Resources";
            }
        }

        protected void setParticipation(bool isEnabled)
        {
            isParticipating = isEnabled;
        }

        void switcher_onModuleRedecorated(ConfigNode templateNode)
        {
            if (switcher.CurrentTemplateName != templateName)
            {
                RebuildDistributionList();
                templateName = switcher.CurrentTemplateName;
            }
        }
        
        public override string GetInfo()
        {
            return "Distributes resources to nearby vessels. Lock individual resources to prevent distribution, or disable the distributor to exclude the part.";
        }

        public virtual void RebuildDistributionList()
        {
            WBIDistributionManager.Log("[WBIResourceDistributor] - Rebuilding distribution list.");
            List<BaseConverter> converters = null;
            int index, totalCount, totalRequired, reqIndex;
            BaseConverter converter;
            PartResource resource;
            ResourceRatio ratio;

            //Clear the map
            distributionMap.Clear();

            //Build the base list
            totalCount = this.part.Resources.Count;
            for (index = 0; index < totalCount; index++)
            {
                resource = this.part.Resources[index];
                if (resourceBlacklist.Contains(resource.resourceName) == false && distributionMap.ContainsKey(resource.resourceName) == false)
                    distributionMap.Add(resource.resourceName, EDistributionModes.DistributionModeOff);
                if (isConsumer)
                    distributionMap[resource.resourceName] = EDistributionModes.DistributionModeConsume;
            }

            //Find all the required resources (if any)
            //Required resources are automatically set to consumer
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

                    //Setup the distribution map
                    if (distributionMap.ContainsKey(ratio.ResourceName))
                        distributionMap[ratio.ResourceName] = EDistributionModes.DistributionModeRequired;
                }
            }

            //Rebuild the cache
            RebuidDistribtuionCache();
        }

        public virtual void RebuidDistribtuionCache()
        {
            WBIDistributionManager.Log("[WBIResourceDistributor] - Rebuilding distribution cache.");
            List<string> requiredResourceNames = new List<string>();
            int index, totalCount;
            PartResource resource;
            EDistributionModes mode;

            //Clear the lists passed in.
            sharedResourcesCache.Clear();
            requiredResourcesCache.Clear();

            //Now go through our resource list and divide them up between the two lists
            totalCount = this.part.Resources.Count;
            for (index = 0; index < totalCount; index++)
            {
                resource = this.part.Resources[index];

                //See if the resource is on the ignore list
                if (resourceBlacklist.Contains(resource.resourceName))
                {
                    WBIDistributionManager.Log("[WBIResourceDistributor] - Skipping " + resource.resourceName + ", it is blacklisted.");
                    continue;
                }

                //Add to the appropriate list
                if (distributionMap.ContainsKey(resource.resourceName) == false)
                {
                    WBIDistributionManager.Log("[WBIResourceDistributor] - Skipping " + resource.resourceName + ", it is not in the resource map.");
                    continue;
                }
                mode = distributionMap[resource.resourceName];
                switch (mode)
                {
                    case EDistributionModes.DistributionModeShare:
                        //Share the resource
                        WBIDistributionManager.Log("[WBIResourceDistributor] - Added " + resource.resourceName + " to shared resources");
                        sharedResourcesCache.Add(resource);
                        break;

                    case EDistributionModes.DistributionModeConsume:
                    case EDistributionModes.DistributionModeRequired:
                        //Consume if our resource isn't full
                        if (resource.amount < resource.maxAmount)
                        {
                            WBIDistributionManager.Log("[WBIResourceDistributor] - Added " + resource.resourceName + " to required resources");
                            requiredResourcesCache.Add(resource);
                        }
                        break;

                    default:
                        WBIDistributionManager.Log("[WBIResourceDistributor] - Skipping " + resource.resourceName + ", it is ignored.");
                        break;
                }
            }
        }

        public virtual void GetResourcesToDistribute(out List<PartResource> sharedList, out List<PartResource> requiredList)
        {
            //If the part does not particupate in resource distribution then we're done.
            if (!isParticipating)
            {
                WBIDistributionManager.Log("[WBIResourceDistributor] - This part is not participating in resource distribution");
                sharedList = null;
                requiredList = null;
                return;
            }

            //Log info
            WBIDistributionManager.Log("[WBIResourceDistributor] - " + this.part.partInfo.title + " is gathering resources to distribute.");

            sharedList = this.sharedResourcesCache;
            requiredList = this.requiredResourcesCache;
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
            Events["SetupDistribution"].guiActive = false;
            Events["SetupDistribution"].guiActiveEditor = false;
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
            if (!isConsumer)
            {
                distributionView.DrawView();
                isParticipating = distributionView.isParticipating;
            }

            else
            {
                GUILayout.BeginVertical();
                isParticipating = GUILayout.Toggle(isParticipating, "Acquire resources");
                GUILayout.EndVertical();
            }
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
