using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;

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
/*
namespace WildBlueIndustries
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class WBIFieldPromotion : MonoBehaviour
    {
        public static WBIFieldPromotion Instance;
        public static double secondsPerCycle = Utils.secondsPerDayKerbin;

        protected double elapsedTime;
        protected double cycleStartTime;

        public void Start()
        {
            Instance = this;
            cycleStartTime = Planetarium.GetUniversalTime();
            elapsedTime = 0f;
            CheckForPromotions(true);

            GameEvents.onVesselRecovered.Add(this.onVesselRecovered);
            GameEvents.onDominantBodyChange.Add(this.onDominantBodyChange);
        }

        public void Destroy()
        {
            GameEvents.onVesselRecovered.Remove(this.onVesselRecovered);
            GameEvents.onDominantBodyChange.Remove(this.onDominantBodyChange);
        }

        public void onDominantBodyChange(GameEvents.FromToAction<CelestialBody, CelestialBody> fromTo)
        {
            resetPromotionTimers(FlightGlobals.ActiveVessel);
        }

        public void onVesselRecovered(ProtoVessel vessel, bool something)
        {
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
                return;

            List<ProtoCrewMember> crew;
            int crewCount;
            ProtoCrewMember[] crewMembers;
            ProtoCrewMember crewMember;

            crew = vessel.GetVesselCrew();
            crewCount = crew.Count;
            crewMembers = crew.ToArray();
            for (int index = 0; index < crewCount; index++)
            {
                crewMember = crewMembers[index];
                WBIPathfinderScenario.Instance.RemovePromotionEntry(crewMember.name);
            }
        }

        protected void resetPromotionTimers(Vessel vessel)
        {
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
                return;

            List<ProtoCrewMember> crew;
            int crewCount;
            ProtoCrewMember[] crewMembers;
            ProtoCrewMember crewMember;

            crew = vessel.GetVesselCrew();
            crewCount = crew.Count;
            crewMembers = crew.ToArray();
            for (int index = 0; index < crewCount; index++)
            {
                crewMember = crewMembers[index];
                WBIPathfinderScenario.Instance.UpdateLastPromotion(crewMember.name);
            }
        }

        public void FixedUpdate()
        {
            if (HighLogic.CurrentGame.Mode != Game.Modes.CAREER)
                return;

            //If we've waited long enough to distribute resources, then distribute them.
            elapsedTime = Planetarium.GetUniversalTime() - cycleStartTime;

            if (elapsedTime / secondsPerCycle > 1.0f)
            {
                cycleStartTime = Planetarium.GetUniversalTime();
                elapsedTime = 0f;
                CheckForPromotions();
            }
        }

        public void CheckForPromotions(bool isStartup = false)
        {
            Vessel vessel;
            int totalVessels, vesselIndex, crewCount;
            List<ProtoCrewMember> crew;
            ProtoCrewMember[] crewMembers;
            ProtoCrewMember crewMember;
            bool kerbalWasPromoted = false;

            //Get the list of all the vessels within physics range (they're loaded)
            totalVessels = FlightGlobals.Vessels.Count;
            for (vesselIndex = 0; vesselIndex < totalVessels; vesselIndex++)
            {
                vessel = FlightGlobals.Vessels[vesselIndex];
                if (vessel.loaded)
                {
                    //If the vessel isn't landed or splashed then we're done.
                    if (vessel.situation != Vessel.Situations.LANDED && vessel.situation != Vessel.Situations.SPLASHED)
                        continue;

                    //If we don't have a valid connection back to Kerbin then we're done.

                    //Check for field promotions on each crew member
                    crew = vessel.GetVesselCrew();
                    crewCount = crew.Count;
                    crewMembers = crew.ToArray();
                    for (int index = 0; index < crewCount; index++)
                    {
                        crewMember = crewMembers[index];
                        kerbalWasPromoted = checkKerbalPromotion(crewMember, vessel, isStartup);
                    }
                }
            }

            //Save the game?
            if (kerbalWasPromoted)
                GamePersistence.SaveGame("persistent", HighLogic.SaveFolder, SaveMode.BACKUP);
        }

        protected bool checkKerbalPromotion(ProtoCrewMember crewMember, Vessel vessel, bool isStartup)
        {
            //If the kerbal isn't elligible for a field promotion then we're done.
            if (WBIPathfinderScenario.Instance.KerbalCanBePromoted(crewMember.name) == false)
                return false;

            FlightLog log = crewMember.careerLog.CreateCopy();
            List<FlightLog.Entry> doomed = new List<FlightLog.Entry>();
            FlightLog.Entry entry;
            FlightLog.Entry[] flightEntries;
            int entryCount;
            float experience;
            int experienceLevel;
            string promotionMessage;

            //Debug.Log("Current experience: " + crewMember.experience + " current experienceLevel: " + crewMember.experienceLevel);
            flightEntries = crewMember.flightLog.Entries.ToArray();
            entryCount = flightEntries.Length;
            for (int index = 0; index < entryCount; index++)
            {
                //Get the entry
                entry = flightEntries[index];
                //Debug.Log("Checking entry " + entry.flight + ", " + entry.target + ", " + entry.type);

                //This is a brute-force way to do this but the number of entries are pretty small..
                log.AddEntry(entry);
                doomed.Add(entry);
                experience = KerbalRoster.CalculateExperience(log);
                experienceLevel = KerbalRoster.CalculateExperienceLevel(experience);
                //Debug.Log("Current experience: " + experience + " current experienceLevel: " + experienceLevel);

                //If the kerbal has enough experience then it's time for a field promotion!
                if (experienceLevel > crewMember.experienceLevel)
                {
                    //Send messages
                    promotionMessage = crewMember.name + " has earned a field promotion to a " + experienceLevel + "-star " + crewMember.experienceTrait.Title;
                    sendPromotionMessage(promotionMessage, vessel);

                    //Clear the career log and rebuild the list
                    crewMember.careerLog.Entries.Clear();
                    foreach (FlightLog.Entry careerEntry in log.Entries)
                        crewMember.careerLog.Entries.Add(careerEntry);

                    //Remove the entries we don't need from the flight log.
                    foreach (FlightLog.Entry doomedEntry in doomed)
                        crewMember.flightLog.Entries.Remove(doomedEntry);

                    //Now set experience and level
                    crewMember.experience = experience;
                    crewMember.experienceLevel = experienceLevel;

                    //Update the portrait.
                    if (!isStartup)
                    {
                        KSP.UI.Screens.Flight.KerbalPortraitGallery.Instance.UnregisterActiveCrew(crewMember.KerbalRef);
                        KSP.UI.Screens.Flight.KerbalPortraitGallery.Instance.RegisterActiveCrew(crewMember.KerbalRef);
                    }

                    //Finally, record the time when we gave the kerbal a field promotion.
                    WBIPathfinderScenario.Instance.UpdateLastPromotion(crewMember.name);
                    return true;
                }
            }

            //If we reach here then there was not enough experience for a field promotion.
            return false;
        }

        protected void sendPromotionMessage(string promotionMessage, Vessel vessel)
        {
            StringBuilder resultsMessage = new StringBuilder();
            MessageSystem.Message msg;

            resultsMessage.AppendLine("From: " + vessel.vesselName);
            resultsMessage.AppendLine("Congratuations!");
            resultsMessage.AppendLine(promotionMessage);
            msg = new MessageSystem.Message("Field Promotion", resultsMessage.ToString(),
                MessageSystemButton.MessageButtonColor.BLUE, MessageSystemButton.ButtonIcons.COMPLETE);
            MessageSystem.Instance.AddMessage(msg);
        }
    }
}
*/