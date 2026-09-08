using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Steuert einen Tag/Nacht-Zyklus mit einem Directional Light.
/// Lichtfarbe und -stärke werden zwischen Tag und Nacht automatisch übergeblendet.
///
/// ZEITSTRUKTUR (Beispiel-Defaults):
///   Tag        08:00 – 18:00   → volle Tageinstellungen
///   Abend      18:00 – 22:00   → Überblendung Nacht → Tag (rückwärts)
///   Nacht      22:00 – 06:00   → volle Nachteinstellungen
///   Morgen     06:00 – 08:00   → Überblendung Nacht → Tag
///
/// SETUP:
/// 1. Script einer beliebigen GameObject hinzufügen.
/// 2. Directional Light (Sonne) im Inspector zuweisen.
/// 3. Tag- und Nacht-Einstellungen konfigurieren.
/// 4. Überblendzeiten und Tagesgeschwindigkeit anpassen.
/// </summary>
public class DayNightCycle : MonoBehaviour
{
    // ─────────────────────────────────────────────
    // Datenstrukturen
    // ─────────────────────────────────────────────

    [Serializable]
    public class PhaseSettings
    {
        [Tooltip("Lichtfarbe dieser Phase.")]
        public Color lightColor = Color.white;

        [Tooltip("Lichtstärke dieser Phase.")]
        [Range(0f, 8f)] public float lightIntensity = 1f;

        [Tooltip("Schattenintensität dieser Phase (0 = kein Schatten, 1 = voller Schatten).")]
        [Range(0f, 1f)] public float shadowStrength = 1f;

        [Tooltip("Umgebungslichtfarbe (RenderSettings.ambientLight) dieser Phase.")]
        public Color ambientColor = new Color(0.2f, 0.2f, 0.2f);
    }

    // ─────────────────────────────────────────────
    // Inspector-Felder
    // ─────────────────────────────────────────────

    [Header("Licht")]
    [Tooltip("Das Directional Light, das als Sonne/Mond fungiert.")]
    [SerializeField] private Light sunLight;

    [Tooltip("Achse, um die die Sonne rotiert (z.B. (1,0,0) = Ost-West-Bahn).")]
    [SerializeField] private Vector3 sunAxis = new Vector3(1f, 0f, 0f);

    [Tooltip("Winkel-Offset in Grad: verschiebt den Sonnenhochstand relativ zu 0 Uhr.\n" +
             "Standard -90: Sonne geht um 6 Uhr auf, steht um 12 Uhr oben, geht um 18 Uhr unter.")]
    [SerializeField] private float sunNoonOffset = -90f;

    [Header("Tag-Phase")]
    [SerializeField] private PhaseSettings daySettings = new PhaseSettings
    {
        lightColor = Color.white,
        lightIntensity = 1f
    };

    [Tooltip("Beginn der vollen Tagphase (Ende der Morgen-Überblendung).")]
    [Range(0f, 24f)] [SerializeField] private float dayStartHour = 8f;

    [Tooltip("Ende der vollen Tagphase (Beginn der Abend-Überblendung).")]
    [Range(0f, 24f)] [SerializeField] private float dayEndHour = 18f;

    [Tooltip("Events für den Beginn und das Ende der Tagphase.")]
    [SerializeField] private UnityEvent onDayStart;
    [SerializeField] private UnityEvent onDayEnd;

    [Header("Nacht-Phase")]
    [SerializeField] private PhaseSettings nightSettings = new PhaseSettings
    {
        lightColor     = new Color(0.3f, 0.4f, 0.8f), // Blau
        lightIntensity = 0.5f,
        shadowStrength = 0.6f
    };

    [Tooltip("Beginn der vollen Nachtphase (Ende der Abend-Überblendung).")]
    [Range(0f, 24f)] [SerializeField] private float nightStartHour = 22f;

    [Tooltip("Ende der vollen Nachtphase (Beginn der Morgen-Überblendung).")]
    [Range(0f, 24f)] [SerializeField] private float nightEndHour = 6f;

    [Tooltip("Events für den Beginn und das Ende der Nachtphase.")]
    [SerializeField] private UnityEvent onNightStart;
    [SerializeField] private UnityEvent onNightEnd;

    [Header("Zeit")]
    [Tooltip("Startzeit beim Spielbeginn (0–24 Stunden).")]
    [Range(0f, 24f)] [SerializeField] private float startHour = 8f;

    [Tooltip("Wie viele Spielminuten pro echter Sekunde vergehen.\n" +
             "1 = Echtzeit | 60 = 1 Min/Sek | 720 = voller Tag in 2 Min.")]
    [SerializeField] private float timeMultiplier = 60f;

    // ─────────────────────────────────────────────
    // Private Felder
    // ─────────────────────────────────────────────

    private float currentHour;
    private bool isRunning = true;

    // Zustandsverfolgung für Events
    private bool isDayActive   = false;
    private bool isNightActive = false;

    // ─────────────────────────────────────────────
    // Interne Enum für die aktuelle Phase
    // ─────────────────────────────────────────────

    private enum CyclePhase { Day, NightToDay, Night, DayToNight }

    // ─────────────────────────────────────────────
    // Singleton
    // ─────────────────────────────────────────────

    /// <summary>
    /// Globale Instanz des DayNightCycle (Singleton).
    /// Zugriff von überall: DayNightCycle.Instance.CurrentHour
    /// </summary>
    public static DayNightCycle Instance { get; private set; }

    // ─────────────────────────────────────────────
    // Properties
    // ─────────────────────────────────────────────

    /// <summary>Aktuelle Uhrzeit in Stunden (0–24).</summary>
    public float CurrentHour => currentHour;

    /// <summary>Ob der Zyklus gerade läuft.</summary>
    public bool IsRunning => isRunning;

    // ─────────────────────────────────────────────
    // Unity Lifecycle
    // ─────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("[DayNightCycle] Zweite Instanz gefunden – wird zerstört.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (sunLight == null)
        {
            Debug.LogError("[DayNightCycle] Kein Directional Light zugewiesen!");
            return;
        }

        currentHour = startHour;

        // Initialen Zustand setzen ohne Events auszulösen
        var phase = GetCurrentPhase();
        float t    = GetBlendT(phase);
        ApplyLightSettings(t);
        ApplyRotation();

        isDayActive   = (phase == CyclePhase.Day);
        isNightActive = (phase == CyclePhase.Night);
    }

    private void Update()
    {
        if (!isRunning) return;

        AdvanceTime();

        var phase = GetCurrentPhase();
        float t   = GetBlendT(phase);

        ApplyLightSettings(t);
        ApplyRotation();
        Debug.DrawRay(rayStart.position, sunAxis, Color.red);
        EvaluatePhaseEvents(phase);
    }
    public Transform rayStart;
    // ─────────────────────────────────────────────
    // Öffentliche Methoden
    // ─────────────────────────────────────────────

    /// <summary>Pausiert den Zyklus.</summary>
    public void Pause()
    {
        isRunning = false;
        Debug.Log("[DayNightCycle] Pausiert.");
    }

    /// <summary>Setzt den Zyklus fort.</summary>
    public void Resume()
    {
        isRunning = true;
        Debug.Log("[DayNightCycle] Fortgesetzt.");
    }

    /// <summary>Wechselt zwischen Pause und Fortsetzen.</summary>
    public void TogglePause()
    {
        if (isRunning) Pause(); else Resume();
    }

    /// <summary>Setzt die aktuelle Uhrzeit direkt (0–24). Kein Event-Fire.</summary>
    public void SetTime(float hour)
    {
        currentHour = Mathf.Repeat(hour, 24f);

        var phase  = GetCurrentPhase();
        float t    = GetBlendT(phase);
        ApplyLightSettings(t);
        ApplyRotation();

        // Zustand synchronisieren ohne Events
        isDayActive   = (phase == CyclePhase.Day);
        isNightActive = (phase == CyclePhase.Night);
    }

    /// <summary>Setzt den Zeitmultiplikator (Spielminuten pro Echtzeitsekunde).</summary>
    public void SetTimeMultiplier(float multiplier)
    {
        timeMultiplier = Mathf.Max(0f, multiplier);
    }

    /// <summary>Gibt die aktuelle Uhrzeit als "HH:MM" zurück.</summary>
    public string GetFormattedTime()
    {
        int h = Mathf.FloorToInt(currentHour);
        int m = Mathf.FloorToInt((currentHour - h) * 60f);
        return $"{h:D2}:{m:D2}";
    }

    // ─────────────────────────────────────────────
    // Kern-Logik
    // ─────────────────────────────────────────────

    private void AdvanceTime()
    {
        float hoursPerSecond = timeMultiplier / 60f;
        currentHour = Mathf.Repeat(currentHour + hoursPerSecond * Time.deltaTime, 24f);
    }

    /// <summary>
    /// Bestimmt die aktuelle Phase basierend auf der Uhrzeit.
    ///
    /// Zeitleiste (Defaults):
    ///   [nightEnd=06] --Morgen-Blend--> [dayStart=08] --Tag--> [dayEnd=18]
    ///   --Abend-Blend--> [nightStart=22] --Nacht--> [nightEnd=06]
    /// </summary>
    private CyclePhase GetCurrentPhase()
    {
        float h = currentHour;

        // Vollständige Tagphase
        if (IsInRange(h, dayStartHour, dayEndHour))
            return CyclePhase.Day;

        // Vollständige Nachtphase (kann über Mitternacht gehen)
        if (IsInRange(h, nightStartHour, nightEndHour))
            return CyclePhase.Night;

        // Überblendung: Abend (Tag → Nacht): dayEnd → nightStart
        if (IsInRange(h, dayEndHour, nightStartHour))
            return CyclePhase.DayToNight;

        // Überblendung: Morgen (Nacht → Tag): nightEnd → dayStart
        // (Rest der Uhr)
        return CyclePhase.NightToDay;
    }

    /// <summary>
    /// Berechnet den Blend-Faktor t (0 = Nacht, 1 = Tag) für die aktuelle Phase.
    /// </summary>
    private float GetBlendT(CyclePhase phase)
    {
        switch (phase)
        {
            case CyclePhase.Day:
                return 1f;

            case CyclePhase.Night:
                return 0f;

            case CyclePhase.DayToNight:
            {
                // Abend: von dayEndHour bis nightStartHour, t geht 1 → 0
                float duration = WrapDuration(dayEndHour, nightStartHour);
                float elapsed  = WrapDuration(dayEndHour, currentHour);
                return Mathf.Clamp01(1f - elapsed / duration);
            }

            case CyclePhase.NightToDay:
            {
                // Morgen: von nightEndHour bis dayStartHour, t geht 0 → 1
                float duration = WrapDuration(nightEndHour, dayStartHour);
                float elapsed  = WrapDuration(nightEndHour, currentHour);
                return Mathf.Clamp01(elapsed / duration);
            }

            default:
                return 1f;
        }
    }

    /// <summary>Wendet Lichtfarbe, -stärke und Schattenintensität interpoliert auf das Directional Light an.</summary>
    private void ApplyLightSettings(float t)
    {
        if (sunLight == null) return;

        sunLight.color          = Color.Lerp(nightSettings.lightColor,      daySettings.lightColor,      t);
        sunLight.intensity      = Mathf.Lerp(nightSettings.lightIntensity,  daySettings.lightIntensity,  t);
        sunLight.shadowStrength = Mathf.Lerp(nightSettings.shadowStrength,  daySettings.shadowStrength,  t);
        RenderSettings.ambientLight = Color.Lerp(nightSettings.ambientColor, daySettings.ambientColor,   t);
    }

    /// <summary>
    /// Rotiert die Sonne 360° um <see cref="sunAxis"/>.
    /// 0 Uhr entspricht 0°, jede Stunde = 15°. sunNoonOffset verschiebt den Orbit,
    /// sodass die Sonne zum gewünschten Zeitpunkt aufgeht/untergeht.
    /// </summary>
    private void ApplyRotation()
    {
        if (sunLight == null) return;

        // 360° / 24h = 15° pro Stunde
        float angle = currentHour * 15f + sunNoonOffset;
        sunLight.transform.rotation = Quaternion.AngleAxis(angle, sunAxis.normalized);
    }

    /// <summary>Feuert Events wenn eine Phase beginnt oder endet.</summary>
    private void EvaluatePhaseEvents(CyclePhase phase)
    {
        bool dayNow   = (phase == CyclePhase.Day);
        bool nightNow = (phase == CyclePhase.Night);

        if (dayNow && !isDayActive)
        {
            isDayActive = true;
            Debug.Log($"[DayNightCycle] Tag beginnt um {GetFormattedTime()}.");
            onDayStart?.Invoke();
        }
        else if (!dayNow && isDayActive)
        {
            isDayActive = false;
            Debug.Log($"[DayNightCycle] Tag endet um {GetFormattedTime()}.");
            onDayEnd?.Invoke();
        }

        if (nightNow && !isNightActive)
        {
            isNightActive = true;
            Debug.Log($"[DayNightCycle] Nacht beginnt um {GetFormattedTime()}.");
            onNightStart?.Invoke();
        }
        else if (!nightNow && isNightActive)
        {
            isNightActive = false;
            Debug.Log($"[DayNightCycle] Nacht endet um {GetFormattedTime()}.");
            onNightEnd?.Invoke();
        }
    }

    // ─────────────────────────────────────────────
    // Hilfsmethoden
    // ─────────────────────────────────────────────

    /// <summary>
    /// Prüft ob 'hour' in [from, to) liegt, mit Wrap über Mitternacht.
    /// </summary>
    private static bool IsInRange(float hour, float from, float to)
    {
        if (from < to)
            return hour >= from && hour < to;
        else // über Mitternacht
            return hour >= from || hour < to;
    }

    /// <summary>
    /// Gibt die Dauer von 'from' bis 'to' in Stunden zurück (mit Mitternacht-Wrap).
    /// </summary>
    private static float WrapDuration(float from, float to)
    {
        float d = to - from;
        return d < 0 ? d + 24f : d;
    }

    // ─────────────────────────────────────────────
    // Editor-Vorschau
    // ─────────────────────────────────────────────

    public bool ItIsDaytime()
    {
        return GetCurrentPhase() == CyclePhase.Day;
    }


#if UNITY_EDITOR
    private void OnValidate()
    {
        if (sunLight == null || Application.isPlaying) return;

        var phase = GetCurrentPhase();
        float t   = GetBlendT(phase);
        ApplyLightSettings(t);
        ApplyRotation();
    }
#endif
}
