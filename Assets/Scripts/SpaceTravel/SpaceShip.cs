using UnityEngine;

/// <summary>
/// Das Schiff: verbraucht Sauerstoff, kühlt aus, lädt Energie per Sonnensegel
/// und erntet Wasser aus dem aktuellen Sektor.
/// Während einer Reise (Travel) gibt es keinen aktiven Sektor: keine Sonne, kein Wasser.
/// </summary>
public class SpaceShip : MonoBehaviour
{
    public MainMachine machine;

    [Header("Reise")]
    [SerializeField] private Travel travel;

    [Header("Feste Container")]
    [SerializeField] private Container o2Container;
    [SerializeField] private Container energyContainer;
    [SerializeField] private Container waterContainer;
    [SerializeField] private Container hydrogenContainer;

    [Header("Treibstoff")]
    [Tooltip("Benötigter Wasserstoff pro Entfernungseinheit")]
    [SerializeField] private float hydrogenPerDistance = 0.75f;
    private float requiredHydrogen;

    [Header("Aktueller Sektor")]
    [SerializeField] private Sector currentSector;

    [Header("Sauerstoff")]
    [Tooltip("O2-Verbrauch pro Sekunde")]
    [SerializeField] private float oxygenLossPerSecond = 0.5f;

    [Header("Temperatur")]
    [SerializeField] private float temperature = 20f;
    [Tooltip("Abkühlung pro Sekunde ohne Sonne (°C)")]
    [SerializeField] private float coolingPerSecond = 0.2f;
    public float machineHeatingFactor = 0.1f;

    [Tooltip("Ab diesem Sonnenfaktor bleibt die Temperatur stabil")]
    [Range(0f, 1f)] [SerializeField] private float sunStableThreshold = 0.5f;
    [Tooltip("Ab diesem Sonnenfaktor steigt die Temperatur")]
    [Range(0f, 1f)] [SerializeField] private float sunHeatingThreshold = 0.75f;
    [Tooltip("Maximale Erwärmung pro Sekunde bei voller Sonne (°C)")]
    [SerializeField] private float maxSunHeatingPerSecond = 0.3f;
    [Tooltip("Untergrenze, nahe dem absoluten Nullpunkt")]
    [SerializeField] private float minTemperature = -270f;

    [Header("Sonnensegel")]
    [SerializeField] private bool solarSailActive = false;
    [Tooltip("Energie pro Sekunde bei Sonnenfaktor 1")]
    [SerializeField] private float solarEnergyPerSecond = 2f;

    [Header("Wasserernter")]
    [SerializeField] private bool waterHarvesterActive = false;
    [Tooltip("Geerntetes Wasser pro Sekunde")]
    [SerializeField] private float waterHarvestPerSecond = 1f;

    public float Temperature => temperature;
    /// <summary>Beim letzten Startversuch berechneter Wasserstoffbedarf.</summary>
    public float RequiredHydrogen => requiredHydrogen;
    public bool IsTraveling => travel && travel.IsTraveling;
    /// <summary>Der Sektor, in dem sich das Schiff befindet. Während einer Reise null.</summary>
    public Sector CurrentSector => IsTraveling ? null : currentSector;
    public bool SolarSailActive => solarSailActive;
    public bool WaterHarvesterActive => waterHarvesterActive;

    private float CurrentSun => CurrentSector ? CurrentSector.Sun : 0f;

    private void Update()
    {
        float dt = Time.deltaTime;

        UpdateOxygen(dt);
        UpdateTemperature(dt);
        UpdateSolarSail(dt);
        UpdateWaterHarvester(dt);
    }

    // ---------------- Sauerstoff ----------------

    private void UpdateOxygen(float dt)
    {
        if (!o2Container || o2Container.IsEmpty) return;
        o2Container.Remove(oxygenLossPerSecond * dt);
    }

    // ---------------- Temperatur ----------------

    private void UpdateTemperature(float dt)
    {
        float sun = CurrentSun;

        if (sun < sunStableThreshold)
        {
            // Zu wenig Sonne: abkühlen. Je mehr Sonne, desto langsamer.
            float coolingFactor = 1f - (sun / Mathf.Max(sunStableThreshold, 0.0001f));
            temperature -= coolingPerSecond * coolingFactor * dt;
        }
        else if (sun >= sunHeatingThreshold)
        {
            // Viel Sonne: erwärmen. Skaliert von 0 (an der Schwelle) bis max (bei Sonne 1).
            float range = Mathf.Max(1f - sunHeatingThreshold, 0.0001f);
            float heatingFactor = (sun - sunHeatingThreshold) / range;
            temperature += maxSunHeatingPerSecond * heatingFactor * dt;
        }
        // Dazwischen: Sonne hält die Temperatur stabil.

        // Abwärme der Maschine
        if (machine && machine.currentPowerLevel >= 0.1f)
        {
            temperature += machine.currentPowerLevel * machineHeatingFactor * dt;
        }

        temperature = Mathf.Max(temperature, minTemperature);
    }

    /// <summary>
    /// Für spätere Heizsysteme: Wärme zuführen (positiv) oder entziehen (negativ).
    /// </summary>
    public void AddHeat(float degrees)
    {
        temperature = Mathf.Max(temperature + degrees, minTemperature);
    }

    // ---------------- Sonnensegel ----------------

    private void UpdateSolarSail(float dt)
    {
        if (!solarSailActive || IsTraveling || !energyContainer || energyContainer.IsFull) return;

        float sun = CurrentSun;
        if (sun <= 0f) return;

        energyContainer.Add(solarEnergyPerSecond * sun * dt);
    }

    /// <summary>Ausfahren ist während einer Reise nicht möglich, Einziehen immer.</summary>
    public void SetSolarSail(bool active)
    {
        if (active && IsTraveling) return;
        solarSailActive = active;
    }

    public void ToggleSolarSail() => SetSolarSail(!solarSailActive);

    // ---------------- Wasserernter ----------------

    private void UpdateWaterHarvester(float dt)
    {
        Sector sector = CurrentSector;
        if (!waterHarvesterActive || IsTraveling || !waterContainer || !sector) return;

        if (!sector.HasWater)
        {
            // Sektor leer: Ernter schaltet sich ab.
            waterHarvesterActive = false;
            return;
        }

        if (waterContainer.IsFull) return;

        // Nur so viel ernten, wie in den Container passt.
        float wanted = waterContainer.GetMaxAmountInsteadOf(waterHarvestPerSecond * dt);
        float harvested = sector.HarvestWater(wanted);
        waterContainer.Add(harvested);
    }

    /// <summary>Ausfahren ist während einer Reise nicht möglich, Einziehen immer.</summary>
    public void SetWaterHarvester(bool active)
    {
        if (active && IsTraveling) return;
        waterHarvesterActive = active;
    }

    public void ToggleWaterHarvester() => SetWaterHarvester(!waterHarvesterActive);

    // ---------------- Treibstoff ----------------

    public float CalculateRequiredHydrogen(float distance) => Mathf.Max(0f, distance) * hydrogenPerDistance;

    /// <summary>
    /// Berechnet und speichert den Wasserstoffbedarf für die Entfernung.
    /// Ist genug vorhanden, wird er abgezogen und true zurückgegeben.
    /// </summary>
    public bool TryConsumeFuel(float distance)
    {
        requiredHydrogen = CalculateRequiredHydrogen(distance);
        if (requiredHydrogen <= 0f) return true;

        if (!hydrogenContainer || !hydrogenContainer.HasAtLeast(requiredHydrogen))
            return false;

        hydrogenContainer.Remove(requiredHydrogen);
        return true;
    }

    // ---------------- Sektor ----------------

    public void SetSector(Sector sector)
    {
        currentSector = sector;
    }

    /// <summary>Startet die Analyse des aktuellen Sektors (z.B. per Button). Während einer Reise ohne Wirkung.</summary>
    public void AnalyseCurrentSector()
    {
        Sector sector = CurrentSector;
        if (sector)
            sector.Analyse();
    }
}
