using UnityEngine;
using Course.PrototypeScripting;
public class ExtensionModuleTriggerCombination : MonoBehaviour
{
    public ExtensionModuleTrigger[] connectedTriggers;
    public GameObject objectToActivate;
    public Sequence sequenceOnBuild;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReportConnectedTrigger()
    {
        foreach (ExtensionModuleTrigger trigger in connectedTriggers)
        {
            if (!trigger.occupied)
                return;
        }
        Activate();
    }

    [ContextMenu("Activate")]
    public void Activate()
    {
        if (objectToActivate)
            objectToActivate.SetActive(true);
        if (sequenceOnBuild)
            sequenceOnBuild.ExecuteCompleteSequence();
    }
}
