using UnityEngine;

public class ProblemManager : MonoBehaviour
{

    public Vector2 problemRandomLimits;

    public float registeredHydrogenOutput;
    public float nextProblemAt;

    public MachineProblem[] problems;
    public void Awake()
    {
        RandomizeNextProblemTime();
    }

    public void IncreaseRegisteredOutput(float amount)
    {
        registeredHydrogenOutput += amount * GetRunningPowerFactor();
        if(registeredHydrogenOutput >= nextProblemAt)
        {
            if(!BuildVersionSetup_Ingame.IsCustomSettingActive("MachineDoesNotBreak"))
            {
                ImposeProblem();
            }
            
            RandomizeNextProblemTime();
        }
    }

    float GetRunningPowerFactor()
    {
        if (MainMachine.instance.runningPower < 0.3f)
            return 0f;
        if (MainMachine.instance.runningPower < 0.8f)
            return 0.5f;
        return 2f;
    }

    void RandomizeNextProblemTime()
    {
        registeredHydrogenOutput = 0;
        nextProblemAt = Mathf.RoundToInt(Random.Range(problemRandomLimits.x, problemRandomLimits.y));

    }

    public void ImposeProblem()
    {
        MachineProblem selectedProblem = problems[Random.Range(0, problems.Length)];
        selectedProblem.Execute();
    }
}
