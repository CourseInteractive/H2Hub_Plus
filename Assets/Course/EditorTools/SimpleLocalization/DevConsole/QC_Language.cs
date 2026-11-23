using UnityEngine;

public class QC_Language : QuickConsole_Entry
{

    public LocaLanguage language;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        SimpleLocalization.LanguageChanged((int)language);
    }
}
