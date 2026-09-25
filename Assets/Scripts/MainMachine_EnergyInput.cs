using UnityEngine;

public class MainMachine_EnergyInput : MonoBehaviour
{
    public MainMachine machine;

    public Container defaultEnergyModule;
    public Container solarModule;
    public bool solarUsed;

    public TMPro.TMP_Text text;

    private void Start()
    {
        UpdateText();
    }

    public void SwitchToMain()
    {
        solarUsed = false;
        machine.Disconnect(solarModule);
        machine.Connect(defaultEnergyModule);
        defaultEnergyModule.gameObject.SetActive(true);
        InGameLog.Log("Switch To NormalEnergy");
        UpdateText();

    }

    public void SwitchToSolar()
    {
        solarUsed = true;
        machine.Disconnect(defaultEnergyModule);
        machine.Connect(solarModule);
        solarModule.gameObject.SetActive(true);
        InGameLog.Log("Switch To SolarModule");
        UpdateText();
    }

    void UpdateText()
    {
        if (solarUsed)
            text.text = "Solar-Modus AN";
        else
            text.text = "Solar-Modus -aus-";
    }
}
