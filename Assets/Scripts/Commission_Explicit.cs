using UnityEngine;

public class Commission_Explicit : MonoBehaviour
{

    public CommissionDummy commission;

    public void AddToList()
    {
        CommissionManager.instance.AddCommission(commission);
    }
}
