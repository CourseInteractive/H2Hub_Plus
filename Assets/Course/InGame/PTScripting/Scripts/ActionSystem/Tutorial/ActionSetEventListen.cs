using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class ActionSetEventListen : Action
{
    public List<GameEvent> eventFlags;
    public Sequence actionOnEvent;


    override public void ExecuteAction()
    {
        GameEventListen listener = new GameEventListen();
        listener.possibleEvents = eventFlags;
        listener.seq = actionOnEvent;
        GameEventManager.Instance.SetNewListener(listener);
        ReportActionEnd();
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {
        return "Set";
    }
}
