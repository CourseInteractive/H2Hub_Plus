using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionFollow : Action
    {
        public enum Type { Start, Stop }
        public Type type;

        public MovementIndirect character;
        public GameObject objectToFollow;

        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            if (type == Type.Start)
                character.FollowObject(objectToFollow);
            else
                character.StopFollow();
            ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            return "";
        }
    }
}