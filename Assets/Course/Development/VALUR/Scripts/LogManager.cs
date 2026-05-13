using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****
 * VALUR - LogManager ## Version 0.5
 * Last Change: 25.10.2015
 * By: Oli
 * 
 * Manager, der MessageDaten als Logs speichert
 * Die Daten können anhand von MessageEigenschaften gefiltert ausgegeben werden
 *****/

namespace VALUR
{

	public class LogManager : MonoBehaviour {

		// Alle gespeicherten Nachrichten
		public List<Message> messages;

		// Die Token, nach denen gefiltert werden soll ( Wenn leer, werden alle ausgegeben)
		public List<string> filteredTokens;

		// Initialisieren -> In MainScript anmelden und Listen erstellen
		void Awake () {
			Data.logManager = this;
			messages = new List<Message>();
			filteredTokens = new List<string>();
		}


		// Funktion um eine Nachricht zu übergeben (bereits gefiltert, danach ob dieses Token gespeichert werden soll oder nicht)
		public void LogMessage(string token, string message)
		{
			Message newMessage = new Message(token, message);
			messages.Add (newMessage);
		}


		// Rückgabe der Nachrichten, gefiltert nach den "filteredTokens"
		public List<string> GetFilteredMessages()
		{
			bool noFilter = false;
			// Wenn kein Token für den Filter definiert ist, werden alle nachrichten zurückgegeben
			if(filteredTokens.Count == 0)
				noFilter = true;

			// Zusammenfassen der Nachrichten in einem Array
			List<string> output = new List<string>();
			foreach(Message msg in messages)
			{
				if(noFilter || filteredTokens.Contains(msg.token))
				{
					output.Add (msg.message);
				}
			}

			// Rückgabe
			return output;
		}
	}

}