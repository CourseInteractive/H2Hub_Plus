using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

/*****
 * VALUR - ConsoleEditor ## Version 0.5
 * Last Change: 25.10.2015
 * By: Oli
 * 
 * Editor um die Grund-Einstellungen von VALUR zu verändern.
 * Abgesehen von der "Deaktivierung" sind diese Einstellungen nicht zwingend zur Nutzung der Console nötig
 *****/

namespace VALUR
{

public class ConsoleEditor : EditorWindow 
{

	// Die gespeicherten Daten
	private static ConsoleEditorData savedData;

	// DateiName bei der Speicherung
	public static string pathToResource = "/Course/VALUR/Resources/";
	public static string pathInResource = "GamePrefs/";
	private static string dataName = "ConsoleData";
	public static Rect windowSize;

	// Die speziell beschriebenen Topics
	public List<ConsoleTopic> topics;

	// Die Grundeinstellungen von LogTokens
	public List<LogEntry> logEntrys;

	// Generelle Deaktivierung von VALUR
	public bool deactivated;
	public bool activateMessages;
	public bool loaded = false;

	// Öffnen des OptionsEditors
	[MenuItem ("Tools/Course/VALUR/ConsoleSettings")]
	static void Open()
	{
			ConsoleEditor window = (ConsoleEditor)EditorWindow.GetWindow (typeof (ConsoleEditor));
		windowSize = window.position;
		Init ();
	}

	// Initislisieren
	static void Init()
	{

		// Laden der Daten (wenn nicht vorhanden -> Anlegen der Datei)
		savedData = (ConsoleEditorData)Resources.Load (pathInResource + dataName);
		if (savedData == null)
		{
			Debug.Log (Application.dataPath + pathToResource + pathInResource);
			if(!Directory.Exists(Application.dataPath + pathToResource + pathInResource))
				Directory.CreateDirectory(Application.dataPath + pathToResource + pathInResource);
			CreateAsset<ConsoleEditorData> ("Assets/" + pathToResource + pathInResource + dataName + ".asset");
		}
		savedData = (ConsoleEditorData)Resources.Load (pathInResource + dataName);
		
	}

	void Load()
	{
		
			// laden der Daten in dieses Skript
			// ( Wenn nicht vorhanden , initialisieren der Listen)
		topics = savedData.topics;
			if(topics == null)
				topics = new List<ConsoleTopic>();
			if(topics.Count == 0)
			{
				topics.Add(new ConsoleTopic());

			}

			logEntrys = savedData.logEntrys;

			if(logEntrys == null)
				logEntrys = new List<LogEntry>();
			if(logEntrys.Count == 0)
			{
				logEntrys.Add(new LogEntry("NEW", false));
				
			}
			deactivated = savedData.deactivated;
			activateMessages = savedData.activateMessages;
			loaded = true;

	}


	void OnGUI()
	{
		if (savedData == null)
		{
			Init ();
			return;
		}
		
		if(loaded == false || topics == null || topics.Count == 0)
		{
			Load();
			return;
		}

		// deaktivierung
		deactivated = EditorGUILayout.Toggle( "Deactivated", deactivated);
			activateMessages = EditorGUILayout.Toggle("Messages active?", activateMessages);
			// Keine Anzeige von Werten, wenn deaktiviert -> Unterstreicht die Deaktivierung
			if (!deactivated)
		{

		// Topics zur Veränderung darstellen
		for(int i = 0; i < topics.Count; i++)
		{

			//Name ist wichtig für den Aufruf im Skript
			EditorGUILayout.BeginHorizontal ();
			topics[i].token = EditorGUILayout.TextField(topics[i].token);
			topics[i].name = EditorGUILayout.TextField(topics[i].name);

			EditorGUILayout.EndHorizontal ();

		}

		// Ein neues Topic hinzufügen
		if (GUILayout.Button ("New Topic"))
		{
			topics.Add(new ConsoleTopic());
		}

	
		if(logEntrys != null && logEntrys.Count > 0)
		{

		// Alle LogToken und ihre Grundeinstellung anzeigen
		foreach(LogEntry entry in logEntrys)
		{
			//Name ist wichtig für den Aufruf im Skript
			EditorGUILayout.BeginHorizontal ();
	
			entry.token = EditorGUILayout.TextField(entry.token);
			entry.save = EditorGUILayout.Toggle(entry.save);
			entry.show = EditorGUILayout.Toggle(entry.show);
	

			EditorGUILayout.EndHorizontal ();
			
		}



		}

		// Neues LogToken anlegen
		if (GUILayout.Button ("New LogEntry"))
		{
			if(logEntrys == null)
						logEntrys = new List<LogEntry>();

			logEntrys.Add(new LogEntry("NEW", false));
		}

		}

		// Gesamte Daten speichern
		if (GUILayout.Button ("Save"))
		{
				SaveData();
		}

	}

		// Speicher Funktion
		void SaveData()
		{
			
			ConsoleEditorData newData = new ConsoleEditorData();

			// daten in neues ScriptableObjekt übertragen
			newData.topics = topics;
			newData.logEntrys = logEntrys;

			newData.deactivated = deactivated;
			newData.activateMessages = activateMessages;
			// ScriptbleObjekt durch neues ersetzen
			ReplaceAsset<ConsoleEditorData>("Assets/" + pathToResource + pathInResource + dataName + ".asset", newData);
			
			savedData =  newData;

			// Erneutes Laden der Daten um den neuesten Stand wiederzugeben
			Load();
		}

		public static void CreateAsset<T>(string path) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();

			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(asset, assetPathAndName);
			AssetDatabase.SaveAssets();

			Selection.activeObject = asset;
		}

		public static void ReplaceAsset<T>(string path, T data) where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T>();
			asset = data;
			Debug.Log(path);
			AssetDatabase.DeleteAsset(path);
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
			AssetDatabase.CreateAsset(asset, assetPathAndName);

			AssetDatabase.SaveAssets();
			EditorUtility.FocusProjectWindow();
		}
	}


}