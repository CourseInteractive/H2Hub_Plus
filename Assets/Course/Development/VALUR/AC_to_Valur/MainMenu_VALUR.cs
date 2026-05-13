using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_VALUR : MonoBehaviour
{
    public int welcomeMsgVarID;
    //public MoreAppsMessage message;
    //public AC.ActionList openPortal;

    void Start()
    {
        VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
        topic.token = "mainmenu";
        topic.name = "Main";
        VALUR.Data.IntroduceTopic(topic, true);
        VALUR.Data.AddSceneSpecificFetchFctToTopic("mainmenu", FetchData1);

    }

    // Update is called once per frame
    public void FetchData1()
    {
        VALUR.Data.AddConsoleButton("Reset Welcome Text", ButtonOrder, "resetWelcome");
        VALUR.Data.AddConsoleButton("Show More Apps Message", ButtonOrder, "moreAppsMsg");
        VALUR.Data.AddConsoleButton("Open Portal", ButtonOrder, "openPortal");
    }

    public void ButtonOrder(string info)
    {
        /*switch(info)
        {
            case "resetWelcome":
                AC.GlobalVariables.SetBooleanValue(welcomeMsgVarID, true);
                break;
            case "moreAppsMsg":
                message.ShowMessageForce();
                break;
            case "openPortal":
                openPortal.Interact();
                break;
        }*/
    }
}
