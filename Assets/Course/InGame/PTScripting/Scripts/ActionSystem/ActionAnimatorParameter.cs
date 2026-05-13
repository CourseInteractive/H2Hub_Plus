using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionAnimatorParameter : Action
    {
        public Animator animatedObject;
        public enum ParameterType { Float, Int, Bool, Trigger }
        public ParameterType parametertype;
        public string parameterName;
        public float floatValue;
        public int intValue;
        public bool boolValue;

        override public void ExecuteAction()
        {
            switch (parametertype)
            {
                case ParameterType.Float:
                    animatedObject.SetFloat(parameterName, floatValue);
                    break;
                case ParameterType.Int:
                    animatedObject.SetInteger(parameterName, intValue);
                    break;
                case ParameterType.Bool:
                    animatedObject.SetBool(parameterName, boolValue);
                    break;
                case ParameterType.Trigger:
                    animatedObject.SetTrigger(parameterName);
                    break;
            }
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (animatedObject == null)
                return "- No Animator set!";
            string val = "";
            switch (parametertype)
            {
                case ParameterType.Float:
                    val = floatValue.ToString();
                    break;
                case ParameterType.Int:
                    val = intValue.ToString();
                    break;
                case ParameterType.Bool:
                    val = boolValue.ToString();
                    break;
                case ParameterType.Trigger:
                    return "(" + animatedObject.name + ") Trigger: " + parameterName;
            }
            return "(" + animatedObject.name + ") " + parameterName + " => " + val;
        }
    }

}