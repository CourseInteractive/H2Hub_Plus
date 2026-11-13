using UnityEngine;

/// <summary>
/// Ein Sektor auf der Karte. Nur interaktive Sektoren (direkt vom aktuellen Sektor erreichbar)
/// reagieren auf Hover und können als Reiseziel gewählt werden.
/// </summary>
public class Map_SectorInteractable : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private Material unselectedMaterial;

    public bool interactable;
    public Sector sector;

    /// <summary>Wird von der Map beim Aufbau aufgerufen.</summary>
    public void Setup(Sector newSector, bool isInteractable)
    {
        sector = newSector;
        if (sector)
            name = "MapSector_" + sector.Id;

        SetInteractable(isInteractable);
    }

    public void SetInteractable(bool value)
    {
        interactable = value;
        SetMaterial(unselectedMaterial);
    }

    public void Hover()
    {
        if (!interactable) return;
        SetMaterial(selectedMaterial);
    }

    public void HoverEnd()
    {
        SetMaterial(unselectedMaterial);
    }

    [ContextMenu("StartTravel")]
    public void TravelTo()
    {
        if (!interactable || !Map.instance) return;
        Map.instance.SectorSelected(this);
    }

    private void SetMaterial(Material material)
    {
        // sharedMaterial: keine Material-Kopien pro Instanz.
        if (meshRenderer && material)
            meshRenderer.sharedMaterial = material;
    }

    private void Reset()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
}
