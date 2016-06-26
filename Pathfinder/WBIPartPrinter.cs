/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using KSP.IO;
using KSP.UI.Screens;
using KIS;

namespace WildBlueIndustries
{
    class WBIPartPrinter : PartModule
    {
        ConfigNode partNode;
        AvailablePart availablePart;

        [KSPEvent(guiActive = true, guiName = "Print Part")]
        public void PrintPart()
        {
            Debug.Log("FRED PrintPart called");
            Quaternion rot;
            Vector3 pos;

            this.availablePart = PartLoader.getPartInfoByName(this.part.partInfo.name);
            this.partNode = new ConfigNode();
//            KIS_Shared.PartSnapshot(this.part).CopyTo(this.partNode);
//            Debug.Log("FRED partNodeSnapshot: " + partNode);

            rot = this.part.transform.rotation;
            pos = this.part.transform.position + new Vector3(0, 10, 0);

//            KIS_Shared.CreatePart(this.partNode, pos, rot, this.part);
            KIS_Shared.CreatePart(this.part.partInfo, pos, rot, this.part);
        }
    }
}
 */
