using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LokaElement : MonoBehaviour
{
    bool initialized = false;
    int lineID = -1;
    public string token;
    // Start is called before the first frame update
    public void Start()
    {
        if(!initialized)
        {
            SimpleLocalization.knownElements.Add(this);
            initialized = true;
        }
        LocalizeElement();
    }

    public virtual void LocalizeElement()
    {
        lineID = SimpleLocalization.inGame.kit.GetLineID(token);
    }

    public void OnEnable()
    {
        LocalizeElement();
    }


    public string GetLocaEntry(string token)
    {
        token = token.Trim().ToLower();
        if (token[0] == '$')
            return SimpleLocalization.GetLocalization(token.Substring(1));
        else
            return SimpleLocalization.GetLocalization(token);
    }
}
