using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class ToSocketInteractable : MonoBehaviour
{
    public string identifier;
    public XRSocketInteractor lockedSocketInteractor;
    public Socket lockedSocket;

    [ContextMenu("Free")]
    public void FreeFromSocket()
    {
        if (lockedSocket)
            lockedSocket.GiveFreeLockedElement();
        ReactToDisconnect();
    }

    public void ReactToDisconnect()
    {
        lockedSocketInteractor = null;
        lockedSocket = null;
    }

    public void LockToSocket(XRSocketInteractor interactor)
    {
        lockedSocketInteractor = interactor;
        lockedSocket = interactor.GetComponent<Socket>();
    }

  
}
