using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QC_UnityEvent : QuickConsole_Entry
{
    public UnityEvent action;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        action.Invoke();
    }
}
