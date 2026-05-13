using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****
 * VALUR - Data ## Version 1.0
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * VALUR-MainScript
 * Global erreichbares Script, das die Befehle an VALUR entgegennimmt und
 * die wichtigsten Daten speichert.
 *****/

namespace VALUR
{

	public static class Data
	{
		// Generelle Deaktivierung der Console!
		public static bool deactivated;
		// Nachrichten aktiv!
		public static bool activateMessages;
		// Generelle Aktivierung von Speichern und Anzeigen von LogNachrichten
		public static bool logsActivated;
		// Gespeicherte Einstellungen
		private static ConsoleEditorData savedData;

		// Derzeitiger Zustand
		public static State state;

		// Manager
		public static DisplayManager displayManager;
		public static LogManager logManager;

		// FetchData-Funktionen anhand ihres Topic-Tokens
		public static Dictionary<string, DataFetch> fetchDataTasks = new Dictionary<string, DataFetch>();

		// SceneSpecific FetchData-Funktionen anhand ihres Topic-Tokens
		public static Dictionary<string, DataFetch> sceneSpecificFetchDataTasks = new Dictionary<string, DataFetch>();

		// Die vorgebrachten Daten, nach dem Fetching
		public static List<ConsoleData> fetchedData;

		// Gespeicherte Einstellungen von Topics
		public static Dictionary<string, ConsoleTopic> topics;

		// Die verschiedenen LogTokens und ihre Einstellungen
		public static Dictionary<string, LogEntry> logEntrys;
		//public static List<string> individualEntrys;

		// Timestamp zu dem die Application gestartet wurde
		public static long startTimestamp;

		// Initialisieren von VALUR (nur wenn nicht deaktiviert)
		public static void Initialize(ConsoleEditorData data)
		{
			savedData = data;
			activateMessages = savedData.activateMessages;
			// Topics initialisieren wenn vorhanden
			topics = new Dictionary<string, ConsoleTopic>();
			foreach (ConsoleTopic topic in savedData.topics)
			{
				topics.Add(topic.token, topic);
			}


			// LogToken initialisieren, wenn vorhanden
			//	individualEntrys = new List<string>();
			logEntrys = new Dictionary<string, LogEntry>();
			foreach (LogEntry entry in savedData.logEntrys)
			{
				logEntrys.Add(entry.token, entry);
			}

			// Individuelle Einstellungen laden
			LoadSettings();
		}

		// Fügt einem Topic eine weitere FetchDataFunktion hinzu (oder erstellt das Topic)
		public static void AddFetchFctToTopic(string topicName, FetchData fct)
		{
			if (fetchDataTasks.ContainsKey(topicName))
			{
				fetchDataTasks[topicName].OnFetchData += fct;
			}
			else
			{
				DataFetch data = new DataFetch();
				data.OnFetchData += fct;
				fetchDataTasks.Add(topicName, data);
			}
		}

		// Fügt einem Topic eine weitere FetchDataFunktion hinzu (oder erstellt das Topic)
		public static void AddSceneSpecificFetchFctToTopic(string topicName, FetchData fct)
		{
			if (sceneSpecificFetchDataTasks.ContainsKey(topicName))
			{
				sceneSpecificFetchDataTasks[topicName].OnFetchData += fct;
			}
			else
			{
				DataFetch data = new DataFetch();
				data.OnFetchData += fct;
				sceneSpecificFetchDataTasks.Add(topicName, data);
			}
		}

		// Gibt den TopicNamen zurück, wenn spezifiziert
		public static string GetTopicIfExist(string token)
		{
			if (topics == null)
				topics = new Dictionary<string, ConsoleTopic>();

			if (topics.ContainsKey(token))
				return topics[token].name;
			else
				return token;

		}

		public static void SetNormalGameMechanics(bool value)
		{
			//AC.KickStarter.stateHandler.SetInteractionSystem(value);
			//AC.KickStarter.stateHandler.SetMovementSystem(value);
			//	AC.KickStarter.playerMenus.SetManualSaveLock(value);
		}

		// Öffnet VALUR ( wenn nicht deaktiviert)
		public static void Open()
		{
			if (deactivated)
				return;
			SetNormalGameMechanics(false);
			//TEAGUE.DI.stateDI.ChangeApplicationState(TEAGUE.GeneralApplicationState.Console);
			displayManager.Open();
		}

		// Schließt VALUR ( wenn nicht deaktiviert)
		public static void Close()
		{
			if (deactivated)
				return;
			SetNormalGameMechanics(true);
			displayManager.Close();
			//TEAGUE.DI.stateDI.ChangeApplicationState(TEAGUE.GeneralApplicationState.Major);
		}

		public static void CloseHard()
		{
			if (deactivated)
				return;
			displayManager.CloseHard();
			//TEAGUE.DI.stateDI.ChangeApplicationState(TEAGUE.GeneralApplicationState.Major);
		}

		// Öffnet ein spezielles Topic ( wenn nicht deaktiviert)
		public static void ShowPage(string token)
		{
			if (deactivated)
				return;
			displayManager.ShowPage(token);
		}

		// Öffnet das VALUR-Menü ( wenn nicht deaktiviert)
		public static void ShowMenu()
		{
			if (deactivated)
				return;
			displayManager.ShowMenu();
		}

		// Löscht die zuletzt akuirierten Daten
		public static void NewDataFetch()
		{
			fetchedData = new List<ConsoleData>();
		}

		// Basis-Funktion zur Hinzufügen von VALUR-UI-Elementen
		public static void AddConsoleData(ConsoleData tmp)
		{
			fetchedData.Add(tmp);
		}

		// LogNachricht mit Token melden
		public static void Log(string message, string token = "")
		{


			if (deactivated)
				return;

			if (!logsActivated)
				return;

			if (token.Trim() != "" && logEntrys.ContainsKey(token))
			{
				// Zeigen, wenn erlaubt
				if (logEntrys[token].show)
					displayManager.ShowMessage(message);

				// speichern, wenn erlaubt
				if (logEntrys[token].save)
					logManager.LogMessage(token, message);
			}


		}

		public static void UpdateMAT()
		{
			displayManager.UpdateActivePage();
		}

		// Setzen in den Side-Modus
		// Anzeige an der Seite
		// ApplicationState in Major gesetzt
		public static void ToSide()
		{

			if (deactivated)
				return;
			//TEAGUE.DI.stateDI.ChangeApplicationState(TEAGUE.GeneralApplicationState.Major);
			displayManager.ToSide();
		}

		// Unterbindet das Aktualisieren angezeigter Daten
		public static void StopUpdating()
		{
			displayManager.updating = false;
		}

		// Lässt das Aktualisieren von angezeigten Daten zu
		public static void ResumeUpdating()
		{
			displayManager.updating = true;
		}

		// Fügt ein LogToken und seine DefaultEinstellung von einem Skript aus zu (nicht Editor)
		public static void IntroduceLogEntry(string identifier, bool save, bool show = false)
		{
			if (logEntrys == null)
				logEntrys = new Dictionary<string, LogEntry>();
			if (!logEntrys.ContainsKey(identifier))
			{
				logEntrys.Add(identifier, new LogEntry(identifier, save, show));

			}

		}

		// Fügt ein LogToken und seine DefaultEinstellung von einem Skript aus zu (nicht Editor)
		public static void IntroduceTopic(ConsoleTopic topic, bool sceneSpecific = false)
		{
			if (topics == null)
				topics = new Dictionary<string, ConsoleTopic>();

			if (!topics.ContainsKey(topic.token))
			{
				topics.Add(topic.token, topic);

			}
			else if (!sceneSpecific)
			{

				fetchDataTasks[topic.token] = new DataFetch();
			}
			else
			{
				sceneSpecificFetchDataTasks[topic.token] = new DataFetch();
			}


		}
		// Die VALUR Einstellungen (zur Laufzeit gemacht) speichern
		public static void SaveSettings()
		{
			string data = "";
			data += logsActivated.ToString();
			if(logEntrys.Count > 0)
			{
				data += "$";
				foreach(string token in logEntrys.Keys)
				{
					data += token +"," + logEntrys[token].save.ToString() + "," + logEntrys[token].show.ToString() + "§";
				}
				data = data.Substring(0, data.Length-1);
			}
			//SaveManager.SaveValue("valur", data);
		}

		// Läd die VALUR-Einstellungen, die vom USER gespeicehrt wurden
		public static void LoadSettings()
		{
			/*string loadedStuff = SaveManager.LoadValue("valur");
			//Debug.Log(loadedStuff);
			if (loadedStuff == null)
				return;
			string[] loadedData = loadedStuff.Split ("$"[0]);
			if(loadedData == null)
				return;
			if(loadedData[0] == "True")
			{
				logsActivated = true;
			}
			else if(loadedData[0] == "False")
			{
				logsActivated = false;
			}

			if(loadedData.Length > 1)
			{
				string[] indEntrys = loadedData[1].Split ("§"[0]);
				foreach(string entry in indEntrys)
				{
					string[] datas = entry.Split (","[0]);
					bool a = false;
					if(datas[1] == "True")
						a = true;
					bool b = false;
					if(datas[2] == "True")
						b = true;
					LogEntry newEntry = new LogEntry(datas[0], a, b);
				//	individualEntrys.Add (datas[0]);
					if(logEntrys.ContainsKey(datas[0]))
					{
						logEntrys[datas[0]] = newEntry;
					}
					else
					{
						logEntrys.Add (datas[0], newEntry);
					}
				}
			}*/
		}

		public static void ClearSceneSpecificTopics()
        {
			sceneSpecificFetchDataTasks = new Dictionary<string, DataFetch>();
        }

		// TextAnzeige melden
		public static void AddConsoleInfo(string _label, string _info, bool bold = false)
		{
			AddConsoleData(new ConsoleInfo(_label, _info, bold));

		}
		// Toggle-Element melden
		public static void AddConsoleToggle(string _label, bool value)
		{
			AddConsoleData(new ConsoleToggle(_label, value));
		}

		// Toggle-Element mit eigener Funktion melden
		public static void AddConsoleToggle(string _label, string identifier, bool value, GiveBoolOrder order)
		{
			AddConsoleData(new ConsoleFunctionToggle(_label, identifier, value, order));
		}

        // Button-Element melden
        public static void AddConsoleButton(string _label, GiveOrder _orderFct, string _orderParameter, Vector2 _size, Sprite image = null)
        {
            AddConsoleData(new ConsoleButton(_label, _orderFct, _orderParameter, _size, image));
        }

        // Button-Element melden
        public static void AddConsoleButton(string _label,GiveOrder _orderFct, string _orderParameter,  Sprite image = null)
		{
			AddConsoleData(new ConsoleButton(_label, _orderFct, _orderParameter, image));
		}

		// TextEingabeFeld-Element melden
		public static void AddConsoleField(string _label, GiveOrder _orderFunction, string value)
		{
			AddConsoleData(new ConsoleField(_label, _orderFunction, value));
		}
		// Bild-Element melden
		public static void AddConsolePicture(Sprite image)
		{
			AddConsoleData(new ConsolePicture(image));
		}

		// Slider-Element melden
		public static void AddConsoleSlider(string _label, GiveFloatOrder _orderFct, float flValue, Vector2 borders, string _info = "")
		{
			AddConsoleData(new ConsoleSlider(_label, _orderFct, flValue, borders, _info));
		}

		// Slider-Element melden
		public static void AddConsoleIntSlider(string _label, GiveIntOrder _orderFct, int intValue, Vector2 borders, string _info = "")
		{
			AddConsoleData(new ConsoleIntSlider(_label, _orderFct, intValue, borders, _info));
		}

		// Slider-Element melden
		public static void AddPopupMenu(string _label, GiveIntOrder _orderFct, int intValue, string[] options)
		{
			AddConsoleData(new ConsolePopup(_label, _orderFct, intValue, options));
		}


    }



}
