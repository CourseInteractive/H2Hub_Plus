#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLESTEAMWORKS
using Steamworks;
#endif

public class MainConsoleConnect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
        topic.token = "game";
        topic.name = "GAME";
        VALUR.Data.IntroduceTopic(topic);
        VALUR.Data.AddFetchFctToTopic("game", FetchData1);

    }

    private void Update()
    {
		int debugVersion = PlayerPrefs.GetInt("ValurDebugVersion", 0);
		//if (AC.GlobalVariables.GetBooleanValue(144) && debugVersion == 0 && !Debug.isDebugBuild)
		//	return;

		if (VALUR.Data.state == VALUR.State.Inactive  && Input.touchCount == 4)
		{
			VALUR.Data.Open();

		}
		if (VALUR.Data.state == VALUR.State.Inactive && Input.GetKeyUp(KeyCode.Keypad0))
		{
			VALUR.Data.Open();

		}
		else if (VALUR.Data.state == VALUR.State.ActiveMain && Input.GetKeyUp(KeyCode.Keypad0))
		{
			VALUR.Data.Close();

		}
#if UNITY_SWITCH && !UNITY_EDITOR
		if (AC.SwitchInputHandler.GetButton("StickRButton") && AC.SwitchInputHandler.GetButton("StickLButton"))
        {
            VALUR.Data.Open();
        }
#endif

	}

	public void FetchData1()
	{

		VALUR.Data.AddConsoleInfo("Application", Application.productName);

		System.DateTime start = System.DateTime.FromFileTime(VALUR.Data.startTimestamp);
		System.DateTime now = System.DateTime.Now;
		VALUR.Data.AddConsoleInfo("Runtime", (now - start).ToString());
		VALUR.Data.AddConsoleInfo("Platform", Application.platform.ToString());
		bool steamDeck = false;
		#if !DISABLESTEAMWORKS
		try
		{
			if (BuildVersionSetup_Ingame.RunningOnSteamDeck)
			{
				steamDeck = true;
			}
			VALUR.Data.AddConsoleInfo("Steam App ID:", SteamUtils.GetAppID().ToString());
			
		}
		catch (System.Exception e)
		{
			//Debug.LogException(e);
		}
		#endif
		if(steamDeck)
			VALUR.Data.AddConsoleInfo("SteamDeck:", "Yes");
		else
			VALUR.Data.AddConsoleInfo("SteamDeck:", "No");
		VALUR.Data.AddConsoleToggle("Force SteamDeck Intern", "setSteamDeck", steamDeck, ToggleOrder);

		VALUR.Data.AddConsoleInfo("VersionNumber", Application.version);

		VALUR.Data.AddConsoleButton("FPSCounter", ButtonOrder, "fps");

		//bool testValue = AC.GlobalVariables.GetBooleanValue(0);
		//bool rtVoiceValue = AC.GlobalVariables.GetBooleanValue(75);
		//bool interactBridgeValue = AC.GlobalVariables.GetBooleanValue(114);
		//VALUR.Data.AddConsoleToggle("Test Mode", "testMode", testValue, ToggleOrder);
		//VALUR.Data.AddConsoleToggle("Interaction Bridge", "interactBridge", interactBridgeValue, ToggleOrder);
		//VALUR.Data.AddConsoleToggle("RealTimeVoice", "rtVoice", rtVoiceValue, ToggleOrder);
		//VALUR.Data.AddConsoleInfo("Save possible", (!AC.PlayerMenus.IsSavingLocked()).ToString());
		//VALUR.Data.AddConsoleToggle("Skip Texts", "skipTexts", GameTestHelper.skipTexts, ToggleOrder);
		VALUR.Data.AddConsoleButton("Test mode display", ButtonOrderTestMode, "bam");

		/*if (AC.KickStarter.player.GetComponent<AC.Char>())
        {
			VALUR.Data.AddConsoleInfo("Player gravity active", (!AC.KickStarter.player.GetComponent<AC.Char>().ignoreGravity).ToString());

		}*/
		VALUR.Data.AddConsoleButton("Reset Player to WP", ButtonOrder, "charToWP");
		VALUR.Data.AddConsoleButton("Delete Player Prefs", ButtonOrder, "deletePlayerPrefs");

		VALUR.Data.AddConsoleInfo("Resolution", Screen.currentResolution.ToString());


		VALUR.Data.AddConsoleButton("LOAD AutoSave", ButtonOrder, "loadAutosave");
		//if(AC.SaveSystem.lastAutoSaveTime != null)
		//	VALUR.Data.AddConsoleInfo("Last AutoSave", AC.SaveSystem.lastAutoSaveTime.ToShortTimeString());
		VALUR.Data.AddConsoleButton("SAVE AutoSave", ButtonOrder, "saveAutosave");
	}

	public void ButtonOrderTestMode(string data)
    {
		//TestModeIndicator.on = true;
    }
	public void ToggleOrder(string data, bool value)
	{
		//AC.GVar v = new AC.GVar();
		switch (data)
		{
			/*case "testMode":
				AC.GlobalVariables.SetBooleanValue(0, value, true);
				break;
			case "rtVoice":
				AC.GlobalVariables.SetBooleanValue(75, value, true);
				break;
			case "interactBridge":
				AC.GlobalVariables.SetBooleanValue(114, value, true);
				break;*/
			case "skipTexts":
				//GameTestHelper.skipTexts = value;
				break;
			case "setSteamDeck":
				BuildVersionSetup_Ingame.ForceInternSteamDeckValue(value);
				break;
		}
		VALUR.Data.UpdateMAT();
	}

	public void ButtonOrder(string data)
	{
		switch (data)
		{
			case "runtime":
				break;
			case "fps":
				VALUR.Data.displayManager.ToggleFPSCounter();
				break;
			case "deletePlayerPrefs":
					PlayerPrefs.DeleteAll();
				break;
			/*case "charToWP":
				if (AC.KickStarter.player.GetComponent<AC.Char>())
				{
					AC.KickStarter.player.GetComponent<AC.Char>().Teleport(GameObject.FindObjectOfType<AC.PlayerStart>().transform.position);

				}
				break;*/
			/*case "loadAutosave":
				AC.SaveSystem.LoadAutoSave();
				break;
			case "saveAutosave":
				AC.SaveSystem.SaveAutoSave();
				break;*/
		}
		print(data);
	}

}
