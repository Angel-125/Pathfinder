using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WildBlueIndustries
{
    public class TestData : PartModule
    {

        [KSPEvent(guiName = "Add Science Data", active = true, guiActive = true)]
        public void AddScienceData()
        {
            ModuleScienceLab sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            sciLab.dataStored = 100f;
        }

        [KSPEvent(guiName = "Add Science", active = true, guiActive = true)]
        public void AddScience()
        {
            ModuleScienceLab sciLab = this.part.FindModuleImplementing<ModuleScienceLab>();
            sciLab.storedScience = 100f;
        }
    }
}
