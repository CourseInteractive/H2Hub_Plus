using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionLookAt : Action
    {
        public GameObject character;
        public GameObject target;
        public float time;
        public bool waitForCompletion = true;

        public enum ActionType { Once, Follow, Stop }
        public ActionType type;

        float timer = 0;
        Vector3 startForward;

        bool active = false;

        Vector3 targetForward
        {
            get
            {
                Vector3 dir = target.transform.position - character.transform.position;
                dir.y = 0;
                return dir;
            }
        }

        override public void ExecuteAction()
        {
            if (type == ActionType.Stop)
            {
                if (character.GetComponent<MovementIndirect>())
                {
                    character.GetComponent<MovementIndirect>().StopLooking();
                }
                return;
            }

            timer = 0;
            startForward = character.transform.forward;
            if (time == 0)
            {
                EndRotation();
                return;
            }

            if (!waitForCompletion)
                ReportActionEnd();
            active = true;
            enabled = true;
        }

        private void Update()
        {
            if (active)
            {
                timer += Time.deltaTime;
                if (timer >= time)
                    EndRotation();
                else
                    character.transform.forward = Vector3.Lerp(startForward, targetForward, timer / time);

            }
        }
        public void EndRotation()
        {
            character.transform.forward = targetForward;
            active = false;
            enabled = false;
            if (waitForCompletion)
                ReportActionEnd();
            if (type == ActionType.Follow)
            {
                if (character.GetComponent<MovementIndirect>())
                {
                    character.GetComponent<MovementIndirect>().KeepLookAtObject(target);
                }
                else
                    Debug.Log("In order for the Character to keep looking at an Object it needs a 'MovementIndirect' Component");
            }
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (character == null)
                return "-No Character set!";
            switch (type)
            {
                case ActionType.Stop:
                    return character.name + " stop looking";
                case ActionType.Follow:
                    if (target == null)
                        return "-No Target set!";
                    return character.name + " follow looking at " + target.name;
                case ActionType.Once:
                    if (target == null)
                        return "-No Target set!";
                    return character.name + " look at " + target.name;

            }
            return "Unexpected error";

        }
    }

}