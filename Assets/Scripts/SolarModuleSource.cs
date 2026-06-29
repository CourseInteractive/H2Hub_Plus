using UnityEngine;

public class SolarModuleSource : Container
{

    public bool running;

    public void ReportDayTime()
    {
        running = true;
    }

    public void ReportNightTime()
    {
        running = false;
    }

    public override void Refill()
    {
        if (running && currentAmount < MaxAmount)
        {
            Add(4);
            ShowFlowIndicator();
        }
    }
}
