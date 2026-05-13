using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Zeigt den Zustand der MainMachine in einem UI-Text an.
/// Auf ein UI-Text-GameObject legen und die Maschine zuweisen.
/// </summary>
public class MachineUI : MonoBehaviour
{
    [Header("Ziel-Maschine")]
    [SerializeField] private MainMachine machine;

    [Header("Texte")]
    [SerializeField] private string labelRunning = "Maschine: AN";
    [SerializeField] private string labelStopped = "Maschine: AUS";

    [Header("Farben")]
    [SerializeField] private Color colorRunning = new Color(0.2f, 0.85f, 0.3f);
    [SerializeField] private Color colorStopped = new Color(0.85f, 0.25f, 0.2f);

    public TMP_Text label;

    public TMP_Text powerLabel;
    public TMP_Text warningLabel;

    private void Update()
    {
        if (machine == null || label == null) return;

        powerLabel.text = (machine.currentPowerLevel * 100).ToString("F0") + "%";

        if (machine.currentState != MainMachine.MachineState.Off)
        {
            label.text  = labelRunning;
            label.color = colorRunning;
        }
        else
        {
            label.text  = labelStopped;
            label.color = colorStopped;
        }
    }
}
