using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{

    public class ActionTransformLibrary : Action
    {
        public ObjectTransformLibrary objToChange;
        public string state;
        public float time;
        public bool useAnimationCurve;
        public AnimationCurve curve;

        override public void ExecuteAction()
        {
            if(time <= 0)
                objToChange.SetStateInstant(state);
            else if(useAnimationCurve)
                objToChange.MoveIntoState(state, time, curve);
            else
                objToChange.MoveIntoState(state, time);
            ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (objToChange == null)
                return "! No Object set!";
            return objToChange.name;
        }
    }
}
