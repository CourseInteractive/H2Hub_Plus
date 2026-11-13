using UnityEngine;
using UnityEngine.Events;

public enum CollectorType
{
    Water,
    Energy
}

/// <summary>
/// Zeigt an, ob der Wassersammler oder das Sonnensegel (Energiesammler) des Schiffs aktiv ist.
/// Wechselt das Material eines MeshRenderers und löst Events beim Ein- und Ausschalten aus.
/// </summary>
public class CollectorIndicator : MonoBehaviour
{
    [Header("Quelle")]
    [SerializeField] private SpaceShip ship;
    [SerializeField] private CollectorType collectorType = CollectorType.Energy;

    [Header("Darstellung")]
    [SerializeField] private MeshRenderer meshRenderer;
    [Tooltip("Index des Materials, falls der Renderer mehrere Materialien hat")]
    [SerializeField] private int materialIndex = 0;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    [Header("Events")]
    public UnityEvent onSwitchedOn;
    public UnityEvent onSwitchedOff;
    [Tooltip("Events auch beim Start einmal mit dem Anfangszustand auslösen")]
    [SerializeField] private bool invokeEventsOnStart = false;

    private bool lastState;

    public bool LastState => lastState;

    private void Start()
    {
        lastState = GetCurrentState();
        ApplyMaterial(lastState);

        if (invokeEventsOnStart)
            InvokeEvent(lastState);
    }

    private void Update()
    {
        bool current = GetCurrentState();
        if (current == lastState) return;

        lastState = current;
        ApplyMaterial(current);
        InvokeEvent(current);
    }

    private bool GetCurrentState()
    {
        if (!ship) return false;

        switch (collectorType)
        {
            case CollectorType.Water:  return ship.WaterHarvesterActive;
            case CollectorType.Energy: return ship.SolarSailActive;
            default:                   return false;
        }
    }

    private void ApplyMaterial(bool isOn)
    {
        if (!meshRenderer) return;

        Material target = isOn ? onMaterial : offMaterial;
        if (!target) return;

        // sharedMaterials verwenden, damit keine Material-Instanzen erzeugt werden.
        Material[] materials = meshRenderer.sharedMaterials;
        if (materialIndex < 0 || materialIndex >= materials.Length) return;

        materials[materialIndex] = target;
        meshRenderer.sharedMaterials = materials;
    }

    private void InvokeEvent(bool isOn)
    {
        if (isOn) onSwitchedOn?.Invoke();
        else onSwitchedOff?.Invoke();
    }

    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
