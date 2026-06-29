using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class LokaKitEditor_Static
{
	public delegate void OpenLokaKitEditor(int index);
	public static OpenLokaKitEditor LokaKitEditorOpenFct;

	public static void OpenWithLineID(int lineID)
	{
#if UNITY_EDITOR
		if (LokaKitEditor.window == null)
			LokaKitEditor.Open();
#endif
		if (LokaKitEditorOpenFct != null)
		{
			LokaKitEditorOpenFct(lineID);
		}
	}

	public static void ResetOpenFct()
	{
		LokaKitEditorOpenFct = null;
	}

}

public class LokaKitEditor : EditorWindow
{
	public static Vector2 version = new Vector2(1,8);
	[SerializeField]
	public static LokaKit manager;
	string entryNumber;
	LokaKitEntry currentSpeech;

	public bool largeDisplay;
	public bool formatDisplay;
	public enum DisplayStyle { AllSmall, OneBig, TwoBig}
	public DisplayStyle displayStyle;
	public int language;
	
	string[] languages = new string[2] { "Deutsch", "English" };

	public static LokaKitEditor window;
	public int overviewWidth = 300;

	private const string lastLokaKitKey = "SimpleLoka_LastEdited";
	public static string lastLokaKitName;

	[MenuItem("Tools/Course/Localization/Editor")]
	public static void Open()
	{

		window = (LokaKitEditor)EditorWindow.GetWindow(typeof(LokaKitEditor));
		LoadLastLokaKitFromEditorPrefs();
		Init();
	}

	static void LoadLastLokaKitFromEditorPrefs()
    {
		string lastLokaKitName = EditorPrefs.GetString(lastLokaKitKey, "");
		if(lastLokaKitName.Trim() != "")
        {
			string[] guids = AssetDatabase.FindAssets($"{lastLokaKitName} t:{typeof(LokaKit).Name}");
			string path = AssetDatabase.GUIDToAssetPath(guids[0]);
			manager = AssetDatabase.LoadAssetAtPath<LokaKit>(path);
			
        }
	}

	// Update is called once per frame
	static void Init()
	{
		//manager = AC.KickStarter.speechManager;
		LokaKitEditor_Static.ResetOpenFct();
		LokaKitEditor_Static.LokaKitEditorOpenFct += window.OpenWithIndex;
	}

	void CheckExternOpenFct()
	{
		window = this;
		if (LokaKitEditor_Static.LokaKitEditorOpenFct == null)
		{

			LokaKitEditor_Static.ResetOpenFct();
			LokaKitEditor_Static.LokaKitEditorOpenFct += window.OpenWithIndex;
		}
	}

	public void OpenWithIndex(int index)
	{
		entryNumber = index.ToString();
		window.LoadFromID(index);
		EditorWindow.FocusWindowIfItsOpen<LokaKitEditor>();
	}

	private void OnGUI()
    {
		manager = (LokaKit)EditorGUILayout.ObjectField("Loka Kit", manager, typeof(LokaKit), false);
		if (manager == null)
		{
			entryNumber = "";
			LoadLastLokaKitFromEditorPrefs();
			if (manager == null)
				return;
		}
		if(manager.name != lastLokaKitName)
        {
			lastLokaKitName = manager.name;
			EditorPrefs.SetString(lastLokaKitKey, lastLokaKitName);
		}

        if (manager.entries == null)
        {
			manager.entries = new List<LokaKitEntry>();
        }

		if(manager.entries.Count == 0)
        {
			CreatenewEntry();
			return;
		}
		if(currentSpeech == null)
        {
			LoadFromID(0);
			return;
        }
		DrawMainArea();

		DrawOverviewArea();

		GUILayout.BeginArea(new Rect(0, window.position.height - 25, window.position.width, 25));
		GUILayout.Label("v" + version.x.ToString() + "." + version.y.ToString());
		GUILayout.EndArea();
	}

	void DrawMainArea()
    {
		GUILayout.BeginArea(new Rect(0, 30, this.position.width - overviewWidth, this.position.height-30));
		

		CheckExternOpenFct();
		entryNumber = EditorGUILayout.TextField("Entry-ID:", entryNumber);
	/*	if(currentSpeech == null)
        {
			LoadFromID("0");
			return;
        }*/
		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Load from ID"))
		{
			LoadFromID(entryNumber);
		}
		if (GUILayout.Button("Load from Name"))
		{
			LoadFromName(entryNumber);
		}



		if (GUILayout.Button("Create New Entry"))
		{
			CreatenewEntry();
		}
		if (currentSpeech != null && GUILayout.Button("<"))
		{
			LoadFromID(currentSpeech.lineID - 1);
			entryNumber = (currentSpeech.lineID).ToString();
		}
		if (currentSpeech != null && GUILayout.Button(">"))
		{
			LoadFromID(currentSpeech.lineID + 1);
			entryNumber = (currentSpeech.lineID).ToString();
		}
		EditorGUILayout.EndHorizontal();
		if (currentSpeech == null)
			return;

		displayStyle = (DisplayStyle)EditorGUILayout.EnumPopup("DisplayStyle:", displayStyle);
		largeDisplay = EditorGUILayout.Toggle("Display One Language big", largeDisplay);
		formatDisplay = EditorGUILayout.Toggle("Display formatted (no edit)", formatDisplay);
		if (currentSpeech != null)
		{
			if(formatDisplay)
				DisplayCurrentSpeechLineFormatted();
			else if (largeDisplay)
				DisplayCurrentSpeechLineLarge();
			else
				DisplayCurrentSpeechLine();
		}
		GUILayout.EndArea();
	}

	void CreatenewEntry()
    {
		GameObject obj = null;
		try
		{
			if (Selection.activeObject != null)
				obj = (GameObject)Selection.activeObject;
		}
		catch (System.Exception e)
		{

		}


		int nextLineID = manager.nextLineID;
		manager.nextLineID++;
		string newName = "";
		string startContent = "";
		if (obj != null)
		{
			newName = obj.name;
			if (obj.GetComponent<UnityEngine.UI.Text>())
				startContent = obj.GetComponent<UnityEngine.UI.Text>().text;
		}

		LokaKitEntry entry = new LokaKitEntry(nextLineID, startContent, "", newName, 2, 0, true);
		manager.entries.Add(entry);
		entryNumber = nextLineID.ToString();
		LoadFromID(nextLineID);
		//	Hotspot hs = obj.GetComponent<Hotspot>();
		//	hs.lineID = nextLineID;
		//	EditorUtility.SetDirty(hs);
	}

	LokaFilter filter;
	bool filterFoldout;
	void DrawOverviewArea()
    {
		GUILayout.BeginArea(new Rect(this.position.width - overviewWidth, 30, overviewWidth, this.position.height-30));

		filterFoldout = EditorGUILayout.Foldout(filterFoldout, "Filter");
		if (filter == null)
			filter = new LokaFilter();
		if (filterFoldout)
        {
			filter.nameFilter = EditorGUILayout.TextField(filter.nameFilter);
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Type:", GUILayout.Width(95));
			filter.type = EditorGUILayout.Popup(filter.type, manager.GetTypeListWithZero(), GUILayout.Width(95));
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("Tag:", GUILayout.Width(95));
			filter.tag = EditorGUILayout.Popup(filter.tag, manager.GetTagListWithZero(), GUILayout.Width(95));
			EditorGUILayout.EndHorizontal();
		}

		
		allEntriesScrollView = GUILayout.BeginScrollView(allEntriesScrollView);
		int entryToLoadAfterwards = -1;
		for (int i = 0; i < manager.entries.Count; i++)
		{
			LokaKitEntry entry = manager.entries[i];
			if (filterFoldout && !filter.CheckLokaKitEntry(entry))
				continue;
			EditorGUILayout.BeginHorizontal();
			if(currentSpeech != null && entry.lineID == currentSpeech.lineID)
            {
				GUI.color = Color.green;
            }
			if (GUILayout.Button(new GUIContent(entry.token, entry.translationText[0]), GUILayout.Width(200)))
			{
				entryToLoadAfterwards = entry.lineID;


			}
			
			if (GUILayout.Button(EditorGUIUtility.IconContent("Clipboard").image, GUILayout.Width(25)))
			{
				EditorGUIUtility.systemCopyBuffer = entry.token;
			}

			GUI.color = Color.white;
			EditorGUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();


		GUILayout.EndArea();
		if (entryToLoadAfterwards != -1)
			LoadFromID(entryToLoadAfterwards);
	}

	Vector2 allEntriesScrollView;

	void DisplayCurrentSpeechLineFormatted()
    {
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("ID: " + currentSpeech.lineID);
		EditorGUILayout.BeginHorizontal();
		currentSpeech.token = EditorGUILayout.TextField("Text: ", currentSpeech.token);
		//currentSpeech.textType = (AC_TextType)EditorGUILayout.EnumPopup("Type: ", currentSpeech.textType);
		if (GUILayout.Button("C", GUILayout.Width(25)))
		{
			EditorGUIUtility.systemCopyBuffer = currentSpeech.token;

		}
		EditorGUILayout.EndHorizontal();
		language = EditorGUILayout.Popup(language, languages);
		string formattedString = currentSpeech.translationText[language].Replace("§§", "\n");

		GUIStyle style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.UpperLeft };

		EditorGUILayout.LabelField(formattedString, style, GUILayout.Height(this.position.height - 220));

	}

	void DisplayCurrentSpeechLineLarge()
	{
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("ID: " + currentSpeech.lineID);
		EditorGUILayout.BeginHorizontal();
		currentSpeech.token = EditorGUILayout.TextField("Text: ", currentSpeech.token);
		//currentSpeech.textType = (AC_TextType)EditorGUILayout.EnumPopup("Type: ", currentSpeech.textType);
		if(GUILayout.Button("C", GUILayout.Width(25)))
        {
			EditorGUIUtility.systemCopyBuffer = currentSpeech.token;

		}
		EditorGUILayout.EndHorizontal();
		language = EditorGUILayout.Popup(language, languages);
		EditorGUI.BeginChangeCheck();
		currentSpeech.translationText[language] = EditorGUILayout.TextArea(currentSpeech.translationText[language], GUILayout.Height(this.position.height-300));
		if (EditorGUI.EndChangeCheck())
		{
			currentSpeech.lastChangedInVersion = Application.version;
		}
		if (currentSpeech != null && GUILayout.Button("Save", GUILayout.Height(50)))
		{
			SaveManagerWithCurrentSpeech();
		}
	}
	
	void DisplayCurrentSpeechLine()
    {
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("ID: " + currentSpeech.lineID);
		EditorGUILayout.BeginHorizontal();
		currentSpeech.token = EditorGUILayout.TextField("Text: ", currentSpeech.token);
		//currentSpeech.textType = (AC_TextType)EditorGUILayout.EnumPopup("Type: ", currentSpeech.textType);
		if (GUILayout.Button("C", GUILayout.Width(25)))
		{
			EditorGUIUtility.systemCopyBuffer = currentSpeech.token;

		}
		EditorGUILayout.EndHorizontal();
		//currentSpeech.token = EditorGUILayout.TextField("Text: ", currentSpeech.token);
		currentSpeech.textType = EditorGUILayout.Popup("Type: ", currentSpeech.textType, manager.types.ToArray());
		currentSpeech.tagMask = EditorGUILayout.MaskField("Tags:", currentSpeech.tagMask, manager.tagList.ToArray());
		for (int i = 0; i < currentSpeech.translationText.Count; i++)
        {
			EditorGUI.BeginChangeCheck();
			currentSpeech.translationText[i] = EditorGUILayout.TextField("Translation #" + i +" : ", currentSpeech.translationText[i]);
			if (EditorGUI.EndChangeCheck())
			{
				currentSpeech.lastChangedInVersion = Application.version;
			}
		/*	if (currentSpeech.customTranslationAudioClips == null)
				currentSpeech.customTranslationAudioClips = new List<AudioClip>();
			if(currentSpeech.customTranslationAudioClips.Count == 0)
            {
				currentSpeech.customTranslationAudioClips.Add(null);
				currentSpeech.customTranslationAudioClips.Add(null);

			}
			if(currentSpeech.customTranslationAudioClips.Count > i)
				currentSpeech.customTranslationAudioClips[i] = (AudioClip)EditorGUILayout.ObjectField("Audio #" + i + " : ", currentSpeech.customTranslationAudioClips[i], typeof(AudioClip), false);*/
		}

		if(currentSpeech != null && GUILayout.Button("Save", GUILayout.Height(50)))
        {
			SaveManagerWithCurrentSpeech();
		}
    }

	void SaveManagerWithCurrentSpeech()
    {
		int idIntern = -1;
		for(int i = 0; i < manager.entries.Count;i++)
        {
			if(manager.entries[i].lineID == currentSpeech.lineID)
            {
				if(idIntern > -1)
                {
					Debug.Log("Double Found in " + idIntern + " and " + i);
                }
				else
				idIntern = i;
				//break;
			}
        }
		if(idIntern != -1)
        {
			manager.entries[idIntern] = currentSpeech;
			//KickStarter.speechManager = manager;
			UnityEditor.EditorUtility.SetDirty(manager);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();

		}
    }

	void LoadFromID(string id_string)
    {
		int id = -1;
		if (int.TryParse(entryNumber, out id))
		{
			LoadFromID(id);
		}
	}

	void LoadFromName(string id_string)
	{
		foreach(LokaKitEntry line in manager.entries)
        {
			if(line.token.ToLower() == id_string.ToLower())
            {
				currentSpeech = line;
				break;
			}
        }
		if (currentSpeech == null)
			Debug.Log("Line " + id_string + " not found");
	}

	void LoadFromID(int id)
    {
		currentSpeech = manager.GetLine(id);
		if (currentSpeech == null)
			Debug.Log("ID " + id + " not found");
		else
        {
			//extension = new SpeechLineExtension(-1);
			//extension = GetSpeechLineExtension(currentSpeech.lineID);
			GUI.FocusControl(null);
		}
			
		
	}

	
}

public class LokaFilter
{
	public string nameFilter;
	public int type;
	public int tag;
	public LokaFilter()
    {
		Reset();

	}

	public bool CheckLokaKitEntry(LokaKitEntry entry)
    {

		if(nameFilter.Trim() != "")
        {
			if (!NameContainsFilter(entry.token))
				return false;
			
        }
		if (type > 0)
		{
			if (type - 1 != entry.textType)
				return false;
		}
		if (tag > 0)
		{
			int tagBit = tag - 1;
			if (entry.tagMask != (entry.tagMask | (1 << tagBit)))
				return false;
		}
		return true;

	}

	public void Reset()
    {
		nameFilter = "";
		type = 0;
		tag = 0;
	}

	bool NameContainsFilter(string name)
    {
		return name.ToLower().Contains(nameFilter.ToLower());
    }
}