using UnityEngine;

public class MachineProblem : MonoBehaviour
{ 
    public enum MachineToBreak { Main }
    public MachineToBreak machine;


    public GameObject objWhenActive;

    private void Start()
    {
        if (objWhenActive)
            objWhenActive.SetActive(false);
    }
    public void Execute()
    {
        switch(machine)
        {
            case MachineToBreak.Main:
                MainMachine.instance.ImposeProblem(this);
                break;
        }
        if (objWhenActive)
            objWhenActive.SetActive(true);
    }

    public void SolveProblem()
    {
        switch (machine)
        {
            case MachineToBreak.Main:
                MainMachine.instance.ProblemSolved();
                break;
        }
        if (objWhenActive)
            objWhenActive.SetActive(false);
    }
}
