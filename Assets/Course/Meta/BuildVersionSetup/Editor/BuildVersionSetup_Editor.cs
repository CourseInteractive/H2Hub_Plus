using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using BuildSetup;

public class BuildVersionSetup_Editor : EditorWindow
{
	static Vector3 versionNumber = new Vector3(1,1,0);
	static string dataName = "BuildSetup";
	static BuildVersionSetup savedData;

	public static string savePath = "/Course/BuildVersionSetup/Resources/";
	public static string steamAppID_directory = "/Plugins/Steamworks.NET/redist/";
	public enum View { Main, Settings}
	public View currentView;

	private SteamBuildKind selectedKind;

	[MenuItem("File/[COURSE] Build Version Setup", false, 210)]
	static void Open()
	{
		BuildVersionSetup_Editor window = (BuildVersionSetup_Editor)EditorWindow.GetWindow(typeof(BuildVersionSetup_Editor));
		window.titleContent = new GUIContent("Build Version Setup");
		Init();
	}

	static void Init()
	{
		savedData = (BuildVersionSetup)Resources.Load(dataName);
		if (savedData == null)
		{
			if (!Directory.Exists(Application.dataPath + savePath))
				Directory.CreateDirectory(Application.dataPath + savePath);
			CreateAsset<BuildVersionSetup>("Assets" + savePath + dataName + ".asset");
			savedData = (BuildVersionSetup)Resources.Load(dataName);
		}
		if (savedData.versionInfos == null)
			savedData.versionInfos = new BuildVersionInfo[4];
	}

	void OnGUI()
	{
		if (savedData == null)
			Init();
		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Select View", GUILayout.Width(150));
		currentView = (View)GUILayout.Toolbar(
			(int)currentView,
			System.Enum.GetNames(typeof(View))
			);
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		/*	currentView = (View)EditorGUILayout.EnumPopup("View:", currentView);*/
		switch (currentView)
        {
			case View.Main:
				DrawMainGUI();
				break;
			case View.Settings:
				DrawSettingsGUI();
				break;
		}
		DrawVersionNumber();
	}

	void DrawVersionNumber()
    {
		string label = "v" + versionNumber.x + "." + versionNumber.y + "." + versionNumber.z;
		GUIContent content = new GUIContent(label);
		Vector2 size = EditorStyles.miniLabel.CalcSize(content);

		Rect rect = new Rect(
			position.width - size.x - 4,
			position.height - size.y - 4,
			size.x,
			size.y
		);

		GUI.Label(rect, content, EditorStyles.miniLabel);
	}
	// Aktuelle Versionsnummer als String (wird dynamisch angepasst)
	private string version;
	void DrawMainGUI()
    {
		SteamBuildKind formerKind = savedData.activeBuildKind;
		savedData.activeBuildKind = (SteamBuildKind)EditorGUILayout.EnumPopup("Active Version:", savedData.activeBuildKind);

		EditorGUILayout.Space();
		if (savedData.activeBuildKind != formerKind)
        {
			ChangeBuildKind();
        }
		EditorGUILayout.LabelField(savedData.activeBuildKind.ToString());
		if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android 
			&& PlayerSettings.Android.useCustomKeystore
			&& PlayerSettings.Android.keyaliasPass == "")
        {
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.HelpBox("No keystore password set.", MessageType.Error);
			if (GUILayout.Button("Player Settings", GUILayout.Height(39)))
			{
				SettingsService.OpenProjectSettings("Project/Player");
			}
			EditorGUILayout.EndHorizontal();
		}

		EditorGUILayout.BeginHorizontal();
		string id = savedData.GetCurrentAppID();
		if(GUILayout.Button("Copy ID"))
        {
			EditorGUIUtility.systemCopyBuffer = id;
        }
		EditorGUILayout.LabelField(id.ToString());
		EditorGUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		version = PlayerSettings.bundleVersion;
		GUILayout.Label("Aktuelle Versionsnummer: " + version, EditorStyles.label);
		
		// Die Version in ihre Teile zerlegen
		string[] versionParts = version.Split('.');
		if (versionParts.Length == 4)
		{
			int major = int.Parse(versionParts[0]);
			int minor = int.Parse(versionParts[1]);
			int patch = int.Parse(versionParts[2]);
			int build = int.Parse(versionParts[3]);

		

			// Button für dritte Stelle der Versionsnummer (Patch)
			if (GUILayout.Button("+1 Patch"))
			{
				patch++;
				version = $"{major}.{minor}.{patch}.{build:D3}"; // Format mit führenden Nullen
			}

			// Button für vierte Stelle der Versionsnummer (Build)
			if (GUILayout.Button("+1 Build"))
			{
				build++;
				version = $"{major}.{minor}.{patch}.{build:D3}"; // Format mit führenden Nullen
			}
			PlayerSettings.bundleVersion = version;

		}
		else
		{
			GUILayout.Label("Fehlerhafte Versionsnummer.", EditorStyles.boldLabel);
		}

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		string buildNummer = GetBuildNumber();
		if (buildNummer.Trim() != "")
			GUILayout.Label("Aktuelle Build-Nummer: " + buildNummer, EditorStyles.label, GUILayout.Width(200));
		if(GUILayout.Button("+", GUILayout.Width(30)))
        {
			IncreaseBuildNUmber();
        }
		GUILayout.EndHorizontal();
		EditorGUILayout.Space(); EditorGUILayout.Space();

		savedData.mainPlayMode = (MainPlayModeType)EditorGUILayout.EnumPopup("PlayMode:", savedData.mainPlayMode);
		switch(savedData.mainPlayMode)
        {
			case MainPlayModeType.Test:
				savedData.testModeType = (TestModeType)EditorGUILayout.EnumPopup("Test Submode:", savedData.testModeType);
				break;
			case MainPlayModeType.Marketing:
				savedData.marketingModeType = (MarketingModeType)EditorGUILayout.EnumPopup("Marketing Submode:", savedData.marketingModeType);
				break;
			case MainPlayModeType.Custom:
				for(int i = 0; i < savedData.customSetups.Length; i++)
                {
					savedData.customSetups[i].active = EditorGUILayout.ToggleLeft(savedData.customSetups[i].name, savedData.customSetups[i].active);
				}
				break;
		}
		EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
		if (GUILayout.Button("Save"))
		{
			ReplaceAsset("Assets" + savePath + dataName + ".asset");

		}
	}

	void DrawSettingsGUI()
	{
		EditorGUILayout.LabelField("Select Version to edit");
		selectedKind = (SteamBuildKind)GUILayout.Toolbar(
			(int)selectedKind,
			System.Enum.GetNames(typeof(SteamBuildKind))
			);

		
		//savedData.steam_releaseGameID = EditorGUILayout.IntField("Main Game ID:", savedData.steam_releaseGameID);
		//savedData.steam_playTestGameID = EditorGUILayout.IntField("Play Test ID:", savedData.steam_playTestGameID);
		//savedData.steam_demoGameID = EditorGUILayout.IntField("Demo ID:", savedData.steam_demoGameID);
		//Editor
		// In OnGUI, nach der Toolbar:
		EditorGUILayout.Space();

		if (savedData.versionInfos == null || savedData.versionInfos.Length < System.Enum.GetValues(typeof(SteamBuildKind)).Length)
		{
			EditorGUILayout.HelpBox("versionInfos array is not initialized!", MessageType.Error);
			savedData.versionInfos = new BuildVersionInfo[4];
		}

		BuildVersionInfo info = savedData.versionInfos[(int)selectedKind];

		EditorGUILayout.LabelField(selectedKind.ToString(), EditorStyles.boldLabel);
		EditorGUILayout.BeginVertical();
		if(selectedKind != SteamBuildKind.MAIN_GAME)
			info.useMainInfo = EditorGUILayout.Toggle("Use Main Info", info.useMainInfo);
		if(!info.useMainInfo)
        {
			info.title = EditorGUILayout.TextField("Game Title", info.title);
			info.identifier = EditorGUILayout.TextField("Identifier (iOS, And., VR)", info.identifier);
			info.steam_GameID = EditorGUILayout.IntField("Steam Game ID", info.steam_GameID);
			if(EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
            {
				info.splitBinary = EditorGUILayout.Toggle("Split Build (obb)", info.splitBinary);
				info.specialKeystore = EditorGUILayout.Toggle("Use Keystore", info.specialKeystore);
				
			}
		}
		EditorGUILayout.EndVertical();

		EditorGUILayout.LabelField("iOS");
		savedData.iOS_appID = EditorGUILayout.IntField("iOS App ID (Not identifier):", savedData.iOS_appID);
		EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();
		if (GUILayout.Button("Save"))
        {
			ReplaceAsset("Assets" + savePath + dataName + ".asset");
			ChangeBuildKind();
		}
	}

	string GetBuildNumber()
	{
		switch (EditorUserBuildSettings.activeBuildTarget)
		{
			case BuildTarget.iOS:
				return PlayerSettings.iOS.buildNumber;

			case BuildTarget.Android:
				return PlayerSettings.Android.bundleVersionCode.ToString();

			case BuildTarget.StandaloneOSX:
				return PlayerSettings.macOS.buildNumber;
			default:
				return PlayerSettings.macOS.buildNumber;
		}
	}

	void IncreaseBuildNUmber()
    {
		PlayerSettings.iOS.buildNumber = (int.Parse(PlayerSettings.iOS.buildNumber) + 1).ToString();
		PlayerSettings.Android.bundleVersionCode = PlayerSettings.Android.bundleVersionCode + 1;
		PlayerSettings.macOS.buildNumber = (int.Parse(PlayerSettings.macOS.buildNumber) + 1).ToString();

	}
	void ChangeBuildKind()
    {
		ReplaceSteamID();
		ReplaceApplicationData();
    }

	void ReplaceSteamID()
    {
		string id = "";
		switch(savedData.activeBuildKind)
        {
			case SteamBuildKind.MAIN_GAME:
				id = savedData.versionInfos[0].steam_GameID.ToString();
				break;
			default:
				BuildVersionInfo info = savedData.versionInfos[(int)savedData.activeBuildKind];
				if(info.useMainInfo)
					id = savedData.versionInfos[0].steam_GameID.ToString();
				else
					id = info.steam_GameID.ToString();
				break;
		}
		TextAsset appIDFile = new TextAsset(id);
		string directory = "Assets" + steamAppID_directory;
		if (!Directory.Exists(Application.dataPath + steamAppID_directory))
			Directory.CreateDirectory(Application.dataPath + steamAppID_directory);
	//	AssetDatabase.CreateAsset(appIDFile, );

		File.WriteAllText(Application.dataPath + steamAppID_directory + "steam_appid.txt", id);

	}

	void ReplaceApplicationData()
	{
		string title = "";
		string identifier = "";
		bool splitBinary;
		bool useKeystore;
		switch (savedData.activeBuildKind)
		{
			case SteamBuildKind.MAIN_GAME:
				title = savedData.versionInfos[0].title;
				identifier = savedData.versionInfos[0].identifier;
				splitBinary = savedData.versionInfos[0].splitBinary;
				useKeystore = savedData.versionInfos[0].specialKeystore;
				break;
			default:
				BuildVersionInfo info = savedData.versionInfos[(int)savedData.activeBuildKind];
				if (info.useMainInfo)
                {

					title = savedData.versionInfos[0].title;
					identifier = savedData.versionInfos[0].identifier;
					splitBinary = savedData.versionInfos[0].splitBinary;
					useKeystore = savedData.versionInfos[0].specialKeystore;
				}
				else
                {
					title = info.title;
					identifier = info.identifier;
					splitBinary = info.splitBinary;
					useKeystore = info.specialKeystore;
				}
				break;
		}
		PlayerSettings.Android.splitApplicationBinary = splitBinary;
		PlayerSettings.Android.useCustomKeystore = useKeystore;
		PlayerSettings.productName = title;
		PlayerSettings.applicationIdentifier = identifier;

	}

	public static void CreateAsset<T>(string path) where T : ScriptableObject
	{
		T asset = ScriptableObject.CreateInstance<T>();

		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		AssetDatabase.SaveAssets();

		Selection.activeObject = asset;
	}

	public static void ReplaceAsset(string path)
	{
		EditorUtility.SetDirty(savedData);

		/*
		BuildVersionSetup asset = ScriptableObject.CreateInstance<BuildVersionSetup>();
		asset.steam_releaseGameID = savedData.steam_releaseGameID;
		asset.steam_demoGameID = savedData.steam_demoGameID;
		asset.steam_playTestGameID = savedData.steam_playTestGameID;
			asset.iOS_appID = savedData.iOS_appID;
		asset.activeBuildKind = savedData.activeBuildKind;
		asset.mainPlayMode = savedData.mainPlayMode;
		asset.testModeType = savedData.testModeType;
		asset.marketingModeType = savedData.marketingModeType;
		asset.customSetups = savedData.customSetups;
		AssetDatabase.DeleteAsset(path);
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
		Debug.Log(assetPathAndName);
		AssetDatabase.CreateAsset(asset, assetPathAndName);
		*/
		AssetDatabase.SaveAssets();
		EditorUtility.FocusProjectWindow();
	}
}
