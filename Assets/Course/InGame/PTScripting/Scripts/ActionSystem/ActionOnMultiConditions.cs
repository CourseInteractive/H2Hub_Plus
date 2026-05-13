using Course.PrototypeScripting;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ActionOnMultiConditions : Action
    { 

        public enum Comparison { Equal, Unequal }
        public enum ExtComparison { Equal, Greater, GreaterOrEqual, Less, LessOrEqual, NotEqual }
        public Comparison compType;
        [System.Serializable]
        public class ComparisonPair
        {
            public string varName;
            public int varValue;
            public ExtComparison comp;

            public ComparisonPair()
            {
                varName = "NEW";
                varValue = 0;
            }

            public bool IsTrueInContext(Comparison _compType)
            {
                switch (comp)
                {
                    case ExtComparison.Equal:
                        return VariableManager.Instance.GetVariable(varName) == varValue;
                    case ExtComparison.Greater:
                        return VariableManager.Instance.GetVariable(varName) > varValue;
                    case ExtComparison.GreaterOrEqual:
                        return VariableManager.Instance.GetVariable(varName) >= varValue;
                    case ExtComparison.Less:
                        return VariableManager.Instance.GetVariable(varName) < varValue;
                    case ExtComparison.LessOrEqual:
                        return VariableManager.Instance.GetVariable(varName) <= varValue;
                    case ExtComparison.NotEqual:
                        return VariableManager.Instance.GetVariable(varName) != varValue;
                    default:
                        return true;
                }
                /*
                if (_compType == Comparison.Equal)
                {
                    return VariableManager.Instance.GetVariable(varName) == varValue;
                }

                else
                {
                    return VariableManager.Instance.GetVariable(varName) != varValue;
                }*/

            }


        }
        public ComparisonPair[] comparisons;

        public Sequence sequenceIfTrue;
        public Sequence sequenceIfFalse;

        override public void ExecuteAction()
        {
            bool claimIsTrue = true;
            foreach(ComparisonPair comparison in comparisons)
            {
           
                claimIsTrue = comparison.IsTrueInContext(compType);
                if (!claimIsTrue)
                    break;
            }
            if(claimIsTrue)
            {
                if (sequenceIfTrue == null)
                    Debug.LogError("No Sequence for TRUE is set!");
                else
                    sequenceIfTrue.ExecuteCompleteSequence();
            }
            else if(sequenceIfFalse)
                sequenceIfFalse.ExecuteCompleteSequence();
            else
                ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (sequenceIfFalse == null && sequenceIfTrue == null)
                return "- No Sequences set!";
            return ""; // Is " + variableName + " " + vergleich.ToString() + " " + value + " ?";
        }

    }
}
