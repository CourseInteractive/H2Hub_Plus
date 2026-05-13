using UnityEngine;

[CreateAssetMenu(fileName = "CommissionDatabase", menuName = "Scriptable Objects/CommissionDatabase")]
public class CommissionDatabase : ScriptableObject
{
    public CommissionPreset[] presets;

    public CommissionDummy GetRandomCommission()
    {
        CommissionPreset preset = presets[Random.Range(0, presets.Length)];
        CommissionDummy newCommission = new CommissionDummy(preset);
        return newCommission;
    }
}

[System.Serializable]
public class CommissionPreset
{
    public CommissionPresetType type;
    public Vector2 amountLimits;
    public Vector2 rewardLimits;

    public int GetAmount()
    {
        return Mathf.RoundToInt( Random.Range(amountLimits.x, amountLimits.y));
    }

    public int GetReward()
    {
        return Mathf.RoundToInt(Random.Range(rewardLimits.x, rewardLimits.y));
    }
}

public class CommissionDummy
{
    public CommissionDummy(CommissionPreset preset)
    {
        type = preset.type;
        amount = preset.GetAmount();
        reward = preset.GetReward();
    }

    public CommissionPresetType type;
    public int amount;
    public int resourceType;
    public int reward;
}

public enum CommissionPresetType
{
    Standard_Small, Standard_Mid, Standard_Big
}