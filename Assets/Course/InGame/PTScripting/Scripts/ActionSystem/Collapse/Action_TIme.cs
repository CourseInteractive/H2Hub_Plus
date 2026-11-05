using UnityEngine;
using Course.PrototypeScripting;

public class Action_Time : Action
{
    public enum TimeAction { SetTime, SetMultiplier}
    public TimeAction timeAction;
    public float value = 1;

    override public void ExecuteAction()
    {
        if (timeAction == TimeAction.SetMultiplier)
            DayNightCycle.Instance.SetTimeMultiplier(value);
        else
            DayNightCycle.Instance.SetTime(value);
        ReportActionEnd();
    }

    override public string GetAdditionalInfo()
    {
        return "";
    }
}
