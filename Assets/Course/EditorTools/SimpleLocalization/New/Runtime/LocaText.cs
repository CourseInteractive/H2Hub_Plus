using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hält einen LocaString und ein UI-Textfeld.
/// Beim Spielstart (und bei jedem Sprachwechsel über SimpleLocalization) wird der Text befüllt.
/// Unterstützt sowohl Legacy-UI-Text als auch TextMeshPro.
/// </summary>
[DisallowMultipleComponent]
public class LocaText : MonoBehaviour
{
    [SerializeField] private LocaString text = new LocaString();

    [Tooltip("Legacy UI-Text (optional, wird automatisch gesucht)")]
    [SerializeField] private Text uiText;

    [Tooltip("TextMeshPro-Text (optional, wird automatisch gesucht)")]
    [SerializeField] private TMP_Text tmpText;

    // Altes string-Feld aus der ersten Version – wird automatisch in 'text' übernommen.
    [SerializeField, HideInInspector] private string token;

    public LocaString Text => text;

    public string Token
    {
        get => text.Token;
        set { text.Token = value; Refresh(); }
    }

    private void Reset()
    {
        FindTextComponents();
    }

    private void OnValidate()
    {
        MigrateLegacyToken();
    }

    private void Awake()
    {
        MigrateLegacyToken();
        FindTextComponents();
    }

    private void OnEnable()
    {
        SimpleLocalization.OnLanguageChanged += Refresh;
        Refresh();
    }

    private void OnDisable()
    {
        SimpleLocalization.OnLanguageChanged -= Refresh;
    }

    /// <summary>Schreibt den lokalisierten Text ins Textfeld.</summary>
    public void Refresh()
    {
        string value = text.Value;
        if (string.IsNullOrEmpty(value))
            return;

        if (tmpText != null) tmpText.text = value;
        if (uiText != null) uiText.text = value;

        if (tmpText == null && uiText == null)
            Debug.LogWarning($"[LocaText] Kein Textfeld auf '{name}' gefunden.", this);
    }

    private void FindTextComponents()
    {
        if (uiText == null) uiText = GetComponent<Text>();
        if (tmpText == null) tmpText = GetComponent<TMP_Text>();
    }

    private void MigrateLegacyToken()
    {
        if (string.IsNullOrEmpty(token)) return;
        if (!text.HasToken) text.Token = token;
        token = null;
    }
}
