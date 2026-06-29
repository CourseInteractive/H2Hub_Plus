using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConsoleDisplay : MonoBehaviour
{
    public QuickConsole console;
    public QuickConsoleKeyDisplay[] keyDisplays;
    public GameObject mainPanel;
    public bool visible;
    [Header("Optics")]
    public float gotPressedFeedbackTime = 0.2f;
    public float gotPressedScaling = 0.85f;
    public float pressDelay = 0.1f;

    public UnityEngine.UI.Text quickInfoText;

    void Start()
    {
        Close();
        if(quickInfoText)
        quickInfoText.text = "";
    }

    public void UpdateDisplay()
    {
        for(int i = 0; i < keyDisplays.Length; i++)
        {
            if (i >= console.currentEntry.content.Length || !console.currentEntry.content[i].gameObject.activeInHierarchy)
                keyDisplays[i].SetUp(false, "", i, console);
            else
            {
                keyDisplays[i].SetUp(true, console.currentEntry.content[i].name, i, console, console.currentEntry.content[i].optionalIcon);
            }
        }
    }

    public void ShowPressFor(int index)
    {
        keyDisplays[index].GotPressed();
    }
    public void Show()
    {
        mainPanel.SetActive(true);
        visible = true;
    }

    public void Close()
    {
        mainPanel.SetActive(false);
        visible = false;
    }

    public void SetInfoText(string info)
    {
        if(quickInfoText != null)
            quickInfoText.text = info;
    }
       
}
