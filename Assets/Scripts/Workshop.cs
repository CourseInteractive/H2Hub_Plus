using System;
using UnityEngine;

/// <summary>
/// Singleton-Manager für den Workshop.
/// Verwaltet das Spielerkapital und benachrichtigt Subscriber bei jeder Änderung.
/// </summary>
public class Workshop : MonoBehaviour
{
    // ── Singleton ────────────────────────────────────────────────────────────

    public static Workshop Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ── Events ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Wird ausgelöst, sobald sich der Geldstand ändert.
    /// Übergibt den neuen Betrag als int.
    /// </summary>
    public event Action<int> OnMoneyChanged;

    // ── Kapital ───────────────────────────────────────────────────────────────

    [SerializeField] private int startMoney = 100;

    private int _money;

    public int Money => _money;

    private void Start()
    {
        // Startwert setzen und UI sofort informieren
        _money = startMoney;
        OnMoneyChanged?.Invoke(_money);
    }

    // ── Public API ────────────────────────────────────────────────────────────

    /// <summary>Fügt einen positiven Betrag hinzu.</summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0) return;

        _money += amount;
        OnMoneyChanged?.Invoke(_money);
    }
    /// <summary>
    /// Zieht einen Betrag ab. Der Kontostand sinkt nie unter 0.
    /// Gibt true zurück, wenn der vollständige Betrag abgezogen werden konnte.
    /// </summary>
    public void SubMoney(int amount)
    {
        if (amount <= 0) return;

        if (_money < amount)
        {
            // Kein ausreichendes Guthaben – nichts abziehen
            return;
        }

        _money -= amount;
        OnMoneyChanged?.Invoke(_money);
    }

    /// <summary>Prüft, ob mindestens <paramref name="amount"/> vorhanden ist.</summary>
    public bool HasMoney(int amount) => _money >= amount;
}
