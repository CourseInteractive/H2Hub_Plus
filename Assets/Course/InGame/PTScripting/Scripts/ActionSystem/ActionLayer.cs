using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionLayer : Action
    {
        public LayerMask visible_layer;

        override public void ExecuteAction()
        {
            Camera.main.cullingMask = visible_layer;
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {

            return "";
        }
    }
}