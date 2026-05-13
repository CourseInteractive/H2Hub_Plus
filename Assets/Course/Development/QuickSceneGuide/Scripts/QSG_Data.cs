using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//-----------------------------------------------------------------------------
// Copyright 2016 Oliver Ziegler - Course Interactive.  All rights reserverd.
//-----------------------------------------------------------------------------

public class QSG_Data : ScriptableObject {

    public bool filterFoldout;
    public bool showOnlyFavs = false;
    public bool hideInactiveScenes = false;
    public bool showPath = false;
    public int searchIn = 0;
    public bool settingsOpened = false;
    public bool hideMainToolbar;
    public bool displayInChapters;
    public string searchInput = "";
    public int foldoutChapter;
    public bool mainSettingsFoldout;
    public bool chapterSettingsFoldout;
    public bool chapterContentFoldout;
    public bool showChaptersHorizontal;
    public bool[] chapterDisplayFoldout;
    public bool showCopySceneNameButton;
    public bool showFirstScene;
    public List<string> chapters;
    public List<string> chapterContent;

    public List<string> favs;
}
