using System.Collections.Generic;

namespace Course.PrototypeScripting
{
    public class VariableAsTextDisplay : DisplayVariables
    {
        public enum Style { Simple, Complex }
        public Style style;

        public string prefix;
        public List<string> variableNames;
        UnityEngine.UI.Text textUI;
        private void Awake()
        {
            textUI = GetComponent<UnityEngine.UI.Text>();

        }

        public override void AdjustUI()
        {
            switch (style)
            {
                case Style.Simple:
                    textUI.text = prefix + VariableManager.Instance.GetVariable(variableName);
                    break;
                case Style.Complex:

                    string[] variableValues = new string[variableNames.Count];
                    for (int i = 0; i < variableNames.Count; i++)
                    {
                        variableValues[i] = VariableManager.Instance.GetVariable(variableNames[i]).ToString();
                    }
                    textUI.text = string.Format(prefix, variableValues); // + VariableManager.Instance.GetVariable(variableName).ToString();
                    break;
            }
        }
    }
}
