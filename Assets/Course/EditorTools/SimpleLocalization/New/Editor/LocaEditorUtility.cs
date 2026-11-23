using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Editor-Zugriff auf das LokaKit der Szene: lesen, suchen, Einträge anlegen, Editor öffnen.
/// Arbeitet direkt auf dem Asset, funktioniert also auch außerhalb des Play Mode.
/// </summary>
public static class LocaEditorUtility
{
    /// <summary>Index der Ausgangssprache in translationText.</summary>
    public const int SourceLanguage = 0;

    public static string SourceLanguageName => ((LocaLanguage)SourceLanguage).ToString();

    public static LokaKit Kit => LocaSceneKit.Kit;

    public const string NoKitMessage = "Kein LokaKit – SimpleLocalization_InGame in der Szene?";

    /// <summary>Eintrag zum Token. Groß-/Kleinschreibung egal – wie GetLocaStringByToken.</summary>
    public static LokaKitEntry FindEntry(string token)
    {
        var kit = Kit;
        if (kit == null || kit.entries == null || string.IsNullOrEmpty(token)) return null;

        foreach (var entry in kit.entries)
        {
            if (entry != null && string.Equals(entry.token, token, StringComparison.OrdinalIgnoreCase))
                return entry;
        }
        return null;
    }

    /// <summary>Text der Ausgangssprache (roh, mit §§). null = Token existiert nicht.</summary>
    public static string GetSourceText(string token)
    {
        var entry = FindEntry(token);
        if (entry == null) return null;
        if (entry.translationText == null || entry.translationText.Count <= SourceLanguage) return string.Empty;
        return entry.translationText[SourceLanguage] ?? string.Empty;
    }

    /// <summary>Sucht ein Token, dessen Ausgangstext genau diesem Text entspricht.</summary>
    public static string FindTokenBySourceText(string text)
    {
        var kit = Kit;
        if (kit == null || kit.entries == null || string.IsNullOrWhiteSpace(text)) return null;

        string needle = text.Trim();
        foreach (var entry in kit.entries)
        {
            if (entry?.translationText == null || entry.translationText.Count <= SourceLanguage) continue;
            if (string.Equals(entry.translationText[SourceLanguage]?.Trim(), needle, StringComparison.Ordinal))
                return entry.token;
        }
        return null;
    }

    /// <summary>Leerzeichen am Rand weg, innere Leerzeichen -> Unterstrich.</summary>
    public static string SanitizeToken(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        return Regex.Replace(raw.Trim(), @"\s+", "_");
    }

    /// <summary>Fehlermeldung oder null, wenn das Token angelegt werden darf.</summary>
    public static string ValidateNewToken(string token)
    {
        if (Kit == null) return NoKitMessage;
        if (string.IsNullOrEmpty(token)) return "Token darf nicht leer sein.";
        var existing = FindEntry(token);
        if (existing != null) return $"Token '{existing.token}' existiert bereits.";
        return null;
    }

    /// <summary>Legt einen neuen Eintrag im LokaKit an. Andere Sprachen bleiben leer.</summary>
    public static LokaKitEntry CreateEntry(string token, string sourceText)
    {
        string error = ValidateNewToken(token);
        if (error != null)
        {
            Debug.LogWarning($"[Loca] {error}");
            return null;
        }

        var kit = Kit;
        Undo.RecordObject(kit, "Loka-Eintrag anlegen");

        if (kit.entries == null)
            kit.entries = new List<LokaKitEntry>();

        // Gleicher Konstruktor wie in LokaKitEditor.CreatenewEntry
        var entry = new LokaKitEntry(0, sourceText, "", token, 2, 0, true);

        int languageCount = Mathf.Max(kit.languageCount, LocaLanguageUtility.Count);
        while (entry.translationText.Count < languageCount)
            entry.translationText.Add("");

        entry.lastChangedInVersion = Application.version;
        kit.AddNewEntry(entry); // vergibt lineID über nextLineID

        EditorUtility.SetDirty(kit);
        AssetDatabase.SaveAssetIfDirty(kit);

        // Im Play Mode alle Texte neu befüllen
        if (Application.isPlaying)
            SimpleLocalization.LanguageChanged(SimpleLocalization.activeLanguage);

        return entry;
    }

    /// <summary>Öffnet den LokaKitEditor mit dem Eintrag dieses Tokens – und zwar im Kit der Szene.</summary>
    public static void OpenEditor(string token)
    {
        var kit = Kit;
        if (kit == null)
        {
            Debug.LogWarning($"[Loca] {NoKitMessage}");
            return;
        }

        var entry = FindEntry(token);
        if (entry == null)
        {
            Debug.LogWarning($"[Loca] Token '{token}' nicht im LokaKit '{kit.name}' gefunden.", kit);
            return;
        }

        // Erst öffnen (lädt sonst das zuletzt bearbeitete Kit), dann auf das Szenen-Kit umstellen
        if (LokaKitEditor.window == null)
            LokaKitEditor.Open();
        LokaKitEditor.manager = kit;

        LokaKitEditor_Static.OpenWithLineID(entry.lineID);
    }
}
