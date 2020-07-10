using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using Experience;
using ContractsPlus.Contracts;
using KSP.UI.Screens;
using KSP.UI.Screens.Flight;


/*
Source code copyright 2018, by Michael Billard (Angel-125)
License: GNU General Public License Version 3
License URL: http://www.gnu.org/licenses/
Wild Blue Industries is trademarked by Michael Billard and may be used for non-commercial purposes. All other rights reserved.
Note that Wild Blue Industries is a ficticious entity 
created for entertainment purposes. It is in no way meant to represent a real entity.
Any similarity to a real entity is purely coincidental.

Portions of this software use code from the Firespitter plugin by Snjo, used with permission. Thanks Snjo for sharing how to switch meshes. :)

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/
namespace WildBlueIndustries
{
    public class WBIClassConverterView : Dialog<WBIClassConverterView>
    {
        private const int kWindowWidth = 640;
        private const int kWindowHeight = 480;

        public string traitsAllowedToConvert = string.Empty;
        public string blacklistedTraits = string.Empty;
        public bool resetExperience = true;
        public Part part;

        private string[] kerbalNames;
        private ExperienceSystemConfig expSysConfig;
        private string[] traitNames;
        private Dictionary<string, ProtoCrewMember> trainees = new Dictionary<string, ProtoCrewMember>();
        private Dictionary<string, string> newProfessions = new Dictionary<string, string>();
        private Vector2 scrollPosKerbals;
        private Vector2 scrollPosTraits;
        private GUILayoutOption[] panelLayoutOptions = new GUILayoutOption[] { GUILayout.Width(320) };
        private int selectedKerbal = 0;
        private int selectedTrait = 0;

        public WBIClassConverterView(string title = "Training Center") :
            base(title, kWindowWidth, kWindowHeight)
        {
            WindowTitle = title;
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
            {
                trainees.Clear();
                newProfessions.Clear();

                //Get a list of available traits
                expSysConfig = new ExperienceSystemConfig();
                expSysConfig.LoadTraitConfigs();
                traitNames = expSysConfig.TraitNames.ToArray();

                //Filter the list if we have any blacklisted traits
                List<string> names = new List<string>();
                if (!string.IsNullOrEmpty(blacklistedTraits))
                {
                    for (int index = 0; index < traitNames.Length; index++)
                    {
                        if (blacklistedTraits.Contains(traitNames[index]) == false)
                            names.Add(traitNames[index]);
                    }
                    if (names.Count > 0)
                        traitNames = names.ToArray();
                    else
                        traitNames = null;
                }
                if (traitNames == null)
                    return;

                //Get the list of kerbals in the part that can be converted.
                int totalCrew = this.part.protoModuleCrew.Count;
                ProtoCrewMember candidate;
                names.Clear();
                for (int index = 0; index < totalCrew; index++)
                {
                    candidate = this.part.protoModuleCrew[index];
                    if (string.IsNullOrEmpty(traitsAllowedToConvert))
                    {
                        trainees.Add(candidate.name, candidate);
                        names.Add(candidate.name);
                    }
                    else if (traitsAllowedToConvert.Contains(candidate.trait))
                    {
                        trainees.Add(candidate.name, candidate);
                        names.Add(candidate.name);
                    }
                }

                //Generate the list of names
                if (names.Count > 0)
                    kerbalNames = names.ToArray();
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            //Failsafe: If we have no traits then we're done.
            if (traitNames == null)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("<color=yellow><b>There are currently no allowed traits that kerbals may convert to.</b></color>");
                GUILayout.EndVertical();
                return;
            }

            //Failsafe: If we have nobody in the part then we're done.
            if (trainees.Count == 0)
            {
                GUILayout.BeginVertical();
                GUILayout.Label("<color=yellow><b>There are currently no kerbals in the classroom that are allowed to retrain.</b></color>");
                if (this.part.protoModuleCrew.Count > 0 && !string.IsNullOrEmpty(traitsAllowedToConvert))
                {
                    string[] traits = traitsAllowedToConvert.Split(new char[] { ';' });
                    GUILayout.Label("<color=yellow><b>Only kerbals with the following trait(s) can be retrained:\r\n</b></color>");
                    for (int index = 0; index < traits.Length; index++)
                        GUILayout.Label("<color=yellow>" + traits[index] + "</color>\r\n");
                }
                GUILayout.EndVertical();
                return;
            }

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //Draw the trainees pane
            scrollPosKerbals = GUILayout.BeginScrollView(scrollPosKerbals, panelLayoutOptions);
            selectedKerbal = GUILayout.SelectionGrid(selectedKerbal, kerbalNames, 1);
            GUILayout.EndScrollView();
            string kerbalName = kerbalNames[selectedKerbal];

            //Draw the traits pane
            GUILayout.BeginVertical();

            //Courage and stupidity.
            float courage = trainees[kerbalName].courage * 100.0f;
            float stupdity = trainees[kerbalName].stupidity * 100.0f;
            string currentTrait = trainees[kerbalName].trait;
            if (newProfessions.ContainsKey(kerbalName))
            {
                if (newProfessions[kerbalName] != currentTrait)
                    currentTrait = newProfessions[kerbalName];
            }
            GUILayout.Label("<color=white><b>Current Trait: </b>" + currentTrait + "</color>");
            GUILayout.Label(string.Format("<color=white><b>Courage: </b>{0:f2}%</color>", courage));
            GUILayout.Label(string.Format("<color=white><b>Stupidity: </b>{0:f2}%</color>", stupdity));

            //Trait list
            scrollPosTraits = GUILayout.BeginScrollView(scrollPosTraits, panelLayoutOptions);
            selectedTrait = GUILayout.SelectionGrid(selectedTrait, traitNames, 1);
            GUILayout.EndScrollView();
            string traitName = traitNames[selectedTrait];

            //Set the map
            if (GUILayout.Button("SET"))
            {
                if (newProfessions.ContainsKey(kerbalName))
                {
                    if (newProfessions[kerbalName] != traitName)
                        newProfessions[kerbalName] = traitName;
                }
                else
                {
                    newProfessions.Add(kerbalName, traitName);
                }

                ScreenMessages.PostScreenMessage("Changes will take effect when you press the OK button.", 3.0f, ScreenMessageStyle.UPPER_CENTER);
            }
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            //OK button
            if (GUILayout.Button("OK"))
            {
                retrainKerbals();
                SetVisible(false);
            }
            GUILayout.EndVertical();
        }

        protected void retrainKerbals()
        {
            ProtoCrewMember trainee;
            foreach (string kerbalName in newProfessions.Keys)
            {
                trainee = trainees[kerbalName];

                //If the kerbal is currently a tourist, then unregister the kerbal from any tourism contracts.
                if (trainee.trait == "Tourist")
                    WBIContractScenario.Instance.unregisterKerbal(kerbalName);

                //Set the new trait
                KerbalPortraitGallery.Instance.UnregisterActiveCrew(trainee.KerbalRef);

                trainee.UnregisterExperienceTraits(trainee.KerbalRef.InPart);
                trainee.KerbalRef.InVessel.CrewListSetDirty();
                KerbalRoster.SetExperienceTrait(trainee, newProfessions[kerbalName]);
                trainee.RegisterExperienceTraits(trainee.KerbalRef.InPart);
                trainee.KerbalRef.InVessel.CrewListSetDirty();

                KerbalPortraitGallery.Instance.RegisterActiveCrew(trainee.KerbalRef);
                KerbalPortraitGallery.Instance.UpdatePortrait(trainee.KerbalRef);
                KerbalPortraitGallery.Instance.StartReset(this.part.vessel);

                //Reset experience
                if (resetExperience)
                    KerbalRoster.SetExperienceLevel(trainee, 0);

                Vessel.CrewWasModified(trainee.KerbalRef.InVessel);
                FlightInputHandler.ResumeVesselCtrlState(trainee.KerbalRef.InVessel);
            }
        }
    }
}
