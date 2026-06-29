using UnityEngine;

public class QC_ToggleTaggedObject : QuickConsole_Entry
{
    public string qdcTagToSearch;
    public enum ToggleAction { Toggle, AlwaysOff, AlwaysOn}
    public ToggleAction toggle_Action;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        QDC_TaggedObject[] objects = GameObject.FindObjectsByType<QDC_TaggedObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach(QDC_TaggedObject obj in objects)
        {
            if(obj.qdc_tag.Trim().ToLower() == qdcTagToSearch.ToLower().Trim())
            {
                switch(toggle_Action)
                {
                    case ToggleAction.Toggle:
                        ToggleObject(obj.gameObject);
                        break;
                    case ToggleAction.AlwaysOff:
                        obj.gameObject.SetActive(false);
                        break;
                    case ToggleAction.AlwaysOn:
                        obj.gameObject.SetActive(true);
                        break;
                }
                
            }
        }
    }

    void ToggleObject(GameObject obj)
    {
        if (obj.activeSelf)
            obj.SetActive(false);
        else
            obj.SetActive(true);
    }
}
