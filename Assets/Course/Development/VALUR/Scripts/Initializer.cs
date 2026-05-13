using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*****
 * VALUR - Initializer ## Version 0.5
 * Last Change: 25.10.2015
 * By: Oli
 * 
 * Initialisiert die VALUR-Console
 * Ist in der TEAGUE-Init gesetzt
 *****/

namespace VALUR
{

public class Initializer : MonoBehaviour {

	// Name der gespeicherten Datei
	private static string dataName = "ConsoleData";
	
	// Use this for initialization
	void Awake () {
		// gespeicherte Daten laden 
		ConsoleEditorData savedData = (ConsoleEditorData)Resources.Load ("GamePrefs/" + dataName);
		Data.startTimestamp = System.DateTime.Now.ToFileTime ();
		if(savedData != null)
		{
			// VALUR initialisieren, wenn nicht deaktiviert
			if(!savedData.deactivated)
			{
				VALUR.Data.Initialize(savedData);
				Resources.UnloadAsset( savedData );
			}
			// Wenn VALUR deaktiviert wurde, wird dieses Objekt gelöscht und VALUR komplett deaktiviert
			else
			{
				Resources.UnloadAsset( savedData );
			
				Data.deactivated = true;
					Destroy(gameObject);
				return;
			}
			


		}

		
	}


		public void Reload()
		{

			Data.Open ();
		}

        public void OnLevelWasLoaded(int level)
        {
			Data.ClearSceneSpecificTopics();

		}

    }

}