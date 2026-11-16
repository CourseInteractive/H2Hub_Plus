using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Horizontale Sektorenkarte.
/// Spalte 0 (vorne) = aktueller Sektor mit Schiff. Jede weitere Spalte = ein Level höher,
/// gezeigt werden nur Sektoren, die über Verbindungen vom aktuellen Sektor aus erreichbar sind.
/// Zustände: Current (aktueller Sektor), Reachable (direkt verbunden), Unreachable (sonst).
/// </summary>
public class Map : MonoBehaviour
{
    public static Map instance;

    [Header("Referenzen")]
    public SpaceShip ship;
    public Travel travelMechanic;
    public GameObject shipOnMap;
    [Tooltip("Marker, der am End-Sektor angezeigt wird (falls er auf der Karte sichtbar ist)")]
    [FormerlySerializedAs("targetOnMap")]
    public GameObject endOnMap;
    [SerializeField] private MapSectorUI sectorUI;

    [Header("Daten")]
    [SerializeField] private SectorMapData mapData;
    [SerializeField] private Map_SectorInteractable sectorPrefab;
    [Tooltip("Verbindungslinie: z.B. ein Cube der Größe 1, zentriert. Wird entlang Z auf die Länge skaliert.")]
    [SerializeField] private MapConnection connectionPrefab;
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
    private readonly List<MapConnection> connections = new List<MapConnection>();
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
        Refresh(true);
    }

    private void Update()
    {
        // Läuft nur, solange die Karte sichtbar ist.
        Refresh(false);
    }

    /// <summary>Baut die Karte nur neu auf, wenn sich der aktuelle Sektor geändert hat.</summary>
    private void Refresh(bool forceMarkers)
    {
        if (ship && !ship.IsTraveling && ship.CurrentSector != builtForSector)
        {
            BuildMap();
            return;
        }

        if (forceMarkers)
        {
            PlaceShip();
            PlaceEndMarker();
        }
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
        if (sectorUI)
            sectorUI.Clear();

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

        if (!current || !mapData || !sectorPrefab)
        {
            PlaceShip();
            PlaceEndMarker();
            return;
        }

        SectorData currentData = mapData.GetSector(current.Id);
        if (currentData == null)
        {
            Debug.LogWarning($"Map: Aktueller Sektor '{current.Id}' ist nicht in der SectorMapData.");
            return;
        }

        int baseLevel = currentData.level;
        int maxLevel = baseLevel + maxDepth;

        // Alle erreichbaren Sektoren im sichtbaren Levelbereich sammeln (Breitensuche).
        // Dabei merken, von welchen Sektoren aus jeder Sektor erreichbar ist.
        HashSet<string> reachable = new HashSet<string>();
        Dictionary<string, List<string>> via = new Dictionary<string, List<string>>();
        List<(string from, string to)> edges = new List<(string from, string to)>();
        HashSet<string> edgeKeys = new HashSet<string>();
        Queue<SectorData> queue = new Queue<SectorData>();
        queue.Enqueue(currentData);

        while (queue.Count > 0)
        {
            SectorData from = queue.Dequeue();
            string fromName = string.IsNullOrEmpty(from.displayName) ? from.id : from.displayName;

            foreach (SectorConnection c in from.connections)
            {
                if (c == null) continue;

                SectorData target = mapData.GetSector(c.targetSectorId);
                if (target == null || target.level <= baseLevel || target.level > maxLevel) continue;

                if (!via.TryGetValue(target.id, out List<string> list))
                {
                    list = new List<string>();
                    via.Add(target.id, list);
                }
                if (!list.Contains(fromName))
                    list.Add(fromName);

                // Verbindung für die Darstellung merken (A->B und B->A nur einmal).
                string key = string.CompareOrdinal(from.id, target.id) < 0
                    ? from.id + "|" + target.id
                    : target.id + "|" + from.id;
                if (edgeKeys.Add(key))
                    edges.Add((from.id, target.id));

                if (reachable.Add(target.id))
                    queue.Enqueue(target);
            }
        }

        // Direkt verbundene Sektoren sind erreichbar (interagierbar).
        HashSet<string> direct = new HashSet<string>();
        foreach (SectorConnection c in currentData.connections)
            if (c != null && reachable.Contains(c.targetSectorId))
                direct.Add(c.targetSectorId);

        // Spalte 0: aktueller Sektor
        currentEntry = CreateEntry(currentData, 0, 0, 1, MapSectorState.Current, 0f, null);

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
            {
                SectorData s = column[i];
                via.TryGetValue(s.id, out List<string> viaList);

                if (direct.Contains(s.id))
                {
                    mapData.TryGetDistance(currentData.id, s.id, out float distance);
                    CreateEntry(s, depth, i, column.Count, MapSectorState.Reachable, distance, viaList);
                }
                else
                {
                    CreateEntry(s, depth, i, column.Count, MapSectorState.Unreachable, 0f, viaList);
                }
            }
        }

        CreateConnections(edges);

        PlaceShip();
        PlaceEndMarker();
    }

    private void CreateConnections(List<(string from, string to)> edges)
    {
        if (!connectionPrefab) return;

        Dictionary<string, Map_SectorInteractable> byId = new Dictionary<string, Map_SectorInteractable>();
        foreach (Map_SectorInteractable entry in interactables)
            if (entry && entry.sector && !byId.ContainsKey(entry.sector.Id))
                byId.Add(entry.sector.Id, entry);

        Vector3 prefabScale = connectionPrefab.transform.localScale;

        foreach ((string fromId, string toId) in edges)
        {
            if (!byId.TryGetValue(fromId, out Map_SectorInteractable a) ||
                !byId.TryGetValue(toId, out Map_SectorInteractable b))
                continue;

            // Alles im lokalen Raum des Root, da die Sektoren dort liegen.
            Vector3 start = a.transform.localPosition;
            Vector3 end = b.transform.localPosition;
            Vector3 dir = end - start;
            float length = dir.magnitude;
            if (length < 0.0001f) continue;

            MapConnection line = Instantiate(connectionPrefab, Root);
            line.name = $"Connection_{fromId}_{toId}";
            line.transform.localPosition = (start + end) * 0.5f;

            Vector3 up = Mathf.Abs(Vector3.Dot(dir / length, Vector3.up)) > 0.99f ? Vector3.forward : Vector3.up;
            line.transform.localRotation = Quaternion.LookRotation(dir, up);
            line.transform.localScale = new Vector3(prefabScale.x, prefabScale.y, length);

            // Erreichbar = Verbindung vom aktuellen Sektor zu einem erreichbaren Sektor.
            line.SetReachable(a.State == MapSectorState.Current && b.State == MapSectorState.Reachable);

            connections.Add(line);
        }
    }

    private Map_SectorInteractable CreateEntry(SectorData data, int depth, int index, int count,
        MapSectorState state, float distance, List<string> via)
    {
        Sector runtime = GetRuntimeSector(data.id);
        if (!runtime) return null;

        Map_SectorInteractable entry = Instantiate(sectorPrefab, Root);

        // Zentriert: Mitte der Spalte liegt auf der X-Achse.
        float rowOffset = (index - (count - 1) * 0.5f) * rowSpacing;
        entry.transform.localPosition = Vector3.right * (depth * columnSpacing)
                                      + rowDirection.normalized * rowOffset;

        entry.Setup(runtime, state, distance, via);
        interactables.Add(entry);
        return entry;
    }

    private void ClearMap()
    {
        if (sectorUI)
            sectorUI.Clear();

        foreach (Map_SectorInteractable entry in interactables)
            if (entry)
                Destroy(entry.gameObject);

        interactables.Clear();
        currentEntry = null;

        foreach (MapConnection line in connections)
            if (line)
                Destroy(line.gameObject);
        connections.Clear();
    }

    private Map_SectorInteractable FindEndEntry()
    {
        foreach (Map_SectorInteractable entry in interactables)
            if (entry && entry.sector && entry.sector.IsEnd)
                return entry;
        return null;
    }

    // ---------------- Marker ----------------

    private void PlaceShip()
    {
        if (!shipOnMap) return;

        shipOnMap.SetActive(currentEntry != null);
        if (currentEntry)
            shipOnMap.transform.position = currentEntry.transform.position;
    }

    /// <summary>Setzt den End-Marker auf den End-Sektor, falls dieser gerade auf der Karte zu sehen ist.</summary>
    private void PlaceEndMarker()
    {
        if (!endOnMap) return;

        Map_SectorInteractable entry = FindEndEntry();
        endOnMap.SetActive(entry != null);
        if (entry)
            endOnMap.transform.position = entry.transform.position;
    }

    // ---------------- Hover & Auswahl ----------------

    public void SectorHovered(Map_SectorInteractable entry)
    {
        if (sectorUI)
            sectorUI.Show(entry);
    }

    public void SectorHoverEnded(Map_SectorInteractable entry)
    {
        if (sectorUI)
            sectorUI.Hide(entry);
    }

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

        // Karte nur schließen, wenn die Reise wirklich startet.
        if (travelMechanic.StartTravelIfPossible())
        {
            Hide();
            return;
        }

        if (travelMechanic.State == TravelState.NoFuel && sectorUI)
            sectorUI.ShowNoFuel(entry, travelMechanic.RequiredFuel);
    }

    public void TriggerTestTravelToEnd()
    {
        Map_SectorInteractable end = FindEndEntry();
        travelMechanic.SetTargetSector(end.sector);
        travelMechanic.SetDistance(40);
        travelMechanic.StartTravelIfPossible(true);
        Hide();
    }
}
