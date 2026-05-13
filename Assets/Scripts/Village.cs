using UnityEngine;

public class Village : MonoBehaviour
{
    public static Village Instance;

    public int waterPrice = 4;
    public int energyPrice = 2;
    public int hydrogenPrice = 10;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Instance = this;
    }

    public void RefillContainer(Container container, int amount)
    {
        amount = Mathf.RoundToInt(container.GetMaxAmountInsteadOf(amount * 1f));
        int price = 0;
        if (container.ResourceType == ResourceType.H2O)
        {
            price = amount * waterPrice;
        }
        else if (container.ResourceType == ResourceType.E)
        {
            price = amount * energyPrice;
        }
        if(Workshop.Instance.HasMoney(price))
        {
            container.Add(amount);
            Workshop.Instance.SubMoney(price);
        }
    }

    public void SellContentOfContainer(Container container, int amount)
    {
        int price = hydrogenPrice;
        if (!container.HasAtLeast(amount))
            amount = (int)container.CurrentAmount;
        int profit = price * amount;
        Workshop.Instance.AddMoney(profit);
        container.Remove(amount);

    }

}
