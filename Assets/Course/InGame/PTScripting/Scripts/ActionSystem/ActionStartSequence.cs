using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionStartSequence : Action
    {
        public Sequence sequenceToStart;
        public bool terminateThisSequenceAfterwards;
        public bool terminateAllOtherSequencesBefore;
        public bool terminateAllSequencesButThis;
        override public void ExecuteAction()
        {

            if (terminateAllSequencesButThis)
            {
                SequenceHandler.Instance.EndAllSequencesBut(parallelSequenceIndex);
                ReportActionEnd();
            }
            else if (terminateAllOtherSequencesBefore)
            {
                SequenceHandler.Instance.EndAllSequences();
                ReportActionEnd();
            }
            else if (terminateThisSequenceAfterwards)
            {

                SequenceHandler.Instance.StartNewSequence(sequenceToStart);
                SequenceHandler.Instance.EndOfSequence(parallelSequenceIndex);
            }
            else
            {
                SequenceHandler.Instance.StartNewSequence(sequenceToStart);
                ReportActionEnd();
            }

        }

        override public string GetAdditionalInfo()
        {
            if (sequenceToStart == null)
                return "- No Sequence set ! -";
            return sequenceToStart.gameObject.name;
        }
    }
}