using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class ActionListenForTutorialEvent : Action
{
    public string tutEventName;
    public bool clear;

    public enum EventSource { Tutorial, InGame}
    public EventSource source;
    override public void ExecuteAction()
    {
        if(clear)
        {
            GameEventManager.Instance.ClearListeners();
            if (tutEventName.Trim() == "")
            {
                ReportActionEnd();
                return;
            }
        }

       

        if (source == EventSource.Tutorial && GameEventManager.Instance.tutEventLibrary)
        {
            GameEventManager.Instance.tutEventLibrary.ListenFor(tutEventName);
        }

        if (source == EventSource.InGame && GameEventManager.Instance.ingameEventLibrary)
        {
            GameEventManager.Instance.ingameEventLibrary.ListenFor(tutEventName);
        }
        ReportActionEnd();
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {

        return "";
    }
}
