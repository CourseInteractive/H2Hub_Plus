using System.Globalization;
using TMPro;
using UnityEngine;

/// <summary>
/// Zeigt die Schiffstemperatur digital (TextMeshPro) und analog (Zeiger) an.
/// Der Zeiger wird auf den Skalenbereich geclampt, die Digitalanzeige zeigt den echten Wert.
/// </summary>
public class TemperatureDisplay : MonoBehaviour
{
    [Header("Quelle")]
    [SerializeField] private SpaceShip ship;

    [Header("Digital")]
    [SerializeField] private TMP_Text digitalText;
    [Tooltip("Deutsches Format mit Komma (z.B. 21,5 °C)")]
    [SerializeField] private bool useGermanFormat = true;

    [Header("Analog")]
    [SerializeField] private Transform needle;
    [SerializeField] private float minTemperature = 0f;
    [SerializeField] private float maxTemperature = 40f;
    [Tooltip("Lokaler Zeigerwinkel bei minTemperature (links)")]
    [SerializeField] private float minAngle = 90f;
    [Tooltip("Lokaler Zeigerwinkel bei maxTemperature (rechts)")]
    [SerializeField] private float maxAngle = -90f;
    [Tooltip("Lokale Achse, um die der Zeiger dreht")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [Tooltip("Zeigerglättung, 0 = sofort")]
    [SerializeField] private float needleSmoothing = 8f;

    private Quaternion needleBaseRotation;
    private float displayedAngle;
    private float lastShownValue = float.NaN;
    private CultureInfo culture;

    private void Awake()
    {
        culture = useGermanFormat ? CultureInfo.GetCultureInfo("de-DE") : CultureInfo.InvariantCulture;

        if (needle)
            needleBaseRotation = needle.localRotation;

        if (ship)
            displayedAngle = TemperatureToAngle(ship.Temperature);
    }

    private void Update()
    {
        if (!ship) return;

        float temp = ship.Temperature;
        UpdateDigital(temp);
        UpdateAnalog(temp);
    }

    private void UpdateDigital(float temp)
    {
        if (!digitalText) return;

        // Nur neu setzen, wenn sich die angezeigte Zahl ändert (spart Garbage).
        float rounded = Mathf.Round(temp * 10f) / 10f;
        if (rounded == lastShownValue) return;

        lastShownValue = rounded;
        digitalText.text = rounded.ToString("0.0", culture) + " °C";
    }

    private void UpdateAnalog(float temp)
    {
        if (!needle) return;

        float targetAngle = TemperatureToAngle(temp);

        displayedAngle = needleSmoothing > 0f
            ? Mathf.Lerp(displayedAngle, targetAngle, 1f - Mathf.Exp(-needleSmoothing * Time.deltaTime))
            : targetAngle;

        needle.localRotation = needleBaseRotation * Quaternion.AngleAxis(displayedAngle, rotationAxis);
    }

    private float TemperatureToAngle(float temp)
    {
        // InverseLerp clampt automatisch auf 0..1
        float t = Mathf.InverseLerp(minTemperature, maxTemperature, temp);
        return Mathf.Lerp(minAngle, maxAngle, t);
    }
}
