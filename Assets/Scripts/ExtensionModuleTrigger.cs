using UnityEngine;
using Course.PrototypeScripting;

public class ExtensionModuleTrigger : MonoBehaviour
{

    public ExtensionModule.ModuleType neededType;

    public ExtensionModuleTriggerCombination combi;

    public GameObject objectToActivate;

    public bool occupied;

    public Sequence sequenceOnBuild;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(objectToActivate)
        objectToActivate.SetActive(false);

        gameObject.SetActive(false);
    }


    private void OnTriggerEnter(Collider other)
    {
        ExtensionModule module = other.gameObject.GetComponent<ExtensionModule>();
        if (module.modulType == neededType)
        {
            occupied = true;
            if (objectToActivate)
                objectToActivate.SetActive(true);
            if (combi)
                combi.ReportConnectedTrigger();
            if(sequenceOnBuild)
                sequenceOnBuild.ExecuteCompleteSequence();

            gameObject.SetActive(false);
            other.gameObject.SetActive(false);
        }
       
    }

}
