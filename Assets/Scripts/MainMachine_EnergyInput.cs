using UnityEngine;

public class MainMachine_EnergyInput : MonoBehaviour
{
    public MainMachine machine;

    public Container defaultEnergyModule;
    public Container solarModule;
    public bool solarUsed;

    public void SwitchToMain()
    {
        solarUsed = false;
        machine.Disconnect(solarModule);
        machine.Connect(defaultEnergyModule);
        defaultEnergyModule.gameObject.SetActive(true);
        InGameLog.Log("Switch To NormalEnergy");
    }

    public void SwitchToSolar()
    {
        solarUsed = true;
        machine.Disconnect(defaultEnergyModule);
        machine.Connect(solarModule);
        solarModule.gameObject.SetActive(true);
        InGameLog.Log("Switch To SolarModule");
    }

}
