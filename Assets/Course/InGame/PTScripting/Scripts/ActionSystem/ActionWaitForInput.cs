using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionWaitForInput : Action
    {
        public enum InputType { Keyboard, Mouse, InputManager }
        public enum MouseButton { Left, Right, Middle }

        public InputType inputType;

        public KeyCode inputKey;
        public MouseButton mouseInput;
        public string inputButtonName;
        bool active = false;

        override public void ExecuteAction()
        {
            active = true;
        }


        private void Update()
        {
            if (active)
            {
                switch (inputType)
                {
                    case InputType.Keyboard:
                      //  if (Input.GetKeyDown(inputKey))
                       //     ContinueSequence();
                        break;
                    case InputType.Mouse:
                      //  if (Input.GetMouseButtonDown((int)mouseInput))
                      //      ContinueSequence();
                        break;
                    case InputType.InputManager:
                     //   if (Input.GetButtonDown(inputButtonName))
                     //       ContinueSequence();
                        break;
                }

            }
        }

        void ContinueSequence()
        {
            active = false;
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            switch (inputType)
            {
                case InputType.Keyboard:
                    return "Wait for input => " + inputKey.ToString();
                case InputType.Mouse:
                    return "Wait for input => " + mouseInput.ToString();
                case InputType.InputManager:
                    return "Wait for input => " + inputButtonName;
            }
            return "Wait for input => " + inputKey.ToString();
        }
    }
}