using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Zeigt den aktuellen Geldstand des Workshop an.
/// Änderungen werden als animierter Hochzähl-/Runterzähl-Effekt dargestellt.
/// </summary>
public class WorkshopUI : MonoBehaviour
{
    // ── Inspector-Felder ──────────────────────────────────────────────────────

    [Header("Referenzen")]
    [SerializeField] private TMP_Text moneyLabel;

    [Header("Zähler-Animation")]
    [Tooltip("Zeit in Sekunden, über die der Betrag animiert wird (0 = sofort).")]
    [SerializeField] private float countDuration = 0.6f;

    [Header("Farben")]
    [SerializeField] private Color colorZero     = new Color(0.7f, 0.15f, 0.15f); // Rot
    [SerializeField] private Color colorPositive = new Color(0.9f, 0.75f, 0.2f);  // Gold

    // ── Interner Zustand ──────────────────────────────────────────────────────

    private int        _displayedValue;
    private int        _targetValue;
    private Coroutine  _countRoutine;

    // ── Unity-Lifecycle ───────────────────────────────────────────────────────

    /*  private void OnEnable()
      {
          if (Workshop.Instance != null)

      }

      private void OnDisable()
      {
          if (Workshop.Instance != null)
              Workshop.Instance.OnMoneyChanged -= OnMoneyChanged;
      }*/

    private void Start()
    {
        // Beim Start sofort den aktuellen Wert übernehmen (ohne Animation)
        if (Workshop.Instance != null)
        {
            Workshop.Instance.OnMoneyChanged += OnMoneyChanged;
            _displayedValue = Workshop.Instance.Money;
            _targetValue    = _displayedValue;
            UpdateLabel(_displayedValue);
        }
    }

    // ── Event-Handler ─────────────────────────────────────────────────────────

    private void OnMoneyChanged(int newAmount)
    {
        _targetValue = newAmount;

        // Laufende Animation abbrechen und neu starten
        if (_countRoutine != null)
            StopCoroutine(_countRoutine);

        if (countDuration <= 0f)
        {
            // Sofort anzeigen
            _displayedValue = _targetValue;
            UpdateLabel(_displayedValue);
        }
        else
        {
            _countRoutine = StartCoroutine(AnimateCount(_displayedValue, _targetValue));
        }
    }

    // ── Animations-Coroutine ──────────────────────────────────────────────────

    private IEnumerator AnimateCount(int from, int to)
    {
        float elapsed = 0f;

        while (elapsed < countDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / countDuration);

            // Smoothstep für ein angenehmeres Einbremsen
            t = t * t * (3f - 2f * t);

            int current = Mathf.RoundToInt(Mathf.Lerp(from, to, t));
            UpdateLabel(current);
            _displayedValue = current;

            yield return null;
        }

        // Sicherstellen, dass am Ende exakt der Zielwert steht
        _displayedValue = to;
        UpdateLabel(to);
        _countRoutine = null;
    }

    // ── Darstellung ───────────────────────────────────────────────────────────

    private void UpdateLabel(int value)
    {
        if (moneyLabel == null) return;

        moneyLabel.text  = $"{value:N0} €";
        moneyLabel.color = value <= 0 ? colorZero : colorPositive;
    }
}
