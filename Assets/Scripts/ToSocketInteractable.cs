using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


public class ToSocketInteractable : MonoBehaviour
{
    public string identifier;
    public XRSocketInteractor lockedSocketInteractor;
    public Socket lockedSocket;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Free")]
    public void FreeFromSocket()
    {
        if (lockedSocket)
            lockedSocket.GiveFreeLockedElement();


    }

    public void LockToSocket(XRSocketInteractor interactor)
    {
        lockedSocketInteractor = interactor;
        lockedSocket = interactor.GetComponent<Socket>();
    }

  
}
