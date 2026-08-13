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
    public enum MachineState { Off, PowerUp, Running}
    public MachineState currentState;
    public bool problemOnMachine;

    public float timeToFullPower = 4f;
    public float currentPowerLevel = 0;
    float powerTimer;

    public Socket repairModuleSocket;

    public Socket[] elektrolyseModuleSockets;

    public Socket inputSocket_Water;

    private void Awake()
    {
        instance = this;
        TurnOff();
        SetRunningPower(0.6f);
        runningPowerKnob.value = runningPower;
    }


    // -------------------------------------------------------------------------
    // Steuerung
    // -------------------------------------------------------------------------

    [ContextMenu("Turn On")]
    public void TurnOn()
    {
        mainLever.value = true;
        if (currentProblem != null)
        {
            // TODO: Problem Indicator
            Invoke("TurnOff", 1.0f);
            return;
        }
        currentState = MachineState.PowerUp;
        powerTimer = timeToFullPower;
        currentPowerLevel = 0f;
        Debug.Log("[MainMachine] Eingeschaltet.");
    }

    public float runningPower;
    public TMPro.TMP_Text powerDisplay;
    public UnityEngine.XR.Content.Interaction.XRKnob runningPowerKnob;

    public void SetRunningPower(float value)
    {
        runningPower = value;
        powerDisplay.text = Mathf.RoundToInt(Mathf.Lerp(0, 130, value)) + "%";
    }

    [ContextMenu("Turn Off")]
    public void TurnOff()
    {
        currentState = MachineState.Off;
        currentPowerLevel = 0f;
        Debug.Log("[MainMachine] Ausgeschaltet.");
        mainLever.value = false;
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
        if(!CheckModulesForProblems())
        {
            ProblemFound();
            return;
        }
        else
        {
            ProblemSolved();
        }

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

    bool CheckModulesForProblems()
    {
        bool elModulePresent = false;
        foreach(Socket elSocket in elektrolyseModuleSockets)
        {
            if (elSocket.lockedInteractable != null)
            {
                elModulePresent = true;
                ToSocketInteractable interactable = elSocket.lockedInteractable;
                RepairModule module = interactable.GetComponent<RepairModule>();
                if (elSocket.kind != module.kind)
                {
                    return false;
                }
                if (!module.working)
                {
                    return false;
                }
            }
             
        }

        if(!elModulePresent)
            return false;
        /*
        if (repairModuleSocket.lockedInteractable != null)
        {
            ToSocketInteractable interactable = repairModuleSocket.lockedInteractable;
            RepairModule module = interactable.GetComponent<RepairModule>();
            if (repairModuleSocket.kind != module.kind)
            {
                Debug.Log("1");
                return false;
            }
                
            if (!module.working)
            {
                Debug.Log("2");
                return false;
            }
        }
        else
        {
            Debug.Log("3");
            return false;
        }*/

        if (inputSocket_Water.lockedInteractable != null)
        {
            ToSocketInteractable interactable = inputSocket_Water.lockedInteractable;
            RepairModule module = interactable.GetComponent<RepairModule>();
            if (inputSocket_Water.kind != module.kind)
            {
                Debug.Log("4");
                return false;
            }
            if (!module.working)
            {
                Debug.Log("5");
                return false;
            }
        }
        else
        {
            Debug.Log("6");
            return false;
        }
        return true;
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
            float realInputUsed = recipe.inputAmount * currentPowerLevel;
            if (module.ResourceType == ResourceType.H2O && !BuildVersionSetup_Ingame.IsCustomSettingActive("UnlimitedWater"))
            {
                module?.Remove(realInputUsed);
            }
            else
                module?.Remove(realInputUsed);
            if(module.ResourceType == ResourceType.H2O)
            {
                ReduceInputDurability(realInputUsed);
            }
        }
    }

    // -------------------------------------------------------------------------
    // Output-Logik
    // -------------------------------------------------------------------------

    private bool AllConnectedOutputsFull()
    {
        return false;
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
        float elModuleFactor = 0f;
        foreach (Socket elSocket in elektrolyseModuleSockets)
        {
            if (elSocket.lockedInteractable)
                elModuleFactor += 0.3333f;
        }


        foreach (ConversionOutput output in outputs)
        {
            Container module = FindModule(outputModules, output.outputType);
            float outputPower = currentPowerLevel * Mathf.Lerp(GameData.Instance.Values.outputFactorLimitsByDial.x, GameData.Instance.Values.outputFactorLimitsByDial.y, runningPower);

            float realOutput = output.outputAmount * outputPower * elModuleFactor;
            float dividedOutput = realOutput * (1f / (realOutput * 3f));
            if (module != null)
            {
                if (!module.IsFull)
                    module.Add(realOutput);

                if(output.outputType == ResourceType.H)
                {
                    ReduceOutputDurability(dividedOutput);
                    problemManager.IncreaseRegisteredOutput(realOutput);
                }
            }
            else
            {
                if (output.outputType == ResourceType.H)
                {
                    ReduceOutputDurability(dividedOutput);
                    problemManager.IncreaseRegisteredOutput(realOutput);
                }
                // Kein Modul angeschlossen: Fallback für z.B. Partikeleffekte
                OnOutputWithoutModule(output.outputType, realOutput);
            }
        }
    }

    void ReduceOutputDurability(float amount)
    {
        foreach (Socket elSocket in elektrolyseModuleSockets)
        {
            if(elSocket.lockedInteractable != null)
                elSocket.lockedInteractable.GetComponent<RepairModule>().UseUpAmount(amount);
        }
         //   repairModuleSocket.lockedInteractable.GetComponent<RepairModule>().UseUpAmount(amount);
    }


    void ReduceInputDurability(float amount)
    {
        inputSocket_Water.lockedInteractable.GetComponent<RepairModule>().UseUpAmount(amount);
    }

    public ProblemManager problemManager;
    /// <summary>
    /// Wird aufgerufen, wenn ein Output erzeugt wird, aber kein Modul angeschlossen ist.
    /// Hier kann z.B. ein Partikelsystem getriggert werden.
    /// </summary>
    protected virtual void OnOutputWithoutModule(ResourceType type, float amount)
    {
        //Debug.Log($"[MainMachine] Output '{type}' ({amount}) – kein Modul angeschlossen.");
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
    #region SocketConnection


    /// <summary>
    /// Schließt einen Container an. Bestimmt anhand des ResourceType automatisch,
    /// ob er in Input- oder Output-Liste gehört. Ein bereits vorhandener Container
    /// desselben Typs wird vorher getrennt.
    /// </summary>
    public void Connect(Container module)
    {
        if (module == null) return;
        InGameLog.Log("Socket Connect " + gameObject.name);
        ResourceType type = module.ResourceType;

        // Prüfe, ob der Typ im Rezept als Input oder Output definiert ist
        bool isInput  = inputs.Exists(r => r.inputType  == type);
        bool isOutput = outputs.Exists(o => o.outputType == type);

        if (isInput)
        {
            Disconnect(type);
            inputModules.Add(module);
            InGameLog.Log($"[MainMachine] Input-Modul '{type}' angeschlossen.");
        }
        else if (isOutput)
        {
            Disconnect(type);
            outputModules.Add(module);
            InGameLog.Log($"[MainMachine] Output-Modul '{type}' angeschlossen.");
        }
        else
        {
            Debug.LogWarning($"[MainMachine] ResourceType '{type}' ist weder Input noch Output im Rezept – ignoriert.");
        }
    }

    public void Connect(RepairModule module)
    {
       // if (module.working)
            
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

    #endregion

    #region ChangeResources

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

    #endregion

    public void ImposeProblem(MachineProblem problem)
    {
     /*   if(repairModuleSocket && repairModuleSocket.lockedInteractable != null)
        {
            repairModuleSocket.lockedInteractable.GetComponent<RepairModule>().Break();
        }
        currentProblem = problem;
        TurnOff();
        problemDisplay.Activate();*/
    }

    void ProblemFound()
    {
        if (problemOnMachine)
            return;
        GameEventManager.Instance.ReportGameEvent("Problem");
        problemOnMachine = true;
        Debug.Log("Problem");
        TurnOff();
        problemDisplay.Activate();
    }

    public void ProblemSolved()
    {
        if (!problemOnMachine)
            return;
        GameEventManager.Instance.ReportGameEvent("ProblemSolved");
        problemDisplay.SetOff();
        currentProblem = null;
        problemOnMachine = false;
    }

    public AlternatingMeshLight problemDisplay;
    public UnityEngine.XR.Content.Interaction.XRLever mainLever;
}
