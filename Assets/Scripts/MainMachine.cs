using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Umwandlungsrezept: definiert Input- und Output-Verhältnisse pro Tick.
/// </summary>
[System.Serializable]
public struct ConversionRecipe
{
    [Tooltip("Ressourcentyp, der verbraucht wird")]
    public ResourceType inputType;
    [Tooltip("Menge pro Tick")]
    public float inputAmount;
}

[System.Serializable]
public struct ConversionOutput
{
    [Tooltip("Ressourcentyp, der erzeugt wird")]
    public ResourceType outputType;
    [Tooltip("Menge pro Tick")]
    public float outputAmount;
}

/// <summary>
/// Hauptmaschine. Zieht aus Input-Containern, wandelt um und befüllt Output-Container.
/// Alle Felder sind im Inspector konfigurierbar.
/// </summary>
public class MainMachine : MonoBehaviour
{
    public static MainMachine instance;

    [Header("Rezept")]
    [SerializeField] private List<ConversionRecipe> inputs = new();
    [SerializeField] private List<ConversionOutput> outputs = new();

    [Header("Takt")]
    [Tooltip("Sekunden zwischen jedem Verarbeitungsschritt")]
    [SerializeField] private float tickInterval = 1f;

    [Header("Module (im Inspector zuweisen)")]
    [SerializeField] private List<Container> inputModules = new();
    [SerializeField] private List<Container> outputModules = new();

    public MachineProblem currentProblem;

    // Laufzeit-Zustand
    private float tickTimer = 0f;

    //public bool IsRunning => currentState == MachineState.Running;
    public enum MachineState { Off, PowerUp, Running }
    public MachineState currentState;


    public float timeToFullPower = 4f;
    public float currentPowerLevel = 0;
    float powerTimer;

    public Socket repairModuleSocket;

    private void Awake()
    {
        instance = this;
        
    }

    private void Start()
    {
        
    }

    // -------------------------------------------------------------------------
    // Steuerung
    // -------------------------------------------------------------------------

    [ContextMenu("Turn On")]
    public void TurnOn()
    {
        if(currentProblem != null)
        {
            // TODO: Problem Indicator
            return;
        }
        currentState = MachineState.PowerUp;
        powerTimer = timeToFullPower;
        currentPowerLevel = 0f;
        Debug.Log("[MainMachine] Eingeschaltet.");
    }
    [ContextMenu("Turn Off")]
    public void TurnOff()
    {
        currentState = MachineState.Off;
        currentPowerLevel = 0f;
        Debug.Log("[MainMachine] Ausgeschaltet.");
    }

    public void Toggle() { 
        if (currentState == MachineState.Off) TurnOn(); 
        else TurnOff(); }

    // -------------------------------------------------------------------------
    // Tick-Schleife
    // -------------------------------------------------------------------------

    private void Update()
    {
        if (currentState == MachineState.Off) return;
        if(currentState == MachineState.PowerUp)
        {
            powerTimer -= Time.deltaTime;
            currentPowerLevel = 1f - (powerTimer / timeToFullPower);
            if(powerTimer < 0)
            {
                currentPowerLevel = 1f;
                currentState = MachineState.Running;
            }
        }


        tickTimer += Time.deltaTime;
        if (tickTimer >= tickInterval)
        {
            tickTimer -= tickInterval;
            Tick();
        }
    }

    private void Tick()
    {
        // 1. Inputs prüfen
        if (!CanConsumeInputs())
        {
            Debug.Log("[MainMachine] Eingabe-Ressourcen fehlen – Maschine stoppt.");
            TurnOff();
            return;
        }

        // 2. Outputs prüfen: alle angeschlossenen und ALLE voll → stopp
        if (AllConnectedOutputsFull())
        {
            Debug.Log("[MainMachine] Alle Output-Module voll – Maschine hält an.");
            return;
        }

        // 3. Inputs verbrauchen
        ConsumeInputs();

        // 4. Outputs befüllen (oder Fallback aufrufen, falls kein Modul)
        ProduceOutputs();
    }

    // -------------------------------------------------------------------------
    // Input-Logik
    // -------------------------------------------------------------------------

    private bool CanConsumeInputs()
    {
        foreach (var recipe in inputs)
        {
            Container module = FindModule(inputModules, recipe.inputType);
            module.TriggerImmediateRefill();

            if (module == null || !module.HasAtLeast(recipe.inputAmount * currentPowerLevel))
                return false;
        }
        return true;
    }

    private void ConsumeInputs()
    {
        foreach (var recipe in inputs)
        {
            Container module = FindModule(inputModules, recipe.inputType);
            module?.Remove(recipe.inputAmount * currentPowerLevel);
        }
    }

    // -------------------------------------------------------------------------
    // Output-Logik
    // -------------------------------------------------------------------------

    private bool AllConnectedOutputsFull()
    {
        foreach (var output in outputs)
        {
            Container module = FindModule(outputModules, output.outputType);
            if (module == null) continue;   // nicht angeschlossen – ignorieren
            if (!module.IsFull) return false;
        }
        // Nur true, wenn es mindestens ein angeschlossenes Modul gibt und alle voll sind
        bool anyConnected = outputs.Exists(o => FindModule(outputModules, o.outputType) != null);
        return anyConnected && true;
    }

    private void ProduceOutputs()
    {
        
        foreach (var output in outputs)
        {
            Container module = FindModule(outputModules, output.outputType);
            float realOutput = output.outputAmount * currentPowerLevel;
            if (module != null)
            {
                if (!module.IsFull)
                    module.Add(realOutput);
                if(output.outputType == ResourceType.H)
                {
                    problemManager.IncreaseRegisteredOutput(realOutput);
                }
            }
            else
            {
                // Kein Modul angeschlossen: Fallback für z.B. Partikeleffekte
                OnOutputWithoutModule(output.outputType, realOutput);
            }
        }
    }
    public ProblemManager problemManager;
    /// <summary>
    /// Wird aufgerufen, wenn ein Output erzeugt wird, aber kein Modul angeschlossen ist.
    /// Hier kann z.B. ein Partikelsystem getriggert werden.
    /// </summary>
    protected virtual void OnOutputWithoutModule(ResourceType type, float amount)
    {
        Debug.Log($"[MainMachine] Output '{type}' ({amount}) – kein Modul angeschlossen.");
    }

    // -------------------------------------------------------------------------
    // Hilfsmethoden
    // -------------------------------------------------------------------------

    public Container FindOutputModule(ResourceType type)
    {
        return outputModules.Find(c => c != null && c.ResourceType == type);
    }

    public Container FindModule(List<Container> list, ResourceType type)
    {
        return list.Find(c => c != null && c.ResourceType == type);
    }

    // -------------------------------------------------------------------------
    // Öffentliche Modul-Verwaltung (zur Laufzeit)
    // -------------------------------------------------------------------------

    /// <summary>
    /// Schließt einen Container an. Bestimmt anhand des ResourceType automatisch,
    /// ob er in Input- oder Output-Liste gehört. Ein bereits vorhandener Container
    /// desselben Typs wird vorher getrennt.
    /// </summary>
    public void Connect(Container module)
    {
        if (module == null) return;

        ResourceType type = module.ResourceType;

        // Prüfe, ob der Typ im Rezept als Input oder Output definiert ist
        bool isInput  = inputs.Exists(r => r.inputType  == type);
        bool isOutput = outputs.Exists(o => o.outputType == type);

        if (isInput)
        {
            Disconnect(type);
            inputModules.Add(module);
            Debug.Log($"[MainMachine] Input-Modul '{type}' angeschlossen.");
        }
        else if (isOutput)
        {
            Disconnect(type);
            outputModules.Add(module);
            Debug.Log($"[MainMachine] Output-Modul '{type}' angeschlossen.");
        }
        else
        {
            Debug.LogWarning($"[MainMachine] ResourceType '{type}' ist weder Input noch Output im Rezept – ignoriert.");
        }
    }

    public void Connect(RepairModule module)
    {
        if (module.working)
            ProblemSolved();
    }

    /// <summary>
    /// Trennt den Container mit dem angegebenen ResourceType (Input oder Output).
    /// </summary>
    public void Disconnect(ResourceType type)
    {
        // Input-Liste durchsuchen
        Container existing = FindModule(inputModules, type);
        if (existing != null)
        {
            existing.HandleDisconnect();
            inputModules.Remove(existing);
            Debug.Log($"[MainMachine] Input-Modul '{type}' getrennt.");
            return;
        }

        // Output-Liste durchsuchen
        existing = FindModule(outputModules, type);
        if (existing != null)
        {
            existing.HandleDisconnect();
            outputModules.Remove(existing);
            Debug.Log($"[MainMachine] Output-Modul '{type}' getrennt.");
        }
    }

    /// <summary>
    /// Trennt den übergebenen Container, indem sein ResourceType ermittelt
    /// und an Disconnect(ResourceType) weitergegeben wird.
    /// </summary>
    public void Disconnect(Container module)
    {
        if (module == null) return;
        Disconnect(module.ResourceType);
    }

    public void Disconnect(RepairModule module)
    {
        if (module == null) return;
        //Disconnect(module.ResourceType);
    }

    public void RemoveHydrogen(int amount)
    {
        Remove(amount, ResourceType.H);
    }

    public void SellHydrogen(int amount)
    {
        Container c = FindModule(outputModules, ResourceType.H);
        c.SellAmount(amount);
    }
    public void SellOxygen(int amount)
    {
        Container c = FindModule(outputModules, ResourceType.O);
        c.SellAmount(amount);
    }

    public void SellHeat(int amount)
    {
        Container c = FindModule(outputModules, ResourceType.W);
        c.SellAmount(amount);
    }

    public Container GetWaterTank()
    {
        return FindModule(inputModules, ResourceType.H2O);
    }

    public Container GetHydrogenTank()
    {
        return FindModule(outputModules, ResourceType.H);
    }

    public void Remove(int amount, ResourceType type)
    {
        Container c = FindModule(outputModules, type);
        c.Remove(amount);
    }

    public bool AmountAvailableInOutput(int amount, ResourceType type)
    {
        Container c = FindModule(outputModules, type);
        if (c == null)
            return false;
        return c.CurrentAmount >= amount;
    }

    public void ImposeProblem(MachineProblem problem)
    {
        if(repairModuleSocket && repairModuleSocket.lockedInteractable != null)
        {
            repairModuleSocket.lockedInteractable.GetComponent<RepairModule>().Break();
        }
        currentProblem = problem;
        TurnOff();
        problemDisplay.Activate();
    }

    public void ProblemSolved()
    {
        problemDisplay.SetOff();
        currentProblem = null;
    }

    public AlternatingMeshLight problemDisplay;
}
