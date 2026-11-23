using UnityEngine;

/// <summary>
/// Legt beim Spielstart die Sprache fest.
/// Läuft direkt nach SimpleLocalization_InGame (-1000), aber vor allen normalen Skripten.
/// </summary>
[DefaultExecutionOrder(-900)]
public class LocaLanguageSetter : MonoBehaviour
{
    [Tooltip("Sprache, die beim Spielstart aktiv sein soll")]
    [SerializeField] private LocaLanguage startLanguage = LocaLanguage.Deutsch;

    private void Awake()
    {
        LocaLanguageUtility.Set(startLanguage);
    }

#if UNITY_EDITOR
    // Sprache im Play Mode direkt im Inspector umschalten können
    private void OnValidate()
    {
        if (Application.isPlaying)
            LocaLanguageUtility.Set(startLanguage);
    }
#endif
}
