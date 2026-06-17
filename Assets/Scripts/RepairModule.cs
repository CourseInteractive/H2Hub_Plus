using UnityEngine;

public class RepairModule : MonoBehaviour
{
    public bool working = true;

    public AlternatingMeshLight errorLight;

    public void Break()
    {
        working = false;
        errorLight.Activate();

    }
}
