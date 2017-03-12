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
    public class WBIPartScrapper : PartModule
    {
        private const float kExplosionPotential = 0.01f;
        private const float kMessageDuration = 5.0f;
        private const string kVesselIsOccupied = "This vessel is occupied! Vacate the vessel before scrapping it.";
        private const string kPartIsOccupied = "The part is occupied. Vacate the part before scrapping it.";
        private const string kInsufficientSkill = "Insufficient skill to scrap the part. You need the following traits: {0:s}";
        private const string kInsufficientExperienceVessel = "Insufficient experience to scrap the vessel. You need to be level {0:f0} to recycle the vessel";
        private const string kInsufficientExperiencePart = "Insufficient experience to scrap the part. You need to be level {0:f0} to recycle the part";
        private const string kResourceRecycled = "Recycled {0:f2} units of {1:s}";
        private const string kPartRecycled = "{0:s} recycled.";
        private const string kPartHasChildren = "Scrap the attached parts first before scrapping this part.";

        [KSPField]
        public double recyclePercentPerSkill = 10f;

        [KSPField]
        public string scrapSkill = "RepairSkill";

        [KSPField]
        public int minimumPartRecycleSkill = 3;

        [KSPField]
        public int minimumVesselRecycleSkill = 5;

        [KSPField]
        public string recycleResource = "Equipment";

        [KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = true, unfocusedRange = 3.0f)]
        public void ScrapVessel()
        {
            //Make sure the vessel isn't occupied.
            if (this.part.vessel.GetCrewCount() > 0)
            {
                ScreenMessages.PostScreenMessage(kVesselIsOccupied, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                return;
            }

            if (FlightGlobals.ActiveVessel.isEVA)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                //Check for skill & experience.
                if (astronaut.HasEffect(scrapSkill) == false)
                {
                    ScreenMessages.PostScreenMessage(string.Format(kInsufficientSkill, Utils.GetTraitsWithEffect(scrapSkill)), kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
                if (Utils.IsExperienceEnabled())
                {
                    //Check for experience level
                    if (astronaut.experienceLevel < minimumVesselRecycleSkill)
                    {
                        ScreenMessages.PostScreenMessage(string.Format(kInsufficientExperienceVessel, minimumVesselRecycleSkill), kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                        return;
                    }
                }

                //Ok, we're good to go, get the resource definition for the recycleResource.
                PartResourceDefinition def = ResourceHelper.DefinitionForResource(recycleResource);
                if (def == null)
                {
                    Debug.Log("[WBIPartScrapper] - Definition not found for " + recycleResource);
                    return;
                }

                //Recycle all parts in the vessel.
                List<Part> doomedParts = new List<Part>();
                foreach (Part doomed in this.part.vessel.parts)
                {
                    if (doomed != this.part)
                        doomedParts.Add(doomed);
                }
                foreach (Part doomed in doomedParts)
                    recyclePart(doomed, def, astronaut);

                //Final recycle
                recyclePart(this.part, def, astronaut);
            }
        }

        [KSPEvent(guiActiveUnfocused = true, externalToEVAOnly = true, unfocusedRange = 3.0f)]
        public void ScrapPart()
        {
            //Make sure that there are no kerbals on the part.
            if (this.part.protoModuleCrew.Count > 0)
            {
                ScreenMessages.PostScreenMessage(kPartIsOccupied, kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
            }

            if (FlightGlobals.ActiveVessel.isEVA)
            {
                Vessel vessel = FlightGlobals.ActiveVessel;
                ProtoCrewMember astronaut = vessel.GetVesselCrew()[0];

                //Check for skill & experience.
                if (astronaut.HasEffect(scrapSkill) == false)
                {
                    ScreenMessages.PostScreenMessage(string.Format(kInsufficientSkill, Utils.GetTraitsWithEffect(scrapSkill)), kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                    return;
                }
                if (Utils.IsExperienceEnabled())
                {
                    //Check for experience level
                    if (astronaut.experienceLevel < minimumPartRecycleSkill)
                    {
                        ScreenMessages.PostScreenMessage(string.Format(kInsufficientExperiencePart, minimumPartRecycleSkill), kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
                        return;
                    }
                }


                //Ok, we're good to go, get the resource definition for the recycleResource.
                PartResourceDefinition def = ResourceHelper.DefinitionForResource(recycleResource);
                if (def == null)
                {
                    Debug.Log("[WBIPartScrapper] - Definition not found for " + recycleResource);
                    return;
                }

                //Recycle the part and its resources
                recyclePart(this.part, def, astronaut);
            }
        }

        protected void recyclePart(Part doomed, PartResourceDefinition def, ProtoCrewMember astronaut)
        {
            //Get the total units of the recycle resource
            double totalRecycleUnits = doomed.mass / def.density;

            //Adjust the units recycled based upon experience level.
            totalRecycleUnits *= (astronaut.experienceLevel * (recyclePercentPerSkill / 100.0f));

            //Shut off any resource distribution
            WBIResourceDistributor distributor = doomed.FindModuleImplementing<WBIResourceDistributor>();
            if (distributor != null)
            {
                distributor.isParticipating = false;
                distributor.distribution = EDistributionModes.DistributionModeOff;
            }

            //Distribute any resources that the part has.
            List<DistributedResource> scrappedResources = new List<DistributedResource>();
            PartResource[] doomedResources = doomed.Resources.ToArray();
            string recycleMessage;
            for (int index = 0; index < doomedResources.Length; index++)
            {
                if (doomedResources[index].amount > 0.0001f)
                {
                    recycleMessage = string.Format(kResourceRecycled, doomedResources[index].amount, doomedResources[index].resourceName);
                    scrappedResources.Add(new DistributedResource(doomedResources[index].resourceName, doomedResources[index].amount, recycleMessage));
                }
            }

            //Now distribute the recycled resources
            recycleMessage = string.Format(kResourceRecycled, totalRecycleUnits, recycleResource);
            scrappedResources.Add(new DistributedResource(recycleResource, totalRecycleUnits, recycleMessage));
            WBIDistributionManager.Instance.DistributeResources(scrappedResources);

            //Finally, poof the part.
            ScreenMessages.PostScreenMessage(string.Format(kPartRecycled, doomed.partInfo.title), kMessageDuration, ScreenMessageStyle.UPPER_CENTER);
            doomed.explosionPotential = kExplosionPotential;
            doomed.explode();
        }

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            Events["ScrapPart"].guiName = "Scrap part for " + recycleResource;
            Events["ScrapVessel"].guiName = "Scrap vessel for " + recycleResource;
        }
    }
}
