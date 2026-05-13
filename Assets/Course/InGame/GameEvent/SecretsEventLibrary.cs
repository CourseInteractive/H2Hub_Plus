using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretsEventLibrary : MonoBehaviour
{
    public GameEventListen[] secretsEvents;
    public GameEventListen[] puzzleEvents;

    // Todo: Inspector/Editor fpr "Game Event Listen" -> Keys als DropDown
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameEventListen listener in secretsEvents)
        {
            GameEventManager.Instance.SetNewListener(listener, false);
        }

        foreach (GameEventListen listener in puzzleEvents)
        {
            GameEventManager.Instance.SetNewListener(listener, false);
        }
    }

}
