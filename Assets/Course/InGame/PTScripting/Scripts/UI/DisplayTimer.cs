using UnityEngine;

namespace Course.PrototypeScripting
{
    public class DisplayTimer : MonoBehaviour
    {

        public string timerName;
        bool isActive;
        [HideInInspector]
        public VariableManager.Timer myTimer;
        private void Start()
        {
            VariableManager.Instance.OnTimerChanged += ReactionToTimerChange;
            ReactionToTimerChange();
        }

        void Update()
        {
            if(isActive)
            {
                if (myTimer == null)
                    isActive = false;
                else
                    AdjustUI();
            }
        }

        public void ReactionToTimerChange()
        {
            isActive = VariableManager.Instance.TimerActive(timerName);
            myTimer = VariableManager.Instance.GetTimer(timerName);
        }

        public virtual void AdjustUI()
        {

        }
    }
}
