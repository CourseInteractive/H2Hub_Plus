using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance;

    public List<GameEventListen> listeners;

    public TutorialEventLibrary tutEventLibrary;

    public bool dev_DisplayAllEvents;

    private void Awake()
    {
        Instance = this;

    }

    public void ClearListeners()
    {
        listeners = new List<GameEventListen>();
    }

    public void SetNewListener(GameEventListen listener, bool clearBefore = true)
    {
        if(clearBefore || listeners == null)
            listeners = new List<GameEventListen>();
        listeners.Add(listener);
    }

    public void ReportGameEvent(string key, string para)
    {
      
        ReportGameEvent(new GameEvent(key, para));
    }


    public void ReportGameEvent(GameEvent gEvent)
    {
        if(dev_DisplayAllEvents)
        {
            InGameLog.Log("[GAME EVENT]:" + gEvent.eventKey + " => " + gEvent.eventParameter);
        }
        foreach(GameEventListen listener in listeners)
        {
            foreach (GameEvent possEvent in listener.possibleEvents)
            {
                if (possEvent.eventKey.Trim() == gEvent.eventKey.Trim())
                {
                    if (possEvent.eventParameter.Trim() == "" || possEvent.eventParameter.Trim() == gEvent.eventParameter)
                    {
                        listener.Execute();
                        return;
                    }

                }
            }
          
        }
    }
}

[System.Serializable]
public class GameEvent
{
    public string eventKey;
    public string eventParameter;

    public GameEvent (string key, string parameter)
    {
        eventKey = key;
        eventParameter = parameter;
    }
}


[System.Serializable]
public class GameEventListen
{
    public string internName;
    public List<GameEvent> possibleEvents;

    public Sequence seq;
    public bool deactivateOnExecution;
    bool active = true;
  
    public void Execute()
    {
        if (!active)
            return;
        if (deactivateOnExecution)
            active = false;
        seq.ExecuteCompleteSequence();
    }
}
