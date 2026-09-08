using UnityEngine;

public class SolarModuleSource : Container
{

    public bool running;
    public GameObject indicator;

    public void ReportDayTime()
    {
        running = true;
      //  if (indicator)
      //      indicator.SetActive(true);
    }

    public void ReportNightTime()
    {
        running = false;
       // if (indicator)
       //     indicator.SetActive(false);
    }

    public void CheckTime()
    {
        if (DayNightCycle.Instance.ItIsDaytime())
            ReportDayTime();
        else
            ReportNightTime();
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
