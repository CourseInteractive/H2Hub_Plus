using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****
 * VALUR - ConsoleRunningSettings ## Version 1
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Stellt die Einstellungsmöglichkeiten von VALUR der eigenen Konsole zur Laufzeit zur Verfügung.
 *****/

namespace VALUR
{

public class ConsoleRuntimeSettings : MonoBehaviour {

	public bool testbool = true;

	// Use this for initialization
	void Start () {
		
		VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
		topic.token = "valur";
		topic.name = "VALUR - Einstellungen";
		VALUR.Data.IntroduceTopic(topic);
		
		Data.AddFetchFctToTopic("valur", FetchData);
		Data.IntroduceLogEntry("bam",true);

	}




	public void FetchData()
	{
		
		// Festlegung, ob Logs gespeichert oder angezeigt werden sollen.
		ConsoleData data = new ConsoleToggle("Logs", Data.logsActivated);
		data.toggleFct.AddListener((on)=>{Data.logsActivated = on;});
		
		Data.AddConsoleData(data);

			ConsoleData msgData = new ConsoleToggle("Messages", Data.activateMessages);
			msgData.toggleFct.AddListener((on) => { Data.activateMessages = on; });

			Data.AddConsoleData(msgData);


			// Wenn Logs zur Laufzeit interessant sind.
			if (Data.logsActivated)
		{

			if((Data.logEntrys != null && Data.logEntrys.Count > 0))
			{
				// Alle bekannten LogToken anzeigen
				foreach(LogEntry entry in Data.logEntrys.Values)
				{
					Data.AddConsoleToggle(entry.token + " Save",entry.token, entry.save, SetTokenSave); 
					Data.AddConsoleToggle(entry.token + " Show",entry.token, entry.show, SetTokenShow); 
					
				}
			}

			// Feld um neues LogToken hinzuzufügen
			Data.AddConsoleField("Token Task", AddMessageToken, "" );

		}

			// Speichert die Einstelungen
			Data.AddConsoleButton("Save Settings", GeneralSettings, "save");
			// Löscht alle gespeicherten Daten
			Data.AddConsoleButton("Delete saved settings", GeneralSettings, "delete");
		
	}
	
	// Rückgabefunktion eines LogToken Toggles hinsichtlich des Einblendens der Nachricht
	public void SetTokenShow(string identifier, bool val)
	{
		print (identifier + " - " + val);
		
		Data.logEntrys[identifier].show = val;
		
	}

		// Rückgabefunktion eines LogToken Toggles hinsichtlich des Speichern der Nachricht
	public void SetTokenSave(string identifier, bool val)
	{
		print (identifier + " - " + val);
		
		Data.logEntrys[identifier].save = val;
			
	}
	
	// Funktion des Buttons der ein Token hinzufügt
	public void AddMessageToken(string data)
	{
		if(Data.logEntrys == null)
			Data.logEntrys = new Dictionary<string, LogEntry>();
		// Token hinzufügen, wenn nicht vorhanden
		if(Data.logEntrys.Count == 0 || !Data.logEntrys.ContainsKey(data))
		{
			Data.logEntrys.Add (data, new LogEntry(data, false));
		//	Data.individualEntrys.Add (data);
		}

		Data.UpdateMAT();
	}
	
	// Funktion die von den Buttons für generelle Einstellungen benutzt wird
	public void GeneralSettings(string para)
		{
			switch(para)
			{
			case "save":
				Data.SaveSettings();
				break;
			case "delete":
				//SaveManager.DeleteValue("valur");
				break;
			}
		}

}

}