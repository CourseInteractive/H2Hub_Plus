using UnityEngine;

/// <summary>
/// Verbindungslinie auf der Karte. Färbt alle MeshRenderer des Objekts
/// je nachdem, ob die Verbindung zu einem erreichbaren Sektor führt.
/// </summary>
public class MapConnection : MonoBehaviour
{
    [SerializeField] private Material reachableMaterial;
    [SerializeField] private Material unreachableMaterial;

    private MeshRenderer[] renderers;

    private void Awake()
    {
        CacheRenderers();
    }

    public void SetReachable(bool reachable)
    {
        if (renderers == null)
            CacheRenderers();

        Material material = reachable ? reachableMaterial : unreachableMaterial;
        if (!material) return;

        // sharedMaterial: keine Material-Kopien pro Instanz.
        foreach (MeshRenderer r in renderers)
            if (r)
                r.sharedMaterial = material;
    }

    private void CacheRenderers()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(true);
    }
}
