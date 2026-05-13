using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionTimer : Action
    {
        public enum Type { Start, StartRandom, Stop, TakeTimeOff }
        public Type type;

        public string timerName;
        public float timeToRun;
        public Vector2 randomLimits;
        public Sequence sequenceWhenRunOut;

        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            if (type == Type.Start)
                VariableManager.Instance.StartTimer(timerName, timeToRun, sequenceWhenRunOut);
            else if (type == Type.StartRandom)
            {
                float randomTime = Random.Range(randomLimits.x, randomLimits.y);
                VariableManager.Instance.StartTimer(timerName, randomTime, sequenceWhenRunOut);
            }
            else if (type == Type.TakeTimeOff)
            {
                VariableManager.Instance.TakeTimeOfTimer(timerName, timeToRun);
            }
            else
                VariableManager.Instance.StopTimer(timerName);
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {


            if (type == Type.Start)
            {
                if (sequenceWhenRunOut == null || timerName.Trim() == "")
                    return "- No Name or Sequence set!";
                else
                    return timerName + " -> " + timeToRun + " -> " + sequenceWhenRunOut.name;
            }
            else if (type == Type.TakeTimeOff)
            {
                return "Take " + timeToRun + " seconds off of " + timerName;
            }
            else
                return "Stop " + timerName;
        }
    }

}