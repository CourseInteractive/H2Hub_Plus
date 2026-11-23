using System;

/// <summary>
/// Markiert ein LocaString-Feld, das Daten aus einem alten string-Feld übernehmen soll.
/// Das Migrations-Tool (Tools/Course/Localization/...) überträgt den alten Wert und leert danach das alte Feld.
///
///     [LocaMigrateFrom("title")]
///     public LocaString titleLoca;
///     [HideInInspector] public string title;   // altes Feld bis nach der Migration behalten!
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class LocaMigrateFromAttribute : Attribute
{
    /// <summary>Serialisierter Name des alten string-Felds.</summary>
    public string LegacyFieldName { get; }

    public LocaMigrateFromAttribute(string legacyFieldName)
    {
        LegacyFieldName = legacyFieldName;
    }
}
