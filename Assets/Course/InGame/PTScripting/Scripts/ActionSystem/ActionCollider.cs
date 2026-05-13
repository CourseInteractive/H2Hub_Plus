using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionCollider : Action
    {
        public Collider _collider;
        public bool state;

        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            if (_collider)
            {
                _collider.enabled = state;
            }
            ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (_collider == null)
                return "No collider set";
            if (state)
                return "Turn Collider " + _collider.name + " on";
            else
                return "Turn Collider " + _collider.name + " off";
        }
    }
}