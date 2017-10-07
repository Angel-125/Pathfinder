using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;

namespace WildBlueIndustries
{
    public class WBIPathfinderLab : WBIGeoLab
    {
        const string kUnavailableMessage = "Experiments are currently unavailable. Perform a biome analysis first.";
        protected ModuleKerbNetAccess kerbNetAccess;
        protected PartModule impactSeismometer;
        protected IScienceDataContainer impactSensor;
        protected TerainUplinkView terrainUplinkView = new TerainUplinkView();
        protected ModuleScienceContainer scienceContainer;
        protected IParentView parentView = null;
        protected WBIExperimentLab experimentLab;

        public override void OnStart(StartState state)
        {
            base.OnStart(state);
            this.geoLabView.drawView = this.drawView;

            if (HighLogic.LoadedSceneIsFlight == false)
                return;

            //Get the experiment lab
            experimentLab = this.part.FindModuleImplementing<WBIExperimentLab>();
            if (experimentLab == null)
                return;
            experimentLab.unavailableMessage = kUnavailableMessage;

            //Hide the experiment lab if the
            if (Utils.IsBiomeUnlocked(this.part.vessel) == false)
                experimentLab.isAvailable = false;
        }

        protected void drawView()
        {
            //Review Data button
            int totalData = scienceContainer.GetStoredDataCount();
            if (GUILayout.Button("Review [" + totalData + "] Data") && totalData > 0)
            {
                scienceContainer.ReviewData();
            }

            //Terrain uplink
            if (GUILayout.Button("T.E.R.R.A.I.N. Uplink"))
            {
                terrainUplinkView.parentView = this.parentView;
                terrainUplinkView.SetVisible(true);
            }
        }

        protected override bool perfomBiomeAnalysys()
        {
            bool result = base.perfomBiomeAnalysys();

            if (result == true)
            {
                experimentLab.isAvailable = true;
                ScreenMessages.PostScreenMessage("Experiments that can improve production efficiency in this biome are now available.", 5.0f, ScreenMessageStyle.UPPER_CENTER);
            }

            return result;
        }

        protected override void setupPartModules()
        {
            base.setupPartModules();

            //Setup the science container
            if (scienceContainer == null)
            {
                scienceContainer = this.part.FindModuleImplementing<ModuleScienceContainer>();
                scienceContainer.Events["ReviewDataEvent"].guiActiveUnfocused = false;
                scienceContainer.Events["ReviewDataEvent"].guiActive = false;

                //Terrain uplink
                terrainUplinkView.part = this.part;
                terrainUplinkView.scienceContainer = scienceContainer;
            }
            
            //Kerbnet access
            kerbNetAccess = this.part.FindModuleImplementing<ModuleKerbNetAccess>();
            if (kerbNetAccess != null)
            {
            }

            //Grab the seismometer (if any)
            foreach (PartModule mod in this.part.Modules)
            {
                if (mod.moduleName == "Seismometer")
                {
                    impactSeismometer = mod;
                    impactSensor = (IScienceDataContainer)impactSeismometer;
                    ScienceData[] impactData = impactSensor.GetData();

                    foreach (ScienceData data in impactData)
                        scienceContainer.AddData(data);
                    foreach (ScienceData doomed in impactData)
                        impactSensor.DumpData(doomed);
                    break;
                }
            }
        }

        public override List<string> GetButtonLabels()
        {
            List<string> labels = new List<string>();

            labels.Add("Biome Analysis");

            return labels;
        }

        public override void SetParentView(IParentView parentView)
        {
            base.SetParentView(parentView);
            this.parentView = parentView;
        }
    }
}
