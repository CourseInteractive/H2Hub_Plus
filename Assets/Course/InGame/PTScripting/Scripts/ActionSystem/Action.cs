using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class Action : MonoBehaviour
    {
        [HideInInspector]
        public int parallelSequenceIndex;

        public void ReportActionEnd()
        {
            SequenceHandler.Instance.ReportActionEnd(parallelSequenceIndex);
        }

        public void ExecuteAction(int index)
        {
            parallelSequenceIndex = index;
            ExecuteAction();
        }

        public void RestoreCompletion(int index)
        {
            parallelSequenceIndex = index;
            RestoreCompletion();
        }

        virtual public void RestoreCompletion()
        {
            ExecuteAction();
        }

        virtual public void ExecuteAction()
        {
        }

        virtual public string GetAdditionalInfo()
        {
            return "";
        }

        public GameObject GetExecutingObject()
        {
            return SequenceHandler.Instance.parallelSequences[parallelSequenceIndex].executingObject;
        }
    }
}



