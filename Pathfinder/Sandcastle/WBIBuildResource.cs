using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

/*
Source code copyrighgt 2018, by Michael Billard (Angel-125)
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
    public class WBIBuildResource
    {
        public static string kBuildNode = "BUILD_RESOURCE";
        public static string kResourceName = "ResourceName";
        public static string kCurrentAmount = "CurrentAmount";
        public static string kRequiredAmount = "RequiredAmount";

        public static string kTimeResourceName = "TimeSecs";
        public static string kTimeResourceDisplayName = "Construction Time";
        public static string kResourceStatus = ": {0:n1} req.";

        public string ResourceName;
        public double CurrentAmount;
        public double RequiredAmount;

        public Part part;

        public WBIBuildResource()
        {

        }

        public WBIBuildResource(ConfigNode node)
        {
            Load(node);
        }

        public void Load(ConfigNode node)
        {
            if (node.HasValue(kResourceName))
                this.ResourceName = node.GetValue(kResourceName);

            if (node.HasValue(kCurrentAmount))
                double.TryParse(node.GetValue(kCurrentAmount), out this.CurrentAmount);

            if (node.HasValue(kRequiredAmount))
                double.TryParse(node.GetValue(kRequiredAmount), out this.RequiredAmount);
        }

        public ConfigNode Save()
        {
            ConfigNode node = new ConfigNode(kBuildNode);

            node.AddValue(kResourceName, this.ResourceName);
            node.AddValue(kCurrentAmount, CurrentAmount);
            node.AddValue(kRequiredAmount, RequiredAmount);

            return node;
        }

        public bool hasAcquiredResource(double buildRate, double crewBonus)
        {
            //Check to see if we've acquired the necessary amount of the resource
            if (this.CurrentAmount >= (this.RequiredAmount * (1.0 - crewBonus)))
            {
                this.CurrentAmount = this.RequiredAmount;
                return true;
            }

            //If the resource is the time resource then accumulate time
            if (ResourceName == kTimeResourceName)
            {
                CurrentAmount += buildRate;
            }

            //Try to obtain the resource
            else
            {
                this.CurrentAmount += this.part.RequestResource(this.ResourceName, buildRate, ResourceFlowMode.ALL_VESSEL);
            }

            //Not done yet..
            return false;
        }

        public string statusString(double crewBonus)
        {
            StringBuilder status = new StringBuilder();
            PartResourceDefinition resourceDef = null;
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            double adjustedRequiredAmount = (this.RequiredAmount * (1.0 - crewBonus));

            //Get the name of the resource.
            if (this.ResourceName == kTimeResourceName)
            {
                status.Append(kTimeResourceDisplayName);
                status.Append(": ");
                status.Append(KSPUtil.dateTimeFormatter.PrintTimeCompact(adjustedRequiredAmount - this.CurrentAmount, true).Replace("T+", "T-"));
            }

            else
            {
                resourceDef = definitions[this.ResourceName];
                status.Append(resourceDef.displayName);
                status.Append(string.Format(kResourceStatus, adjustedRequiredAmount - this.CurrentAmount));
            }

            return status.ToString();
        }

        public static string GetRequirements(ConfigNode node)
        {
            StringBuilder requirements = new StringBuilder();
            PartResourceDefinition resourceDef = null;
            PartResourceDefinitionList definitions = PartResourceLibrary.Instance.resourceDefinitions;
            string resourceName = string.Empty;
            string displayName;
            double amount;

            if (node.HasValue(kResourceName))
            {
                resourceName = node.GetValue(kResourceName);

                if (resourceName != kTimeResourceName)
                {
                    resourceDef = definitions[resourceName];
                    displayName = resourceDef.displayName;
                }

                else
                {
                    displayName = kTimeResourceDisplayName;
                }

                requirements.Append(displayName);
                requirements.Append(": ");
            }

            if (node.HasValue(kRequiredAmount))
            {
                double.TryParse(node.GetValue(kRequiredAmount), out amount);

                if (resourceName != kTimeResourceName)
                    requirements.Append(string.Format("{0:n1}", amount));
                else
                    requirements.Append(KSPUtil.dateTimeFormatter.PrintTimeLong(amount));
            }

            return requirements.ToString();
        }
    }
}
