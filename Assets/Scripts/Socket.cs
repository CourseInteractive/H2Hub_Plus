using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Socket : MonoBehaviour
{
    public MainMachine machine;

    public ToSocketInteractable lockedInteractable;
    XRSocketInteractor thisInteractor;

    public RepairModule.RepairKind kind;

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


            
            GameEventManager.Instance.ReportGameEvent("SocketConnect", gameObject.name + "-" + obj.identifier);
            if (!machine)
                return;
            if (cont)
            {
                machine.Connect(cont);
            }
            else if(repairM)
            {
                if(repairM.kind == kind)
                {
                    machine.Connect(repairM);
                    Debug.Log("Connect Module");
                }
                else
                {
                    Debug.Log("Wrong Module");
                }
            }
        }
    }

    public void Disconnect(SelectExitEventArgs args)
    {
        ToSocketInteractable obj = args.interactableObject.transform.gameObject.GetComponent<ToSocketInteractable>();
        Container cont = obj.GetComponent<Container>();
        RepairModule repairM = obj.GetComponent<RepairModule>();
        GameEventManager.Instance.ReportGameEvent("SocketDisconnect", gameObject.name + "-" + obj.identifier);
        if (!machine)
            return;
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
