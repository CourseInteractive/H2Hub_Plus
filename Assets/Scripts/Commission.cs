using System;
using UnityEngine;

/// <summary>
/// Represents a commission (Auftrag) with a reward, amount, and resource type.
/// </summary>
[Serializable]
public class Commission : MonoBehaviour
{
    [Header("Commission Data")]
    [Tooltip("The reward granted upon completing this commission.")]
    public float reward;

    [Tooltip("The required amount of the resource.")]
    public int amount;

    [Tooltip("The type of resource required (as integer ID).")]
    public int resourceType;

    public int internIndex;

    // ── Constructors ────────────────────────────────────────────────────────

    // ── Helpers ─────────────────────────────────────────────────────────────

    /// <summary>Returns a human-readable summary of the commission.</summary>
    public override string ToString()
    {
        return $"Commission [ResourceType={resourceType}, Amount={amount}, Reward={reward}]";
    }

    public void PerformAndComplete()
    {
       
        MainMachine.instance.RemoveHydrogen(amount);
        Workshop.Instance.AddMoney((int)reward);
        CommissionManager.instance.Remove(internIndex);
    }

    public bool CanBePerformed()
    {
        if (!MainMachine.instance.AmountAvailableInOutput(amount, ResourceType.H))
        {
            return false;
        }
        return true;
    }

    public void Initialize(CommissionDummy dummy, int index)
    {
        reward = dummy.reward;
        amount = dummy.amount;
        internIndex = index;
    }
}
