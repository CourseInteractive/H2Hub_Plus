using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEventLibrary : MonoBehaviour
{
    public GameEventListen[] tutorialEvents;

    private void Start()
    {
        if (GameEventManager.Instance)
            GameEventManager.Instance.tutEventLibrary = this;

    }

    public void ListenFor(string name, bool clearBefore = false)
    {
        foreach(GameEventListen listener in tutorialEvents)
        {
            if(listener.internName == name)
            {
                GameEventManager.Instance.SetNewListener(listener,false);
                return;
            }
        }
    }
}
