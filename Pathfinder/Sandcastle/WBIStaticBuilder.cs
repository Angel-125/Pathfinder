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
    public class WBIStaticBuilder : PartModule, IAnimatedModule
    {
        private const string kStaticNode = "STATIC";
        private const string kTagsField = "tags";
        private const string kNameField = "name";
        private const double kCatchupTime = 3600.0f;

        #region Fields
        /// <summary>
        /// Specialist skill needed in order to receive a bonus that reduces required resources.
        /// </summary>
        [KSPField]
        public string SpecialistSkill;

        /// <summary>
        /// Bonus per experience level. Default 0.01 (1% reduction of required resources)
        /// </summary>
        [KSPField]
        public double SpecialistBonus = 0.01;

        /// <summary>
        /// Tag that tells us which statics we're allowed to build. STATIC definition needs the "tags" field.
        /// </summary>
        [KSPField]
        public string tag = string.Empty;

        /// <summary>
        /// Name of the start build process
        /// </summary>
        [KSPField]
        public string startBuildProcessName = "Start Bulldozing";

        /// <summary>
        /// Name of the stop build
        /// </summary>
        [KSPField]
        public string stopBuildProcessName = "Stop Bulldozing";

        /// <summary>
        /// Flag to indicate whether or not we're building.
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool isBuilding;

        /// <summary>
        /// Flag to indicate that the build is completed. Lets us delay spawning the static.
        /// </summary>
        [KSPField(isPersistant = true)]
        public bool buildCompleted;

        /// <summary>
        /// Last update time index.
        /// </summary>
        [KSPField(isPersistant = true)]
        public double lastUpdatedTime;

        /// <summary>
        /// Name of the static object to spawn.
        /// </summary>
        [KSPField(isPersistant = true)]
        public string staticToSpawn = "wbiSmallPlatform";
        #endregion

        #region Housekeeping
        public List<ConfigNode> staticList = new List<ConfigNode>();
        public List<WBIBuildResource> requiredResources = new List<WBIBuildResource>();
        public ConfigNode spawnStaticNode;
        protected WBIPlatformBuildView builderView;
        #endregion

        #region Events
        [KSPEvent(guiActive = true, guiActiveUnfocused = true, unfocusedRange = 3.0f, guiName = "Toggle Build View")]
        public void ToggleBuildProcess()
        {
            toggleBuildWindow();
        }
        #endregion

        #region Overrides
        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            builderView = new WBIPlatformBuildView();
            builderView.OnStaticSelected = startBuilding;
            builderView.OnCancelBuild = cancelBuild;
            builderView.OnSpawnStatic = spawnStaticObject;
            builderView.isBuilding = this.isBuilding;
            builderView.buildCompleted = this.buildCompleted;

            setupGUI();

            if (HighLogic.LoadedSceneIsFlight)
            {
                findBuildableStatics();
            }
        }

        public override void OnLoad(ConfigNode node)
        {
            base.OnLoad(node);
            loadRequiredResources(node);
        }

        public override void OnSave(ConfigNode node)
        {
            base.OnSave(node);

            int count = requiredResources.Count;
            if (count > 0)
            {
                for (int index = 0; index < count; index++)
                    node.AddNode(requiredResources[index].Save());
            }
        }

        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;

            //Update the build window
            builderView.isBuilding = this.isBuilding;
            builderView.buildCompleted = this.buildCompleted;

            if (!isBuilding)
                return;
            int resourceCount = requiredResources.Count;
            if (resourceCount == 0)
                return;

            //Get elapsed time and update lastUpdatedTime
            double currentTime = Planetarium.GetUniversalTime();
            double elapsedTime = currentTime - lastUpdatedTime;
            lastUpdatedTime = currentTime;

            //Get crewBonus
            double crewBonus = getCrewSkill() * SpecialistBonus;
            builderView.crewBonus = crewBonus;

            //Now go through all the resources and see if they're acquired.
            //Account for long periods of absence from the vessel.
            bool allResourcesAcquired = true;
            while (elapsedTime > kCatchupTime)
            {
                for (int index = 0; index < resourceCount; index++)
                    allResourcesAcquired = requiredResources[index].hasAcquiredResource(kCatchupTime, crewBonus);

                elapsedTime -= kCatchupTime;
            }

            //Handle the remainder of elapsed time
            for (int index = 0; index < resourceCount; index++)
                allResourcesAcquired = requiredResources[index].hasAcquiredResource(elapsedTime, crewBonus);

            //If not all of the resources have been acquired then we're done.
            if (!allResourcesAcquired)
                return;

            //If we're not the active vessel then we're done.
            if (FlightGlobals.ActiveVessel != this.part.vessel)
                return;

            //All checks out! Set the build complete flag.
            this.buildCompleted = true;
            this.isBuilding = false;
            builderView.buildCompleted = true;
            builderView.isBuilding = false;
        }
        #endregion

        #region IAnimatedModule
        public void DisableModule()
        {
            this.isEnabled = false;
            isBuilding = false;
            builderView.isBuilding = false;
        }

        public void EnableModule()
        {
            this.isEnabled = true;
        }

        public bool IsSituationValid()
        {
            return this.part.vessel.situation == Vessel.Situations.LANDED || this.part.vessel.situation == Vessel.Situations.PRELAUNCH;
        }

        public bool ModuleIsActive()
        {
            return isBuilding;
        }
        #endregion

        #region Helpers
        protected int getCrewSkill()
        {
            if (string.IsNullOrEmpty(this.SpecialistSkill))
                return 0;

            int count = this.part.vessel.GetCrewCount();
            List<ProtoCrewMember> vesselCrew = this.part.vessel.GetVesselCrew();
            ProtoCrewMember crewMember;
            int totalSkill = 0;

            for (int index = 0; index < count; index++)
            {
                crewMember = vesselCrew[index];
                if (crewMember.HasEffect(this.SpecialistSkill))
                    totalSkill += crewMember.experienceLevel;
            }

            return totalSkill;
        }

        protected void cancelBuild()
        {
            this.staticToSpawn = string.Empty;
            this.spawnStaticNode = null;
            this.isBuilding = false;
            this.requiredResources.Clear();

            setupGUI();
        }

        protected void startBuilding(string staticName, ConfigNode staticNode)
        {
            this.lastUpdatedTime = Planetarium.GetUniversalTime();
            this.staticToSpawn = staticName;
            this.spawnStaticNode = staticNode;
            this.isBuilding = true;

            setupGUI();
            loadRequiredResources(staticNode);
        }

        protected void loadRequiredResources(ConfigNode node)
        {
            if (node.HasNode(WBIBuildResource.kBuildNode))
            {
                requiredResources.Clear();
                ConfigNode[] requiredNodes = node.GetNodes(WBIBuildResource.kBuildNode);
                WBIBuildResource buildResource;
                for (int index = 0; index < requiredNodes.Length; index++)
                {
                    buildResource = new WBIBuildResource(requiredNodes[index]);
                    buildResource.part = this.part;
                    requiredResources.Add(buildResource);
                }
            }
        }

        protected void toggleBuildWindow()
        {
            builderView.staticList = this.staticList;
            builderView.requiredResources = this.requiredResources;
            builderView.currentStaticNode = this.spawnStaticNode;
            builderView.currentStaticName = this.staticToSpawn;
            builderView.isBuilding = this.isBuilding;

            builderView.SetVisible(!builderView.IsVisible());
        }

        protected void findBuildableStatics()
        {
            if (string.IsNullOrEmpty(this.tag))
                return;
            ConfigNode[] staticNodes = GameDatabase.Instance.GetConfigNodes(kStaticNode);
            ConfigNode node;
            string staticTag;

            for (int index = 0; index < staticNodes.Length; index++)
            {
                node = staticNodes[index];
                if (node.HasValue(kTagsField))
                {
                    staticTag = node.GetValue(kTagsField);

                    //Add static to the list if it's one we're interested in.
                    if (staticTag.Contains(this.tag))
                        staticList.Add(node);

                    //If the node is one we're currently building, then record that too.
                    if (staticToSpawn == node.GetValue(kNameField))
                        spawnStaticNode = node;
                }
            }
        }

        protected void spawnStaticObject()
        {
            isBuilding = false;
            buildCompleted = false;
            KKWrapper.Init();
            KKAPI.SpawnObject(staticToSpawn);
        }

        protected void setupGUI()
        {
        }
        #endregion
    }
}
