using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Socket : MonoBehaviour
{
    public MainMachine machine;

    public ToSocketInteractable lockedInteractable;
    XRSocketInteractor thisInteractor;

    private void Awake()
    {
        thisInteractor = GetComponent<XRSocketInteractor>();
    }

    public void SomethingSnappedToSocket(SelectEnterEventArgs args)
    {
        ToSocketInteractable obj = args.interactableObject.transform.gameObject.GetComponent<ToSocketInteractable>();
        if (obj != null)
        {
            obj.LockToSocket(thisInteractor);
            lockedInteractable = obj;

            Container cont = obj.GetComponent<Container>();
            RepairModule repairM = obj.GetComponent<RepairModule>();
            if (cont)
            {
                machine.Connect(cont);
            }
            else if(repairM)
            {
                machine.Connect(repairM);
                Debug.Log("Connect Module");
            }
        }
    }

    public void Disconnect(SelectExitEventArgs args)
    {
        ToSocketInteractable obj = args.interactableObject.transform.gameObject.GetComponent<ToSocketInteractable>();
        Container cont = obj.GetComponent<Container>();
        RepairModule repairM = obj.GetComponent<RepairModule>();
        if (cont)
        {
            machine.Disconnect(cont);
        }
        else if (repairM)
        {
            machine.Disconnect(repairM);
        }
    }

    public void GiveFreeLockedElement()
    {
        thisInteractor.socketActive = false;
        Invoke("ReenableSocket", 2f);
    }

    public void ReenableSocket()
    {
        thisInteractor.socketActive = true;
    }
}
