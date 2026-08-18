using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "CommissionDatabase", menuName = "Scriptable Objects/CommissionDatabase")]
public class CommissionDatabase : ScriptableObject
{
    public CommissionPreset[] presets;
    public CommissionPresetLimits[] limits;
    public float unlockOxygenRatio = 0.5f;
    /*public CommissionDummy GetRandomCommission(List<Commission> currentCommission)
    {
        CommissionPreset preset = presets[Random.Range(0, presets.Length)];
        CommissionDummy newCommission = new CommissionDummy(preset);
        return newCommission;
    }*/

    public CommissionDummy GetRandomCommission(List<Commission> currentCommission)
    {
        // Zähle aktuelle Aufträge pro Typ
        Dictionary<CommissionPresetType, int> currentCounts = new Dictionary<CommissionPresetType, int>();
        Dictionary<ResourceType, int> currentResourceCounts = new Dictionary<ResourceType, int>();
        currentResourceCounts.Add(ResourceType.H, 0);
        currentResourceCounts.Add(ResourceType.O, 0);
        foreach (Commission c in currentCommission)
        {
            if (!currentCounts.ContainsKey(c.presetType))
                currentCounts[c.presetType] = 0;

            currentCounts[c.presetType]++;
            currentResourceCounts[c.resourceType]++;
        }

        float ratio = ((currentResourceCounts[ResourceType.H]+1) * 1f / (currentResourceCounts[ResourceType.O]+1) * 1f)-1f;
        bool oxygenPossible = ratio > unlockOxygenRatio;
        Debug.Log($"Oxygen possible {ratio}: => {oxygenPossible}");
        // Filtere Presets, die ihr Limit.y (Maximum) noch nicht erreicht haben
        List<CommissionPreset> available = 
            presets.Where(p => 
            
            {
                if (!oxygenPossible && p.resource == ResourceType.O)
                    return false;
                CommissionPresetLimits l = limits.FirstOrDefault(l => l.type == p.type);
            if (l == null) return true; // Kein Limit definiert → immer erlaubt
            int count = currentCounts.ContainsKey(p.type) ? currentCounts[p.type] : 0;
            return count < l.limits.y;
        }
            ).ToList();

        // Wenn nichts verfügbar → komplett random als Fallback
        if (available.Count == 0)
        {
            CommissionPreset fallback = presets[Random.Range(0, presets.Length)];
            return new CommissionDummy(fallback);
        }

        // Priorisiere Typen, die ihr Minimum (limit.x > 0) noch nicht erreicht haben
        List<CommissionPreset> prioritized = available.Where(p => {
            CommissionPresetLimits l = limits.FirstOrDefault(l => l.type == p.type);
            if (l == null || l.limits.x <= 0) return false;
            int count = currentCounts.ContainsKey(p.type) ? currentCounts[p.type] : 0;
            return count < l.limits.x;
        }).ToList();

        List<CommissionPreset> pool = prioritized.Count > 0 ? prioritized : available;
        CommissionPreset preset = pool[Random.Range(0, pool.Count)];
        return new CommissionDummy(preset);
    }
}
[System.Serializable]
public class CommissionPresetLimits
{
    public CommissionPresetType type;
    public Vector2 limits;
}

    [System.Serializable]
public class CommissionPreset
{
    public CommissionPresetType type;
    public Vector2 amountLimits;
    public Vector2 rewardLimits;
    public ResourceType resource;
    public string receiver;
    public int icon;
    public string message;

    public int GetAmount()
    {
        return Mathf.RoundToInt( Random.Range(amountLimits.x, amountLimits.y));
    }

    public int GetReward()
    {
        return Mathf.RoundToInt(Random.Range(rewardLimits.x, rewardLimits.y));
    }
}

[System.Serializable]
public class CommissionDummy
{
    public CommissionDummy(CommissionPreset preset)
    {
        type = preset.type;
        amount = preset.GetAmount();
        reward = preset.GetReward();
        resourceType = preset.resource;
        receiver = preset.receiver;
        icon = preset.icon;
        message = preset.message;
    }

    public CommissionPresetType type;
    public int amount;
    public ResourceType resourceType;
    public int reward;
    public string receiver;
    public int icon;
    public string message;
}

public enum CommissionPresetType
{
    Standard_Small, Standard_Mid, Standard_Big
}