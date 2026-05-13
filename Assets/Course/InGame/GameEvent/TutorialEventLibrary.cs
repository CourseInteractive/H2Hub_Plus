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

    public void ListenFor(string name)
    {
        foreach(GameEventListen listener in tutorialEvents)
        {
            if(listener.internName == name)
            {
                GameEventManager.Instance.SetNewListener(listener);
                return;
            }
        }
    }
}
