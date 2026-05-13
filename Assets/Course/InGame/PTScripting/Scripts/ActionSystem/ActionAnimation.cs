using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionAnimation : Action
    {
        public enum ActionType { Play, Stop }
        public ActionType actionType;

        public Animation animationComponent;
        public string animationName;
        public bool waitUntilEnded;
        override public void ExecuteAction()
        {

            AnimationState clip = animationComponent[animationName];
            switch (actionType)
            {
                case ActionType.Play:
                    if (animationName.Trim() == "")
                        animationComponent.Play();
                    else
                        animationComponent.Play(animationName);

                    if (waitUntilEnded)
                        Invoke("GoOn", clip.length);
                    else
                        GoOn();
                    break;
                case ActionType.Stop:
                    animationComponent.Stop();
                    GoOn();
                    break;
            }


        }

        void GoOn()
        {
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (animationComponent == null)
                return "- No ObjectSelected ! -";
            return animationComponent.gameObject.name + " => " + animationName;
        }
    }
}