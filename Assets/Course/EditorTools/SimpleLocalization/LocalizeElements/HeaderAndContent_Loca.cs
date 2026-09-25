using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HeaderAndContent_Loca : LokaElement
{
    public TMP_Text header;
    public TMP_Text content;

    public override void LocalizeElement()
    {
        if (header)
            header.text = "";
        if (content)
            content.text = "";


        string[] parts = GetLocaEntry(token).Split('~');
        if (parts.Length == 1 && content)
            content.text = parts[0];
        else if(header)
            header.text = parts[0];
        if (content && parts.Length > 1)
        {
            content.text = parts[1];
        }
    }
}
