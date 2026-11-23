using System;
using UnityEngine;

/// <summary>
/// Lokalisierter Text als Feld-Typ – in jedem Script verwendbar:
///
///     public LocaString title;
///     label.text = title;            // implizit -> lokalisierter Text
///     label.text = title.Format(42); // mit Platzhaltern
///
/// Holt den Text über SimpleLocalization aus dem LokaKit der Szene.
/// Im Inspector zeigt der LocaStringDrawer Token, deutschen Text und den Knopf zum Anlegen.
/// </summary>
[Serializable]
public class LocaString
{
    [SerializeField] private string token = "";

    // Text, der noch kein Token hat. Wird angezeigt, solange kein Token gesetzt ist.
    [SerializeField] private string draftText = "";

    public LocaString() { }
    public LocaString(string token) { this.token = token; }

    public string Token
    {
        get => token;
        set => token = value;
    }

    public bool HasToken => !string.IsNullOrEmpty(token);

    /// <summary>Weder Token noch Entwurfstext gesetzt.</summary>
    public bool IsEmpty => string.IsNullOrEmpty(token) && string.IsNullOrEmpty(draftText);

    /// <summary>
    /// Lokalisierter Text in der aktiven Sprache.
    /// Leere Übersetzung -> Ausgangssprache (Index 0). Unbekanntes Token -> Token selbst.
    /// Kein Token -> unübersetzter Entwurfstext.
    /// </summary>
    public string Value
    {
        get
        {
            if (!HasToken)
                return (draftText ?? string.Empty).Replace("§§", "\n");

            string text = SimpleLocalization.GetLocalization(token);

            if (string.IsNullOrEmpty(text) && SimpleLocalization.activeLanguage != 0)
            {
                var kit = SimpleLocalization.inGame != null ? SimpleLocalization.inGame.kit : null;
                if (kit != null)
                    text = kit.GetLocaStringByToken(token, 0);
            }

            return text ?? string.Empty;
        }
    }

    public string Format(params object[] args) => string.Format(Value, args);

    public override string ToString() => Value;

    public static implicit operator string(LocaString s) => s?.Value ?? string.Empty;
}
