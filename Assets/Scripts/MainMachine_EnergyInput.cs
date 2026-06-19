using UnityEngine;

public class MainMachine_EnergyInput : MonoBehaviour
{
    public MainMachine machine;

    public Container defaultEnergyModule;
    public Container solarModule;

    public void SwitchToMain()
    {
        machine.Disconnect(solarModule);
        machine.Connect(defaultEnergyModule);
    }

    public void SwitchToSolar()
    {
        machine.Disconnect(defaultEnergyModule);
        machine.Connect(solarModule);
    }

}
