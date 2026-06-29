using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleLabelLoca : LokaElement
{


    public override void LocalizeElement()
    {
        TMP_Text text = GetComponent<TMP_Text>();
        if(text)
            text.text = GetLocaEntry(token);
        else
            GetComponent<Text>().text = GetLocaEntry(token);
    }

    
}
