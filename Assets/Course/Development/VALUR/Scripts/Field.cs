using UnityEngine;
using System.Collections;

/*****
 * VALUR - Field ## Version 1
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Script zur korrekten Abarbeitung eines VALUR-UI-Textfeldes
 *****/

namespace VALUR
{

public class Field : MonoBehaviour {

	// Funktion (string para) der die Eingabe übergegeben wird
	public GiveOrder orderFct;

	// Funktion die aufgerufen wird, wenn eine Eingabe erfolgt ist
	public void Action(string input)
	{
			if(input.Trim () == "")
				return;
			// Löschen des Textes
			GetComponent<UnityEngine.UI.InputField>().text = "";
			// Ausführen der Funktion mit der Eingabe als Parameter
			orderFct(input);

		
	}

		public void Execute()
        {
			Action(GetComponent<UnityEngine.UI.InputField>().text);

		}

	// Blockiert das Updaten der Console, wenn gerade Text in das Feld eingegeben wird
	public void StopUpdating()
	{
		Data.StopUpdating();
	}
	
	// Erlaubt das Updaten der Console
	public void ResumeUpdating()
	{
		Data.ResumeUpdating();
	}
		public void DisplayValue(string value)
        {
			GetComponent<UnityEngine.UI.InputField>().text = value;

		}
}


}