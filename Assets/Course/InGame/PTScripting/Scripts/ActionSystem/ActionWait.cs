using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionWait : Action
    {
        public float waitTimeInSeconds;
        override public void ExecuteAction()
        {
            Invoke("GoOn", waitTimeInSeconds);
        }

        void GoOn()
        {
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            return "Time : " + waitTimeInSeconds + "s";
        }
    }
}