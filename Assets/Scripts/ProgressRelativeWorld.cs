using UnityEngine;
using System.Collections.Generic;

public class ProgressRelativeWorld : MonoBehaviour
{
    public static ProgressRelativeWorld instance;
    public int money
    {
        get { return Workshop.Instance.Money; } //
    }
    public float water
    {
        get { return waterContainer.CurrentAmount; }    //
    }
    public Container waterContainer;
    public List<Container> containers;  //
    public Container startContainer;

    public float hydrogenReady; //
    public float oxygenReady; //

    public bool hasOxygenContainer; //

    public bool hasSolarModule; //
    public bool hasWarmthModule;    //

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        containers = new List<Container>();
        containers.Add(startContainer);
    }

    public void ReportNewContainer(Container c)
    {
        if (!containers.Contains(c))
            containers.Add(c);
    }

    public void ReportSolarModule()
    {
        hasSolarModule = true;
    }

    public void ReportWarmthModule()
    {
        hasWarmthModule = true;
    }

    void FetchReadyResources()
    {
        hydrogenReady = 0;
        oxygenReady = 0;
        foreach(Container c in containers)
        {
            if (c.ResourceType == ResourceType.H)
                hydrogenReady += c.CurrentAmount;
            else if (c.ResourceType == ResourceType.O)
            {
                hasOxygenContainer = true;
                oxygenReady += c.CurrentAmount;
            }
              
        }
    }

    // Update is called once per frame
    void Update()
    {
        FetchReadyResources();
    }



    public float GeneralProgressValue()
    {
        FetchReadyResources();
        float moneyFactor = Mathf.InverseLerp(800, 3000, money);
        float hydrogenFactor = Mathf.InverseLerp(50, 110, hydrogenReady);
        float moduleFactor = 0;
        if (hasSolarModule)
            moduleFactor += 0.6f;
        if (hasWarmthModule)
            moduleFactor += 0.4f;
        float additionFactor = (Mathf.InverseLerp(50, 100, water) * 0.5f) + moduleFactor * 0.5f;
        return Mathf.Clamp01(moneyFactor * 0.7f + hydrogenFactor * 0.4f + additionFactor * 0.3f);
    }

}
