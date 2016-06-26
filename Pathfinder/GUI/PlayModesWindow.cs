using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using KSP.IO;

namespace WildBlueIndustries
{
    public delegate void ChangePlayMode();

    public class PlayModesWindow : Window<PlayModesWindow>
    {
        private const string kDefaultDescription = "The default Play Mode enables Pathfinder with all its bells and whistles. You get a default set of templates and converters for your base building needs.";

        public ChangePlayMode changePlayModeDelegate;

        public string playModesPath;

        public string currentPlayMode;
        public string currentPlayModeFile;

        public bool payToRemodel;
        public bool requireSkillCheck;
        public bool repairsRequireResources;
        public bool partsCanBreak;

        protected string[] playModeFiles;
        protected string[] playModeFileNames;
        protected int selectedIndex;
        protected int prevIndex;
        protected string description;
        protected ConfigNode nodePlayMode;

        private Vector2 _scrollPos, _scrollPos2;

        public PlayModesWindow() :
        base("Select A Play Mode", 800, 600)
        {
            Resizable = false;
            _scrollPos = new Vector2(0, 0);
        }

        public override void SetVisible(bool newValue)
        {
            string tempFileName;

            base.SetVisible(newValue);

            if (string.IsNullOrEmpty(playModesPath))
                playModesPath = KSPUtil.ApplicationRootPath.Replace("\\", "/") + "GameData/WildBlueIndustries/Pathfinder/PlayModes/";

            playModeFiles = Directory.GetFiles(playModesPath);

            List<string> playModeNames = new List<string>();
            playModeNames.Add("Default");

            foreach (string filePath in playModeFiles)
            {
                tempFileName = filePath.Replace(playModesPath, "");
                tempFileName = tempFileName.Replace(".txt", "");
                tempFileName = tempFileName.Replace(".cfg", "");
                playModeNames.Add(tempFileName);
            }
            playModeFileNames = playModeNames.ToArray();

            selectedIndex = 0;
            prevIndex = -1;

        }

        protected override void DrawWindowContents(int windowId)
        {
            GUILayout.BeginVertical();

            GUILayout.Label("<color=yellow><b>You will need to restart Kerbal Space Program for these changes to take effect.</b></color>");

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, new GUILayoutOption[] { GUILayout.Width(375) });
            selectedIndex = GUILayout.SelectionGrid(selectedIndex, playModeFileNames, 1);
            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();

            _scrollPos2 = GUILayout.BeginScrollView(_scrollPos2, new GUILayoutOption[] { GUILayout.Width(425) });

            if (selectedIndex != prevIndex)
                loadConfig();

            GUILayout.Label(description);

            GUILayout.EndScrollView();

            drawOkCancelButtons();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
        }

        protected void loadConfig()
        {
            prevIndex = selectedIndex;

            if (selectedIndex == 0)
            {
                description = kDefaultDescription;

                payToRemodel = true;
                requireSkillCheck = true;
                repairsRequireResources = true;
                partsCanBreak = true;
            }

            else
            {

                string configFile = System.IO.File.ReadAllText(playModeFiles[selectedIndex - 1]);
                nodePlayMode = ConfigNode.Parse(configFile);

                if (nodePlayMode.HasNode("PATHFINDER_PLAYMODE"))
                    nodePlayMode = nodePlayMode.GetNode("PATHFINDER_PLAYMODE");

                if (nodePlayMode.HasValue("description"))
                    description = nodePlayMode.GetValue("description");
                else
                    description = "Not available";

                if (nodePlayMode.HasValue("payToRemodel"))
                    payToRemodel = bool.Parse(nodePlayMode.GetValue("payToRemodel"));
                else
                    payToRemodel = true;

                if (nodePlayMode.HasValue("requireSkillCheck"))
                    requireSkillCheck = bool.Parse(nodePlayMode.GetValue("requireSkillCheck"));
                else
                    requireSkillCheck = true;

                if (nodePlayMode.HasValue("repairsRequireResources"))
                    repairsRequireResources = bool.Parse(nodePlayMode.GetValue("repairsRequireResources"));
                else
                    repairsRequireResources = true;

                if (nodePlayMode.HasValue("partsCanBreak"))
                    partsCanBreak = bool.Parse(nodePlayMode.GetValue("partsCanBreak"));
                else
                    partsCanBreak = true;
            }

            //List the default values
            description = description + "\r\n\r\n<b>Default Settings</b>\r\n\r\n";
            description = description + "Require resources to reconfigure modules: " + payToRemodel.ToString() + "\r\n";
            description = description + "Require skill check to reconfigure modules: " + requireSkillCheck.ToString() + "\r\n";
            description = description + "Repairs require resources: " + repairsRequireResources.ToString() + "\r\n";
            description = description + "Parts can break: " + partsCanBreak.ToString() + "\r\n";
        }

        protected void drawOkCancelButtons()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                if (selectedIndex == 0)
                {
                    currentPlayMode = "Default";
                    currentPlayModeFile = "";

                    //Go through all the paths and make sure they're all set to .txt
                    foreach (string filePath in playModeFiles)
                    {
                        if (filePath.EndsWith(".cfg"))
                            System.IO.File.Move(filePath, filePath.Replace(".cfg", ".txt"));
                    }
                }

                else
                {
                    currentPlayModeFile = playModeFiles[selectedIndex - 1];

                    if (nodePlayMode.HasValue("name"))
                        currentPlayMode = nodePlayMode.GetValue("name");
                    else
                        currentPlayMode = currentPlayModeFile.Replace(playModesPath, "");

                    //Go through all the paths and make sure they're all set to .txt
                    foreach (string filePath in playModeFiles)
                    {
                        if (filePath.EndsWith(".cfg"))
                            System.IO.File.Move(filePath, filePath.Replace(".cfg", ".txt"));
                    }

                    //Now rename the selected file to .cfg
                    if (currentPlayModeFile.EndsWith(".txt"))
                    {
                        System.IO.File.Move(currentPlayModeFile, currentPlayModeFile.Replace(".txt", ".cfg"));
                        currentPlayModeFile = currentPlayModeFile.Replace(".txt", ".cfg");
                    }
                }

                //Cleanup
                if (changePlayModeDelegate != null)
                    changePlayModeDelegate();
                SetVisible(false);
            }

            if (GUILayout.Button("Cancel"))
            {
                SetVisible(false);
            }
            GUILayout.EndHorizontal();
        }


    }
}
