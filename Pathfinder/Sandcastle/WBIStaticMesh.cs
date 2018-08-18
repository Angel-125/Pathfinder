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
    public class WBIStaticMesh
    {
        public string name;

        public string title;

        public string mesh;

        public float volume;

        public Dictionary<string, double> buildResources;

        public string meshPath;

        public GameObject prefab;

        public WBIStaticMesh()
        {
            buildResources = new Dictionary<string, double>();
        }

        public static Dictionary<string, WBIStaticMesh> LoadStaticMeshes()
        {
            Dictionary<string, WBIStaticMesh> staticMeshes = new Dictionary<string, WBIStaticMesh>();
            UrlDir.UrlConfig[] staticNodes = GameDatabase.Instance.GetConfigs("WBISANDCASTLE");
            WBIStaticMesh staticMesh;

            for (int index = 0; index < staticNodes.Length; index++)
            {
                staticMesh = new WBIStaticMesh();
                staticMesh.Load(staticNodes[index].config, staticNodes[index].url);
            }

            return staticMeshes;
        }

        public void Load(ConfigNode node, string url = "")
        {
            if (node.HasValue("name"))
                name = node.GetValue("name");

            if (node.HasValue("title"))
                title = node.GetValue("title");

            if (node.HasValue("mesh"))
                mesh = node.GetValue("mesh");

            if (node.HasValue("volume"))
                float.TryParse(node.GetValue("volume"), out volume);

            if (node.HasNode("BUILD_RESOURCE"))
            {
                ConfigNode[] resources = node.GetNodes("BUILD_RESOURCE");
                string resourceName;
                double amount;
                for (int index = 0; index < resources.Length; index++)
                {
                    resourceName = resources[index].GetValue("name");
                    amount = 0;
                    double.TryParse(resources[index].GetValue("amount"), out amount);
                    buildResources.Add(resourceName, amount);
                }
            }
        }
    }
}
