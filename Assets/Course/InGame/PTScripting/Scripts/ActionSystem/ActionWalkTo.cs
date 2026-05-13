using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
namespace Course.PrototypeScripting
{
    public class ActionWalkTo : Action
    {
        public MovementIndirect character;
        public GameObject target;
        public bool waitForArrival = false;
        public float arrivalDistance = 0.5f;
        bool active = false;

        override public void ExecuteAction()
        {
            if (character == null || target == null)
            {
                ReportActionEnd();
                return;
            }
            character.MoveTo(target);
            if (!waitForArrival)
                ReportActionEnd();
            active = true;
        }

        private void Update()
        {
            if (waitForArrival && active)
            {
                if (Vector3.Distance(character.transform.position, target.transform.position) < arrivalDistance)
                {
                    character.GetComponent<NavMeshAgent>().enabled = false;
                    active = false;
                    ReportActionEnd();
                }
            }
        }

        override public string GetAdditionalInfo()
        {
            if (character == null || target == null)
                return "- No Character or target set! -";
            return character.name + " => " + target.name;
        }
    }

}