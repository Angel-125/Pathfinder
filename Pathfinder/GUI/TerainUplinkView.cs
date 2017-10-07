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
    public struct TerrainStatus
    {
        public string vesselName;
        public string status;
        public float scienceAdded;
        public Vessel vessel;
    }

    public class TerainUplinkView : Dialog<TerainUplinkView>
    {
        private const int kWindowWidth = 300;
        private const int kWindowHeight = 310;

        public Part part;
        public ModuleScienceContainer scienceContainer;
        public IParentView parentView;

        Vector2 scrollPos = new Vector2();
        List<TerrainStatus> terrainStatuses = new List<TerrainStatus>();

        public TerainUplinkView(string title = "T.E.R.R.A.I.N. Uplink") :
            base(title, kWindowWidth, kWindowHeight)
        {
            WindowTitle = title;
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue)
                findTerrainSats();
        }
        
        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(40));
            GUILayout.Label("<color=white><b>Satellites In Orbit:</b> " + terrainStatuses.Count + "</color>");
            GUILayout.EndScrollView();

            scrollPos = GUILayout.BeginScrollView(scrollPos, new GUIStyle(GUI.skin.textArea));
            foreach (TerrainStatus terrainStatus in terrainStatuses)
            {
                GUILayout.BeginScrollView(new Vector2(0, 0), new GUIStyle(GUI.skin.textArea), GUILayout.Height(130));
                GUILayout.Label("<color=white><b>Vessel: </b>" + terrainStatus.vesselName + "</color>");
                GUILayout.Label("<color=white><b>Last Update: </b>" + terrainStatus.status + "</color>");
                GUILayout.Label(string.Format("<color=white><b>Science Collected: </b>{0:f1}</color>", terrainStatus.scienceAdded));
                if (GUILayout.Button("Switch to vessel"))
                {
                    //Hide the ops view. If we don't we get a crash in the game and we can't control the ship.
                    parentView.SetParentVisible(false);
                    FlightGlobals.SetActiveVessel(terrainStatus.vessel);
                    break;
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndScrollView();

            GUILayout.EndVertical();
        }

        protected virtual void findTerrainSats()
        {
            TerrainStatus terrainStatus;

            terrainStatuses.Clear();

            foreach (Vessel vessel in FlightGlobals.VesselsUnloaded)
            {
                if (vessel.mainBody != this.part.vessel.mainBody)
                    continue;
                if (vessel.situation != Vessel.Situations.ORBITING)
                    continue;

                ProtoVessel protoVessel = vessel.protoVessel;
                foreach (ProtoPartSnapshot partSnapshot in protoVessel.protoPartSnapshots)
                {
                    foreach (ProtoPartModuleSnapshot moduleSnapshot in partSnapshot.modules)
                    {
                        //If we find a geo survey camera, then we have a terrain satellite
                        if (moduleSnapshot.moduleName == "GeoSurveyCamera")
                        {
                            terrainStatus = new TerrainStatus();

                            terrainStatus.vesselName = protoVessel.vesselName;
                            terrainStatus.status = moduleSnapshot.moduleValues.GetValue("status");
                            terrainStatus.scienceAdded = float.Parse(moduleSnapshot.moduleValues.GetValue("scienceAdded"));
                            terrainStatus.vessel = vessel;

                            terrainStatuses.Add(terrainStatus);
                        }
                    }
                }
            }
        }

    }
}
