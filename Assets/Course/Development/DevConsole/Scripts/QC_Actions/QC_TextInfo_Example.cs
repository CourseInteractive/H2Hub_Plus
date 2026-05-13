using UnityEngine;

public class QC_TextInfo_Example : QuickConsole_Entry
{
    int counter = 0;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        QuickConsole.instance.display.SetInfoText("Info from last Button: " + counter);
            counter++;
    }
}
