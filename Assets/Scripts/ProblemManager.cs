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
        registeredHydrogenOutput+= amount;
        if(registeredHydrogenOutput >= nextProblemAt)
        {
            ImposeProblem();
            RandomizeNextProblemTime();
        }
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
