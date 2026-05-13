using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    [System.Serializable]
    public class ConditionCase
    {
        public ActionOnCondition.Comparison comparison;
        public int value;
        public Sequence sequence;
    }

    public class ActionConditionSwitch : Action
    {
        public string variableName;

        public ActionOnCondition.Source source;

        public List<ConditionCase> cases = new List<ConditionCase>();

        public Sequence defaultSequence;

        override public void ExecuteAction()
        {
            int variableContent = 0;

            if (source == ActionOnCondition.Source.Global)
            {
                variableContent = VariableManager.Instance.GetVariable(variableName);
            }
            else
            {
                variableContent = VariableManager.Instance.GetLocalVariable(variableName);
            }

            Debug.Log("ConditionSwitch: Checking variable '" + variableName + "' = " + variableContent);

            foreach (ConditionCase conditionCase in cases)
            {
                bool result = EvaluateComparison(variableContent, conditionCase.comparison, conditionCase.value);
                if (result)
                {
                    if (conditionCase.sequence != null)
                        conditionCase.sequence.ExecuteCompleteSequence();

                    ReportActionEnd();
                    return;
                }
            }

            if (defaultSequence != null)
                defaultSequence.ExecuteCompleteSequence();

            ReportActionEnd();
        }

        bool EvaluateComparison(int variableContent, ActionOnCondition.Comparison comparison, int compareValue)
        {
            switch (comparison)
            {
                case ActionOnCondition.Comparison.Equal:
                    return variableContent == compareValue;
                case ActionOnCondition.Comparison.Greater:
                    return variableContent > compareValue;
                case ActionOnCondition.Comparison.GreaterOrEqual:
                    return variableContent >= compareValue;
                case ActionOnCondition.Comparison.Less:
                    return variableContent < compareValue;
                case ActionOnCondition.Comparison.LessOrEqual:
                    return variableContent <= compareValue;
                default:
                    return false;
            }
        }

        override public string GetAdditionalInfo()
        {
            if (cases == null || cases.Count == 0)
                return "- No Cases set!";
            return "Switch on: " + variableName + " (" + cases.Count + " case(s))";
        }
    }
}
