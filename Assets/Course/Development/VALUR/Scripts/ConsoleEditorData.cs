using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****
 * VALUR - ConsoleEditorData ## Version 0.5
 * Last Change: 25.10.2015
 * By: Oli
 * 
 * Dieses DateiFormat speichert die im Editor festgelegten Einstellungen von VALUR
 *****/

namespace VALUR
{

	[System.Serializable]
	public class ConsoleTopic
	{
		public string token;
		public string name;


		public ConsoleTopic()
		{
			token = "NEW";
		}
	}



public class ConsoleEditorData  : ScriptableObject
{
	// Topics und ihre Eigenschaften (nicht spezifizierte werden defaultmäßig angezeigt)
	public List<ConsoleTopic> topics;
	// Standard-LogTokens und ihre Eigenschaften
	public List<LogEntry> logEntrys;

	// Generelle Deaktivierung von VALUR
	public bool deactivated;
		public bool activateMessages = true;

}


}