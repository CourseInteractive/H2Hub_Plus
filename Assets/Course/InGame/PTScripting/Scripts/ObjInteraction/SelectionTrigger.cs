using Course.PrototypeScripting;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class SelectionTrigger : SelectableObject
    {
        private void Start()
        {
        
        }

        private void OnTriggerEnter(Collider other)
        {
            if(enabled)
                RuntimeGlobal.Select(this);

        }

        private void OnTriggerExit(Collider other)
        {
    
            RuntimeGlobal.ClearSelection();

        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (enabled)
                RuntimeGlobal.Select(this);

        }

        private void OnTriggerExit2D(Collider2D other)
        {

            RuntimeGlobal.ClearSelection();

        }
    }
}
