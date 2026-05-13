using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Course.PrototypeScripting
{
    public class ActionUnityEvent : Action
    {

        public UnityEvent _event;
        override public void ExecuteAction()
        {

            _event.Invoke();
            GoOn();
        }

        void GoOn()
        {
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {

            return "Start Event";
        }
    }
}
