using UnityEngine;

public class Recycler : MonoBehaviour
{
    public int payment = 50;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<RepairModule>())
        {
            Destroy(other.gameObject);
            Workshop.Instance.AddMoney(payment);
        }
    }
}
