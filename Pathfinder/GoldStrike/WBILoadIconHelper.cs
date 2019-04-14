using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;

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
    [KSPAddon(KSPAddon.Startup.SpaceCentre, true)]
    public class WBILodeIconHelper : MonoBehaviour
    {
        public void Awake()
        {
            DontDestroyOnLoad(this);
            GameEvents.onCustomWaypointLoad.Add(OnCustomWaypointLoad);

            SetupLodeIcons();
        }

        public void OnCustomWaypointLoad(GameEvents.FromToAction<Waypoint, ConfigNode> fta)
        {
            Waypoint waypoint = fta.from;
            ConfigNode node = fta.to;
            string location = string.Format("Lon: {0:f2} Lat: {1:f2}", waypoint.longitude, waypoint.latitude);

            if (WBIGoldStrikeScenario.Instance.IsLodeWaypoint(waypoint.navigationId.ToString()))
            {
                waypoint.id = WBIGoldStrikeScenario.kLodeIcon;
                waypoint.nodeCaption1 = location;
            }
        }

        public void SetupLodeIcons()
        {
            try
            {
                Dictionary<string, Dictionary<string, GoldStrikeLode>> goldStrikeLodes = WBIGoldStrikeScenario.Instance.goldStrikeLodes;
                Dictionary<string, GoldStrikeLode>[] lodeMaps = null;
                Dictionary<string, GoldStrikeLode> lodeMap = null;
                GoldStrikeLode[] lodes = null;
                GoldStrikeLode lode = null;
                Waypoint waypoint = null;
                string location = string.Empty;

                lodeMaps = goldStrikeLodes.Values.ToArray();
                for (int index = 0; index < lodeMaps.Length; index++)
                {
                    lodeMap = lodeMaps[index];
                    lodes = lodeMap.Values.ToArray();
                    for (int lodeIndex = 0; lodeIndex < lodes.Length; lodeIndex++)
                    {
                        lode = lodes[lodeIndex];
                        if (string.IsNullOrEmpty(lode.navigationID))
                            continue;

                        waypoint = WaypointManager.FindWaypoint(new Guid(lode.navigationID));
                        location = string.Format("Lon: {0:f2} Lat: {1:f2}", waypoint.longitude, waypoint.latitude);
                        if (waypoint != null)
                        {
                            WaypointManager.RemoveWaypoint(waypoint);
                            waypoint.id = WBIGoldStrikeScenario.kLodeIcon;
                            waypoint.nodeCaption1 = location;
                            WaypointManager.AddWaypoint(waypoint);
                        }
                    }
                }
            }
            catch { }
        }
    }
}
