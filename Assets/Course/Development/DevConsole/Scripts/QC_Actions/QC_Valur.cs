using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QC_Valur : QuickConsole_Entry
{
    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        QuickConsole.instance.Close();
        VALUR.Data.Open();
    }
}
