using Course.PrototypeScripting;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class SequenceTrigger_Object : MonoBehaviour
    {
        public GameObject neededObject;
        public Sequence sequenceOnTriggerEnter;
        public Sequence sequenceOnTriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            if (neededObject == other.gameObject && sequenceOnTriggerEnter != null)
                sequenceOnTriggerEnter.ExecuteCompleteSequence();
        }

        private void OnTriggerExit(Collider other)
        {
            if (neededObject == other.gameObject && sequenceOnTriggerExit != null)
                sequenceOnTriggerExit.ExecuteCompleteSequence();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (neededObject == other.gameObject && sequenceOnTriggerEnter != null)
                sequenceOnTriggerEnter.ExecuteCompleteSequence();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (neededObject == other.gameObject && sequenceOnTriggerExit != null)
                sequenceOnTriggerExit.ExecuteCompleteSequence();
        }

    }
}
