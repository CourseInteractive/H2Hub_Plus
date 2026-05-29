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
            if(cont)
            {
                machine.Connect(cont);
            }
        }
    }

    public void Disconnect(SelectExitEventArgs args)
    {
        ToSocketInteractable obj = args.interactableObject.transform.gameObject.GetComponent<ToSocketInteractable>();
        Container cont = obj.GetComponent<Container>();
        machine.Disconnect(cont);
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
