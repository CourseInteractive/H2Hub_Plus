using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Steuert einen Tag/Nacht-Zyklus mit einem Directional Light.
/// 
/// SETUP:
/// 1. Dieses Script einer beliebigen GameObject hinzufügen.
/// 2. Das Directional Light (Sonne) im Inspector zuweisen.
/// 3. Zeitbereiche (Time Ranges) im Inspector konfigurieren.
/// 4. Tagesgeschwindigkeit anpassen.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Datenstrukturen
    // ─────────────────────────────────────────────

    [Serializable]
    public class TimeRange
    {
        [Tooltip("Name dieses Zeitbereichs (z.B. 'Morgen', 'Nacht').")]
        public string rangeName = "Neuer Bereich";

        [Tooltip("Startzeit in Stunden (0–24).")]
        [Range(0f, 24f)] public float startHour = 6f;

        [Tooltip("Endzeit in Stunden (0–24).")]
        [Range(0f, 24f)] public float endHour = 8f;

        [Tooltip("Wird aufgerufen, wenn dieser Bereich beginnt.")]
        public UnityEvent onRangeStart;

        [Tooltip("Wird aufgerufen, wenn dieser Bereich endet.")]
        public UnityEvent onRangeEnd;

        // Interne Zustandsverfolgung (nicht sichtbar im Inspector)
        [HideInInspector] public bool isActive = false;
    }

    // ─────────────────────────────────────────────
    // Inspector-Felder
    // ─────────────────────────────────────────────

    [Header("Licht")]
    [Tooltip("Das Directional Light, das als Sonne fungiert.")]
    [SerializeField] private Light sunLight;

    [Tooltip("Rotation bei 0 Uhr (Mitternacht).")]
    [SerializeField] private Vector3 rotationAtMidnight = new Vector3(-90f, 0f, 0f);

    [Tooltip("Rotation bei 12 Uhr (Mittag).")]
    [SerializeField] private Vector3 rotationAtNoon = new Vector3(90f, 0f, 0f);

    [Header("Zeit")]
    [Tooltip("Startzeit beim Spielbeginn (0–24 Stunden).")]
    [Range(0f, 24f)]
    [SerializeField] private float startHour = 6f;

    [Tooltip("Wie viele Spielminuten pro echter Sekunde vergehen.\nZ.B. 1 = Echtzeit, 60 = 1 Spielminute pro Sekunde.")]
    [SerializeField] private float timeMultiplier = 60f;

    [Header("Zeitbereiche")]
    [Tooltip("Definiere Zeitbereiche, die beim Start/Ende Events auslösen.")]
    [SerializeField] private List<TimeRange> timeRanges = new List<TimeRange>();

    // ─────────────────────────────────────────────
    // Private Felder
    // ─────────────────────────────────────────────

    // Aktuelle Tageszeit in Stunden (0–24)
    private float currentHour;

    // Ob der Zyklus läuft
    private bool isRunning = true;

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    /// <summary>Aktuelle Uhrzeit in Stunden (0–24).</summary>
    public float CurrentHour => currentHour;

    /// <summary>Ob der Zyklus gerade läuft.</summary>
    public bool IsRunning => isRunning;

    public static DayNightCycle instance;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Start()
    {
        currentHour = startHour;
        instance = this;
        if (sunLight == null)
        {
            Debug.LogError("[DayNightCycle] Kein Directional Light zugewiesen! Bitte im Inspector setzen.");
        }

        // Initialen Zustand der Zeitbereiche evaluieren
        foreach (var range in timeRanges)
        {
            range.isActive = IsHourInRange(currentHour, range);
        }

        ApplyRotation();
    }

    private void Update()
    {
        if (!isRunning) return;

        AdvanceTime();
        ApplyRotation();
        EvaluateTimeRanges();
    }

    // ─────────────────────────────────────────────
    // Öffentliche Methoden
    // ─────────────────────────────────────────────

    /// <summary>Pausiert den Tag/Nacht-Zyklus.</summary>
    public void Pause()
    {
        isRunning = false;
        Debug.Log("[DayNightCycle] Zyklus pausiert.");
    }

    /// <summary>Setzt den Tag/Nacht-Zyklus fort.</summary>
    public void Resume()
    {
        isRunning = true;
        Debug.Log("[DayNightCycle] Zyklus fortgesetzt.");
    }

    /// <summary>Wechselt zwischen Pause und Fortsetzen.</summary>
    public void TogglePause()
    {
        if (isRunning) Pause();
        else Resume();
    }

    /// <summary>Setzt die aktuelle Uhrzeit direkt (0–24).</summary>
    public void SetTime(float hour)
    {
        currentHour = Mathf.Repeat(hour, 24f);
        ApplyRotation();

        // Zeitbereiche neu evaluieren ohne Events auszulösen
        foreach (var range in timeRanges)
        {
            range.isActive = IsHourInRange(currentHour, range);
        }
    }

    /// <summary>Setzt den Zeitmultiplikator (Spielminuten pro Echtzeit-Sekunde).</summary>
    public void SetTimeMultiplier(float multiplier)
    {
        timeMultiplier = Mathf.Max(0f, multiplier);
    }

    /// <summary>
    /// Gibt die aktuelle Uhrzeit als formatierten String zurück (HH:MM).
    /// </summary>
    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);
        return $"{hours:D2}:{minutes:D2}";
    }

    // ─────────────────────────────────────────────
    // Private Methoden
    // ─────────────────────────────────────────────

    /// <summary>Schreitet die Zeit basierend auf dem Multiplikator fort.</summary>
    private void AdvanceTime()
    {
        // timeMultiplier = Spielminuten/Echtzeitsekunde
        // Pro Update: (timeMultiplier / 60) Spielstunden vergehen pro Echtzeitsekunde
        float hoursPerSecond = timeMultiplier / 60f;
        currentHour += hoursPerSecond * Time.deltaTime;
        currentHour = Mathf.Repeat(currentHour, 24f); // 0–24 wrap
    }

    /// <summary>Rotiert das Directional Light basierend auf der aktuellen Uhrzeit.</summary>
    private void ApplyRotation()
    {
        if (sunLight == null) return;

        // t = 0 bei Mitternacht (0h), t = 0.5 bei Mittag (12h), t = 1 bei Mitternacht (24h)
        float t = currentHour / 24f;
        Vector3 rotation = Vector3.Lerp(rotationAtMidnight, rotationAtNoon, Mathf.PingPong(t * 2f, 1f));
        sunLight.transform.rotation = Quaternion.Euler(rotation);
    }

    /// <summary>Überprüft alle Zeitbereiche und löst Events aus bei Änderungen.</summary>
    private void EvaluateTimeRanges()
    {
        foreach (var range in timeRanges)
        {
            bool inRange = IsHourInRange(currentHour, range);

            if (inRange && !range.isActive)
            {
                range.isActive = true;
                Debug.Log($"[DayNightCycle] Zeitbereich '{range.rangeName}' beginnt um {GetFormattedTime()}.");
                range.onRangeStart?.Invoke();
            }
            else if (!inRange && range.isActive)
            {
                range.isActive = false;
                Debug.Log($"[DayNightCycle] Zeitbereich '{range.rangeName}' endet um {GetFormattedTime()}.");
                range.onRangeEnd?.Invoke();
            }
        }
    }

    /// <summary>
    /// Prüft ob eine Uhrzeit in einem Zeitbereich liegt.
    /// Unterstützt Bereiche über Mitternacht (z.B. 22:00–02:00).
    /// </summary>
    private bool IsHourInRange(float hour, TimeRange range)
    {
        if (range.startHour <= range.endHour)
        {
            // Normaler Bereich (z.B. 06:00–18:00)
            return hour >= range.startHour && hour < range.endHour;
        }
        else
        {
            // Über Mitternacht (z.B. 22:00–04:00)
            return hour >= range.startHour || hour < range.endHour;
        }
    }

    // ─────────────────────────────────────────────
    // Gizmos (Editor-Hilfe)
    // ─────────────────────────────────────────────

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Vorschau im Editor: Rotation aktualisieren wenn Werte geändert werden
        if (sunLight != null && !Application.isPlaying)
        {
            float t = startHour / 24f;
            Vector3 rotation = Vector3.Lerp(rotationAtMidnight, rotationAtNoon, Mathf.PingPong(t * 2f, 1f));
            sunLight.transform.rotation = Quaternion.Euler(rotation);
        }
    }
#endif
}
