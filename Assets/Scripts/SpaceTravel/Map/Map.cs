using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Horizontale Sektorenkarte.
/// Spalte 0 (vorne) = aktueller Sektor mit Schiff. Jede weitere Spalte = ein Level höher,
/// gezeigt werden nur Sektoren, die über Verbindungen vom aktuellen Sektor aus erreichbar sind.
/// Nur direkt verbundene Sektoren sind interaktiv.
/// </summary>
public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("Referenzen")]
    public SpaceShip ship;
    public Travel travelMechanic;
    public GameObject shipOnMap;

    [Header("Daten")]
    [SerializeField] private SectorMapData mapData;
    [SerializeField] private Map_SectorInteractable sectorPrefab;
    [Tooltip("Unter diesem Objekt werden die Kartensektoren erzeugt. Leer = dieses Objekt")]
    [SerializeField] private Transform contentRoot;
    [Tooltip("Setzt das Schiff beim Start in den Start-Sektor der SectorMapData")]
    [SerializeField] private bool setShipToStartSector = true;

    [Header("Layout (lokal)")]
    [Tooltip("Abstand zwischen den Level-Spalten in X")]
    [SerializeField] private float columnSpacing = 0.2f;
    [Tooltip("Abstand zwischen Sektoren innerhalb einer Spalte")]
    [SerializeField] private float rowSpacing = 0.15f;
    [Tooltip("Richtung, in der die Sektoren einer Spalte aufgereiht werden")]
    [SerializeField] private Vector3 rowDirection = Vector3.forward;
    [Tooltip("Wie viele Level nach vorne maximal angezeigt werden")]
    [Min(1)] [SerializeField] private int maxDepth = 3;

    private readonly Dictionary<string, Sector> runtimeSectors = new Dictionary<string, Sector>();
    private readonly List<Map_SectorInteractable> interactables = new List<Map_SectorInteractable>();
    private Map_SectorInteractable currentEntry;
    private Sector builtForSector;
    private bool visible;

    private Transform Root => contentRoot ? contentRoot : transform;
    public IReadOnlyList<Map_SectorInteractable> Interactables => interactables;

    // ---------------- Lebenszyklus ----------------

    private void Awake()
    {
        instance = this;
        CreateRuntimeSectors();

        if (setShipToStartSector && ship && mapData)
        {
            Sector start = GetRuntimeSector(mapData.StartSectorId);
            if (start)
                ship.SetSector(start);
        }
    }

    private void Start()
    {
        Hide();
    }

    private void OnEnable()
    {
        // Nur neu aufbauen, wenn sich der aktuelle Sektor seit dem letzten Aufbau geändert hat.
        if (ship && !ship.IsTraveling && ship.CurrentSector != builtForSector)
            BuildMap();
        else
            PlaceShip();
    }

    // ---------------- Anzeigen ----------------

    public void Toggle()
    {
        if (visible) Hide();
        else Show();
    }

    public void Show()
    {
        gameObject.SetActive(true);
        visible = true;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        visible = false;
    }

    // ---------------- Laufzeit-Sektoren ----------------

    /// <summary>
    /// Erzeugt für jeden Sektor der Daten genau ein Sector-Objekt.
    /// Diese liegen außerhalb der Map, damit Analysen weiterlaufen, wenn die Map ausgeblendet ist,
    /// und damit Wasserstand und Analyse-Zustand pro Sektor erhalten bleiben.
    /// </summary>
    private void CreateRuntimeSectors()
    {
        if (!mapData) return;

        Transform holder = new GameObject("RuntimeSectors").transform;

        foreach (SectorData data in mapData.Sectors)
        {
            if (data == null || string.IsNullOrEmpty(data.id) || runtimeSectors.ContainsKey(data.id))
                continue;

            GameObject go = new GameObject("Sector_" + data.id);
            go.transform.SetParent(holder, false);

            Sector sector = go.AddComponent<Sector>();
            sector.Initialize(data);
            runtimeSectors.Add(data.id, sector);
        }
    }

    public Sector GetRuntimeSector(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        runtimeSectors.TryGetValue(id, out Sector sector);
        return sector;
    }

    // ---------------- Kartenaufbau ----------------

    /// <summary>Baut die Karte ausgehend vom aktuellen Sektor des Schiffs neu auf.</summary>
    public void BuildMap()
    {
        ClearMap();

        Sector current = ship ? ship.CurrentSector : null;
        builtForSector = current;

        if (!current || !mapData || !sectorPrefab) return;

        SectorData currentData = mapData.GetSector(current.Id);
        if (currentData == null)
        {
            Debug.LogWarning($"Map: Aktueller Sektor '{current.Id}' ist nicht in der SectorMapData.");
            return;
        }

        int baseLevel = currentData.level;
        int maxLevel = baseLevel + maxDepth;

        // Alle erreichbaren Sektoren im sichtbaren Levelbereich sammeln (Breitensuche).
        HashSet<string> reachable = new HashSet<string>();
        Queue<SectorData> queue = new Queue<SectorData>();
        queue.Enqueue(currentData);

        while (queue.Count > 0)
        {
            SectorData from = queue.Dequeue();
            foreach (SectorConnection c in from.connections)
            {
                if (c == null || reachable.Contains(c.targetSectorId)) continue;

                SectorData target = mapData.GetSector(c.targetSectorId);
                if (target == null || target.level <= baseLevel || target.level > maxLevel) continue;

                reachable.Add(target.id);
                queue.Enqueue(target);
            }
        }

        // Direkt verbundene Sektoren sind interaktiv.
        HashSet<string> direct = new HashSet<string>();
        foreach (SectorConnection c in currentData.connections)
            if (c != null && reachable.Contains(c.targetSectorId))
                direct.Add(c.targetSectorId);

        // Spalte 0: aktueller Sektor
        currentEntry = CreateEntry(currentData, 0, 0, 1, false);

        // Weitere Spalten nach Level
        List<SectorData> column = new List<SectorData>();
        for (int depth = 1; depth <= maxDepth; depth++)
        {
            column.Clear();
            int level = baseLevel + depth;

            foreach (SectorData s in mapData.Sectors)
                if (s != null && s.level == level && reachable.Contains(s.id))
                    column.Add(s);

            for (int i = 0; i < column.Count; i++)
                CreateEntry(column[i], depth, i, column.Count, direct.Contains(column[i].id));
        }

        PlaceShip();
    }

    private Map_SectorInteractable CreateEntry(SectorData data, int depth, int index, int count, bool isInteractable)
    {
        Sector runtime = GetRuntimeSector(data.id);
        if (!runtime) return null;

        Map_SectorInteractable entry = Instantiate(sectorPrefab, Root);

        // Zentriert: Mitte der Spalte liegt auf der X-Achse.
        float rowOffset = (index - (count - 1) * 0.5f) * rowSpacing;
        entry.transform.localPosition = Vector3.right * (depth * columnSpacing)
                                      + rowDirection.normalized * rowOffset;

        entry.Setup(runtime, isInteractable);
        interactables.Add(entry);
        return entry;
    }

    private void ClearMap()
    {
        foreach (Map_SectorInteractable entry in interactables)
            if (entry)
                Destroy(entry.gameObject);

        interactables.Clear();
        currentEntry = null;
    }

    private void PlaceShip()
    {
        if (!shipOnMap) return;

        shipOnMap.SetActive(currentEntry != null);
        if (currentEntry)
            shipOnMap.transform.position = currentEntry.transform.position;
    }

    // ---------------- Auswahl ----------------

    public void SectorSelected(Map_SectorInteractable entry)
    {
        if (!entry || !entry.interactable || !entry.sector || !ship || !travelMechanic) return;

        Sector current = ship.CurrentSector;
        if (!current) return;

        if (!mapData.TryGetDistance(current.Id, entry.sector.Id, out float distance))
        {
            Debug.LogWarning($"Map: Keine Verbindung von '{current.Id}' nach '{entry.sector.Id}'.");
            return;
        }

        travelMechanic.SetTargetSector(entry.sector);
        travelMechanic.SetDistance(distance);

        // Karte nur schließen, wenn die Reise wirklich startet (z.B. nicht bei NoFuel).
        if (travelMechanic.StartTravelIfPossible())
            Hide();
    }
}
