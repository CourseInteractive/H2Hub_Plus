using UnityEngine;

namespace Course.PrototypeScripting
{
    public class TimerAsTextDisplay : DisplayTimer
    {
        public enum Style { Simple, Complex }
        [HideInInspector]
        public Style style = Style.Simple;

        public string prefix;
        UnityEngine.UI.Text textUI;
        public string timeFormatShort;
        private void Awake()
        {
            textUI = GetComponent<UnityEngine.UI.Text>();

        }

        public override void AdjustUI()
        {
            switch (style)
            {
                case Style.Simple:
                    textUI.text = prefix + Mathf.Clamp(myTimer.remainingTime, 0, Mathf.Infinity).ToString(timeFormatShort);
                    break;
                case Style.Complex:
                    textUI.text = string.Format(prefix, myTimer.remainingTime); // + VariableManager.Instance.GetVariable(variableName).ToString();
                    break;
            }
        }
    }
}
