using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/*****
 * VALUR - DisplayManager ## Version 1.0
 * Last Change: 01.11.2015
 * By: Oli
 * 
 * Manager, der Die VALUR-UI-Daten eines Topics darstellt
 *****/

namespace VALUR
{

public class DisplayManager: MonoBehaviour {

	// Aktives Topic
	public string activeToken = "";
	public Canvas canvas;

	public GameObject title;
	public GameObject menu;
		
	public GameObject data;
	public GameObject inter;
		
	public GameObject rightSide;
	public GameObject leftSide;

	// Buttons
	public GameObject closeButton;
	public GameObject toSideButton;
	public GameObject toMiddleButton;

	public GameObject menuButtonGroup;
	public GameObject dataElementGroup;
	public Text dataElementsHeader;
		
	public Vector3 standardRightPosition;

	private bool reset = false;
	
	// Darstellung-Origin
	public Vector3 buttonStart;
	// Darstellung-Offset
	public Vector3 buttonOffset;

	// Prefabs für die UI-Elemente
	public Button menuButtonPrefab;


	public Button buttonPrefab;
	public Toggle togglePrefab;
	public Text textPrefab;
	public InputField fieldPrefab;
	public Image imagePrefab;
    public Slider sliderPrefab;
		public Dropdown dropDownPrefab;

    // MessageBox Object (bisher nur eins)
        public MessageBox messageBox;

	// Informationen zum Aktualisieren der angezeigten Daten
	public bool updating = true;
	public int updateRate = 100;
	public int updateCounter = 0;
        public int blockUpdater = 0;

	private bool waitForLeftSide;
	private bool waitForRightSide;

	public GameObject fpscounterPrefab;
	GameObject fpscounter;

        private void Awake()
        {
			Data.displayManager = this;
		}

        // Use this for initialization
        void Start () {
			
			standardRightPosition = rightSide.transform.localPosition;
			messageBox.gameObject.SetActive(false);
			ShowMenu();
			CloseHard();

	}

		public void ToggleFPSCounter()
		{
			if (fpscounter != null)
				Destroy (fpscounter);
			else {
				fpscounter = Instantiate (fpscounterPrefab);
           //     if (CameraManager.ActiveCamera && !CameraManager.ActiveCamera.gameObject.GetComponent<GUILayer>())
           //         CameraManager.ActiveCamera.gameObject.AddComponent<GUILayer>();

            }

		}
	
	// Update is called once per frame
	void Update ()
	{
		// Nur Aktualisieren, wenn in den aktiven Zuständen
		if(Data.state == State.ActiveMain || Data.state == State.ActiveSide)
		{
                if (blockUpdater > 0)
                    blockUpdater--;
                else
                {
                    updateCounter++;
                    //..und das aktualisieren generell erlaubt ist (bspw. nicht wenn im TextFeld)
                    if (updating && updateCounter > updateRate)
                    {
                        updateCounter = 0;
                        // Topic erneut Öffnen
                        ShowPage(activeToken);
                    }
                }

               
		}

		if(waitForLeftSide && !menu.GetComponent<Animation>().isPlaying)
		{
			DeleteChildren(menuButtonGroup);
			leftSide.SetActive(false);
		}
		
		if(waitForRightSide && !data.GetComponent<Animation>().isPlaying)
		{
			DeleteChildren(dataElementGroup);
			rightSide.SetActive(false);
		}
	}

	public void UpdateActivePage()
	{
			ShowPage(activeToken);
	}

	// Öffnet das zuletzt geöffnete Topic ( Wenn keines aktiv -> Öffnet das Menü)
	public void Open()
	{
			updating = true;
			ShowMenu();
		
			// Daten-Liste wieder an die Ausgangsposition setzen.
			if(reset)
			{
				reset = false;
				rightSide.transform.localPosition = standardRightPosition;
			}

			// Interface einstellen
			toMiddleButton.SetActive(false);
			toSideButton.SetActive(true);

			// Seiten initialisieren
			title.SetActive(true);
			menu.SetActive(true);

			// Menu einblenden
			if(Data.state != State.ActiveMain)
			{
				title.GetComponent<Animation>().Play("Title_In");

				menu.GetComponent<Animation>().Play("Menu_In");
			}
	
			// Daten in die Mitte rücken
			if(Data.state == State.ActiveSide)
			{
				rightSide.GetComponent<Animation>().Play("DataToMiddle");

			}

			// Daten aus dem Off holen
			else
			{
				data.SetActive(true);
				data.GetComponent<Animation>().Play("DataIn");
				inter.SetActive(true);
				inter.GetComponent<Animation>().Play("InterfaceIn");
			}

			Data.state = State.ActiveMain;

	
	}

	// Öffnet das Menü 
	public void ShowMenu()
	{
			DeleteChildren(menuButtonGroup);
			waitForLeftSide = false;
			leftSide.SetActive(true);
			// Erstellt einen Button für jedes Topic
			string firstToken = "";
			foreach(string token in Data.fetchDataTasks.Keys)
			{
				//print (token);
				if(firstToken == "")
					firstToken = token;
				CreateMenuButton(Data.GetTopicIfExist(token), token);
	
			}

			foreach (string token in Data.sceneSpecificFetchDataTasks.Keys)
			{
				//print (token);
				if (firstToken == "")
					firstToken = token;
				CreateMenuButton(Data.GetTopicIfExist(token), token);

			}

			menuButtonGroup.GetComponent<PanelSizeFromChildren>().Adjust();
			if(activeToken == "")
			{
				if(firstToken != "")
					ShowPage(firstToken);
			}
			else
			{
				ShowPage(activeToken);
			}

	}
        public bool forceAdjust = false;
    // Öffnet das topic "token"
        public void ShowPage(string token, bool newDataFcts = true)
	{
			bool readjust = true;
			waitForRightSide = false;
			if(activeToken == token && !forceAdjust)
				readjust = false;
			activeToken = token;

			dataElementsHeader.text = Data.GetTopicIfExist(token);
			DeleteChildren(dataElementGroup);

			rightSide.SetActive(true);
			Data.NewDataFetch();
			if (Data.fetchDataTasks.ContainsKey(token))
				Data.fetchDataTasks[token].FetchData();
			if (Data.sceneSpecificFetchDataTasks.ContainsKey(token))
				Data.sceneSpecificFetchDataTasks[token].FetchData();

			foreach (ConsoleData data in Data.fetchedData)
			{
				
				
				switch(data.type)
				{
				case DataType.Info:
					CreateText(data);
					break;
				case DataType.Button:
						if (data.sprite != null)
							CreateOrderButton(data);// Button nur mit Text
						else
							CreateOrderButton(data.info, data.label, data.orderFct);		// Button mit Bild
					break;
				case DataType.Toggle:
						CreateToggle(data);
					break;
				case DataType.Field:
						CreateField(data);
					break;
				case DataType.Picture:
					CreateImage(data.sprite);
					break;
                case DataType.Slider:
						CreateSlider(data);
                    break;
				case DataType.Popup:
						CreatePopup(data); 
					break;
				case DataType.IntSlider:
						CreateIntSlider(data);
					break;
				}
			
			}
			if(readjust)
				dataElementGroup.GetComponent<PanelSizeFromChildren>().Adjust();
            if(forceAdjust)
                forceAdjust = false;
            updating = true;
		
	}

	// Ein Bild aus dem Prefab erstellen und positionieren
	public Image CreateImage(Sprite sprite)
	{
			Image image = Instantiate(imagePrefab);
		
			return image;
	}

		// Ein Bild aus dem Prefab erstellen und positionieren
	public Slider CreateSlider(ConsoleData data)
    {
			Slider slider = Instantiate(sliderPrefab);
			slider.wholeNumbers = false;
			slider.minValue = data.size.x;
            slider.maxValue = data.size.y;
            slider.value = data.floatValue;
            slider.transform.SetParent(dataElementGroup.transform, false);
            slider.onValueChanged = data.sliderFct;
            slider.onValueChanged.AddListener(SliderUpdateBlock);
            slider.gameObject.SetActive(true);
            slider.transform.Find("Label").GetComponent<Text>().text = data.label + "\n" + "( " + data.floatValue  + " )";  
            return slider;
    }

    public void SliderUpdateBlock(float val)
    {
        blockUpdater = 20;
    }

		// Ein Bild aus dem Prefab erstellen und positionieren
		public Slider CreateIntSlider(ConsoleData data) 
		{
			Slider slider = Instantiate(sliderPrefab);
			slider.wholeNumbers = true;
			slider.minValue = data.size.x;
			slider.maxValue = data.size.y;
			slider.value = data.intValue;
			slider.transform.SetParent(dataElementGroup.transform, false);
			slider.onValueChanged = data.sliderFct;
			slider.onValueChanged.AddListener(SliderUpdateBlock);
			slider.gameObject.SetActive(true);
			slider.transform.Find("Label").GetComponent<Text>().text = data.label + "\n" + "( " + data.intValue + " )";
			return slider;
		}

		// Ein TextEingabeFeld aus dem Prefab erstellen und positionieren
		public InputField CreateField(ConsoleData data)
	{
		InputField field = Instantiate(fieldPrefab);
		field.transform.SetParent(dataElementGroup.transform,false);
	
		field.GetComponent<Field>().orderFct = data.orderFct;
		field.GetComponent<Field>().DisplayValue(data.info);
		field.gameObject.SetActive(true);
	
		return field;
	}
	
	// Ein Toggle aus dem Prefab erstellen und positionieren
	public Toggle CreateToggle(ConsoleData data) 
	{
			Toggle tgl = Instantiate(togglePrefab);
			tgl.transform.SetParent(dataElementGroup.transform,false);
		
			tgl.isOn = data.toggleValue;
			tgl.onValueChanged = data.toggleFct;
			tgl.transform.Find("Label").GetComponent<Text>().text = data.label;
			tgl.gameObject.SetActive(true);
		
			return tgl;
	}

		// Eine Textdarstellung aus dem Prefab erstellen und positionieren
		public Text CreateText(ConsoleData data)
		{
			Text txt = Instantiate(textPrefab);
			txt.transform.SetParent(dataElementGroup.transform, false);

			txt.name = data.label;
			txt.text = data.label;
			if (data.info.Trim() != "")
				txt.text += " : " + data.info;
			if (data.special)
			{
				txt.fontStyle = FontStyle.Bold;
				txt.alignment = TextAnchor.MiddleCenter;
			}

			txt.gameObject.SetActive(true);
			return txt;
		}

		// Ein Button aus dem Prefab erstellen und positionieren ( nut Text)
		public Button CreateMenuButton(string name, string token)
	{
			Button btn = Instantiate(menuButtonPrefab);
			btn.transform.SetParent(menuButtonGroup.transform,false);
		
			btn.name = name;
			btn.gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = name;
	
			btn.GetComponent<VALUR_MenuButton>().token = token;
	
			btn.gameObject.SetActive(true);
			return btn;
	}

	// Ein Button aus dem Prefab erstellen und positionieren ( mit Bild)
	public Button CreateOrderButton(ConsoleData data) //Sprite sprite, string name, GiveOrder order, string _info,  Vector2 size)
	{

		Button btn = Instantiate(buttonPrefab);
		btn.transform.SetParent(dataElementGroup.transform,false);

		btn.name = name;
		btn.gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = "";
		btn.image.sprite = data.sprite;
        btn.GetComponent<VALUR_MenuButton>().token = data.info;
        btn.GetComponent<VALUR_MenuButton>().orderFct = data.orderFct;
        if(data.size != Vector2.zero)
        {
                btn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, data.size.x);
                btn.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, data.size.y);
            }
        

        btn.gameObject.SetActive(true);
		return btn;
	}

		// Ein Button aus dem Prefab erstellen und positionieren (ohen Bild)
	public Button CreateOrderButton(string token, string name, GiveOrder order)
	{
			Button btn = CreateOrderButton(token, name);
			btn.GetComponent<VALUR_MenuButton>().orderFct = order;
           
            return btn;
	}

	// Ein Button aus dem Prefab erstellen und positionieren (ohen Bild )
	// ohne explizite Funktion, weil der Typ die Funktion diktiert
	public Button CreateOrderButton(string token, string name)
		{
			Button btn = Instantiate(buttonPrefab);
			btn.transform.SetParent(dataElementGroup.transform,false);

			btn.name = name;
			btn.gameObject.transform.Find("Text").gameObject.GetComponent<Text>().text = name;
			btn.GetComponent<VALUR_MenuButton>().token = token;
	
			btn.gameObject.SetActive(true);
			return btn;
		}

	// Alle dargestellten Elemente Löschen
	public void DeleteChildren(GameObject obj)
	{
		// Children eines Transforms löschen (und schonmal aus der Child-Liste werfen)
		for(int i = 0; i < obj.transform.childCount; i++)
		{
				Destroy(obj.transform.GetChild(i).gameObject);
		}

		obj.transform.DetachChildren();
	}

		public Dropdown CreatePopup(ConsoleData data)
		{
			Dropdown dropDown = Instantiate(dropDownPrefab);
			List<Dropdown.OptionData> oList = new List<Dropdown.OptionData>();
			foreach(string s in data.options)
            {
				oList.Add(new Dropdown.OptionData(s));
            }
			dropDown.options = oList;
			dropDown.value = data.intValue;
			dropDown.transform.SetParent(dataElementGroup.transform, false);

			dropDown.onValueChanged = data.dropDownFct;
			//dropDown.onValueChanged.AddListener(DropDownUpdateBlock);
			dropDown.gameObject.SetActive(true);
			dropDown.transform.Find("Label").GetComponent<Text>().text = data.label;
			return dropDown;
		}

		public void DropDownUpdateBlock(int val)
		{
			blockUpdater = 120;
		}


		// Ansicht schließen
		public void Close()
	{		
		CloseLeft();
		CloseRight();
		Data.state = State.Inactive;

	}

	public void CloseHard()
	{
		title.SetActive(false);
		menu.SetActive(false);
		data.SetActive(false);
		inter.SetActive(false);
		toMiddleButton.SetActive(false);
		Data.state = State.Inactive;
		
	}

	public void CloseLeft()
	{
		if(Data.state != State.ActiveMain)
				return;

		title.GetComponent<Animation>().Play("Title_Out");
		menu.GetComponent<Animation>().Play("Menu_Out");
		waitForLeftSide = true;
			
	}
		
	public void CloseRight()
	{
			
		data.GetComponent<Animation>().Play("DataOut");
		inter.GetComponent<Animation>().Play("InterfaceOut");
		waitForRightSide = true;
		reset = true;
		
	}

	// Zur Seite verschieben
	public void ToSide()
	{
		CloseLeft ();
		rightSide.GetComponent<Animation>().Play("DataToSide");
		toMiddleButton.SetActive(true);
		toSideButton.SetActive(false);

		Data.state = State.ActiveSide;
		
	}

	// Nachricht anzeigen
	public void ShowMessage(string message)
	{
			if(Data.activateMessages)
				messageBox.ShowMessage(message);
	}
		public InteractionList interactionList;
		// Nachricht anzeigen
		public void AddInteractionToList(Interaction action)
		{
			interactionList.AddInteraction(action);
		}

		public void ClearInteractionList()
        {
			interactionList.Reset();
        }

		public void MainTasks(string para)
	{
		switch(para)
			{
			case "open":
				VALUR.Data.Open();
				break;
			case "close":
				VALUR.Data.Close();
				break;
			case "toside":
				VALUR.Data.ToSide();
				break;
			}
	}
}


}