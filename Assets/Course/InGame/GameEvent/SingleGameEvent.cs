using UnityEngine;

public class SingleGameEvent : MonoBehaviour
{
    public string eventKey;
    public bool useOwnGameObjectNameAsParameter;

    [ContextMenu("Report")]
    public void ReportGameEvent(string parameter)
    {
        GameEvent nEvent = new GameEvent(eventKey, parameter);
        if (useOwnGameObjectNameAsParameter)
            nEvent.eventParameter = gameObject.name;
        GameEventManager.Instance.ReportGameEvent(nEvent);
    }
}
