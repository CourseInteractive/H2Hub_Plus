using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Zeigt den in Travel gesetzten Zielsektor und den Reisefortschritt an.
/// Fortschritt als Image (FillAmount) und als Prozenttext.
/// </summary>
public class TravelDisplay : MonoBehaviour
{
    [Header("Quelle")]
    [SerializeField] private Travel travel;

    [Header("Zielsektor")]
    [SerializeField] private TMP_Text targetSectorText;
    [SerializeField] private string noTargetText = "Kein Ziel gesetzt";

    [Header("Fortschritt")]
    [Tooltip("Image Type muss auf 'Filled' stehen")]
    [SerializeField] private Image progressImage;
    [SerializeField] private TMP_Text progressText;

    [Tooltip("{0} wird durch die benötigte Menge ersetzt")]
    [SerializeField] private string noFuelText = "{0} H2 benötigt";

    private Sector lastTarget;
    private TravelState lastState;
    private float lastRequiredFuel = -1f;
    private bool targetInitialized;
    private int lastPercent = -1;
    private static readonly CultureInfo Culture = CultureInfo.GetCultureInfo("de-DE");

    private void Update()
    {
        if (!travel) return;

        UpdateTarget();
        UpdateProgress();
    }

    private void UpdateTarget()
    {
        if (!targetSectorText) return;

        Sector target = travel.TargetSector;
        TravelState state = travel.State;
        float requiredFuel = travel.RequiredFuel;

        if (targetInitialized && target == lastTarget && state == lastState && requiredFuel == lastRequiredFuel)
            return;

        targetInitialized = true;
        lastTarget = target;
        lastState = state;
        lastRequiredFuel = requiredFuel;

        if (state == TravelState.NoFuel)
            targetSectorText.text = string.Format(noFuelText, requiredFuel.ToString("0.##", Culture));
        else
            targetSectorText.text = target ? target.SectorName : noTargetText;
    }

    private void UpdateProgress()
    {
        float progress = travel.IsTraveling ? travel.Progress : 0f;

        if (progressImage)
            progressImage.fillAmount = progress;

        if (progressText)
        {
            // Nur neu setzen, wenn sich der Prozentwert ändert (spart Garbage).
            int percent = Mathf.FloorToInt(progress * 100f);
            if (percent != lastPercent)
            {
                lastPercent = percent;
                progressText.text = percent + "%";
            }
        }
    }
}
