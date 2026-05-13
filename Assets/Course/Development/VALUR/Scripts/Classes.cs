using UnityEngine;
using System.Collections;

/*****
 * VALUR - Classes ## Version 1
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Klassen, Enums und globale Funktionen für die VALUR-Console
 *****/

namespace VALUR
{
	// Funktion zum Aquirieren von Daten
	public delegate void FetchData();
	// Funktion die einem UI-Element mitgegeben werden kann (Ein Parameter)
	public delegate void GiveOrder(string order);
	// Funktion die einem Toggle-Element mitgegeben werden kann (string Parameter -> token, order -> eingegebener Wert Parameter)
	public delegate void GiveBoolOrder(string info, bool order);
    // Funktion die einem UI-Element mitgegeben werden kann (Ein Parameter)
    public delegate void GiveFloatOrder(string info, float value);
	// Funktion die einem UI-Element mitgegeben werden kann (Ein Parameter)
	public delegate void GiveIntOrder(string info, int value);

	// Typ eines VALUR-UI-Elements
	public enum DataType { Info, Toggle, Button, Picture, Field, Slider, Popup, IntSlider }
	// Typ eines VALUR-Buttons ( Order ist ein eigentlicher Button)
	public enum ButtonType {  Menu, Order }
	// Status der VALUR-Console
	public enum State { 
		Inactive, 	// Geschlossen
		ActiveMain, // Aktiv in der Mitte
		ActiveSide // Aktiv an der Seite ( kein eigener AppState )
	}


	// EventHandler für das aquirieren von Daten
	// Pro Topic-Token wird ein DataFetch angelegt, der all seine FetchData-Funktionen hält
	// Der OnFetchData-Event aquiriert die Daten
	public class DataFetch
	{
		public event FetchData OnFetchData;
		
		public void FetchData()
		{
			OnFetchData();
		}
	}



	// Basis-Klasse für die VALUR-UI-Elemente
	public class ConsoleData
	{
		public string label;					// Angezeigter Name des Elements
		public DataType type;					// Typ des Elements
		public string info;						// Jegliche TextInformation, die für die Funktion nötig ist (meist parameter)
		public GiveOrder orderFct;				// Funktion mit einem string Paramter, definiert von dem Skript, das die ConsoleData erstellt hat
		public GiveBoolOrder boolOrderFct;      // Funktion mit einem string und einem bool Parameter, definiert von dem Skript, das die ConsoleData erstellt hat
        public GiveFloatOrder floatOrderFct;
		public GiveIntOrder intOrderFct;
		public UnityEngine.UI.Toggle.ToggleEvent toggleFct; // Besondere ToggleEvent Funktion, die beim ändern des Toggle, die gesetzte Variable direkt verändert
        public UnityEngine.UI.Slider.SliderEvent sliderFct; // Besondere ToggleEvent Funktion, die beim ändern des Toggle, die gesetzte Variable direkt verändert
		public UnityEngine.UI.Dropdown.DropdownEvent dropDownFct;
		public bool toggleValue;				// Mitgegebener Boolwert des Toggles
		public Sprite sprite;                   // Bild zur Verwendung als Image oder Button
        public Vector2 size = Vector2.zero;
        public float floatValue;
		public int intValue;
		public string[] options;
		public bool special;
	}
	// ConsoleData mit Daten für Textdarstellung
	public class ConsoleInfo : ConsoleData
	{

		public ConsoleInfo(string _label, string _info, bool bold = false)
		{
			type = DataType.Info;
			label = _label;
			info = _info;
			special = bold;
		}

	}

	// ConsoleData mit Daten für Buttondarstellung
	public class ConsoleButton : ConsoleData
	{

        public ConsoleButton(string _label, GiveOrder _orderFct, string _orderParameter, Vector2 _size, Sprite image = null)
        {
            type = DataType.Button;
            label = _label;
            info = _orderParameter;
            orderFct = _orderFct;
            sprite = image;
            size = _size;
        }

        public ConsoleButton(string _label,GiveOrder _orderFct, string _orderParameter,  Sprite image = null)
		{
			type = DataType.Button;
			label = _label;
			info = _orderParameter;
			orderFct = _orderFct;
			sprite = image;
			
		}
		
	}

	// ConsoleData mit Daten für Toggle, das beim Klick speziell reagieren soll
	public class ConsoleFunctionToggle : ConsoleData
	{
		
		public ConsoleFunctionToggle(string _label, string identifier, bool value, GiveBoolOrder order )
		{
			label = _label;
			type = VALUR.DataType.Toggle;
			toggleValue = value;
			info = identifier;
			toggleFct = new UnityEngine.UI.Toggle.ToggleEvent();
			boolOrderFct = order;
			toggleFct.AddListener(ToggleFunction);
		}

		public void ToggleFunction(bool on)
		{
	
			boolOrderFct(info, on);
		}

	}


	// ConsoleData mit Daten für Toggle, das die gesetzte Variable direkt verändern soll
	public class ConsoleToggle : ConsoleData
	{
		
		public ConsoleToggle(string _label, bool value)
		{
			label = _label;
			type = VALUR.DataType.Toggle;
			toggleValue = value;
			toggleFct = new UnityEngine.UI.Toggle.ToggleEvent();
		}
	}

	// ConsoleData mit Daten für die Darstellugn als Textfeld
	public class ConsoleField : ConsoleData
	{
		
		public ConsoleField(string _label, GiveOrder _orderFunction, string _value)
		{
			label = "test04";
			info = _value;
			type = VALUR.DataType.Field;
			orderFct = _orderFunction;
		}
		
	}

	// ConsoleData mit Daten für Darstellung eines Bildes
	public class ConsolePicture : ConsoleData
	{
		
		public ConsolePicture(Sprite image)
		{
			type = VALUR.DataType.Picture;
			sprite = image;
		}
		
	}

    // ConsoleData mit Daten für die Darstellugn als Textfeld
    public class ConsoleSlider : ConsoleData
    {

		public ConsoleSlider(string _label, GiveFloatOrder _orderFunction, float _floatValue, Vector2 borders, string _info = "")
		{
			label = _label;
			floatValue = _floatValue;
			type = VALUR.DataType.Slider;
			size = borders;
			info = _info;
			floatOrderFct = _orderFunction;
			sliderFct = new UnityEngine.UI.Slider.SliderEvent();
			floatOrderFct = _orderFunction;
			sliderFct.AddListener(SliderFunction);
		}

		public void SliderFunction(float value)
        {
            Debug.Log("Slider moved");

            floatOrderFct(info, value);
        }

    }

	public class ConsoleIntSlider : ConsoleData
	{

		public ConsoleIntSlider(string _label, GiveIntOrder _orderFunction, int _intValue, Vector2 borders, string _info = "")
		{
			label = _label;
			intValue = _intValue;
			type = VALUR.DataType.IntSlider;
			size = borders;
			//floatOrderFct = _orderFunction;
			sliderFct = new UnityEngine.UI.Slider.SliderEvent();
			intOrderFct = _orderFunction;
			info = _info;
			sliderFct.AddListener(SliderIntFunction);
		}

		public void SliderIntFunction(float value)
		{
			intOrderFct(info, Mathf.RoundToInt(value));
		}

	}

	public class ConsolePopup : ConsoleData
	{

		public ConsolePopup(string _label, GiveIntOrder _orderFunction, int _intValue, string[] _options)
		{
			label = _label;
			intValue = _intValue;
			type = VALUR.DataType.Popup;
			options = _options;
			intOrderFct = _orderFunction;
			dropDownFct = new UnityEngine.UI.Dropdown.DropdownEvent();
			dropDownFct.AddListener(PopupSelected);
		}

		public void PopupSelected(int value)
		{
			intOrderFct(info, value);
		}

	}


	// Klasse die beschreibt, wie mit einer Nachricht, die das Token "token" enthält umgegangen werden soll
	[System.Serializable]
	public class LogEntry
	{
		public string token;
		public bool save;
		public bool show;
		
		
		public LogEntry(string _token, bool _save, bool _show = false)
		{
			token = _token;
			save = _save;
			show = _show;
		}
	}

	// Eine Nachricht mit ihren filterbaren Eigenschaften
	[System.Serializable]
	public class Message
	{
		public string token;
		public string message;

		
		
		public Message(string _token, string _message)
		{
			token = _token;
			message = _message;
		}
	}


}