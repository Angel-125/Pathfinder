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
    [KSPModule("Resource Distributor")]
    public class WBIResourceDistributor : PartModule, IOpsView
    {
        [KSPField(guiName = "Distributor", isPersistant = true, guiActiveEditor = true, guiActive = true)]
        [UI_Toggle(enabledText = "On", disabledText = "Off")]
        public bool distributeResources;

        public override string GetInfo()
        {
            return "Distributes resources to nearby vessels. Lock individual resources to prevent distribution, or disable the distributor to exclude the part.";
        }

        public void GetResourcesToDistribute(List<PartResource> sharedResources, List<PartResource> requiredResources)
        {
            List<string> requiredResourceNames = new List<string>();
            List<BaseConverter> converters = null;

            //Clear the lists passed in.
            sharedResources.Clear();
            requiredResources.Clear();

            //If the part does not particupate in resource distribution then we're done.
            if (distributeResources == false)
                return;

            //Find all the required resources (if any)
            converters = this.part.FindModulesImplementing<BaseConverter>();
            foreach (BaseConverter converter in converters)
            {
                foreach (ResourceRatio ratio in converter.reqList)
                    requiredResourceNames.Add(ratio.ResourceName);
            }

            //Now go through our resource list and divide them up between the two lists
            foreach (PartResource resource in this.part.Resources)
            {
                //If the resource is locked then move to the next resource.
                if (resource.isTweakable == false)
                    continue;

                //Add to the appropriate list
                if (requiredResourceNames.Contains(resource.name) == false)
                    sharedResources.Add(resource);

                //Only add resource to required resource list if it's not full.
                else if (resource.amount < resource.maxAmount)
                    requiredResources.Add(resource);
            }
        }

        public double FillRequiredResource(string resourceName, double grandTotal)
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

        public void TakeShare(string resourceName, double sharePercent)
        {
            //Make sure we have the resource in question
            if (string.IsNullOrEmpty(resourceName))
                return;
            if (this.part.Resources.Contains(resourceName) == false)
                return;
            if (sharePercent == 0f)
                return;

            //Set our share amount
            this.part.Resources[resourceName].amount = this.part.Resources[resourceName].maxAmount * sharePercent;
        }

        #region IOpsView
        public string GetPartTitle()
        {
            return this.part.partInfo.title;
        }

        public void SetContextGUIVisible(bool isVisible)
        {
            Fields["distributeResources"].guiActive = false;
            Fields["distributeResources"].guiActiveEditor = false;
        }

        public void DrawOpsWindow(string buttonLabel)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(), new GUIStyle(GUI.skin.textArea), new GUILayoutOption[] { GUILayout.Height(480) });
            distributeResources = GUILayout.Toggle(distributeResources, "Enable resource distribution");
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
