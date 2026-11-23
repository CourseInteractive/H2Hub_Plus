using System;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Kleines Popup: Token-Namen eintippen und Loka-Eintrag anlegen.
/// Enter = Anlegen, Escape = Abbrechen.
/// </summary>
public class LocaTokenPopup : PopupWindowContent
{
    private const string FieldName = "LocaTokenField";

    private readonly string _sourceText;
    private readonly Action<string> _onCreated;
    private string _token = "";
    private bool _focused;

    public LocaTokenPopup(string sourceText, Action<string> onCreated)
    {
        _sourceText = sourceText;
        _onCreated = onCreated;
    }

    public override Vector2 GetWindowSize() => new Vector2(340, 130);

    public override void OnGUI(Rect rect)
    {
        // Tasten abfangen, bevor das Textfeld sie verarbeitet
        var e = Event.current;
        bool enterPressed = e.type == EventType.KeyDown &&
                            (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter);
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            editorWindow.Close();
            e.Use();
            return;
        }

        EditorGUILayout.LabelField("Neuen Loka-Eintrag anlegen", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"{LocaEditorUtility.SourceLanguageName}: \"{_sourceText}\"",
                                   EditorStyles.wordWrappedMiniLabel);

        GUI.SetNextControlName(FieldName);
        _token = EditorGUILayout.TextField("Token", _token);
        if (!_focused)
        {
            EditorGUI.FocusTextInControl(FieldName);
            _focused = true;
        }

        string clean = LocaEditorUtility.SanitizeToken(_token);
        string error = LocaEditorUtility.ValidateNewToken(clean);

        if (!string.IsNullOrEmpty(_token) && error != null)
            EditorGUILayout.HelpBox(error, MessageType.Warning);
        else if (clean != _token && !string.IsNullOrEmpty(clean))
            EditorGUILayout.LabelField($"Wird angelegt als: {clean}", EditorStyles.miniLabel);

        GUILayout.FlexibleSpace();

        using (new EditorGUI.DisabledScope(error != null))
        {
            bool create = GUILayout.Button("Anlegen") || (enterPressed && error == null);
            if (create && LocaEditorUtility.CreateEntry(clean, _sourceText) != null)
            {
                _onCreated?.Invoke(clean);
                editorWindow.Close();
                GUIUtility.ExitGUI();
            }
        }
    }
}
