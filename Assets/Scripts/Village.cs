using UnityEngine;

public class Village : MonoBehaviour
{
    public static Village Instance;

    public int defaultWaterPrice = 4;
    public int defaultEnergyPrice = 2;
    public int defaultWarmthPrice = 2;

    public int waterPrice = 4;
    public int energyPrice = 2;
    public int hydrogenPrice = 10;
    public int warmthPrice;

    public int waterSpikePrice = 10;
    public int energySpikePrice = 10;
    public int warmthSpikePrice = 5;

    public Vector2 waterProblemTimeLimits;
    public Vector2 energyProblemTimeLimits;
    public Vector2 warmthProblemTimeLimits;
    public Vector2 waterProblemRunningTimeLimits;
    public Vector2 energyProblemRunningTimeLimits;
    public Vector2 warmthProblemRunningTimeLimits;
    float waterProblemTimer;
    float energyProblemTimer;
    float warmthProblemTimer;

    public bool waterProblemRunning;
    public bool energyProblemRunning;
    public bool warmthProblemRunning;



    public bool openForProblems;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        waterProblemTimer = Random.Range(waterProblemTimeLimits.x, waterProblemTimeLimits.y);
        energyProblemTimer = Random.Range(energyProblemTimeLimits.x, energyProblemTimeLimits.y);
        warmthProblemTimer = Random.Range(warmthProblemTimeLimits.x, warmthProblemTimeLimits.y);
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
        waterProblemTimer -= Time.deltaTime * ProgressRelativeWorld.instance.GeneralProgressValue(); ;
        if(waterProblemTimer < 0)
        {
            if(waterProblemRunning)
            {
                waterProblemRunning = false;
                waterPrice = defaultWaterPrice;
                waterProblemTimer = Random.Range(waterProblemTimeLimits.x, waterProblemTimeLimits.y);
                IncomingMessageUI.instance.FreeFromPosition();
                MessageInbox.instance.AddMessage(MessageInfo.Warning("Wasserknappheit", "Wir haben eine Lieferung Wasser bekommen, erhöhte Preise aufgehoben.", icon: 6, buttonText: 1));
            }
            else
            {
                WaterProblem();
            }
        }
        energyProblemTimer -= Time.deltaTime * ProgressRelativeWorld.instance.GeneralProgressValue(); ;
        if (energyProblemTimer < 0)
        {
            if (energyProblemRunning)
            {
                energyProblemRunning = false;
                energyPrice = defaultEnergyPrice;
                energyProblemTimer = Random.Range(energyProblemTimeLimits.x, energyProblemTimeLimits.y);
                IncomingMessageUI.instance.FreeFromPosition();
                MessageInbox.instance.AddMessage(MessageInfo.Warning("Brennstoffmangel", "Neue Energie-Speicher und Module sind fertig. Energiepreise wieder herabgesetzt..", icon: 1, buttonText: 1));

                //IncomingMessageUI.instance.ShowMessage("Brennstoffmangel", "Neue Energie-Speicher und Module sind fertig. Energiepreise wieder herabgesetzt..", 1, 1);
            }
            else
            {
                EnergyProblem();
            }
        }
        warmthProblemTimer -= Time.deltaTime * ProgressRelativeWorld.instance.GeneralProgressValue();
        if (warmthProblemTimer < 0)
        {
            if (warmthProblemRunning)
            {
                warmthProblemRunning = false;
                warmthPrice = defaultWarmthPrice;
                warmthProblemTimer = Random.Range(warmthProblemTimeLimits.x, warmthProblemTimeLimits.y);
                IncomingMessageUI.instance.FreeFromPosition();
                MessageInbox.instance.AddMessage(MessageInfo.Warning("Kälte", "Temperaturen normal. Wärmepreise wieder normal..", icon: 1, buttonText: 1));
            }
            else
            {
                WarmthProblem();
            }
        }
    }

    public void EnergyProblem()
    {
        energyProblemRunning = true;
        energyPrice = energySpikePrice;
        energyProblemTimer = Random.Range(energyProblemRunningTimeLimits.x, energyProblemRunningTimeLimits.y);
        IncomingMessageUI.instance.FreeFromPosition();
        MessageInbox.instance.AddMessage(MessageInfo.Warning("Brennstoffmangel", "Problem! Energie wird teuer. Wir arbeiten dran", icon: 1, buttonText: 1));

    }

    public void WaterProblem()
    {
        waterProblemRunning = true;
        waterPrice = waterSpikePrice;
        waterProblemTimer = Random.Range(waterProblemRunningTimeLimits.x, waterProblemRunningTimeLimits.y);

        IncomingMessageUI.instance.FreeFromPosition();
        MessageInbox.instance.AddMessage(MessageInfo.Warning("Wasserknappheit", "Problem! Wasser wird teuer. Wir arbeiten dran.", icon: 6, buttonText: 1));
    }

    public void WarmthProblem()
    {
        warmthProblemRunning = true;
        warmthPrice = warmthSpikePrice;
        warmthProblemTimer = Random.Range(warmthProblemRunningTimeLimits.x, warmthProblemRunningTimeLimits.y);

        IncomingMessageUI.instance.FreeFromPosition();
        MessageInbox.instance.AddMessage(MessageInfo.Warning("Kälte", "Module für die Wärmeversorgung sind ausgefallen. Mit Wärme aus deiner Maschine könntest du uns versorgen.", icon: 6, buttonText: 1));
    }

    public void PrintTimers()
    {
        InGameLog.Log("Energie Problem in " + energyProblemTimer);
        InGameLog.Log("Wasser Problem in " + waterProblemTimer);
        InGameLog.Log("Wärme Problem in " + warmthProblemTimer);
    }

}
