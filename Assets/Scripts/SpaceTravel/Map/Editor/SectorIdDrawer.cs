using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Zeichnet Felder mit [SectorId] als Dropdown aller Sektoren der SectorMapData.
/// Die Liste wird bei jedem Zeichnen neu aufgebaut und ist damit immer aktuell.
/// Gespeichert wird die ID, angezeigt der Name.
/// Muss in einem Ordner namens "Editor" liegen.
/// </summary>
[CustomPropertyDrawer(typeof(SectorIdAttribute))]
public class SectorIdDrawer : PropertyDrawer
{
    private static readonly Regex OwnSectorRegex = new Regex(@"^sectors\.Array\.data\[(\d+)\]\.connections");

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty sectors = property.serializedObject.FindProperty("sectors");

        // Fallback: kein string-Feld oder keine Sektorliste gefunden -> normales Textfeld.
        if (property.propertyType != SerializedPropertyType.String || sectors == null || !sectors.isArray)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        // Bei Verbindungen den eigenen Sektor nicht anbieten.
        int ownIndex = -1;
        Match match = OwnSectorRegex.Match(property.propertyPath);
        if (match.Success)
            ownIndex = int.Parse(match.Groups[1].Value);

        List<string> ids = new List<string> { "" };
        List<GUIContent> options = new List<GUIContent> { new GUIContent("(keiner)") };

        for (int i = 0; i < sectors.arraySize; i++)
        {
            if (i == ownIndex) continue;

            SerializedProperty sector = sectors.GetArrayElementAtIndex(i);
            string id = sector.FindPropertyRelative("id").stringValue;
            if (string.IsNullOrEmpty(id)) continue;

            string displayName = sector.FindPropertyRelative("displayName").stringValue;
            int level = sector.FindPropertyRelative("level").intValue;

            if (string.IsNullOrEmpty(displayName))
                displayName = id;

            // "/" würde im Dropdown ein Untermenü erzeugen.
            displayName = displayName.Replace('/', '-');

            ids.Add(id);
            options.Add(new GUIContent($"L{level}  {displayName}  [{id}]"));
        }

        string current = property.stringValue;
        int index = ids.IndexOf(current);

        // Eingetragene ID existiert nicht mehr: sichtbar machen statt still zu überschreiben.
        if (index < 0)
        {
            ids.Add(current);
            options.Add(new GUIContent($"⚠ {current} (fehlt)"));
            index = ids.Count - 1;
        }

        EditorGUI.BeginProperty(position, label, property);
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUI.Popup(position, label, index, options.ToArray());
        if (EditorGUI.EndChangeCheck())
            property.stringValue = ids[newIndex];
        EditorGUI.EndProperty();
    }
}
