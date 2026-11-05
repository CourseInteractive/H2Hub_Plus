using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// Welcher Spielwert von einer Bedingung beobachtet wird.
/// </summary>
public enum ProgressionType
{
    Money,            // aufaddiertes verdientes Geld
    Hydrogen,         // aufaddierter produzierter Wasserstoff
    Time,             // Spielzeit in Stunden (8.5 = 08:30)
    CompletedOrders   // Anzahl erfüllter Aufträge
}

/// <summary>
/// Kleinste Bedingungseinheit: "Wert >= Schwelle" -> GameEvent("Progression", eventParameter)
/// </summary>
[Serializable]
public class ProgressionCondition
{
    [Tooltip("Welcher Wert geprüft wird.")]
    public ProgressionType type;

    [Tooltip("Schwelle. Feuert, wenn der Wert von unter der Schwelle auf >= Schwelle steigt. Zeit in Stunden (8.5 = 08:30).")]
    public double threshold;

    [Tooltip("Parameter des GameEvents. Der Key ist immer \"Progression\".")]
    public string eventParameter;
}

/// <summary>
/// Beobachtet den Spielfortschritt und feuert GameEvents, sobald eine Schwelle überschritten wird.
///
/// Das Spiel meldet nur Zuwächse (Deltas). Der Observer führt intern eigene Zähler,
/// die per ResetValue / ResetAllValues auf 0 gesetzt werden können. Danach zählt er neu,
/// und Bedingungen können beim nächsten Überschreiten erneut feuern.
/// </summary>
public class ProgressionObserver : MonoBehaviour
{
    public static ProgressionObserver Instance { get; private set; }

    private const string EventKey = "Progression";

    [SerializeField]
    private ProgressionCondition[] conditions = new ProgressionCondition[0];

    // Interne Zähler (seit Start bzw. seit letztem Reset)
    private int money;
    private float hydrogen;
    private float time;
    private int completedOrders;

    // Bedingungen nach Typ gruppiert und aufsteigend nach Schwelle sortiert.
    // Bei gleicher Schwelle bleibt die Reihenfolge aus dem Inspector erhalten (stabile Sortierung).
    private Dictionary<ProgressionType, ProgressionCondition[]> conditionsByType;

    #region Unity

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[ProgressionObserver] Es existiert bereits eine Instanz. Diese wird zerstört.", this);
            Destroy(this);
            return;
        }
        Instance = this;
        BuildLookup();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void OnValidate()
    {
        // Erlaubt Änderungen an den Bedingungen im Inspector während Play Mode
        if (Application.isPlaying) BuildLookup();
    }

    #endregion

    #region Zuwächse melden (vom Spiel aufgerufen)

    /// <summary>Spieler hat Geld verdient.</summary>
    public void AddMoney(int amount)
    {
        if (!IsValidDelta(amount, ProgressionType.Money)) return;
        int oldValue = money;
        money += amount;
        CheckConditions(ProgressionType.Money, oldValue, money);
    }

    /// <summary>Spieler hat Wasserstoff produziert.</summary>
    public void AddHydrogen(float amount)
    {
        if (!IsValidDelta(amount, ProgressionType.Hydrogen)) return;
        float oldValue = hydrogen;
        hydrogen += amount;
        CheckConditions(ProgressionType.Hydrogen, oldValue, hydrogen);
    }

    /// <summary>Spielzeit ist um deltaHours vorangeschritten.</summary>
    public void AddTime(float deltaHours)
    {
        if (!IsValidDelta(deltaHours, ProgressionType.Time)) return;
        float oldValue = time;
        time += deltaHours;
        CheckConditions(ProgressionType.Time, oldValue, time);
    }

    /// <summary>Spieler hat Aufträge erfüllt (Standard: einen).</summary>
    public void AddCompletedOrders(int amount = 1)
    {
        if (!IsValidDelta(amount, ProgressionType.CompletedOrders)) return;
        int oldValue = completedOrders;
        completedOrders += amount;
        CheckConditions(ProgressionType.CompletedOrders, oldValue, completedOrders);
    }

    #endregion

    #region Reset

    /// <summary>Setzt einen einzelnen Zähler auf 0. Bedingungen dieses Typs können danach erneut feuern.</summary>
    public void ResetValue(ProgressionType type)
    {
        switch (type)
        {
            case ProgressionType.Money:           money = 0; break;
            case ProgressionType.Hydrogen:        hydrogen = 0; break;
            case ProgressionType.Time:            time = 0f; break;
            case ProgressionType.CompletedOrders: completedOrders = 0; break;
        }
    }

    /// <summary>Setzt alle Zähler auf 0.</summary>
    public void ResetAllValues()
    {
        money = 0;
        hydrogen = 0;
      //  time = 0f;
        completedOrders = 0;
    }

    #endregion

    #region Werte fest setzen

    // Setzt einen Zähler direkt auf einen Wert, z.B. um den Startpunkt festzulegen.
    // Standardmäßig wird dabei NICHTS gefeuert: Schwellen, die durch das Setzen
    // übersprungen werden, gelten als bereits erreicht.
    // Mit fireEvents = true wird ein Anstieg wie ein normaler Zuwachs behandelt
    // (alle dazwischenliegenden Schwellen feuern). Ein Absenken feuert nie.

    /// <summary>Setzt das gezählte Geld auf einen festen Wert.</summary>
    public void SetMoney(int value, bool fireEvents = false)
    {
        if (!IsValidSetValue(value, ProgressionType.Money)) return;
        int oldValue = money;
        money = value;
        if (fireEvents) CheckConditions(ProgressionType.Money, oldValue, money);
    }

    /// <summary>Setzt den gezählten Wasserstoff auf einen festen Wert.</summary>
    public void SetHydrogen(float value, bool fireEvents = false)
    {
        if (!IsValidSetValue(value, ProgressionType.Hydrogen)) return;
        float oldValue = hydrogen;
        hydrogen = value;
        if (fireEvents) CheckConditions(ProgressionType.Hydrogen, oldValue, hydrogen);
    }

    /// <summary>Setzt die Zeit auf einen festen Wert in Stunden (8.5 = 08:30).</summary>
    public void SetTime(float currentTime, bool fireEvents = false)
    {
        if (!IsValidSetValue(currentTime, ProgressionType.Time)) return;
        float oldValue = time;
        time = currentTime;
        if (fireEvents) CheckConditions(ProgressionType.Time, oldValue, time);
    }

    /// <summary>Setzt die Anzahl erfüllter Aufträge auf einen festen Wert.</summary>
    public void SetCompletedOrders(int value, bool fireEvents = false)
    {
        if (!IsValidSetValue(value, ProgressionType.CompletedOrders)) return;
        int oldValue = completedOrders;
        completedOrders = value;
        if (fireEvents) CheckConditions(ProgressionType.CompletedOrders, oldValue, completedOrders);
    }

    /// <summary>Setzt alle vier Zähler auf einmal.</summary>
    public void SetAllValues(int money, int hydrogen, float time, int completedOrders, bool fireEvents = false)
    {
        SetMoney(money, fireEvents);
        SetHydrogen(hydrogen, fireEvents);
        SetTime(time, fireEvents);
        SetCompletedOrders(completedOrders, fireEvents);
    }

    #endregion

    #region Debug (extern aufrufbar, zusätzlich per Rechtsklick auf die Component im Inspector)

    /// <summary>Gibt alle aktuellen Zählerstände in der Konsole aus.</summary>
    [ContextMenu("Debug: Aktuelle Werte ausgeben")]
    public void LogCurrentValues()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[ProgressionObserver] Aktuelle Werte:");
        sb.AppendLine($"  Geld:              {money}");
        sb.AppendLine($"  Wasserstoff:       {hydrogen}");
        sb.AppendLine($"  Zeit:              {time:0.###} h ({FormatTime(time)})");
        sb.AppendLine($"  Erfüllte Aufträge: {completedOrders}");
        Debug.Log(sb.ToString(), this);
        InGameLog.Log(sb.ToString());
    }

    /// <summary>Gibt alle Bedingungen mit ihren Events aus, gruppiert und sortiert wie sie geprüft werden.</summary>
    [ContextMenu("Debug: Bedingungen ausgeben")]
    public void LogConditions()
    {
        if (conditionsByType == null) BuildLookup();

        var sb = new StringBuilder();
        sb.AppendLine($"[ProgressionObserver] {conditions?.Length ?? 0} Bedingung(en):");

        foreach (ProgressionType type in Enum.GetValues(typeof(ProgressionType)))
        {
            ProgressionCondition[] list = conditionsByType[type];
            if (list.Length == 0) continue;

            double current = GetValue(type);
            sb.AppendLine($"  {type}:");
            foreach (ProgressionCondition c in list)
            {
                string thresholdText = type == ProgressionType.Time
                    ? $"{c.threshold:0.###} h ({FormatTime((float)c.threshold)})"
                    : c.threshold.ToString("0.##");
                string state = current >= c.threshold ? "erreicht" : "offen";
                sb.AppendLine($"    >= {thresholdText}  ->  GameEvent(\"{EventKey}\", \"{c.eventParameter}\")  [{state}]");
            }
        }
        Debug.Log(sb.ToString(), this);
        InGameLog.Log(sb.ToString());
    }

    #endregion

    #region Intern

    private void BuildLookup()
    {
        conditionsByType = new Dictionary<ProgressionType, ProgressionCondition[]>();
        IEnumerable<ProgressionCondition> valid = (conditions ?? new ProgressionCondition[0]).Where(c => c != null);

        foreach (ProgressionType type in Enum.GetValues(typeof(ProgressionType)))
        {
            // OrderBy ist stabil: gleiche Schwellen behalten die Inspector-Reihenfolge
            conditionsByType[type] = valid
                .Where(c => c.type == type)
                .OrderBy(c => c.threshold)
                .ToArray();
        }
    }

    /// <summary>
    /// Feuert alle Bedingungen, deren Schwelle zwischen altem (exklusiv) und neuem Wert (inklusiv) liegt,
    /// also: oldValue < Schwelle <= newValue. Reihenfolge aufsteigend nach Schwelle.
    /// </summary>
    private void CheckConditions(ProgressionType type, double oldValue, double newValue)
    {
        if (conditionsByType == null) BuildLookup();

        foreach (ProgressionCondition c in conditionsByType[type])
        {
            if (c.threshold > newValue) break;          // sortiert: alle weiteren liegen auch drüber
            if (oldValue < c.threshold) Fire(c);        // vorher drunter, jetzt >= Schwelle
        }
    }

    private void Fire(ProgressionCondition condition)
    {
        if (GameEventManager.Instance == null)
        {
            Debug.LogWarning($"[ProgressionObserver] Kein GameEventManager vorhanden. Event \"{condition.eventParameter}\" wurde nicht gesendet.", this);
            return;
        }

        GameEvent nEvent = new GameEvent(EventKey, condition.eventParameter);
        GameEventManager.Instance.ReportGameEvent(nEvent);
    }

    private bool IsValidDelta(double delta, ProgressionType type)
    {
        if (delta < 0)
        {
            Debug.LogWarning($"[ProgressionObserver] Negativer Zuwachs ({delta}) für {type} ignoriert. Werte können nur steigen.", this);
            return false;
        }
        return delta > 0;
    }

    private bool IsValidSetValue(double value, ProgressionType type)
    {
        if (value < 0)
        {
            Debug.LogWarning($"[ProgressionObserver] Negativer Wert ({value}) für {type} ignoriert.", this);
            return false;
        }
        return true;
    }

    private double GetValue(ProgressionType type)
    {
        switch (type)
        {
            case ProgressionType.Money:           return money;
            case ProgressionType.Hydrogen:        return hydrogen;
            case ProgressionType.Time:            return time;
            case ProgressionType.CompletedOrders: return completedOrders;
            default:                              return 0;
        }
    }

    private static string FormatTime(float hours)
    {
        TimeSpan t = TimeSpan.FromHours(hours);
        return $"{(int)t.TotalHours:00}:{t.Minutes:00}:{t.Seconds:00}";
    }

    #endregion
}
