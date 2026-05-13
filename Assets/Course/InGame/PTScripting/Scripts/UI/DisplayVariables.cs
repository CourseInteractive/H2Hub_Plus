using UnityEngine;

namespace Course.PrototypeScripting
{
    public class DisplayVariables : MonoBehaviour
    {
   
        public string variableName;
        public bool permanentUpdate;


        private void Start()
        {
            VariableManager.Instance.OnVariableChanged += AdjustUI;
            AdjustUI();
        }

        // Update is called once per frame
        void Update()
        {
            if (permanentUpdate)
                AdjustUI();
      
        }

        public virtual void AdjustUI()
        {
       
        }
    }
}
