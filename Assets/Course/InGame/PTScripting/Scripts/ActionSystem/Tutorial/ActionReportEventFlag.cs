using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class ActionReportEventFlag : Action
{
    public string eventKey;
    public string eventParameter;
    override public void ExecuteAction()
    {
      //  GameEvent gEvent = new GameEvent();
     //   gEvent
        GameEventManager.Instance.ReportGameEvent(eventKey, eventParameter);
        ReportActionEnd();
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {
        return "";
    }
}
