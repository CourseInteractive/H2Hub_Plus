using UnityEngine;

public class Village : MonoBehaviour
{
    public static Village Instance;

    public int defaultWaterPrice = 4;
    public int defaultEnergyPrice = 2;

    public int waterPrice = 4;
    public int energyPrice = 2;
    public int hydrogenPrice = 10;

    public int waterSpikePrice = 10;
    public int energySpikePrice = 10;

    public Vector2 waterProblemTimeLimits;
    public Vector2 energyProblemTimeLimits;
    public Vector2 waterProblemRunningTimeLimits;
    public Vector2 energyProblemRunningTimeLimits;
    float waterProblemTimer;
    float energyProblemTimer;

    public bool waterProblemRunning;
    public bool energyProblemRunning;

    public int warmthPrice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        waterProblemTimer = Random.Range(waterProblemTimeLimits.x, waterProblemTimeLimits.y);
        energyProblemTimer = Random.Range(energyProblemTimeLimits.x, energyProblemTimeLimits.y);
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
        switch (container.ResourceType)
        {
            case ResourceType.O:
                break;
            case ResourceType.W:
                price = warmthPrice;
                break;
        }

      
        if (!container.HasAtLeast(amount))
            amount = (int)container.CurrentAmount;
        int profit = price * amount;
        Workshop.Instance.AddMoney(profit);
        container.Remove(amount);

    }

    private void Update()
    {
        waterProblemTimer -= Time.deltaTime;
        if(waterProblemTimer < 0)
        {
            if(waterProblemRunning)
            {
                waterProblemRunning = false;
                waterPrice = defaultWaterPrice;
                waterProblemTimer = Random.Range(waterProblemTimeLimits.x, waterProblemTimeLimits.y);
            }
            else
            {
                waterProblemRunning = true;
                waterPrice = waterSpikePrice;
                waterProblemTimer = Random.Range(waterProblemRunningTimeLimits.x, waterProblemRunningTimeLimits.y);
            }
        }
        energyProblemTimer -= Time.deltaTime;
        if (energyProblemTimer < 0)
        {
            if (energyProblemRunning)
            {
                energyProblemRunning = false;
                energyPrice = defaultEnergyPrice;
                energyProblemTimer = Random.Range(energyProblemTimeLimits.x, energyProblemTimeLimits.y);
            }
            else
            {
                energyProblemRunning = true;
                energyPrice = energySpikePrice;
                energyProblemTimer = Random.Range(energyProblemRunningTimeLimits.x, energyProblemRunningTimeLimits.y);
            }
        }
    }

}
