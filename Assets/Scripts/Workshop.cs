using System;
using System.Collections.Generic;
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

    public Socket[] sellSockets;
    List<Container> connectedContainers;

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

    public bool AmountAvailable(int amount, ResourceType type)
    {
        FetchContainersFromSellSockets();
        List<Container> containers = FindModules(connectedContainers, type);
        float amountAvailable = 0;
        foreach(Container c in containers)
        {
            amountAvailable += c.CurrentAmount;
        }
        Container mainContainer = MainMachine.instance.FindOutputModule(type);
        if (mainContainer)
            amountAvailable += mainContainer.CurrentAmount;
        return amountAvailable >= amount;
    }

    public void RemoveAmount(int amount, ResourceType type)
    {
        FetchContainersFromSellSockets();
        List<Container> containers = FindModules(connectedContainers, type);
        float amountLeft = amount;
        foreach (Container c in containers)
        {
            if(c.CurrentAmount >= amountLeft)
            {
                c.Remove(amountLeft);
                return;
            }
            float x = c.CurrentAmount;
            c.Remove(c.CurrentAmount);
            amountLeft -= x;
        }
        Container mainContainer = MainMachine.instance.FindOutputModule(type);
        mainContainer.Remove(amountLeft);
       
    }

    void FetchContainersFromSellSockets()
    {
        connectedContainers = new List<Container>();
        foreach(Socket socket in sellSockets)
        {
            if(socket.lockedInteractable != null)
            {
                Container c = socket.lockedInteractable.GetComponent<Container>();
                if (c)
                    connectedContainers.Add(c);
            }
        }

    }

    private List<Container> FindModules(List<Container> list, ResourceType type)
    {
        return list.FindAll(c => c != null && c.ResourceType == type);
    }
}
