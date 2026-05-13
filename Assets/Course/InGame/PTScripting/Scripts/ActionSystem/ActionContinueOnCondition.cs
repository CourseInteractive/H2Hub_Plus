using UnityEngine;


namespace Course.PrototypeScripting
{
    public class ActionContinueOnCondition : Action
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


            }


        }
        public ComparisonPair[] comparisons;

        public Sequence sequenceIfFalse;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        override public void ExecuteAction()
        {
            bool claimIsTrue = true;
            foreach (ComparisonPair comparison in comparisons)
            {

                claimIsTrue = comparison.IsTrueInContext(compType);
                if (!claimIsTrue)
                    break;
            }
            if (claimIsTrue)
            {
                ReportActionEnd();
            }
            else if (sequenceIfFalse)
            {
                SequenceHandler.Instance.StartNewSequence(sequenceIfFalse);
                SequenceHandler.Instance.EndOfSequence(parallelSequenceIndex);
            }
            else
                SequenceHandler.Instance.EndOfSequence(parallelSequenceIndex);
        }

        override public string GetAdditionalInfo()
        {
            return "";
        }
    }
}
