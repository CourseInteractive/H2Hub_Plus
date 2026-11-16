using UnityEngine;
using UnityEngine.Events;

public enum TravelState
{
    Idle,
    Traveling,
    NoFuel
}

/// <summary>
/// Steuert die Reise des Schiffs zu einem neuen Sektor.
/// Ablauf: SetTargetSector() -> SetDistance() -> StartTravel().
/// Beim Start wird Wasserstoff aus dem Schiff verbraucht. Reicht er nicht, wechselt der State auf NoFuel.
/// Nach Ablauf der Reisezeit wird der Zielsektor zum aktiven Sektor des Schiffs.
/// </summary>
public class Travel : MonoBehaviour
{
    [Header("Referenzen")]
    [SerializeField] private SpaceShip ship;

    [Header("Reise")]
    [SerializeField] private Sector targetSector;
    [SerializeField] private float distance = 0f;
    [Tooltip("Zurückgelegte Entfernung pro Sekunde. Reisedauer = Entfernung / Geschwindigkeit")]
    [SerializeField] private float travelSpeed = 1f;

    [Header("Events")]
    public UnityEvent onTravelStarted;
    public UnityEvent onTravelFinished;
    public UnityEvent onNoFuel;

    private TravelState state = TravelState.Idle;
    private float travelDuration;
    private float travelTimer;

    public TravelState State => state;
    public bool IsTraveling => state == TravelState.Traveling;
    public Sector TargetSector => targetSector;
    public float Distance => distance;
    /// <summary>Beim letzten Startversuch berechneter Wasserstoffbedarf.</summary>
    public float RequiredFuel => ship ? ship.RequiredHydrogen : 0f;
    public float TravelDuration => distance / Mathf.Max(travelSpeed, 0.0001f);
    public float RemainingTime => IsTraveling ? Mathf.Max(travelDuration - travelTimer, 0f) : 0f;
    /// <summary>Fortschritt der aktuellen Reise von 0 bis 1.</summary>
    public float Progress => IsTraveling && travelDuration > 0f ? Mathf.Clamp01(travelTimer / travelDuration) : 0f;

    // ---------------- Vorbereitung ----------------

    /// <summary>Setzt den Zielsektor. Während einer Reise nicht möglich.</summary>
    public void SetTargetSector(Sector sector)
    {
        if (IsTraveling)
        {
            Debug.LogWarning("Travel: Zielsektor kann während einer Reise nicht geändert werden.");
            return;
        }
        targetSector = sector;
        ClearNoFuel();
    }

    public float testTravelSpeed = 8f;
    public void UseTestSpeed()
    {
        travelSpeed = testTravelSpeed;
    }

    /// <summary>Setzt die Entfernung zum Zielsektor. Während einer Reise nicht möglich.</summary>
    public void SetDistance(float newDistance)
    {
        if (IsTraveling)
        {
            Debug.LogWarning("Travel: Entfernung kann während einer Reise nicht geändert werden.");
            return;
        }

        distance = Mathf.Max(0f, newDistance);
        ClearNoFuel();
    }

    private void ClearNoFuel()
    {
        if (state == TravelState.NoFuel)
            state = TravelState.Idle;
    }

    // ---------------- Reise ----------------

    public void StartTravel()
    {
        StartTravelIfPossible();
    }

    /// <summary>
    /// Startet die Reise. Verbraucht Wasserstoff und zieht Sonnensegel und Wassersammler ein.
    /// Gibt false zurück, wenn die Reise nicht gestartet werden konnte.
    /// </summary>
    public bool StartTravelIfPossible(bool force = false)
    {
        if (IsTraveling)
        {
            Debug.LogWarning("Travel: Es läuft bereits eine Reise.");
            return false;
        }
        if (!targetSector)
        {
            Debug.LogWarning("Travel: Kein Zielsektor gesetzt.");
            return false;
        }
        if (ship && targetSector == ship.CurrentSector)
        {
            Debug.LogWarning("Travel: Das Schiff befindet sich bereits in diesem Sektor.");
            return false;
        }

        // Treibstoff prüfen und abziehen.
        // Umgehen für Testing
        if (!force && ship && !ship.TryConsumeFuel(distance))
        {
            state = TravelState.NoFuel;
            onNoFuel?.Invoke();
            return false;
        }

        // Ausrüstung einziehen (vor dem State-Wechsel, damit nichts blockiert).
        if (ship)
        {
            ship.SetSolarSail(false);
            ship.SetWaterHarvester(false);

            // Laufende Analyse im verlassenen Sektor abbrechen.
            Sector leaving = ship.CurrentSector;
            if (leaving)
                leaving.CancelAnalysis();
        }

        travelDuration = TravelDuration;
        travelTimer = 0f;
        state = TravelState.Traveling;

        onTravelStarted?.Invoke();

        // Entfernung 0: sofort ankommen.
        if (travelDuration <= 0f)
            FinishTravel();

        return true;
    }

    private void Update()
    {
        if (!IsTraveling) return;

        travelTimer += Time.deltaTime;
        if (travelTimer >= travelDuration)
            FinishTravel();
    }

    private void FinishTravel()
    {
        state = TravelState.Idle;
        travelTimer = 0f;

        if (ship)
            ship.SetSector(targetSector);


        if(targetSector.IsEnd)
        {
            OnEndSectorReached?.Invoke();
        }
        targetSector = null;
        distance = 0f;

        onTravelFinished?.Invoke();
    }

    public UnityEvent OnEndSectorReached;
}
