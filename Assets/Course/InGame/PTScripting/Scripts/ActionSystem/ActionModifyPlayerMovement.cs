using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionModifyPlayerMovement : Action
    {
        public enum MovementType { Direct, Indirect }
        public MovementType type;

        public UnityEngine.AI.NavMeshAgent character;

        public bool charIsPlayer = true;
        public bool changeSpeed;
        public float speed;
        public bool changeJumpForce;
        public float jumpForce;

        override public void ExecuteAction()
        {
            if (type == MovementType.Direct)
                ExecuteActionDirectMovement();
            else
                ExecuteActionIndirectMovement();
        }

        void ExecuteActionDirectMovement()
        {
          /*  if (changeSpeed)
                RuntimeGlobal.keyboardMovement.SetMaxSpeed(speed);
            if (changeJumpForce)
                RuntimeGlobal.keyboardMovement.SetJumpForce(jumpForce);*/
        }

        void ExecuteActionIndirectMovement()
        {
       /*     if (charIsPlayer)
                RuntimeGlobal.mouseMovement.SetMaxSpeed(speed);
            else
                character.speed = speed;*/
        }

        override public string GetAdditionalInfo()
        {
            if (type == MovementType.Indirect)
            {
                if (charIsPlayer)
                    return "Changes player speed to " + speed;
                else if (character == null)
                    return "No Character Set !";
                else
                    return character.name + " => New Speed:" + speed;
            }

            else
                return "Changes player speed to " + speed;
        }
    }
}