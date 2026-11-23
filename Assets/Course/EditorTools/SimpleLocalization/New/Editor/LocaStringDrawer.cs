using UnityEditor;
using UnityEngine;

/// <summary>
/// Inspector-Darstellung für jedes LocaString-Feld – in jedem Script, Array oder ScriptableObject.
///
/// Mit Token:
///   [Label]  [ TOKEN_NAME        ][✎][×]
///            German: Spiel starten
///
/// Ohne Token:
///   [Label]  [ neuer Text...     ][+ Token]
///            Hinweis / "Existiert bereits als X  [Nutzen]"
/// </summary>
[CustomPropertyDrawer(typeof(LocaString))]
public class LocaStringDrawer : PropertyDrawer
{
    private const float SmallButton = 22f;
    private const float WideButton = 72f;
    private const float Gap = 2f;

    private static float Line => EditorGUIUtility.singleLineHeight;
    private static float Spacing => EditorGUIUtility.standardVerticalSpacing;

    private static GUIStyle _previewStyle;
    private static GUIStyle PreviewStyle => _previewStyle ??= new GUIStyle(EditorStyles.label)
    {
        fontStyle = FontStyle.Italic,
        clipping = TextClipping.Clip
    };

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return Line * 2 + Spacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var tokenProp = property.FindPropertyRelative("token");
        var draftProp = property.FindPropertyRelative("draftText");

        EditorGUI.BeginProperty(position, label, property);

        var row1 = new Rect(position.x, position.y, position.width, Line);
        Rect field = EditorGUI.PrefixLabel(row1, label);
        var row2 = new Rect(field.x, position.y + Line + Spacing, field.width, Line);

        int indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        if (!string.IsNullOrEmpty(tokenProp.stringValue))
            DrawLinked(field, row2, tokenProp, draftProp);
        else
            DrawDraft(field, row2, property, tokenProp, draftProp);

        EditorGUI.indentLevel = indent;
        EditorGUI.EndProperty();
    }

    // ------------------------------------------------------------------
    // Token vorhanden: Token + Vorschau der Ausgangssprache
    // ------------------------------------------------------------------
    private static void DrawLinked(Rect field, Rect row2, SerializedProperty tokenProp, SerializedProperty draftProp)
    {
        string token = tokenProp.stringValue;

        var tokenRect = new Rect(field.x, field.y, field.width - 2 * (SmallButton + Gap), Line);
        var editRect = new Rect(tokenRect.xMax + Gap, field.y, SmallButton, Line);
        var clearRect = new Rect(editRect.xMax + Gap, field.y, SmallButton, Line);

        EditorGUI.BeginChangeCheck();
        string newToken = EditorGUI.DelayedTextField(tokenRect, token);
        if (EditorGUI.EndChangeCheck())
            tokenProp.stringValue = newToken.Trim();

        if (GUI.Button(editRect, Icon("editicon.sml", "✎", "Im LokaKitEditor öffnen"), EditorStyles.miniButton))
            LocaEditorUtility.OpenEditor(token);

        if (GUI.Button(clearRect, new GUIContent("×", "Token lösen (Text bleibt als Entwurf erhalten)"), EditorStyles.miniButton))
        {
            draftProp.stringValue = LocaEditorUtility.GetSourceText(token) ?? string.Empty;
            tokenProp.stringValue = string.Empty;
            return;
        }

        // Vorschau
        if (LocaEditorUtility.Kit == null)
        {
            Warning(row2, LocaEditorUtility.NoKitMessage);
            return;
        }

        string source = LocaEditorUtility.GetSourceText(token);
        if (source == null)
            Warning(row2, $"Token '{token}' nicht in der Tabelle");
        else if (source.Length == 0)
            EditorGUI.LabelField(row2, $"({LocaEditorUtility.SourceLanguageName}: kein Text)", EditorStyles.miniLabel);
        else
            EditorGUI.LabelField(row2, new GUIContent($"{LocaEditorUtility.SourceLanguageName}: {source.Replace("§§", " ")}", source.Replace("§§", "\n")), PreviewStyle);
    }

    // ------------------------------------------------------------------
    // Kein Token: Text eingeben -> Eintrag anlegen
    // ------------------------------------------------------------------
    private static void DrawDraft(Rect field, Rect row2, SerializedProperty property,
                                  SerializedProperty tokenProp, SerializedProperty draftProp)
    {
        var textRect = new Rect(field.x, field.y, field.width - (WideButton + Gap), Line);
        var createRect = new Rect(textRect.xMax + Gap, field.y, WideButton, Line);

        EditorGUI.BeginChangeCheck();
        string draft = EditorGUI.TextField(textRect, draftProp.stringValue);
        if (EditorGUI.EndChangeCheck())
            draftProp.stringValue = draft;

        bool hasText = !string.IsNullOrWhiteSpace(draft);
        bool hasKit = LocaEditorUtility.Kit != null;

        using (new EditorGUI.DisabledScope(!hasText || !hasKit))
        {
            if (GUI.Button(createRect, new GUIContent("+ Token", "Aus diesem Text einen Loka-Eintrag machen"), EditorStyles.miniButton))
            {
                // Pfade merken – das Popup lebt länger als dieser OnGUI-Aufruf
                var targets = property.serializedObject.targetObjects;
                string tokenPath = tokenProp.propertyPath;
                string draftPath = draftProp.propertyPath;

                PopupWindow.Show(createRect, new LocaTokenPopup(draft.Trim(), created =>
                {
                    var so = new SerializedObject(targets);
                    so.FindProperty(tokenPath).stringValue = created;
                    so.FindProperty(draftPath).stringValue = string.Empty;
                    so.ApplyModifiedProperties();
                }));
            }
        }

        // Zweite Zeile: Duplikat-Hinweis oder Info
        if (!hasKit)
        {
            Warning(row2, LocaEditorUtility.NoKitMessage);
            return;
        }

        // 1. Eingegebener Text ist selbst ein Token? (hat Vorrang)
        // 2. Sonst: Text existiert schon als Ausgangstext eines Tokens?
        var tokenMatch = hasText ? LocaEditorUtility.FindEntry(draft.Trim()) : null;
        string existing = tokenMatch != null ? tokenMatch.token
                        : hasText ? LocaEditorUtility.FindTokenBySourceText(draft) : null;

        if (existing != null)
        {
            var infoRect = new Rect(row2.x, row2.y, row2.width - (WideButton + Gap), Line);
            var useRect = new Rect(infoRect.xMax + Gap, row2.y, WideButton, Line);

            string info = tokenMatch != null ? $"Token '{existing}' erkannt" : $"Existiert bereits als '{existing}'";
            string buttonText = tokenMatch != null ? "Verknüpfen" : "Nutzen";

            EditorGUI.LabelField(infoRect,
                new GUIContent(info, EditorGUIUtility.IconContent("console.infoicon.sml").image),
                EditorStyles.miniLabel);

            if (GUI.Button(useRect, new GUIContent(buttonText, $"Token '{existing}' verwenden"), EditorStyles.miniButton))
            {
                tokenProp.stringValue = existing;
                draftProp.stringValue = string.Empty;
            }
        }
        else
        {
            EditorGUI.LabelField(row2,
                hasText ? "Noch kein Token – Text wird unübersetzt angezeigt" : "Text eingeben oder Token setzen",
                EditorStyles.miniLabel);
        }
    }

    // ------------------------------------------------------------------
    // Hilfen
    // ------------------------------------------------------------------
    private static void Warning(Rect rect, string message)
    {
        EditorGUI.LabelField(rect,
            new GUIContent(message, EditorGUIUtility.IconContent("console.warnicon.sml").image),
            EditorStyles.miniLabel);
    }

    private static GUIContent Icon(string iconName, string fallbackText, string tooltip)
    {
        var icon = EditorGUIUtility.IconContent(iconName);
        return icon?.image != null
            ? new GUIContent(icon.image, tooltip)
            : new GUIContent(fallbackText, tooltip);
    }
}
