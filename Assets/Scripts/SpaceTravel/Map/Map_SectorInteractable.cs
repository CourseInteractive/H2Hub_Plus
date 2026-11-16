using System.Collections.Generic;
using UnityEngine;

public enum MapSectorState
{
    Current,     // aktueller Sektor, nicht interagierbar
    Reachable,   // direkt verbunden, interagierbar
    Unreachable  // sichtbar, aber nicht direkt verbunden, nicht interagierbar
}

/// <summary>
/// Ein Sektor auf der Karte. Nur erreichbare Sektoren (Reachable) können als Reiseziel gewählt werden.
/// Hover wird immer an die Map gemeldet, damit die MapSectorUI auch Gründe für Nicht-Erreichbarkeit zeigen kann.
/// </summary>
public class Map_SectorInteractable : MonoBehaviour
{
    [SerializeField] private MeshRenderer meshRenderer;

    [Header("Materialien")]
    [Tooltip("Erreichbar, bei Hover")]
    [SerializeField] private Material selectedMaterial;
    [Tooltip("Erreichbar")]
    [SerializeField] private Material unselectedMaterial;
    [Tooltip("Aktueller Sektor")]
    [SerializeField] private Material currentMaterial;
    [Tooltip("Nicht erreichbar")]
    [SerializeField] private Material unreachableMaterial;

    [Header("Zustand")]
    public bool interactable;
    public Sector sector;
    [SerializeField] private MapSectorState state = MapSectorState.Unreachable;

    private readonly List<string> reachableVia = new List<string>();

    public MapSectorState State => state;
    /// <summary>Entfernung vom aktuellen Sektor (nur bei Reachable gesetzt).</summary>
    public float Distance { get; private set; }
    /// <summary>Namen der sichtbaren Sektoren, von denen aus dieser Sektor erreichbar ist.</summary>
    public IReadOnlyList<string> ReachableVia => reachableVia;

    /// <summary>Wird von der Map beim Aufbau aufgerufen.</summary>
    public void Setup(Sector newSector, MapSectorState newState, float distance = 0f, List<string> via = null)
    {
        sector = newSector;
        if (sector)
            name = "MapSector_" + sector.Id;

        Distance = distance;
        reachableVia.Clear();
        if (via != null)
            reachableVia.AddRange(via);

        SetState(newState);
    }

    /// <summary>Setzt den Zustand. Nur Reachable ist interagierbar.</summary>
    public void SetState(MapSectorState newState)
    {
        state = newState;
        interactable = state == MapSectorState.Reachable;
        ApplyBaseMaterial();
    }

    public void Hover()
    {
        if (state == MapSectorState.Reachable)
            SetMaterial(selectedMaterial);

        if (Map.instance)
            Map.instance.SectorHovered(this);
    }

    public void HoverEnd()
    {
        ApplyBaseMaterial();

        if (Map.instance)
            Map.instance.SectorHoverEnded(this);
    }

    [ContextMenu("StartTravel")]
    public void TravelTo()
    {
        if (!interactable || !Map.instance) return;
        Map.instance.SectorSelected(this);
    }

    private void ApplyBaseMaterial()
    {
        switch (state)
        {
            case MapSectorState.Current:     SetMaterial(currentMaterial); break;
            case MapSectorState.Reachable:   SetMaterial(unselectedMaterial); break;
            case MapSectorState.Unreachable: SetMaterial(unreachableMaterial); break;
        }
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
