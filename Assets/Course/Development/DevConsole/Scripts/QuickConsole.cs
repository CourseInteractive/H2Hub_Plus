using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickConsole : MonoBehaviour
{
    public static QuickConsole instance;
    public QuickConsole_Entry rootEntry;
    public QuickConsole_Entry currentEntry;
    public QuickConsole_Entry lastEntry;
    public QuickConsoleDisplay display;

    public List<InputActionReference> numKeys = new List<InputActionReference>(10);

    private readonly int[] keyMappings = new int[10]
    {
        6, // num1
        7, // num2
        8, // num3
        3, // num4
        4, // num5
        5, // num6
        0, // num7
        1, // num8
        2, // num9
       -1  // num0 → special case: LayerUp
    };

    void Start()
    {
        if (instance != null)
            return;

        instance = this;

        foreach (var actionRef in numKeys)
        {
            actionRef?.action.Enable();
        }

        rootEntry = currentEntry;
        OpenEntry(currentEntry);
    }

    void Update()
    {
        for (int i = 0; i < numKeys.Count; i++)
        {
            if (numKeys[i] != null && numKeys[i].action.triggered)
            {
                if (i == 9) // num0
                {
                    LayerUp();
                }
                else
                {
                    ExecuteByKeyboard(keyMappings[i]);
                }
            }
        }
    }

    void OpenEntry(QuickConsole_Entry entry)
    {
        if (currentEntry != null)
            lastEntry = currentEntry;

        currentEntry = entry;

        if (lastEntry == null)
            lastEntry = currentEntry;

        display.UpdateDisplay();
    }

    public void LayerUp()
    {
        OpenEntry(currentEntry.GetParent());
    }

    public void ExecuteByKeyboard(int index)
    {
        display.ShowPressFor(index);
        pressedIndex = index;
        Invoke("ExecuteOnEntry", display.pressDelay);
    }

    int pressedIndex;

    public void ExecuteEntryByButton(int index)
    {
        pressedIndex = index;
        Invoke("ExecuteOnEntry", display.pressDelay);
    }

    public void ExecuteLayerUpByButton()
    {
        LayerUp();
    }

    void ExecuteOnEntry()
    {
        ExecuteOnEntry(pressedIndex);
    }

    void ExecuteOnEntry(int index)
    {
        if (currentEntry.content.Length <= index)
        {
            Debug.Log("No Entry!");
            return;
        }

        if (currentEntry.content[index].type == QuickConsole_Entry.EntryType.Action)
        {
            currentEntry.content[index].ExecuteAction();
            display.UpdateDisplay();
        }
        else
        {
            OpenEntry(currentEntry.content[index]);
        }
    }

    public void Close() => display.Close();
    public void Open() => display.Show();
    public void ResetToRoot() => OpenEntry(rootEntry);
}
