using UnityEngine;

/// <summary>
/// Beschreibt den Weltraumsektor, in dem sich das Schiff gerade befindet.
/// Liefert den Sonnenfaktor und einen begrenzten Wasservorrat, den das Schiff abernten kann.
/// Der Inhalt ist erst nach einer Analyse bekannt.
/// </summary>
public class Sector : MonoBehaviour
{
    [Header("Sektor")]
    [Tooltip("ID aus der SectorMapData")]
    [SerializeField] private string id;
    [SerializeField] private string sectorName = "Unbekannter Sektor";

    [Tooltip("0 = keine Sonne, 1 = volle Sonne")]
    [Range(0f, 1f)]
    [SerializeField] private float sun = 0.5f;

    [Header("Wasser")]
    [SerializeField] private float waterAmount = 200f;

    [Header("Analyse")]
    [Tooltip("Dauer der Analyse in Sekunden")]
    [SerializeField] private float analysisDuration = 5f;

    private float initialWaterAmount;
    private bool isAnalysing;
    private bool isAnalysed;
    private float analysisTimer;

    public string Id => id;
    public string SectorName => sectorName;
    public float Sun => sun;
    public float WaterAmount => waterAmount;
    public float InitialWaterAmount => initialWaterAmount;
    public bool HasWater => waterAmount > 0f;
    /// <summary>Restwasser relativ zum ursprünglichen Vorrat (0 bis 1).</summary>
    public float WaterRatio => initialWaterAmount > 0f ? Mathf.Clamp01(waterAmount / initialWaterAmount) : 0f;

    public bool IsAnalysing => isAnalysing;
    public bool IsAnalysed => isAnalysed;
    /// <summary>Fortschritt der laufenden Analyse (0 bis 1).</summary>
    public float AnalysisProgress => isAnalysed ? 1f :
        isAnalysing && analysisDuration > 0f ? Mathf.Clamp01(analysisTimer / analysisDuration) : 0f;

    private void Awake()
    {
        initialWaterAmount = waterAmount;
    }

    /// <summary>Übernimmt alle Werte aus der SectorMapData und setzt den Zustand zurück.</summary>
    public void Initialize(SectorData data)
    {
        if (data == null) return;

        id = data.id;
        sectorName = data.displayName;
        sun = Mathf.Clamp01(data.sun);
        waterAmount = Mathf.Max(0f, data.waterAmount);
        initialWaterAmount = waterAmount;
        analysisDuration = data.analysisDuration;

        isAnalysing = false;
        isAnalysed = false;
        analysisTimer = 0f;
    }

    private void Update()
    {
        if (!isAnalysing) return;

        analysisTimer += Time.deltaTime;
        if (analysisTimer >= analysisDuration)
        {
            isAnalysing = false;
            isAnalysed = true;
        }
    }

    // ---------------- Analyse ----------------

    /// <summary>Startet die Analyse des Sektors. Ohne Wirkung, wenn sie läuft oder schon abgeschlossen ist.</summary>
    public void Analyse()
    {
        if (isAnalysing || isAnalysed) return;

        analysisTimer = 0f;
        isAnalysing = true;

        if (analysisDuration <= 0f)
        {
            isAnalysing = false;
            isAnalysed = true;
        }
    }

    /// <summary>Bricht eine laufende Analyse ab (z.B. beim Verlassen des Sektors).</summary>
    public void CancelAnalysis()
    {
        isAnalysing = false;
        analysisTimer = 0f;
    }

    // ---------------- Wasser ----------------

    /// <summary>
    /// Entnimmt Wasser aus dem Sektor. Unterschreitet nie Null.
    /// Gibt die tatsächlich entnommene Menge zurück.
    /// </summary>
    public float HarvestWater(float amount)
    {
        if (amount <= 0f || !HasWater) return 0f;

        float taken = Mathf.Min(amount, waterAmount);
        waterAmount -= taken;
        return taken;
    }

    /// <summary>
    /// Erlaubt es, den Sonnenfaktor zur Laufzeit zu ändern (z.B. Sonnenfinsternis, Nebel).
    /// </summary>
    public void SetSun(float value)
    {
        sun = Mathf.Clamp01(value);
    }
}
