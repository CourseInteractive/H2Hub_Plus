using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

/// <summary>
/// Überträgt alte string-Token-Felder in LocaString-Felder, die mit [LocaMigrateFrom("altesFeld")] markiert sind.
/// Durchsucht alle Prefabs, Szenen und ScriptableObjects unter Assets.
///
/// Regeln pro Feld:
///   "$Token" oder Text, der als Token in einem LokaKit existiert -> LocaString.token
///   "$Token", das in keinem LokaKit existiert                    -> LocaString.token (Drawer warnt)
///   alles andere (Klartext, noch nicht lokalisiert)              -> LocaString.draftText (-> "+ Token" im Inspector)
/// Danach wird das alte Feld geleert. Ein bereits befülltes LocaString wird nie überschrieben (-> Konflikt im Bericht).
/// "Nur prüfen" ändert nichts und schreibt nur den Bericht.
/// </summary>
public static class LocaMigrationTool
{
    private const string MenuPath = "Tools/Course/Localization/Alte Tokens nach LocaString migrieren";

    private enum Kind { Token, MissingToken, Draft }

    private class Report
    {
        public readonly StringBuilder Text = new StringBuilder();
        public int Tokens, MissingTokens, Drafts, Conflicts, ChangedFiles;
        public void Line(string line) => Text.AppendLine(line);
    }

    [MenuItem(MenuPath)]
    private static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            EditorUtility.DisplayDialog("Loca-Migration", "Bitte zuerst den Play Mode beenden.", "OK");
            return;
        }

        int choice = EditorUtility.DisplayDialogComplex(
            "Loca-Migration",
            "Überträgt alte Token-Felder (z.B. LokaElement.token) in die neuen LocaString-Felder.\n\n" +
            "Betroffen: alle Prefabs, Szenen und ScriptableObjects unter Assets.\n" +
            "Bitte vorher committen bzw. ein Backup machen.\n\n" +
            "\"Nur prüfen\" ändert nichts und erstellt nur einen Bericht.",
            "Migrieren", "Abbrechen", "Nur prüfen");

        if (choice == 1) return;          // Abbrechen
        bool apply = choice == 0;         // 0 = Migrieren, 2 = Nur prüfen

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            return;

        Migrate(apply);
    }

    private static void Migrate(bool apply)
    {
        var report = new Report();
        report.Line($"Loca-Migration {(apply ? "AUSGEFÜHRT" : "NUR PRÜFUNG – nichts geändert")}  {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
        report.Line("");

        var known = CollectKnownTokens();
        var sceneSetup = EditorSceneManager.GetSceneManagerSetup();

        try
        {
            MigratePrefabs(apply, known, report);
            MigrateScenes(apply, known, report);
            MigrateScriptableObjects(apply, known, report);
        }
        finally
        {
            EditorUtility.ClearProgressBar();
            if (sceneSetup.Length > 0)
                EditorSceneManager.RestoreSceneManagerSetup(sceneSetup);
        }

        string summary =
            $"Tokens übernommen: {report.Tokens}\n" +
            $"Tokens mit $, aber in keinem LokaKit: {report.MissingTokens}\n" +
            $"Klartexte als Entwurf übernommen: {report.Drafts}\n" +
            $"Konflikte (nicht angefasst): {report.Conflicts}\n" +
            $"{(apply ? "Geänderte" : "Betroffene")} Dateien: {report.ChangedFiles}";

        report.Line("");
        report.Line(summary);

        string fileName = $"LocaMigration_{(apply ? "" : "Pruefung_")}{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        string reportPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, fileName);
        File.WriteAllText(reportPath, report.Text.ToString(), Encoding.UTF8);

        Debug.Log($"[Loca] Migration {(apply ? "fertig" : "geprüft")}. Bericht: {reportPath}\n{summary}");
        EditorUtility.DisplayDialog("Loca-Migration", summary + $"\n\nBericht:\n{fileName}", "OK");
        EditorUtility.RevealInFinder(reportPath);
    }

    // ------------------------------------------------------------------
    // Prefabs, Szenen, ScriptableObjects
    // ------------------------------------------------------------------

    private static void MigratePrefabs(bool apply, Dictionary<string, string> known, Report report)
    {
        // Basis-Prefabs vor Prefabs, die sie verschachteln/varianten -> möglichst wenige Overrides
        var paths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase))
            .Distinct()
            .OrderBy(p => AssetDatabase.GetDependencies(p, true)
                                       .Count(d => d.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)))
            .ToList();

        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            EditorUtility.DisplayProgressBar("Loca-Migration: Prefabs", path, (float)i / paths.Count);

            // Schneller Vorab-Check, um nicht jedes Prefab öffnen zu müssen
            var asset = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (asset == null || !asset.GetComponentsInChildren<MonoBehaviour>(true)
                                       .Any(c => c != null && GetMigrationFields(c.GetType()).Count > 0))
                continue;

            var root = PrefabUtility.LoadPrefabContents(path);
            try
            {
                if (ProcessHierarchy(new[] { root }, path, apply, known, report))
                {
                    report.ChangedFiles++;
                    if (apply) PrefabUtility.SaveAsPrefabAsset(root, path);
                }
            }
            finally
            {
                PrefabUtility.UnloadPrefabContents(root);
            }
        }
    }

    private static void MigrateScenes(bool apply, Dictionary<string, string> known, Report report)
    {
        var paths = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Distinct()
            .ToList();

        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            EditorUtility.DisplayProgressBar("Loca-Migration: Szenen", path, (float)i / paths.Count);

            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            if (ProcessHierarchy(scene.GetRootGameObjects(), path, apply, known, report))
            {
                report.ChangedFiles++;
                if (apply)
                {
                    EditorSceneManager.MarkSceneDirty(scene);
                    EditorSceneManager.SaveScene(scene);
                }
            }
        }
    }

    private static void MigrateScriptableObjects(bool apply, Dictionary<string, string> known, Report report)
    {
        var paths = AssetDatabase.FindAssets("t:ScriptableObject", new[] { "Assets" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Distinct()
            .ToList();

        for (int i = 0; i < paths.Count; i++)
        {
            string path = paths[i];
            EditorUtility.DisplayProgressBar("Loca-Migration: ScriptableObjects", path, (float)i / paths.Count);

            bool fileChanged = false;
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (!(obj is ScriptableObject so) || GetMigrationFields(so.GetType()).Count == 0)
                    continue;

                if (ProcessObject(so, $"{path} › {so.name}", apply, known, report))
                {
                    fileChanged = true;
                    if (apply) EditorUtility.SetDirty(so);
                }
            }
            if (fileChanged) report.ChangedFiles++;
        }

        if (apply) AssetDatabase.SaveAssets();
    }

    // ------------------------------------------------------------------
    // Kern
    // ------------------------------------------------------------------

    private static bool ProcessHierarchy(IEnumerable<GameObject> roots, string file, bool apply,
                                         Dictionary<string, string> known, Report report)
    {
        bool changed = false;
        foreach (var root in roots)
        {
            foreach (var component in root.GetComponentsInChildren<MonoBehaviour>(true))
            {
                if (component == null) continue; // fehlendes Script
                if (GetMigrationFields(component.GetType()).Count == 0) continue;

                string context = $"{file} › {GetPath(component.transform)} ({component.GetType().Name})";
                changed |= ProcessObject(component, context, apply, known, report);
            }
        }
        return changed;
    }

    /// <returns>true, wenn etwas migriert wurde bzw. (im Prüfmodus) würde.</returns>
    private static bool ProcessObject(UnityEngine.Object target, string context, bool apply,
                                      Dictionary<string, string> known, Report report)
    {
        var so = new SerializedObject(target);
        var migratedLegacyFields = new List<string>();

        foreach (var (locaField, legacyField) in GetMigrationFields(target.GetType()))
        {
            var legacyProp = so.FindProperty(legacyField);
            var locaProp = so.FindProperty(locaField);

            if (legacyProp == null || locaProp == null || legacyProp.propertyType != SerializedPropertyType.String)
            {
                report.Line($"[FEHLER]   {context}: Feld '{legacyField}' oder '{locaField}' nicht gefunden bzw. kein string.");
                continue;
            }

            string oldValue = legacyProp.stringValue;
            if (string.IsNullOrWhiteSpace(oldValue))
                continue;

            var tokenProp = locaProp.FindPropertyRelative("token");
            var draftProp = locaProp.FindPropertyRelative("draftText");

            bool locaEmpty = string.IsNullOrEmpty(tokenProp.stringValue) && string.IsNullOrEmpty(draftProp.stringValue);
            // Prefab-Instanz: altes Feld überschrieben, neues nur vom (bereits migrierten) Prefab geerbt
            bool instanceOverride = legacyProp.prefabOverride && !locaProp.prefabOverride;

            if (!locaEmpty && !instanceOverride)
            {
                report.Conflicts++;
                report.Line($"[KONFLIKT] {context}.{legacyField}: \"{oldValue}\" – LocaString ist schon befüllt " +
                            $"(Token \"{tokenProp.stringValue}\", Entwurf \"{draftProp.stringValue}\"). Nicht angefasst.");
                continue;
            }

            Kind kind = Classify(oldValue, known, out string newToken, out string newDraft);
            switch (kind)
            {
                case Kind.Token:
                    report.Tokens++;
                    report.Line($"[TOKEN]    {context}.{legacyField}: \"{oldValue}\" → Token \"{newToken}\"");
                    break;
                case Kind.MissingToken:
                    report.MissingTokens++;
                    report.Line($"[FEHLT]    {context}.{legacyField}: \"{oldValue}\" → Token \"{newToken}\" (in keinem LokaKit!)");
                    break;
                default:
                    report.Drafts++;
                    report.Line($"[ENTWURF]  {context}.{legacyField}: \"{oldValue}\" → Entwurfstext");
                    break;
            }

            if (apply)
            {
                tokenProp.stringValue = newToken;
                draftProp.stringValue = newDraft;
                legacyProp.stringValue = string.Empty;
            }
            migratedLegacyFields.Add(legacyField);
        }

        if (migratedLegacyFields.Count == 0)
            return false;

        if (apply)
        {
            so.ApplyModifiedPropertiesWithoutUndo();

            // Geleertes altes Feld auf Prefab-Instanzen nicht als Override stehen lassen
            if (PrefabUtility.IsPartOfPrefabInstance(target))
            {
                so.Update();
                foreach (string legacyField in migratedLegacyFields)
                {
                    var legacyProp = so.FindProperty(legacyField);
                    if (legacyProp != null && legacyProp.prefabOverride)
                        PrefabUtility.RevertPropertyOverride(legacyProp, InteractionMode.AutomatedAction);
                }
            }
        }
        return true;
    }

    private static Kind Classify(string legacy, Dictionary<string, string> known, out string token, out string draft)
    {
        string trimmed = legacy.Trim();
        bool explicitToken = trimmed.StartsWith("$");
        string candidate = explicitToken ? trimmed.Substring(1).Trim() : trimmed;

        if (known.TryGetValue(candidate, out string original))
        {
            token = original;   // Schreibweise aus dem LokaKit übernehmen
            draft = string.Empty;
            return Kind.Token;
        }

        if (explicitToken)
        {
            token = candidate;
            draft = string.Empty;
            return Kind.MissingToken;
        }

        token = string.Empty;
        draft = trimmed;
        return Kind.Draft;
    }

    // ------------------------------------------------------------------
    // Hilfen
    // ------------------------------------------------------------------

    /// <summary>Alle Tokens aus allen LokaKits (Groß-/Kleinschreibung egal).</summary>
    private static Dictionary<string, string> CollectKnownTokens()
    {
        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string guid in AssetDatabase.FindAssets("t:LokaKit"))
        {
            var kit = AssetDatabase.LoadAssetAtPath<LokaKit>(AssetDatabase.GUIDToAssetPath(guid));
            if (kit == null || kit.entries == null) continue;

            foreach (var entry in kit.entries)
            {
                if (entry == null || string.IsNullOrWhiteSpace(entry.token)) continue;
                string token = entry.token.Trim();
                if (!result.ContainsKey(token))
                    result[token] = token;
            }
        }
        return result;
    }

    private static readonly Dictionary<Type, List<(string loca, string legacy)>> FieldCache =
        new Dictionary<Type, List<(string, string)>>();

    /// <summary>Alle LocaString-Felder mit [LocaMigrateFrom] – auch private und geerbte.</summary>
    private static List<(string loca, string legacy)> GetMigrationFields(Type type)
    {
        if (FieldCache.TryGetValue(type, out var list))
            return list;

        list = new List<(string, string)>();
        for (var t = type; t != null && t != typeof(MonoBehaviour) && t != typeof(ScriptableObject); t = t.BaseType)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public |
                                       BindingFlags.NonPublic | BindingFlags.DeclaredOnly;
            foreach (var field in t.GetFields(flags))
            {
                if (field.FieldType != typeof(LocaString)) continue;
                var attribute = field.GetCustomAttribute<LocaMigrateFromAttribute>();
                if (attribute != null)
                    list.Add((field.Name, attribute.LegacyFieldName));
            }
        }

        FieldCache[type] = list;
        return list;
    }

    private static string GetPath(Transform t)
    {
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "/" + path;
        }
        return path;
    }
}
