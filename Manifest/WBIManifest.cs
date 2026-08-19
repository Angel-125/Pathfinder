using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using FinePrint;
using KSP.UI.Screens;
using KSP.Localization;


/*
Source code copyrighgt 2017, by Michael Billard (Angel-125)
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
    /// <summary>
    /// This is the base class for transfering things from one vessel to another.
    /// </summary>
    public class WBIManifest
    {
        public const string kManifestType = "manifestType";
        public const string kDestinationID = "destinationID";
        public const string kCreationDate = "creationDate";
        public const string kDeliveryTime = "deliveryTime";
        public const string kManifest = "MANIFEST";
        public const string kNoType = "NOTYPE";

        /// <summary>
        /// This is a unique identifier, usually a guid, that identifies the manifest.
        /// </summary>
        public string destinationID = string.Empty;

        /// <summary>
        /// Tag to help determine what type of manifest this is. Example: WBIResourceManifest
        /// </summary>
        public string manifestType = kNoType;

        /// <summary>
        /// Date of creation, in KSP game time
        /// </summary>
        public double creationDate = 0;

        /// <summary>
        /// Number of seconds until the package is delivered.
        /// </summary>
        public double deliveryTime = 0;

        public WBIManifest()
        {
            //Set creation time
            creationDate = Planetarium.GetUniversalTime();
        }

        public virtual void Load(ConfigNode node)
        {
            string value;
            destinationID = node.GetValue(kDestinationID);
            manifestType = node.GetValue(kManifestType);

            value = node.GetValue(kCreationDate);
            creationDate = double.Parse(value);

            value = node.GetValue(kDeliveryTime);
            deliveryTime = double.Parse(value);
        }

        public virtual void Save(ConfigNode node)
        {
            node.AddValue(kDestinationID, destinationID);
            node.AddValue(kManifestType, manifestType);
            node.AddValue(kCreationDate, creationDate);
            node.AddValue(kDeliveryTime, deliveryTime);
        }
    }
}
