using UnityEngine;

public class WarmthPipeModule : Container
{
    public bool running;

    public void LateUpdate()
    {
        if(currentAmount >= 1f)
        {
            ShowFlowIndicator();
            Village.Instance.SellContentOfContainer(this, Mathf.FloorToInt(currentAmount));
        }
            
    }
}
