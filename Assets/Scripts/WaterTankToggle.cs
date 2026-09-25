using UnityEngine;

public class WaterTankToggle : MonoBehaviour
{
    public Container waterContainer;

    public ObjectTransformLibrary animator;

    private void Awake()
    {
        if (waterContainer.ImmediateRefill)
        {
            animator.SetStateInstant("Open");
           
        }
        else
        {
            animator.SetStateInstant("Closed");
        }
    }

    public void ToggleHit()
    {
        if(waterContainer.ImmediateRefill)
        {
            animator.MoveIntoState("Closed", 0.7f);
            waterContainer.ImmediateRefill = false;
        }
        else
        {
            animator.MoveIntoState("Open", 0.7f);
            waterContainer.ImmediateRefill = true;
        }
    }

}
