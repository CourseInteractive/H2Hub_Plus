using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using UnityEditor.SceneManagement;
using System;

//-----------------------------------------------------------------------------
// Copyright 2020 Oliver Ziegler - Course Interactive.  All rights reserverd.
//-----------------------------------------------------------------------------
namespace Course.QuickSceneGuide
{
    public class QuickSceneGuide : EditorWindow
    {

        #region Variables
        static bool showOnlyFavs = false;
        static bool hideInactiveScenes = false;
        static bool showPath = false;
        public enum SearchTarget { Path, Filename }
        static SearchTarget searchIn;
        static bool showChaptersHorizontal;
        static bool displaySettingsFoldout;
        static bool chapterSettingsFoldout;
        static bool chapterContentFoldout;
        static bool showCopySceneNameButton;
        static bool[] chapterDisplayFoldout;
        static bool showFirstScene;
        static string searchInput = "";
        static bool noChapters;
        static List<string> favs;

	    static QSG_Data data;
	    static string dataName = "QSG_Data";

        static Texture favOffSymbol;
        static Texture favOnSymbol;
        static Texture copySymbol;
        static Texture arrowUpSymbol;
        static Texture arrowDownSymbol;
        static Vector2 scrollPos = new Vector2(0, 0);

        static Vector2 vNumber = new Vector2(2, 9);

        public string[] toolbarCategories = { "All Scenes", "Chapter", "Filter", "Settings" };
        public int toolbarIndex = 0;

        static int buildSettingsScenesLastTime = -1;

        static List<string> sceneNames;
        static string[] sceneNamesArray;
        public List<List<int>> chapterContentIndices;
        public List<List<string>> chapterContent;

        static public GUIStyle styleRight;
        static public GUIStyle styleCenter;

        static string path = "/Course/QuickSceneGuide/Datas/Resources/";

    #endregion

        [MenuItem("Tools/Course/Quick Scene Guide")]
        static void Open()
        {

            QuickSceneGuide window = (QuickSceneGuide)EditorWindow.GetWindow(typeof(QuickSceneGuide));
            Init();
        }

  	    static void Init()
	    {
            data = (QSG_Data)Resources.Load (dataName);
		    if (data == null)
		    {
			    if(!Directory.Exists(Application.dataPath + path))
				    Directory.CreateDirectory(Application.dataPath + path);
			    CreateAsset<QSG_Data> ("Assets" + path + dataName + ".asset");
		    }
            data = (QSG_Data)Resources.Load (dataName);
		    favs = data.favs;
		    showOnlyFavs = data.showOnlyFavs;
		    hideInactiveScenes = data.hideInactiveScenes; 
		    showPath = data.showPath; 
            searchIn = (SearchTarget)data.searchIn;
		    searchInput = data.searchInput;
            showChaptersHorizontal = data.showChaptersHorizontal;
            chapterDisplayFoldout = data.chapterDisplayFoldout;
            showCopySceneNameButton = data.showCopySceneNameButton;
            showFirstScene = data.showFirstScene;
            LoadGraphics ();
        }
        static KeyCode controlToggle = KeyCode.LeftControl;
        static bool controlInputToggled = false;
        void HandleControlInput()
        {
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == controlToggle)
                controlInputToggled = true;
            if (Event.current.type == EventType.KeyUp && Event.current.keyCode == controlToggle)
                controlInputToggled = false;
        }

        void OnGUI()
	    {
            HandleControlInput();
            SetStyles();
            if (EditorBuildSettings.scenes.Length == 0)
            {
                EditorGUILayout.LabelField("This tool presents only the scenes in your build settings but there are currently no scenes set in the build settings.", EditorStyles.wordWrappedLabel);
                return;
            }
            noChapters = false;

            if (favs == null)
			    favs = new List<string> ();
		
		    if (data == null)
			    Init ();
            if (!sceneListReactionInitialized)
                InitSceneListChangeReaction();        
            if (sceneNamesArray == null || sceneNamesArray.Length != EditorBuildSettings.scenes.Length)
            {
                    if (Event.current.type == EventType.Layout)
                    {
                GetSceneNamesFromBuildSettings();
                if (chapterContentIndices == null || chapterContentIndices.Count != data.chapterContent.Count)
                    MatchChapterContentToIndices();
                    }
                    else
                return;
            }
            if (chapterContentIndices == null || chapterContentIndices.Count != data.chapterContent.Count || chapterContent == null)
            {
                GetSceneNamesFromBuildSettings();
                MatchChapterContentToIndices();
            }
            if (data.chapters == null || data.chapters.Count == 0)
                noChapters = true;
            styleRight = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleRight };
            styleCenter = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };


            toolbarIndex = GUI.Toolbar(new Rect(0, this.position.height-30, this.position.width, 30), toolbarIndex, toolbarCategories);
        
       
            EditorBuildSettingsScene[] scenes;
            switch (toolbarIndex)
            {

                #region Toolbar "Scenes"
                case 0:
                    if (showFirstScene)
                        {
                            DisplayFirstScene();
                            DisplayAllScenes();
                        }
                        else
                        {
                            DisplayAllScenes();
                        }


                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();

                    break;
                #endregion
                #region Toolbar "Chapters"
                case 1:
                if (showFirstScene)
                        DisplayFirstScene();
                    if (noChapters)
                    {
                        EditorGUILayout.LabelField("In order to display the scenes in chapters you need to define chapters in the settings and connect scenes to them.", EditorStyles.wordWrappedLabel);
                        break;
                    }
                  

                    if (chapterContentIndices == null || chapterContentIndices.Count != data.chapterContent.Count)
                        MatchChapterContentToIndices();

                    switch(showChaptersHorizontal)
                    {
                        case false:
                            if(chapterDisplayFoldout == null || chapterDisplayFoldout.Length != data.chapters.Count)
                            {
                                chapterDisplayFoldout = new bool[data.chapters.Count];
                            }
                            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                      
                                for (int chapter = 0; chapter < data.chapters.Count; chapter++)
                                {
                                EditorGUI.BeginChangeCheck();
                                chapterDisplayFoldout[chapter] = EditorGUILayout.Foldout(chapterDisplayFoldout[chapter], data.chapters[chapter]);
                                if (EditorGUI.EndChangeCheck())
                                    Save();
                                    if (chapterDisplayFoldout[chapter])
                                    {
                                        for (int i = 0; i < chapterContentIndices[chapter].Count; i++)
                                        {
                                        if (chapterContentIndices[chapter][i] >= EditorBuildSettings.scenes.Length)
                                            continue;
                                            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[chapterContentIndices[chapter][i]];
                                        GUI.color = Color.white;
                                            if (!scene.enabled && hideInactiveScenes)
                                                continue;

                                            if (!scene.enabled && !hideInactiveScenes)
                                                GUI.color = new Color(0.8f, 0.8f, 0.8f);

                                            if (scene.path == EditorSceneManager.GetActiveScene().path)
                                                GUI.color = new Color(0.6f, 1f, 0.6f);

                                            if (scene.path == EditorSceneManager.GetActiveScene().path && !scene.enabled && !hideInactiveScenes)
                                                GUI.color = new Color(0.3f, 0.6f, 0.3f);
                                        EditorGUILayout.BeginHorizontal();
                                            if (GUILayout.Button(sceneNamesArray[chapterContentIndices[chapter][i]]))
                                            {
                                                if (controlInputToggled)
                                                    ToggleSceneInBuildSettings(chapterContent[chapter][i]);
                                                else
                                                    OpenScene(chapterContent[chapter][i]);
                                            }
                                            if (showCopySceneNameButton)
                                            {
                                                if (GUILayout.Button(copySymbol, GUILayout.Width(25)))
                                                {
                                                    EditorGUIUtility.systemCopyBuffer = sceneNamesArray[chapterContentIndices[chapter][i]];
                                                }
                                            }
                                        GUI.color = Color.white;
                                        EditorGUILayout.EndHorizontal();
                                    }
                                    }
                            


                            }
                            EditorGUILayout.EndScrollView();
                            EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();



                            break;
                        case true:
                            float width = this.position.width / data.chapters.Count;

                            EditorGUILayout.BeginHorizontal();
                            for (int title = 0; title < data.chapters.Count; title++)
                            {
                                EditorGUILayout.LabelField(data.chapters[title], headerStyle, GUILayout.Width(width - 5));
                            }
                            EditorGUILayout.EndHorizontal();

                            if (chapterContent == null || chapterContent.Count == 0)
                                return;

                            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                            for (int i = 0; i < 100; i++)
                            {
                                int checkSum = 0;
                                EditorGUILayout.BeginHorizontal();
                                for (int chapter = 0; chapter < data.chapters.Count; chapter++)
                                {
                                    if (chapterContent[chapter].Count > i)
                                    {
                                
                                        EditorBuildSettingsScene scene = EditorBuildSettings.scenes[chapterContentIndices[chapter][i]];
                                        GUI.color = Color.white;
                                        //if (!scene.enabled && hideInactiveScenes)
                                        //    continue;

                                        if (!scene.enabled)// && !hideInactiveScenes)
                                            GUI.color = new Color(0.8f, 0.8f, 0.8f);

                                        if (scene.path == EditorSceneManager.GetActiveScene().path)
                                            GUI.color = new Color(0.6f, 1f, 0.6f);

                                        if (scene.path == EditorSceneManager.GetActiveScene().path && !scene.enabled && !hideInactiveScenes)
                                            GUI.color = new Color(0.3f, 0.6f, 0.3f);
                                        if (GUILayout.Button(sceneNamesArray[chapterContentIndices[chapter][i]], GUILayout.Width(width - 7)))
                                        {
                                            if (controlInputToggled)
                                                ToggleSceneInBuildSettings(chapterContent[chapter][i]);
                                            else
                                                OpenScene(chapterContent[chapter][i]);
                                        }
                                        GUI.color = Color.white;
                                    }
                                    else
                                    {
                                        EditorGUILayout.LabelField("", GUILayout.Width(width - 7));
                                        checkSum++;
                                    }

                                }

                                EditorGUILayout.EndHorizontal();
                                if (checkSum == data.chapters.Count)
                                    break;


                            }
                            EditorGUILayout.EndScrollView();
                            EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();

                            break;
                    }

               



                    break;
                #endregion
                #region Toolbar "Filter"
                case 2:

                    searchIn = (SearchTarget)EditorGUILayout.EnumPopup("Search in:", searchIn);
                    searchInput = EditorGUILayout.TextField("Filter: ", searchInput);
                    if (GUILayout.Button("Clear"))
                    {
                        showOnlyFavs = false;
                        hideInactiveScenes = false;
                        searchInput = "";
                    }
                    if (searchInput.Trim() != "")
                    {
                        EditorGUILayout.LabelField("Scenes found", headerStyle);

                            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
                        scenes = EditorBuildSettings.scenes;
                    
                        foreach (EditorBuildSettingsScene scene in scenes)
                        {
                            int last = scene.path.LastIndexOf("/");
                            string text = scene.path.Substring(last + 1, scene.path.Length - 6 - last - 1);
                            searchInput = searchInput.ToLower();
                            if (searchIn == SearchTarget.Filename && searchInput != "" && !text.ToLower().Contains(searchInput))
                                continue;
                            else if (searchIn == SearchTarget.Path && searchInput != "" && !scene.path.ToLower().Contains(searchInput))
                                continue;

                            DisplayScene(scene, text, false, true);

                        }
                        EditorGUILayout.EndScrollView();
                        EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                    }
     
                    break;
                #endregion
                #region Toolbar "Settings"
                case 3:
                    if (data.chapters == null)
                        data.chapters = new List<string>();

                    ShowDisplaySettings();
                    ShowChapterHeaders();

                    #region Chapter Content Settings
                    if (chapterContentFoldout)
                        GUI.color = settingsSelectedColor;
                    if (noChapters)
                        GUI.enabled = false;
                    if (GUILayout.Button("Chapter Content"))
                        SettingsFoldoutBtnPressed(2);
                    GUI.enabled = true;
                    GUI.color = Color.white;
                    if (chapterContentFoldout)
                    {
                        
                        chapterSettingsIndex = GUILayout.Toolbar(chapterSettingsIndex, chapterSettingsCategories);
                        if (chapterSettingsIndex == 1)
                            ShowChapterContentByChapter();
                        if (chapterSettingsIndex == 0)
                            ShowChapterContentByScenes();
                    }
                    
               
                    #endregion


                
                    EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator(); EditorGUILayout.Separator();
                    if (GUI.Button(new Rect(0, this.position.height - 60, this.position.width/2, 25), "Save"))
                    {
                        Save();
                    }
                    EditorGUI.LabelField(new Rect(this.position.width / 2, this.position.height - 55, this.position.width / 2, 30), "Quick Scene Guide  -  v" + vNumber.x.ToString() + "." + vNumber.y.ToString(), styleRight);

                    break;
                    #endregion
            }
	    }
   
            void DisplayAllScenes()
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                foreach (EditorBuildSettingsScene scene in scenes)
                {
                    int last = scene.path.LastIndexOf("/");
                    if (scene.path.Length <= 0 || (scene.path.Length - 6 - last - 1) < 0)
                        continue;
                    string name = scene.path.Substring(last + 1, scene.path.Length - 6 - last - 1);

                    DisplaySceneWithFavOption(scene, name);

                }

    
                EditorGUILayout.EndScrollView();
            }

        #region Settings

        public void ShowDisplaySettings()
        {
            if(displaySettingsFoldout)
                GUI.color = settingsSelectedColor;

            if (GUILayout.Button("Display Settings"))
                SettingsFoldoutBtnPressed(0);
            GUI.color = Color.white;
            if (displaySettingsFoldout)
            {
                showOnlyFavs = EditorGUILayout.Toggle("Show Only Favorites:", showOnlyFavs);
                hideInactiveScenes = EditorGUILayout.Toggle("Hide Inactive Scenes:", hideInactiveScenes);
                showPath = EditorGUILayout.Toggle("Show Path:", showPath);
                if (data.chapters != null && data.chapters.Count > 0)
                    showChaptersHorizontal = EditorGUILayout.Toggle("List chapters horizontal:", showChaptersHorizontal);
                showCopySceneNameButton = EditorGUILayout.Toggle("Show Copy Scene Button:", showCopySceneNameButton);
                showFirstScene = EditorGUILayout.Toggle("Show First Scene On Top:", showFirstScene);
            }
      

        }
        static Color settingsSelectedColor = new Color(0.85f, 0.85f, 0.85f);
        void ShowChapterHeaders()
        {
            if (chapterSettingsFoldout)
                GUI.color = settingsSelectedColor;
            if (GUILayout.Button("Chapter Definition"))
                SettingsFoldoutBtnPressed(1);
            GUI.color = Color.white;
            if (chapterSettingsFoldout)
            { 
                for (int i = 0; i < data.chapters.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    data.chapters[i] = EditorGUILayout.TextField(data.chapters[i]);
                    if (GUILayout.Button("X"))
                    {
                        data.chapters.RemoveAt(i);
                        data.chapterContent.RemoveAt(i);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                if (GUILayout.Button("Add Chapter"))
                {
                    data.chapters.Add("- - -");
                    if (data.chapterContent == null)
                        data.chapterContent = new List<string>();
                    data.chapterContent.Add("");
                    if (chapterContentIndices == null)
                        chapterContentIndices = new List<List<int>>();
                    chapterContentIndices.Add(new List<int>());
                    if (chapterContent == null)
                        chapterContent = new List<List<string>>();
                    chapterContent.Add(new List<string>());
                }
                EditorGUILayout.Separator(); EditorGUILayout.Separator();
            }
        }
        public string[] chapterSettingsCategories = { "By Scenes", "By Chapter" };
        public int chapterSettingsIndex = 0;
        void ShowChapterContentByChapter()
        {
            if (data.chapters == null || data.chapters.Count == 0)
                return;

            if (chapterContentIndices == null || chapterContentIndices.Count != data.chapterContent.Count)
                MatchChapterContentToIndices();
    
            bool closeAll = true;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
            for (int i = 0; i < data.chapters.Count; i++)
            {

                if (chapterContentIndices[i].Count != chapterContent[i].Count)
                    MatchChapterContentToIndices();

                if (EditorGUILayout.Foldout(data.foldoutChapter == i, data.chapters[i]))
                {
                    data.foldoutChapter = i;
                    closeAll = false;
                }

            

                if (data.foldoutChapter == i)
                {
                    if (data.chapterContent == null)
                        data.chapterContent = new List<string>();
                    if (data.chapterContent[i] == null)
                        data.chapterContent[i] = "";
                    for (int k = 0; k < chapterContent[i].Count; k++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        chapterContentIndices[i][k] = EditorGUILayout.Popup(chapterContentIndices[i][k], sceneNamesArray);
                        chapterContent[i][k] = sceneNames[chapterContentIndices[i][k]];
                        if (GUILayout.Button(arrowUpSymbol, GUILayout.Width(20)))
                        {
                            if (k > 0)
                            {
                                chapterContentIndices[i] = Swap(chapterContentIndices[i], k, k - 1);
                                chapterContent[i] = Swap(chapterContent[i], k, k - 1);
                                EditorGUILayout.EndHorizontal();
                                return;
                            }

                        }
                        if (GUILayout.Button(arrowDownSymbol, GUILayout.Width(20)))
                        {
                            if (k < chapterContentIndices[i].Count - 1)
                            {
                                chapterContentIndices[i] = Swap(chapterContentIndices[i], k, k + 1);
                                chapterContent[i] = Swap(chapterContent[i], k, k + 1);
                                EditorGUILayout.EndHorizontal();
                                return;
                            }

                        }
                        if (GUILayout.Button("X", GUILayout.Width(20)))
                        {
                            chapterContentIndices[i].RemoveAt(k);
                            chapterContent[i].RemoveAt(k);
                            EditorGUILayout.EndHorizontal();
                            return;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (GUILayout.Button("Add Scene"))
                    {
                        chapterContent[i].Add("");
                        chapterContentIndices[i].Add(0);
                    }

                }

            }
            if (closeAll)
                data.foldoutChapter = -1;
            EditorGUILayout.EndScrollView();
        }
        int selectedChapterToApplyToNULL;
        void ShowChapterContentByScenes()
        {
            if (data.chapters == null || data.chapters.Count == 0)
                return;

            if (chapterContentIndices == null || chapterContentIndices.Count != data.chapterContent.Count)
                MatchChapterContentToIndices();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            string[] chapterNames = data.chapters.ToArray();
            selectedChapterToApplyToNULL = EditorGUILayout.Popup("Default Chapter:", selectedChapterToApplyToNULL, chapterNames);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
        
        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Scene in Build Settings", headerStyle);
            EditorGUILayout.LabelField("Belongs to chapter", headerStyle);
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < scenes.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                if (showPath)
                    EditorGUILayout.LabelField(sceneNamesArray[i]);
                else
                    EditorGUILayout.LabelField(sceneNames[i]);
                int currentChapterIndex = -1;
                for (int j = 0; j < chapterNames.Length; j++)
                {
                    if (chapterContentIndices == null || chapterContentIndices.Count == 0)
                        break;
                    if (chapterContentIndices[j].Contains(i))
                    {
                        currentChapterIndex = j;
                        break;
                    }
                }
                if (currentChapterIndex == -1)
                {
                    if (GUILayout.Button("-ADD TO DEFAULT-"))
                    {
                        AddSceneIndexToChapter(i, selectedChapterToApplyToNULL);
                    }
                }
                else
                {
                    int oldIndex = currentChapterIndex;
                    currentChapterIndex = EditorGUILayout.Popup(currentChapterIndex, chapterNames);
                    if (oldIndex != currentChapterIndex)
                    {
                        AddSceneIndexToChapter(i, currentChapterIndex);
                        RemoveSceneIndexFromChapter(i, oldIndex);
                    }
                }


                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();
        }

        void SettingsFoldoutBtnPressed(int index)
        {
            switch(index)
            {
                case 0:
                    if (displaySettingsFoldout)
                        displaySettingsFoldout = false;
                    else
                    {
                        FoldInAllSettings();
                        displaySettingsFoldout = true;
                    }
                    break;
                case 1:
                    if (chapterSettingsFoldout)
                        chapterSettingsFoldout = false;
                    else
                    {
                        FoldInAllSettings();
                        chapterSettingsFoldout = true;
                    }
                    break;
                case 2:
                    if (chapterContentFoldout)
                        chapterContentFoldout = false;
                    else
                    {
                        FoldInAllSettings();
                        chapterContentFoldout = true;
                    }
                    break;
            }
        }

        void FoldInAllSettings()
        {
            displaySettingsFoldout = false;
            chapterSettingsFoldout = false;
            chapterContentFoldout = false;
        }

        #endregion

        #region SceneActions
        public void AddSceneIndexToChapter(int sceneIndex, int chapterIndex)
        {
            if (chapterContentIndices[chapterIndex].Contains(sceneIndex))
                return;

            chapterContentIndices[chapterIndex].Add(sceneIndex);
            chapterContent[chapterIndex].Add(sceneNames[sceneIndex]);

        }

        public void RemoveSceneIndexFromChapter(int sceneIndex, int chapterIndex)
        {
            if (!chapterContentIndices[chapterIndex].Contains(sceneIndex))
                return;
        
            chapterContentIndices[chapterIndex].Remove(sceneIndex);
            chapterContent[chapterIndex].Remove(sceneNames[sceneIndex]);
        }

        void ToggleSceneInBuildSettings(string path)
        {
            for(int i = 0; i < EditorBuildSettings.scenes.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path == path)
                {
                    EditorBuildSettingsScene[] scenesZ = EditorBuildSettings.scenes;
                    EditorBuildSettingsScene s = EditorBuildSettings.scenes[i];
                    s.enabled = !s.enabled;
                    scenesZ[i] = s;
                    EditorBuildSettings.scenes = scenesZ;
                    return;
                }
                
            }
            Debug.Log(path + " was not found");
        }

        public void OpenScene(string path)
	    {
		    if (EditorSceneManager.GetActiveScene ().isDirty) 
		    {
			    QSG_RequestWindow newWindow = PopupWindow.GetWindow<QSG_RequestWindow> (true, "Scene has been modified.");
			    newWindow.minSize = newWindow.maxSize = new Vector2 (380, 100);
			    newWindow.parent = this;
			    newWindow.newScenePath = path;
		    }
		    else
			    EditorApplication.OpenScene(path);
	    }

        void DisplayFirstScene()
        {
            EditorBuildSettingsScene scene = EditorBuildSettings.scenes[0];
            int last = scene.path.LastIndexOf("/");
            string text = scene.path.Substring(last + 1, scene.path.Length - 6 - last - 1);
            DisplayScene(scene, text);
        }

        void DisplaySceneWithFavOption(EditorBuildSettingsScene scene, string name)
        {
            DisplayScene(scene, name, true);
        }
        void DisplayScene(EditorBuildSettingsScene scene, string name, bool favOption = false, bool noBySceneFilter = false)
        {
            if (showOnlyFavs && !favs.Contains(scene.path) && !noBySceneFilter)
                return;

            if (!scene.enabled && hideInactiveScenes && !noBySceneFilter)
                return;

            if (!scene.enabled)
                GUI.color = new Color(0.8f, 0.8f, 0.8f);

            if (scene.path == EditorSceneManager.GetActiveScene().path)
                GUI.color = new Color(0.6f, 1f, 0.6f);

            if (scene.path == EditorSceneManager.GetActiveScene().path && !scene.enabled && !hideInactiveScenes)
                GUI.color = new Color(0.3f, 0.6f, 0.3f);


            EditorGUILayout.BeginHorizontal();
            if (!showPath)
            {
               // Debug.Log("Right Here");
                if (GUILayout.Button(name))
                {
                    if (controlInputToggled)
                        ToggleSceneInBuildSettings(scene.path);
                    else
                        OpenScene(scene.path);
                }
                
            }
            else
            {
                string path = scene.path.Substring(7, scene.path.Length - 13);
                if (GUILayout.Button(path))
                {
                    if (controlInputToggled)
                        ToggleSceneInBuildSettings(scene.path);
                    else
                        OpenScene(scene.path);
                }
                
            }
            GUI.color = Color.white;
            if (showCopySceneNameButton)
            {
                if (GUILayout.Button(copySymbol, GUILayout.Width(25)))
                {
                    EditorGUIUtility.systemCopyBuffer = name;
                }
            }

            if (favOption)
            {
                if (!favs.Contains(scene.path))
                {
                    if (GUILayout.Button(favOffSymbol, GUILayout.Width(25)))
                    {
                        favs.Add(scene.path);

                        EditorGUILayout.EndHorizontal();
                        Save();
                    }
                }
                else
                {
                    if (GUILayout.Button(favOnSymbol, GUILayout.Width(25)))
                    {
                        favs.Remove(scene.path);

                        EditorGUILayout.EndHorizontal();
                        Save();
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        #endregion

        #region RequesterReaction

        public void DontSaveScene(string _path)
	    {
		    EditorApplication.OpenScene(_path);
	    }

	    public void SaveScene(string _path)
	    {
		    EditorSceneManager.SaveScene (EditorSceneManager.GetActiveScene ());
		    EditorApplication.OpenScene(_path);
	    }

	    public void Save()
	    {


            //QSG_Data newData = new QSG_Data();

            data.favs = favs;
            data.chapterContent = new List<string>();
            data.showChaptersHorizontal = showChaptersHorizontal;
            data.chapterDisplayFoldout = chapterDisplayFoldout;
            data.showCopySceneNameButton = showCopySceneNameButton;
            data.showFirstScene = showFirstScene;
            data.hideInactiveScenes = hideInactiveScenes;
            data.searchIn = (int)searchIn;
            List<string> tempList = new List<string>();

            foreach (List<string> c in chapterContent)
               {
                tempList.Add(string.Join(",", c.ToArray()));
               }
            data.chapterContent = tempList;
              // ReplaceAsset<QSG_Data> ("Assets" + path + dataName + ".asset", newData);

            //data =  newData;

            EditorUtility.SetDirty(data);
            /* asset = data;
             Debug.Log(path);
             AssetDatabase.DeleteAsset (path);
             data
             string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path);
             AssetDatabase.CreateAsset (asset, assetPathAndName);
             */
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
        }

        #endregion

        #region Meta-Functions
        static GUIStyle headerStyle;

        static void SetStyles()
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.alignment = TextAnchor.MiddleCenter;
            headerStyle.fontStyle = FontStyle.Bold;
        }

        public void OnBuildSettingsChanged()
        {
            GetSceneNamesFromBuildSettings();
            MatchChapterContentToIndices();
        }

        static void LoadGraphics()
        {
            favOffSymbol = (Texture)Resources.Load("QSG_Star_No");
            favOnSymbol = (Texture)Resources.Load("QSG_Star_Yes");
            copySymbol = (Texture)Resources.Load("QSG_Copy_Symbol");
            arrowUpSymbol = (Texture)Resources.Load("QSG_Arrow_Up");
            arrowDownSymbol = (Texture)Resources.Load("QSG_Arrow_Down");
        }

        public void GetSceneNamesFromBuildSettings()
        {
            sceneNames = new List<string>();
            sceneNamesArray = new string[EditorBuildSettings.scenes.Length];
            for (int i = 0; i < sceneNamesArray.Length; i++)
            {
                if (EditorBuildSettings.scenes[i].path.Length <= 0)
                    continue;
                int last = EditorBuildSettings.scenes[i].path.LastIndexOf("/");
                string text = EditorBuildSettings.scenes[i].path.Substring(last + 1, EditorBuildSettings.scenes[i].path.Length - 6 - last - 1);
                sceneNamesArray[i] = text;
                sceneNames.Add(EditorBuildSettings.scenes[i].path);
            }
        }
        public string[] chapterParts;
        public void MatchChapterContentToIndices()
        {

            chapterContentIndices = new List<List<int>>();
            chapterContent = new List<List<string>>();
            if (data.chapters == null || data.chapters.Count == 0)
                return;
            foreach (string chapter in data.chapterContent)
            {
                chapterParts = chapter.Split(',');
                List<string> list = new List<string>();
                List<int> newList = new List<int>();
                foreach (string path in chapterParts)
                {
                    list.Add(path);
                    int index = sceneNames.IndexOf(path);
                    if (index == -1)
                        newList.Add(0);
                    else
                        newList.Add(index);
                }
                chapterContent.Add(list);
                chapterContentIndices.Add(newList);
            }
        }



        bool sceneListReactionInitialized = false;
        private void InitSceneListChangeReaction()
        {
            sceneListReactionInitialized = true;
            EditorBuildSettings.sceneListChanged += OnBuildSettingsChanged;
        }
        public static void CreateAsset<T> (string path) where T : ScriptableObject
	    {
		    T asset = ScriptableObject.CreateInstance<T> ();
	
		    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path);
            AssetDatabase.CreateAsset (asset, assetPathAndName);
		    AssetDatabase.SaveAssets ();

		    Selection.activeObject = asset;
	    }

	    public static void SaveA()
	    {
            EditorUtility.SetDirty(data);
           /* asset = data;
            Debug.Log(path);
            AssetDatabase.DeleteAsset (path);
            data
		    string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path);
		    AssetDatabase.CreateAsset (asset, assetPathAndName);
            */
		    AssetDatabase.SaveAssets ();
		    EditorUtility.FocusProjectWindow ();
	    }

        public static T[] SwapWithPredecessor<T>(T[] a, int i)
        {
            return Swap(a, i, i - 1);
        }

        public static T[] SwapWithSuccesor<T>(T[] a, int i)
        {
            return Swap(a, i, i + 1);
        }

        public static T[] Swap<T>(T[] a, int i1, int i2)
        {
            T t = a[i1];
            a[i1] = a[i2];
            a[i2] = t;
            return a;
        }

        public static List<T> SwapWithPredecessor<T>(List<T> a, int i)
        {
            return Swap(a,i, i - 1);
        }

        public static List<T> SwapWithSuccesor<T>(List<T> a, int i)
        {
            return Swap(a, i, i + 1);
        }

        public static List<T> Swap<T>(List<T> a, int i1, int i2)
        {
            T t = a[i1];
            a[i1] = a[i2];
            a[i2] = t;
            return a;
        }
        #endregion
    }
}