using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class SequenceHandler : MonoBehaviour
    {
        [System.Serializable]
        public class OpenSequence
        {
            public Sequence seq;
            public int actionPointer;

            public OpenSequence(Sequence _seq)
            {
                seq = _seq;
                actionPointer = 0;
            }

            public bool ActionLeft()
            {
                if (seq == null || seq.actions == null || seq.actions.Length == 0)
                    return false;

                return actionPointer < seq.actions.Length;
            }

            public void RestoreCurrentAction()
            {
                if (actionPointer <= seq.actions.Length - 1 && seq.actions[actionPointer] != null)
                {
                    seq.actions[actionPointer].RestoreCompletion(seq.parallelSequenceIndex);
                }
                else
                    SequenceHandler.Instance.ReportActionEnd(seq.parallelSequenceIndex);
            }

            public void ExecuteCurrentAction()
            {
                if (actionPointer <= seq.actions.Length - 1 && seq.actions[actionPointer] != null)
                {
                    seq.actions[actionPointer].ExecuteAction(seq.parallelSequenceIndex);
                }
                else
                    SequenceHandler.Instance.ReportActionEnd(seq.parallelSequenceIndex);
            }

            public void InvokeEndOfSequence()
            {
                seq.InvokeEndOfSequence();
            }
        }
        [System.Serializable]
        public class ParallelSequence
        {
            public Stack<OpenSequence> openSequences;
            public OpenSequence currentSequence;
            public bool silent;
            public int internIndex;
            public GameObject executingObject = null;

            public ParallelSequence()
            {
                openSequences = new Stack<OpenSequence>();
            }

            public void AddSequenceToStack(OpenSequence newSeq)
            {
                if (currentSequence != null && currentSequence.seq != null && currentSequence.seq.actions.Length > 0)
                    openSequences.Push(currentSequence);
                currentSequence = newSeq;
                if (silent)
                    currentSequence.RestoreCurrentAction();
                else
                    currentSequence.ExecuteCurrentAction();
            }

            public void ExecuteNextAction()
            {
                currentSequence.actionPointer++;
                if (currentSequence.ActionLeft())
                {
                    if (silent)
                        currentSequence.RestoreCurrentAction();
                    else
                        currentSequence.ExecuteCurrentAction();
                }
                else
                    EndOfSequence();
            }

            public void EndOfSequence()
            {
                if (currentSequence != null)
                    currentSequence.InvokeEndOfSequence();
                currentSequence = null;
                if (openSequences != null && openSequences.Count > 0)
                {
                    currentSequence = openSequences.Pop();
                    ExecuteNextAction();
                }
                else
                {
                    openSequences = new Stack<OpenSequence>();
                }

            }

        }

        public static SequenceHandler _instance;
        public static SequenceHandler Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No SequenceHandler in the scene. Import the prefab SimpleGameEngine from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }

        public ParallelSequence[] parallelSequences = new ParallelSequence[10];

        // Start is called before the first frame update
        void Awake()
        {
            openSequences = new Stack<OpenSequence>();
            if (_instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public Stack<OpenSequence> openSequences;
        public OpenSequence currentSequence;

        int ActionPointer
        {
            get { return currentSequence.actionPointer; }
            set { currentSequence.actionPointer = value; }
        }

        public void StartNewSequence(Sequence seq, int existingIndex, Sequence.EndOfSequence endOfSeqFct = null)
        {
            seq.parallelSequenceIndex = existingIndex;
            OpenSequence newSeq = new OpenSequence(seq);
            if (endOfSeqFct != null)
                newSeq.seq.OnTimeEndOfSequenceEvent += endOfSeqFct;
            parallelSequences[existingIndex].AddSequenceToStack(newSeq);
        }

        public void StartNewSequence(Sequence seq, Sequence.EndOfSequence endOfSeqFct = null, GameObject executingObject = null)
        {
            int parallelSequenceIndex = GetFreeParallelIndex();
            ParallelSequence sequenceMain = new ParallelSequence();
            sequenceMain.executingObject = executingObject;
            sequenceMain.internIndex = parallelSequenceIndex;
            sequenceMain.silent = false;
            parallelSequences[parallelSequenceIndex] = sequenceMain;
            StartNewSequence(seq, parallelSequenceIndex, endOfSeqFct);
        }

        public void RestoreNewSequence(Sequence seq, int existingIndex, Sequence.EndOfSequence endOfSeqFct = null)
        {
            seq.parallelSequenceIndex = existingIndex;
            OpenSequence newSeq = new OpenSequence(seq);
            if (endOfSeqFct != null)
                newSeq.seq.OnTimeEndOfSequenceEvent += endOfSeqFct;
            parallelSequences[existingIndex].AddSequenceToStack(newSeq);
        }

        public void RestoreNewSequence(Sequence seq, Sequence.EndOfSequence endOfSeqFct = null)
        {
            int parallelSequenceIndex = GetFreeParallelIndex();
            ParallelSequence sequenceMain = new ParallelSequence();
            sequenceMain.silent = true;
            parallelSequences[parallelSequenceIndex] = sequenceMain;
            RestoreNewSequence(seq, parallelSequenceIndex, endOfSeqFct);
        }

        int GetFreeParallelIndex()
        {
            for (int i = 0; i < parallelSequences.Length; i++)
            {
                if (parallelSequences[i] == null || parallelSequences[i].currentSequence == null || parallelSequences[i].currentSequence.seq == null)
                    return i;
            }
           
            return -1;
        }

        public void ReportActionEnd(int sequenceIndex)
        {
            if (parallelSequences[sequenceIndex] == null)
            {
                EndOfSequence(sequenceIndex);
                return;
            }
            else
                parallelSequences[sequenceIndex].ExecuteNextAction();
        }

        public void ReportActionEnd()
        {
            /*  if (currentSequence == null)
              {
                  EndOfSequence();
                  return;
              }
              ExecuteNextAction();*/

        }
        public void EndAllSequences()
        {
            for (int i = 0; i < parallelSequences.Length; i++)
            {
                parallelSequences[i] = null;
            }
        }

        public void EndAllSequencesBut(int parallelSequenceIndex)
        {
            for (int i = 0; i < parallelSequences.Length; i++)
            {
                if (i != parallelSequenceIndex)
                    parallelSequences[i] = null;
            }
        }

        public void EndOfSequence(int parallelSequenceIndex)
        {
            Debug.Log("End of Sequence -> Empty Seq " + parallelSequenceIndex);
            parallelSequences[parallelSequenceIndex] = null;
        }



        GameObject instantiatedSequenceHolder;
        public void InstantiateAssetSequence(GameObject asset)
        {
            instantiatedSequenceHolder = (GameObject)Instantiate(asset);
            StartNewSequence(instantiatedSequenceHolder.GetComponent<Sequence>(), DestroyAfterExecution);
        }

        void DestroyAfterExecution()
        {
            Destroy(instantiatedSequenceHolder);
        }

        private void OnLevelWasLoaded(int level)
        {
            EndAllSequences();
        }

    }
}