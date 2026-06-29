using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLocalization_InGame : MonoBehaviour
{
    //public TextAsset text;
    public LokaKit kit;
   
    public Dictionary<string, SimpleLocalization_LocaEntry> entries;

    bool initialized = false;

    // Start is called before the first frame update
    void Awake()
    {
        if (initialized)
            return;
        if (SimpleLocalization.inGame != null)
        {
            Destroy(this);
            return;
        }
        initialized = true;
        Debug.Log("Register Loca Ingame");
        SimpleLocalization.knownElements = new List<LokaElement>();
        SimpleLocalization.inGame = this;
       // ParseText();
    }

}

[System.Serializable]
public class SimpleLocalization_LocaEntry
{
    public string token;
    public string german;
    public string english;
}
