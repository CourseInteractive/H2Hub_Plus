using UnityEngine;
using UnityEngine.UI;
using TMPro;
/// <summary>
/// Zeigt den Zustand eines Containers in einem UI-Text an.
/// Auf ein UI-Text-GameObject legen und den Container zuweisen.
/// </summary>
public class ContainerUI : MonoBehaviour
{
    [Header("Ziel-Container")]
    [SerializeField] private Container container;

    [Header("Format")]
    [Tooltip("Verfügbare Platzhalter: {type}, {current}, {max}, {percent}")]
    [SerializeField] private string format = "{type}: {current} / {max} ({percent}%)";

    [Tooltip("Nachkommastellen für Mengenangaben")]
    [SerializeField] private int decimalPlaces = 1;

    public TMP_Text label;

    public Image fillAmountDisplay;


    private void Update()
    {
        if (container == null || label == null) return;

        float percent = container.MaxAmount > 0f
            ? (container.CurrentAmount / container.MaxAmount) * 100f
            : 0f;

        string current = container.CurrentAmount.ToString($"F{decimalPlaces}");
        string max     = container.MaxAmount.ToString($"F{decimalPlaces}");
        string pct     = percent.ToString("F0");

        label.text = format
            .Replace("{type}",    container.ResourceType.ToString())
            .Replace("{current}", current)
            .Replace("{max}",     max)
            .Replace("{percent}", pct)
            .Replace("$$", "\n");
        if(fillAmountDisplay)
        fillAmountDisplay.fillAmount = percent / 100f;
    }
}
