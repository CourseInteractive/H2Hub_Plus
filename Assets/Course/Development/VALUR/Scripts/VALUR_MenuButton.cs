using UnityEngine;
using System.Collections;

/*****
 * VALUR - MenuButton ## Version 1
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Skript das je nach Typ des Buttons den Druck handhabt
 *****/

namespace VALUR
{

public class VALUR_MenuButton : MonoBehaviour {
	
	// Page: Name des Topics das bei Druck geöddnet werden soll
	// Order: Parameter der an die Funktion (oderFct) übergeben wird
	[HideInInspector]
	public string token;

	// Funktion die vom InformationsSkript bereitgestellt wurde
	// Bei Druck wird das token als parameter an diese Funktion übergeben
	public GiveOrder orderFct;

	// Typ des Buttons
	public ButtonType type;


	// Funktion wird beim Druck des Buttons aufgerufen
	public void Action()
	{
		switch(type)
		{
		case ButtonType.Menu:		// Topic öffnen
			Data.ShowPage(token);
			break;
		case ButtonType.Order:		// Funktion aufrufen
			orderFct(token);
            Data.UpdateMAT();
            break;
		}

	}
}

}
