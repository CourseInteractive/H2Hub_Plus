using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{

    public class ActionOnCondition : Action
    {
        public string variableName;

        public enum Source { Global, Local }
        public Source source;
        public enum Comparison { Equal, Greater, GreaterOrEqual, Less, LessOrEqual }
        public Comparison vergleich;

        public int value;

        public Sequence sequenceIfTrue;
        public Sequence sequenceIfFalse;

        override public void ExecuteAction()
        {
            int variableContent = 0;
            if(source == Source.Global)
            {
                variableContent = VariableManager.Instance.GetVariable(variableName);
            }
            else
            {
                variableContent = VariableManager.Instance.GetLocalVariable(variableName);
            }

            Debug.Log("Condition");
            switch (vergleich)
            {
                case Comparison.Equal:
                    ExecuteResultOfComparison(variableContent == value);
                    break;
                case Comparison.Greater:
                    ExecuteResultOfComparison(variableContent > value);
                    break;
                case Comparison.GreaterOrEqual:
                    ExecuteResultOfComparison(variableContent >= value);
                    break;
                case Comparison.Less:
                    ExecuteResultOfComparison(variableContent < value);
                    break;
                case Comparison.LessOrEqual:
                    ExecuteResultOfComparison(variableContent <= value);
                    break;

            }
            //ReportActionEnd();
        }

        void ExecuteResultOfComparison(bool value)
        {
            if (value && sequenceIfTrue != null)
                sequenceIfTrue.ExecuteCompleteSequence();
            else if (!value && sequenceIfFalse != null)
                sequenceIfFalse.ExecuteCompleteSequence();
            ReportActionEnd();
        }
        override public string GetAdditionalInfo()
        {
            if (sequenceIfFalse == null && sequenceIfTrue == null)
                return "- No Sequences set!";
            return "Is " + variableName + " " + vergleich.ToString() + " " + value + " ?";
        }

    }

}