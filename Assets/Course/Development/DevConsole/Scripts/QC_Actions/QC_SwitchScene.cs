using UnityEngine;

public class QC_SwitchScene : QuickConsole_Entry
{
    public string sceneName;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
