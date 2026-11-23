using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Ermittelt im Editor das LokaKit der aktuellen Szene.
/// Sucht beim Laden/Wechseln von Szenen nach SimpleLocalization_InGame und nimmt dessen 'kit'.
/// Das zuletzt gefundene Kit wird gemerkt (EditorPrefs), damit der Drawer auch funktioniert,
/// wenn die geöffnete Szene / Prefab keins enthält.
/// </summary>
[InitializeOnLoad]
public static class LocaSceneKit
{
    private const string PrefKey = "LocaString_SceneKitGuid";

    private static SimpleLocalization_InGame _inGame;
    private static LokaKit _lastKit;
    private static bool _searchPending = true;

    static LocaSceneKit()
    {
        EditorSceneManager.sceneOpened += (scene, mode) => _searchPending = true;
        EditorSceneManager.sceneClosed += scene => _searchPending = true;
        EditorSceneManager.activeSceneChangedInEditMode += (a, b) => _searchPending = true;
        EditorApplication.playModeStateChanged += state => _searchPending = true;
        // Komponente wurde evtl. neu hinzugefügt / gelöscht
        EditorApplication.hierarchyChanged += () => { if (_inGame == null) _searchPending = true; };
    }

    /// <summary>Das LokaKit, das der Drawer benutzen soll (kann null sein).</summary>
    public static LokaKit Kit
    {
        get
        {
            // Im Play Mode zählt, was tatsächlich registriert ist
            if (Application.isPlaying && SimpleLocalization.inGame != null && SimpleLocalization.inGame.kit != null)
                return Remember(SimpleLocalization.inGame.kit);

            if (_searchPending)
                Search();

            // Feld wird jedes Mal frisch gelesen -> Änderungen am Kit-Feld greifen sofort
            if (_inGame != null && _inGame.kit != null)
                return Remember(_inGame.kit);

            if (_lastKit == null)
                _lastKit = LoadRemembered();
            return _lastKit;
        }
    }

    /// <summary>Gefundene Komponente in der Szene (für Hinweise im Drawer).</summary>
    public static SimpleLocalization_InGame SceneComponent
    {
        get
        {
            if (_searchPending) Search();
            return _inGame;
        }
    }

    private static void Search()
    {
        _searchPending = false;
        _inGame = null;

        Scene active = SceneManager.GetActiveScene();
        foreach (var candidate in Resources.FindObjectsOfTypeAll<SimpleLocalization_InGame>())
        {
            if (candidate == null || EditorUtility.IsPersistent(candidate)) continue; // Prefab-Assets ignorieren
            if (!candidate.gameObject.scene.IsValid()) continue;

            // Aktive Szene bevorzugen, sonst die erste gefundene
            if (_inGame == null || candidate.gameObject.scene == active)
                _inGame = candidate;
        }
    }

    private static LokaKit Remember(LokaKit kit)
    {
        if (kit == _lastKit) return kit;
        _lastKit = kit;
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(kit, out string guid, out long _))
            EditorPrefs.SetString(PrefKey, guid);
        return kit;
    }

    private static LokaKit LoadRemembered()
    {
        string guid = EditorPrefs.GetString(PrefKey, "");
        if (string.IsNullOrEmpty(guid)) return null;
        string path = AssetDatabase.GUIDToAssetPath(guid);
        return string.IsNullOrEmpty(path) ? null : AssetDatabase.LoadAssetAtPath<LokaKit>(path);
    }
}
