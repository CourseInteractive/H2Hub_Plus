using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionVariable : Action
    {
        public string variableName;

        public enum Source { Global, Local }
        public Source source;
        public enum Actions { Change, SetExplicit }
        public Actions action;
        public int value;

        override public void ExecuteAction()
        {
            if(source == Source.Global)
            {
                switch (action)
                {
                    case Actions.Change:
                        VariableManager.Instance.SetVariable(variableName, VariableManager.Instance.GetVariable(variableName) + value);
                        break;
                    case Actions.SetExplicit:
                        VariableManager.Instance.SetVariable(variableName, value);
                        break;
                }
            }
            else
            {
                switch (action)
                {
                    case Actions.Change:
                        VariableManager.Instance.SetLocalVariable(variableName, VariableManager.Instance.GetVariable(variableName) + value);
                        break;
                    case Actions.SetExplicit:
                        VariableManager.Instance.SetLocalVariable(variableName, value);
                        break;
                }
            }
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (action == Actions.Change)
                return variableName + " + " + value;
            else
                return variableName + " => " + value;
        }
    }
}
