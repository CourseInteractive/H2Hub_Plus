using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class RepairModule : MonoBehaviour
{
    public bool working = true;

    public AlternatingMeshLight errorLight;

    public ToSocketInteractable socketConnection;

    public enum RepairKind { MainMachine, Water }
    public RepairKind kind;

    public float amountUsed;
    public float nextDefect;
    public Vector2 randomDefectLimits;

    private void Awake()
    {
        nextDefect = Random.Range(randomDefectLimits.x, randomDefectLimits.y);
    }

    [ContextMenu("Break")]
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

    public void UseUpAmount(float amount)
    {
        amountUsed += amount;
        if(amountUsed > nextDefect)
        {
            Break();
        }
    }
    [ContextMenu("Destroy")]
    public void DestroyForTesting()
    {
        socketConnection.FreeFromSocket();
        Destroy(gameObject);
    }

    public void SetNearlyBroken(int amountLeft)
    {
        amountUsed = nextDefect - amountLeft;
    }

    Rigidbody rBody;
    XRGrabInteractable grabable;
    public void SetGrabActivity(bool value)
    {
        grabable = GetComponent<XRGrabInteractable>();
        rBody = GetComponent<Rigidbody>();
        if (grabable)
        {
            if(value)
            {
                rBody.useGravity = true;
                grabable.enabled = true;
                rBody.isKinematic = false;
            }
            else
            {
                rBody.useGravity = false;
                rBody.isKinematic = true;
                grabable.enabled = false;
            }

        }
    }
}
