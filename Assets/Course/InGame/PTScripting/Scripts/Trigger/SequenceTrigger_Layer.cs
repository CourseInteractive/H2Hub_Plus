using Course.PrototypeScripting;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class SequenceTrigger_Layer : MonoBehaviour
    {
        public LayerMask checkedLayers;
        public Sequence sequenceOnTriggerEnter;
        public Sequence sequenceOnTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (LayerIsInteresting(other.gameObject.layer) && sequenceOnTriggerEnter != null)
                sequenceOnTriggerEnter.ExecuteCompleteSequence();
        }

        private void OnTriggerExit(Collider other)
        {
            if (LayerIsInteresting(other.gameObject.layer) && sequenceOnTriggerExit != null)
                sequenceOnTriggerExit.ExecuteCompleteSequence();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (LayerIsInteresting(other.gameObject.layer) && sequenceOnTriggerEnter != null)
                sequenceOnTriggerEnter.ExecuteCompleteSequence();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (LayerIsInteresting(other.gameObject.layer) && sequenceOnTriggerExit != null)
                sequenceOnTriggerExit.ExecuteCompleteSequence();
        }


        public bool LayerIsInteresting(int layer)
        {
            return checkedLayers == (checkedLayers | (1 << layer));
        }
    }
}
