using System.Globalization;
using System.Text;
using TMPro;
using UnityEngine;

/// <summary>
/// Zeigt Infos zum Sektor an, über dem gerade gehovert wird.
/// Nicht interagierbar: Grund. Erreichbar: Eigenschaften, Entfernung und Wasserstoffbedarf.
/// </summary>
public class MapSectorUI : MonoBehaviour
{
    [Header("Referenzen")]
    [SerializeField] private TMP_Text text;
    [Tooltip("Für die Berechnung des Wasserstoffbedarfs")]
    [SerializeField] private SpaceShip ship;
    [Tooltip("Optional: wird bei Hover eingeblendet, sonst ausgeblendet (nicht das Objekt mit diesem Script)")]
    [SerializeField] private GameObject panel;

    [Header("Texte")]
    [SerializeField] private string endLabel = "Endsektor";
    [SerializeField] private string currentReason = "Aktueller Standort";
    [SerializeField] private string noConnectionReason = "Keine direkte Verbindung";
    [SerializeField] private string viaLabel = "Erreichbar über: ";
    [SerializeField] private string sunLabel = "Sonne: ";
    [SerializeField] private string waterLabel = "Wasser: ";
    [SerializeField] private string distanceLabel = "Entfernung: ";
    [Tooltip("{0} wird durch die benötigte Menge ersetzt")]
    [SerializeField] private string noFuelText = "Zu wenig Wasserstoff. {0} H2 benötigt.";

    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-DE");
    private readonly StringBuilder sb = new StringBuilder();
    private Map_SectorInteractable shown;

    private void Awake()
    {
        Clear();
    }

    public void Show(Map_SectorInteractable entry)
    {
        if (!entry || !text) return;

        shown = entry;
        text.text = BuildText(entry);

        if (panel)
            panel.SetActive(true);
    }

    /// <summary>
    /// Ersetzt den kompletten Text durch den Treibstoff-Hinweis.
    /// Bei HoverEnd bzw. erneutem Hover wird wieder normal angezeigt.
    /// </summary>
    public void ShowNoFuel(Map_SectorInteractable entry, float requiredHydrogen)
    {
        if (!text) return;

        shown = entry;
        text.text = string.Format(noFuelText, requiredHydrogen.ToString("0.##", Culture));

        if (panel)
            panel.SetActive(true);
    }

    /// <summary>Leert die Anzeige nur, wenn genau dieser Sektor gerade angezeigt wird.</summary>
    public void Hide(Map_SectorInteractable entry)
    {
        if (entry == shown)
            Clear();
    }

    public void Clear()
    {
        shown = null;

        if (text)
            text.text = "";
        if (panel)
            panel.SetActive(false);
    }

    private string BuildText(Map_SectorInteractable entry)
    {
        Sector sector = entry.sector;
        sb.Clear();

        sb.Append("<b>").Append(sector ? sector.SectorName : "?").Append("</b>");
        if (sector && sector.IsEnd)
            sb.Append("  (").Append(endLabel).Append(')');
        sb.Append('\n');

        switch (entry.State)
        {
            case MapSectorState.Current:
                sb.Append(currentReason);
                break;

            case MapSectorState.Unreachable:
                sb.Append(noConnectionReason);
               // if (entry.ReachableVia.Count > 0)
               //     sb.Append('\n').Append(viaLabel).Append(string.Join(", ", entry.ReachableVia));
                break;

            case MapSectorState.Reachable:
                if (sector)
                {
                    sb.Append(sunLabel).Append(Mathf.RoundToInt(sector.Sun * 100f)).Append(" %\n");
                    sb.Append(waterLabel).Append(sector.WaterAmount.ToString("0.#", Culture)).Append(" L\n");
                }

                float hydrogen = ship ? ship.CalculateRequiredHydrogen(entry.Distance) : 0f;
                sb.Append(distanceLabel)
                  .Append(entry.Distance.ToString("0.#", Culture))
                  .Append(" (").Append(hydrogen.ToString("0.##", Culture)).Append(" H2)");
                break;
        }

        return sb.ToString();
    }
}
