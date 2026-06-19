using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

/// <summary>
/// Basisklasse für alle Module. Hält eine Ressource bis zu einem Maximalwert.
/// </summary>
public class Container : MonoBehaviour
{
    [Header("Container Settings")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private float maxAmount = 100f;
    [SerializeField] private float currentAmount = 0f;

    public ResourceType ResourceType => resourceType;
    public float MaxAmount => maxAmount;
    public float CurrentAmount => currentAmount;
    public bool IsFull => currentAmount >= maxAmount;
    public bool IsEmpty => currentAmount <= 0f;

    public bool ImmediateRefill;
    public float timePerLoss = -1;
    float lossTimer = 0;

    private void Update()
    {
        if (timePerLoss < 0 || IsEmpty)
            return;

        if(lossTimer > 0)
        {
            lossTimer -= Time.deltaTime;
            if (lossTimer < 0)
            {
                lossTimer = timePerLoss;
                Remove(1f);
            }
                
        }
    }

    /// <summary>
    /// Fügt eine Menge hinzu. Überschreitet nie das Maximum.
    /// Gibt die tatsächlich hinzugefügte Menge zurück.
    /// </summary>
    public float Add(float amount)
    {
        if (amount <= 0f) return 0f;

        float space = maxAmount - currentAmount;
        float added = Mathf.Min(amount, space);
        currentAmount += added;
        OnAmountChanged(currentAmount);
        if(lossTimer <= 0)
            lossTimer = timePerLoss;
        return added;
    }

    /// <summary>
    /// Entnimmt eine Menge. Unterschreitet nie Null.
    /// Gibt die tatsächlich entnommene Menge zurück.
    /// </summary>
    public float Remove(float amount)
    {
        if (amount <= 0f) return 0f;

        float removed = Mathf.Min(amount, currentAmount);
        currentAmount -= removed;
        if(ImmediateRefill)
        {
            RefillAmountForMoney((int)(maxAmount - currentAmount));
        }

        OnAmountChanged(currentAmount);
        return removed;
    }

    /// <summary>
    /// Prüft, ob mindestens eine bestimmte Menge verfügbar ist.
    /// </summary>
    public bool HasAtLeast(float amount) => currentAmount >= amount;

    /// <summary>
    /// Prüft, ob mindestens eine bestimmte Menge Platz vorhanden ist.
    /// </summary>
    public bool HasSpaceFor(float amount) => (maxAmount - currentAmount) >= amount;


    public float GetMaxAmountInsteadOf(float amount)
    {
        if (HasSpaceFor(amount))
            return amount;
        if (IsFull)
            return 0;
        return MaxAmount - CurrentAmount;
    }
    /// <summary>
    /// Wird aufgerufen, wenn sich der Füllstand ändert. Kann in Unterklassen überschrieben werden.
    /// </summary>
    protected virtual void OnAmountChanged(float newAmount) { }

    public void DisconnectFromMachine()
    {
        MainMachine.instance.Disconnect(this);
        
    }

    public void HandleDisconnect()
    {
        if(GetComponent<XRGrabInteractable>())
        {

        }
        else
        {
            gameObject.SetActive(false);
        }

    }

    public void ConnectToMachine()
    {
        MainMachine.instance.Connect(this);
        gameObject.SetActive(true);
    }

    public void RefillAmountForMoney(int amount)
    {
        Village.Instance.RefillContainer(this, amount);
    }

    public void TriggerImmediateRefill()
    {
        Refill();
    }

    public void SellAmount(int amount)
    {
        Village.Instance.SellContentOfContainer(this, amount);
    }

    public virtual void Refill()
    {
        if (ImmediateRefill)
        {
            RefillAmountForMoney((int)maxAmount);
        }
    }

}
