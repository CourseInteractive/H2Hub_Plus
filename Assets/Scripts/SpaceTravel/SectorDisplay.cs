using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zeigt den Inhalt des aktuellen Sektors an.
/// Während der Analyse: Fortschritt im Image.
/// Nach der Analyse: Sonnenstärke, Restwasser in Litern und Wasser-Füllstand im Image.
/// Während einer Reise (kein Sektor) wird das Display geleert.
/// </summary>
public class SectorDisplay : MonoBehaviour
{
    [Header("Quelle")]
    [SerializeField] private SpaceShip ship;

    [Header("Anzeigen")]
    [Tooltip("Image Type muss auf 'Filled' stehen")]
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text sunText;
    [SerializeField] private TMP_Text waterText;
    [Tooltip("Optional: Statuszeile")]
    [SerializeField] private TMP_Text statusText;

    [Header("Texte")]
    [SerializeField] private string noSectorStatus = "";
    [SerializeField] private string notAnalysedStatus = "Nicht analysiert";
    [SerializeField] private string analysingStatus = "Analyse läuft...";
    [SerializeField] private string analysedStatus = "Analyse abgeschlossen";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-DE");

    private enum DisplayMode { Empty, NotAnalysed, Analysing, Analysed }

    private DisplayMode lastMode = (DisplayMode)(-1);
    private int lastSunPercent = -1;
    private float lastWater = -1f;

    private void Update()
    {
        Sector sector = ship ? ship.CurrentSector : null;

        DisplayMode mode;
        if (!sector) mode = DisplayMode.Empty;
        else if (sector.IsAnalysed) mode = DisplayMode.Analysed;
        else if (sector.IsAnalysing) mode = DisplayMode.Analysing;
        else mode = DisplayMode.NotAnalysed;

        if (mode != lastMode)
            ApplyMode(mode);

        switch (mode)
        {
            case DisplayMode.Analysing:
                SetFill(sector.AnalysisProgress);
                break;

            case DisplayMode.Analysed:
                SetFill(sector.WaterRatio);
                UpdateValues(sector);
                break;
        }
    }

    private void ApplyMode(DisplayMode mode)
    {
        lastMode = mode;
        lastSunPercent = -1;
        lastWater = -1f;

        // Werte leeren, sie werden im Analysed-Modus neu gesetzt.
        SetText(sunText, "");
        SetText(waterText, "");
        SetFill(0f);

        switch (mode)
        {
            case DisplayMode.Empty:       SetText(statusText, noSectorStatus); break;
            case DisplayMode.NotAnalysed: SetText(statusText, notAnalysedStatus); break;
            case DisplayMode.Analysing:   SetText(statusText, analysingStatus); break;
            case DisplayMode.Analysed:    SetText(statusText, analysedStatus); break;
        }
    }

    private void UpdateValues(Sector sector)
    {
        // Texte nur neu setzen, wenn sich der angezeigte Wert ändert (spart Garbage).
        int sunPercent = Mathf.RoundToInt(sector.Sun * 100f);
        if (sunPercent != lastSunPercent)
        {
            lastSunPercent = sunPercent;
            SetText(sunText, sunPercent + " %");
        }

        float water = Mathf.Round(sector.WaterAmount * 10f) / 10f;
        if (water != lastWater)
        {
            lastWater = water;
            SetText(waterText, water.ToString("0.0", Culture) + " L");
        }
    }

    private void SetFill(float value)
    {
        if (fillImage)
            fillImage.fillAmount = value;
    }

    private static void SetText(TMP_Text text, string value)
    {
        if (text)
            text.text = value;
    }
}
