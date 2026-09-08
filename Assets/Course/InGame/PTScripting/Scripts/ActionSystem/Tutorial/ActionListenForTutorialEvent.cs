using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class ActionListenForTutorialEvent : Action
{
    public string tutEventName;
    public bool clear;
    override public void ExecuteAction()
    {
        if(clear && tutEventName.Trim() == "")
        {
            GameEventManager.Instance.ClearListeners();
            ReportActionEnd();
            return;
        }

        if(GameEventManager.Instance.tutEventLibrary)
        {
            GameEventManager.Instance.tutEventLibrary.ListenFor(tutEventName, clear);
        }
        ReportActionEnd();
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {

        return "";
    }
}
