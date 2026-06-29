using UnityEngine;

public class RepairModule : MonoBehaviour
{
    public bool working = true;

    public AlternatingMeshLight errorLight;

    public ToSocketInteractable socketConnection;

    public void Break()
    {
        working = false;
        errorLight.Activate();

    }

    public void PC_Interaction()
    {
        if(socketConnection.lockedSocket != null)
        {
            socketConnection.FreeFromSocket();
            gameObject.SetActive(false);
        }
        else
        {
            transform.position = MainMachine.instance.repairModuleSocket.transform.position;
            transform.rotation = MainMachine.instance.repairModuleSocket.transform.rotation;
        }
    }
}
