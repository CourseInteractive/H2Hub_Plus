using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConsole_Entry : MonoBehaviour
{
    public enum EntryType { Layer, Action }
    public EntryType type;

    public QuickConsole_Entry[] content;
    QuickConsole_Entry parent;

    public Sprite optionalIcon;

    // Start is called before the first frame update
    public void Awake()
    {
        IdentifyType();
    }

    public void IdentifyType()
    {
        content = GetDirectChildQuickConsoleEntries();
        if (content.Length > 0)
            type = EntryType.Layer;
        else
            type = EntryType.Action;
    }

    public QuickConsole_Entry GetParent()
    {
        if (parent == null)
            return this;
        return parent;
    }

    public void RegisterParent(QuickConsole_Entry p)
    {
        parent = p;
    }

    public string GetContent()
    {
        string c = "";
        for(int i = 0; i < content.Length; i++) // (QuickConsole_Entry entry in content)
        {
            if(content[i].type == EntryType.Layer)
            {
                c += i + ":" + " Layer: " + content[i].name;
            }
            else
            {
                c += i + ":" + " Action: " + content[i].name;
            }
            c += " | ";
        }
        return c;
    }

    public virtual void ExecuteAction()
    {
        Debug.Log("Action " + gameObject.name);
    }

    QuickConsole_Entry[] GetDirectChildQuickConsoleEntries()
    {
        Transform parentTransform = transform;
        int childCount = parentTransform.childCount;

        // Liste zur Zwischenspeicherung
        var entryList = new System.Collections.Generic.List<QuickConsole_Entry>();

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            QuickConsole_Entry[] list = child.GetComponents<QuickConsole_Entry>();
            if (list == null || list.Length == 0)
                continue;
            QuickConsole_Entry entry = list[0];
            if (entry != null)
            {
                entryList.Add(entry);
                entry.RegisterParent(this);
            }
        }

        return entryList.ToArray();
    }
}
