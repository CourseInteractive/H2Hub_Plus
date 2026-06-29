using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LokaKitNew", menuName = "COURSE/LokaKit", order = 1)]
public class LokaKit : ScriptableObject
{
    public int nextLineID = 0;
    public List<LokaKitEntry> entries;
    public List<string> tagList;
    public List<string> types;
    public int languageCount = 1;

    public List<LokaKitExportOption> exportOptions;

    public LokaKit()
    {
        tagList = new List<string>();
        tagList.Add("TEST");
    }


    public LokaKitEntry GetLine(int lineID)
    {
        if (lineID > entries.Count)
        {
            LokaKitEntry entry = entries[lineID];
            if (entry != null && entry.lineID == lineID)
                return entry;
        }
            

        foreach(LokaKitEntry entry in entries)
        {
            if(entry.lineID == lineID)
                return entry;
        }
        return null;
    }

    public int GetLineID(string token)
    {
        foreach (LokaKitEntry entry in entries)
        {
            if (entry.token == token)
                return entry.lineID;
        }
        return -1;
    }

    public string GetLocaStringByToken(string token, int language)
    {
        foreach (LokaKitEntry entry in entries)
        {
            if (entry.token.ToLower() == token.ToLower())
                return entry.translationText[language].Replace("§§", "\n");
        }
        token = token.Replace("§§", "\n");
        return token;
        // Todo: Build Setup: Highlight plain Texts (also Texte die noch nicht lokalisiert wurden besonders kennzeichnen)
        return "-" + token + "-";
    }

    public string[] GetTypeListWithZero()
    {
        List<string> newList = new List<string>();
        newList.Add("All");
        foreach (string type in types)
        {
            newList.Add(type);
        }
        return newList.ToArray();
    }

    public string[] GetTagListWithZero()
    {
        List<string> newList = new List<string>();
        newList.Add("All");
        foreach (string tag in tagList)
        {
            newList.Add(tag);
        }
        return newList.ToArray();
    }

    public void AddNewEntry(LokaKitEntry newEntry)
    {
        newEntry.lineID = nextLineID;
        nextLineID++;
        entries.Add(newEntry);
    }
}
[System.Serializable]

public class LokaKitEntry
{
    public int lineID;
    public string token;
    public int textType;
    public List<string> translationText;
    public string lastChangedInVersion;
    public int tagMask;

    public LokaKitEntry()
    {

    }

    public LokaKitEntry(LokaKitEntry oldEntry)
    {
        lineID = oldEntry.lineID;
        token = oldEntry.token;
        textType = oldEntry.textType;
        translationText = oldEntry.translationText;
        lastChangedInVersion = oldEntry.lastChangedInVersion;
        tagMask = oldEntry.tagMask;
    }

    public LokaKitEntry(int _lineID, string startContent, string b, string _token, int c, int type, bool jo)
    {
        lineID = _lineID;
        token = _token;
        translationText = new List<string>();
        translationText.Add(startContent);
        translationText.Add("");
    }
}

[System.Serializable]
public class LokaKitExportOption
{
    public string name;
    public List<int> categoryIndices;
}
