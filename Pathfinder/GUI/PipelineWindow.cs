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
    public struct PipeEndpointNode
    {
        public PartModule endpoint;
        public string vesselName;
        public ConfigNode moduleValues;
    }

    public class PipelineWindow : Dialog<PipelineWindow>
    {
        PipeEndpointNode[] pipeEndpoints;

        public PipelineWindow(string title = "Pipelines") :
            base(title, 800, 600)
        {
            WindowTitle = title;
            Resizable = false;
        }

        public override void SetVisible(bool newValue)
        {
            base.SetVisible(newValue);

            if (newValue && FlightGlobals.ActiveVessel.situation == Vessel.Situations.LANDED)
            {
                Vessel[] vessels = FlightGlobals.VesselsUnloaded.ToArray();
                Vessel vessel;
                PipeEndpointNode endpointNode;
                List<PipeEndpointNode> endpoints = new List<PipeEndpointNode>();
                bool foundEndpipe;

                //Find vessels that have a pipe endpoint that are on the active vessel's celestial body.
                for (int index = 0; index < vessels.Length; index++)
                {
                    vessel = vessels[index];
                    if (vessel.mainBody == FlightGlobals.ActiveVessel.mainBody)
                    {
                        foundEndpipe = false;
                        foreach (ProtoPartSnapshot protoPart in vessel.protoVessel.protoPartSnapshots)
                        {
                            foreach (ProtoPartModuleSnapshot protoModule in protoPart.modules)
                            {
                                if (protoModule.moduleName == "WBIPipeEndpoint")
                                {
                                    endpointNode = new PipeEndpointNode();
                                    endpointNode.endpoint = protoModule.moduleRef;
                                    endpointNode.moduleValues = protoModule.moduleValues; //Use this to tell who connects to whom
                                    endpointNode.vesselName = vessel.vesselName;
                                    endpoints.Add(endpointNode);
                                    foundEndpipe = true;
                                    break;
                                }
                            }
                            if (foundEndpipe)
                                break;
                        }
                    }
                }
                pipeEndpoints = endpoints.ToArray();
            }
        }

        protected override void DrawWindowContents(int windowId)
        {
            PipeEndpointNode endpoint;

            GUILayout.BeginVertical();

            for (int index = 0; index < pipeEndpoints.Length; index++)
            {
                endpoint = pipeEndpoints[index];
                GUILayout.Label(endpoint.vesselName);
            }

            GUILayout.EndVertical();
        }
    }
}
