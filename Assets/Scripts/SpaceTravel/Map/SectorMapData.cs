using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enthält alle Sektoren der Karte mit ihren Eigenschaften und Verbindungen.
/// Erstellen über: Rechtsklick im Project -> Create -> Space -> Sector Map
/// </summary>
[CreateAssetMenu(fileName = "SectorMap", menuName = "Space/Sector Map")]
public class SectorMapData : ScriptableObject
{
    [Tooltip("ID des Sektors, in dem das Spiel beginnt")]
    [SectorId]
    [SerializeField] private string startSectorId;

    [SerializeField] private List<SectorData> sectors = new List<SectorData>();

    private Dictionary<string, SectorData> lookup;

    public string StartSectorId => startSectorId;
    public IReadOnlyList<SectorData> Sectors => sectors;

    // ---------------- Abfragen ----------------

    public SectorData StartSector => GetSector(startSectorId);

    /// <summary>Gibt den Sektor mit der ID zurück oder null.</summary>
    public SectorData GetSector(string id)
    {
        TryGetSector(id, out SectorData sector);
        return sector;
    }

    public bool TryGetSector(string id, out SectorData sector)
    {
        sector = null;
        if (string.IsNullOrEmpty(id)) return false;

        BuildLookup();
        return lookup.TryGetValue(id, out sector);
    }

    /// <summary>Alle Sektoren eines Levels (für die spätere Anordnung auf der Karte).</summary>
    public List<SectorData> GetSectorsOfLevel(int level)
    {
        List<SectorData> result = new List<SectorData>();
        foreach (SectorData sector in sectors)
            if (sector.level == level)
                result.Add(sector);
        return result;
    }

    public int MinLevel
    {
        get
        {
            if (sectors.Count == 0) return 0;
            int min = int.MaxValue;
            foreach (SectorData s in sectors) min = Mathf.Min(min, s.level);
            return min;
        }
    }

    public int MaxLevel
    {
        get
        {
            if (sectors.Count == 0) return 0;
            int max = int.MinValue;
            foreach (SectorData s in sectors) max = Mathf.Max(max, s.level);
            return max;
        }
    }

    /// <summary>
    /// Sucht die Verbindung von einem Sektor zu einem Ziel.
    /// Verbindungen sind gerichtet: nur die beim Startsektor eingetragenen zählen.
    /// </summary>
    public bool TryGetConnection(string fromId, string toId, out SectorConnection connection)
    {
        connection = null;
        if (!TryGetSector(fromId, out SectorData from)) return false;

        foreach (SectorConnection c in from.connections)
        {
            if (c.targetSectorId == toId)
            {
                connection = c;
                return true;
            }
        }
        return false;
    }

    public bool TryGetDistance(string fromId, string toId, out float distance)
    {
        distance = 0f;
        if (!TryGetConnection(fromId, toId, out SectorConnection c)) return false;
        distance = c.distance;
        return true;
    }

    // ---------------- Intern ----------------

    private void BuildLookup()
    {
        if (lookup != null) return;

        lookup = new Dictionary<string, SectorData>();
        foreach (SectorData sector in sectors)
        {
            if (sector == null || string.IsNullOrEmpty(sector.id)) continue;
            if (!lookup.ContainsKey(sector.id))
                lookup.Add(sector.id, sector);
        }
    }

    private void OnEnable()
    {
        lookup = null;
    }

    /// <summary>Prüft die Daten im Editor und warnt bei Fehlern.</summary>
    private void OnValidate()
    {
        lookup = null;

        HashSet<string> ids = new HashSet<string>();
        foreach (SectorData sector in sectors)
        {
            if (sector == null) continue;

            if (string.IsNullOrEmpty(sector.id))
                Debug.LogWarning($"SectorMap '{name}': Ein Sektor ('{sector.displayName}') hat keine ID.", this);
            else if (!ids.Add(sector.id))
                Debug.LogWarning($"SectorMap '{name}': Doppelte Sektor-ID '{sector.id}'.", this);
        }

        if (!string.IsNullOrEmpty(startSectorId) && !ids.Contains(startSectorId))
            Debug.LogWarning($"SectorMap '{name}': Start-Sektor '{startSectorId}' existiert nicht.", this);

        foreach (SectorData sector in sectors)
        {
            if (sector == null || sector.connections == null) continue;

            HashSet<string> targets = new HashSet<string>();
            foreach (SectorConnection c in sector.connections)
            {
                if (c == null) continue;

                if (string.IsNullOrEmpty(c.targetSectorId))
                    Debug.LogWarning($"SectorMap '{name}': Verbindung in '{sector.id}' ohne Ziel.", this);
                else if (c.targetSectorId == sector.id)
                    Debug.LogWarning($"SectorMap '{name}': '{sector.id}' ist mit sich selbst verbunden.", this);
                else if (!ids.Contains(c.targetSectorId))
                    Debug.LogWarning($"SectorMap '{name}': Verbindung '{sector.id}' -> '{c.targetSectorId}': Ziel existiert nicht.", this);
                else if (!targets.Add(c.targetSectorId))
                    Debug.LogWarning($"SectorMap '{name}': Doppelte Verbindung '{sector.id}' -> '{c.targetSectorId}'.", this);

                if (c.distance <= 0f)
                    Debug.LogWarning($"SectorMap '{name}': Verbindung '{sector.id}' -> '{c.targetSectorId}' hat keine Entfernung.", this);
            }
        }
    }
}

/// <summary>
/// Markiert ein string-Feld als Sektor-ID. Im Inspector erscheint dann ein Dropdown
/// mit allen Sektoren der Karte (siehe Editor/SectorIdDrawer.cs).
/// </summary>
public class SectorIdAttribute : PropertyAttribute { }

/// <summary>Alle Eigenschaften eines Sektors.</summary>
[Serializable]
public class SectorData
{
    [Tooltip("Eindeutige ID, wird für Verbindungen verwendet")]
    public string id;
    public string displayName = "Neuer Sektor";
    [Tooltip("Ebene auf der Karte, für die spätere Anordnung")]
    public int level = 0;

    [Header("Eigenschaften")]
    [Tooltip("0 = keine Sonne, 1 = volle Sonne")]
    [Range(0f, 1f)] public float sun = 0.5f;
    public float waterAmount = 200f;
    [Tooltip("Dauer der Analyse in Sekunden")]
    public float analysisDuration = 5f;

    [Header("Verbindungen")]
    public List<SectorConnection> connections = new List<SectorConnection>();
}

/// <summary>Gerichtete Verbindung von einem Sektor zu einem Ziel-Sektor.</summary>
[Serializable]
public class SectorConnection
{
    [SectorId]
    public string targetSectorId;
    public float distance = 10f;
}
