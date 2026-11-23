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
        foreach (Commission c in currentCommission)
        {
            if (c.resourceType == ResourceType.O)
                continue;
            if (!currentCounts.ContainsKey(c.presetType))
                currentCounts[c.presetType] = 0;

            currentCounts[c.presetType]++;
        }
        // Filtere Presets, die ihr Limit.y (Maximum) noch nicht erreicht haben
        List<CommissionPreset> available = 
            presets.Where(p => 
            
            {
                if (p.resource == ResourceType.O)
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
            InGameLog.Log("New Commission - Fallback (No Available)");
            CommissionPreset fallback = presets[Random.Range(0, presets.Length)];
            return new CommissionDummy(fallback);
        }
        List<CommissionPreset> pool = available;
        List<CommissionPreset> progressRelativePool = new List<CommissionPreset>();
        float progress = ProgressRelativeWorld.instance.GeneralProgressValue();
        foreach(CommissionPreset presetX in pool)
        {
            if (progress < 0.3f && presetX.type == CommissionPresetType.Standard_Small)
                progressRelativePool.Add(presetX);
            else if (progress < 0.7f && presetX.type == CommissionPresetType.Standard_Mid)
                progressRelativePool.Add(presetX);
            else if (progress > 0.7f && presetX.type == CommissionPresetType.Standard_Big)
                progressRelativePool.Add(presetX);
        }
        // Wenn nichts verfügbar → komplett random als Fallback
        if (progressRelativePool.Count == 0)
        {
            InGameLog.Log("New Commission: Fallback after No Progress Relative   (" + progress + ")");
            CommissionPreset fallback = presets[Random.Range(0, presets.Length)];
            return new CommissionDummy(fallback);
        }

        InGameLog.Log("New Commission - ProgressRelative (" + progress + ")");
        CommissionPreset preset = progressRelativePool[Random.Range(0, progressRelativePool.Count)];
        return new CommissionDummy(preset);
    }

    public CommissionDummy GetRandomOxygenCommission(List<Commission> currentCommission)
    {
        // Zähle aktuelle Aufträge pro Typ
        Dictionary<CommissionPresetType, int> currentCounts = new Dictionary<CommissionPresetType, int>();
        int oxygenCommissionAmounts = 0;
        foreach (Commission c in currentCommission)
        {
            if (c.resourceType != ResourceType.O)
                continue;
            if (!currentCounts.ContainsKey(c.presetType))
            currentCounts[c.presetType] = 0;

            currentCounts[c.presetType]++;
           
            oxygenCommissionAmounts++;
        }
        Debug.Log("O2 Comm: " + oxygenCommissionAmounts);
        if (ProgressRelativeWorld.instance.chapter < 2)
        {
            return null;
        }

        if (ProgressRelativeWorld.instance.chapter == 2)
        {
            if (oxygenCommissionAmounts >= 1)
            {
                Debug.Log("No oxygenCommission - Blocked in ch2 - too many o commissions.");
                return null;
            }
        }
        if (ProgressRelativeWorld.instance.chapter >= 3)
        {
            if (oxygenCommissionAmounts >= 2)
            {
                Debug.Log("No oxygenCommission - Blocked in ch3 - too many o commissions.");
                return null;
            }
        }
            

        // Filtere Presets, die ihr Limit.y (Maximum) noch nicht erreicht haben
           List<CommissionPreset> available =
               presets.Where(p =>

               {
                   if (p.resource != ResourceType.O)
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
            return null;
        }
        List<CommissionPreset> pool = available;
        List<CommissionPreset> progressRelativePool = new List<CommissionPreset>();
        float progress = ProgressRelativeWorld.instance.GeneralProgressValue();
        foreach (CommissionPreset presetX in pool)
        {
            if (progress < 0.3f && presetX.type == CommissionPresetType.Standard_Small)
                progressRelativePool.Add(presetX);
            else if (progress < 0.7f && presetX.type == CommissionPresetType.Standard_Mid)
                progressRelativePool.Add(presetX);
            else if (progress > 0.7f && presetX.type == CommissionPresetType.Standard_Big)
                progressRelativePool.Add(presetX);
        }
        // Wenn nichts verfügbar → komplett random als Fallback
        if (progressRelativePool.Count == 0)
        {
            return null;
        }

        InGameLog.Log("New Commission - ProgressRelative (" + progress + ")");
        CommissionPreset preset = progressRelativePool[Random.Range(0, progressRelativePool.Count)];
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
    public LocaString receiverToken;
    public int icon;
    public string message;

    public int GetAmount()
    {
        return Mathf.RoundToInt( Random.Range(amountLimits.x, amountLimits.y));
    }

    public int GetReward(int selectedAmount)
    {
        float v = Mathf.InverseLerp(amountLimits.x, amountLimits.y, selectedAmount);
        //return Mathf.RoundToInt(Random.Range(rewardLimits.x, rewardLimits.y));
        return Mathf.RoundToInt(Mathf.Lerp(rewardLimits.x, rewardLimits.y, v));
    }
}

[System.Serializable]
public class CommissionDummy
{
    public CommissionDummy(CommissionPreset preset)
    {
        type = preset.type;
        amount = preset.GetAmount();
        reward = preset.GetReward(amount);
        resourceType = preset.resource;
        receiver = preset.receiverToken;
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