using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLoca : LokaElement
{
     public override void LocalizeElement()
    {
        base.LocalizeElement();
        FindText();
        textElement.text = GetLocaEntry(token);
       
    }
    Text textElement;
    void FindText()
    {
        if (textElement != null)
            return;
        textElement = transform.GetComponentInChildren<Text>();
    }
}
