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

    public bool openForProblems;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        waterProblemTimer = Random.Range(waterProblemTimeLimits.x, waterProblemTimeLimits.y);
        energyProblemTimer = Random.Range(energyProblemTimeLimits.x, energyProblemTimeLimits.y);
        Instance = this;
    }

    public void SetActivity(bool value)
    {
        openForProblems = value;
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
        if (!openForProblems)
            return;
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
                WaterProblem();
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
                EnergyProblem();
            }
        }
    }

    public void EnergyProblem()
    {
        energyProblemRunning = true;
        energyPrice = energySpikePrice;
        energyProblemTimer = Random.Range(energyProblemRunningTimeLimits.x, energyProblemRunningTimeLimits.y);
        IncomingMessageUI.instance.FreeFromPosition();
        IncomingMessageUI.instance.ShowMessage("Brennstoffmangel", "Problem! Energie wird teuer. Wir arbeiten dran.", 1, 1);
    }

    public void WaterProblem()
    {
        waterProblemRunning = true;
        waterPrice = waterSpikePrice;
        waterProblemTimer = Random.Range(waterProblemRunningTimeLimits.x, waterProblemRunningTimeLimits.y);

        IncomingMessageUI.instance.FreeFromPosition();
        IncomingMessageUI.instance.ShowMessage("Wasserknappheit", "Problem! Wasser wird teuer. Wir arbeiten dran.", 4, 1);
    }

    public void PrintTimers()
    {
        InGameLog.Log("Energy Problem in " + energyProblemTimer);
        InGameLog.Log("Water Problem in " + waterProblemTimer);
    }

}
