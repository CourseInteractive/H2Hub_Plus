using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Course.PrototypeScripting
{
    public class VariableManager : MonoBehaviour
    {
        public delegate void VariableChangeReaction();
        public event VariableChangeReaction OnVariableChanged;
        public event VariableChangeReaction OnTimerChanged;

        public static List<GenericVariable> currentLocalVars;

        public static VariableManager _instance;
        public static VariableManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No VariableManager in the scene. Import the prefab SimpleGameEngine from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }
        public VariableData variableSavedData;

        void Awake()
        {
            if (_instance == null)
            {
                Instance = this;
                LoadData();
                listOfTimers = new List<Timer>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadData()
        {
            VariableData loadedData = Resources.Load<VariableData>("VariableData");
            if (loadedData == null)
                return;
            variableSavedData = new VariableData();
            variableSavedData.variableInfos = new List<GenericVariable>();
            string[] array = new string[loadedData.sceneNameKeys.Count];
            loadedData.sceneNameKeys.CopyTo(array);
            variableSavedData.sceneNameKeys = array.ToList();
            variableSavedData.localVarData = new List<SceneLocalData>();
            foreach (SceneLocalData data in loadedData.localVarData)
            {
                variableSavedData.localVarData.Add(data.GetCopy());
            }

            foreach (GenericVariable variable in loadedData.variableInfos)
            {
                GenericVariable genV = new GenericVariable();
                genV.name = variable.name;
                genV.value = variable.value;
                variableSavedData.variableInfos.Add(genV);
            }
            
        }

        public void OnLevelWasLoaded(int level)
        {
            OnVariableChanged = null;
            LoadLocalVars();
        }

        void LoadLocalVars()
        {
            if (_instance != null && _instance != this)
                return;
                currentLocalVars = variableSavedData.GetLocalVars(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }


        public void SetVariable(string name, int value)
        {
            foreach (GenericVariable genVar in variableSavedData.variableInfos)
            {
                if (genVar.name == name)
                    genVar.value = value;
            }
            if (OnVariableChanged != null)
                OnVariableChanged.Invoke();
        }

        public void SetLocalVariable(string name, int value)
        {
            foreach (GenericVariable genVar in variableSavedData.localVarData[variableSavedData.GetSceneIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)].data)
            {
                if (genVar.name == name)
                    genVar.value = value;
            }
            if (OnVariableChanged != null)
                OnVariableChanged.Invoke();
        }

        public void ExecuteVariableChangedEvent()
        {
            if (OnVariableChanged != null)
                OnVariableChanged.Invoke();
        }

        public int GetVariable(string name)
        {
            if (variableSavedData == null)
            {
                Debug.LogError("No Variable Infos set - Open 'Variable Editor' in 'Simple Game' in the menu.");
                return 0;
            }
           
            foreach(GenericVariable genVar in variableSavedData.variableInfos)
            {
                if (genVar.name == name)
                    return genVar.value;
            }
            return 0;
        }

        public int GetLocalVariable(string name)
        {
            if (variableSavedData == null)
            {
                Debug.LogError("No Variable Infos set - Open 'Variable Editor' in 'Simple Game' in the menu.");
                return 0;
            }
            int sceneIndex = variableSavedData.GetSceneIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            foreach (GenericVariable genVar in variableSavedData.localVarData[sceneIndex].data)
            {
                if (genVar.name == name)
                    return genVar.value;
            }
            return 0;
        }

        private void Update()
        {
            int timerToRemove = -1;
            if(listOfTimers.Count > 0)
            {
                for(int i = 0; i < listOfTimers.Count; i++)
                {
                    if (listOfTimers[i].ended)
                        timerToRemove = i;
                    else
                        listOfTimers[i].Countdown(Time.deltaTime);
                }
            }
            if (timerToRemove > -1)
            {
                listOfTimers.RemoveAt(timerToRemove);
                if (OnTimerChanged != null)
                    OnTimerChanged.Invoke();
            }
        }

        public void StartTimer(string name, float time, Sequence sequenceOnEnd)
        {
            if (listOfTimers == null)
                listOfTimers = new List<Timer>();
            foreach (Timer t in listOfTimers)
            {
                if (t.name == name)
                {
                    Debug.LogError("Timer with name " + name + " already exists. Stop it before starting a new one.");
                    return;
                }
            }
            Timer newTimer = new Timer(name, time, sequenceOnEnd);
            listOfTimers.Add(newTimer);
            if (OnTimerChanged != null)
                OnTimerChanged.Invoke();
        }

        public void StopTimer(string name)
        {
            int timerToRemove = -1;
            for(int i = 0; i < listOfTimers.Count; i++)
            {
                if (listOfTimers[i].name == name.Trim())
                    timerToRemove = i;
            }
            if(timerToRemove == -1)
                Debug.LogError("No timer with name " + name + " exists.");
            else
            {
                listOfTimers.RemoveAt(timerToRemove);
                if (OnTimerChanged != null)
                    OnTimerChanged.Invoke();
            }
            

        }

        public void TakeTimeOfTimer(string name, float time)
        {
            for (int i = 0; i < listOfTimers.Count; i++)
            {
                if (listOfTimers[i].name == name)
                {
                    listOfTimers[i].Countdown(time);
                    return;
                }

            }
        }

        public bool TimerActive(string name)
        {
            foreach(Timer timer in listOfTimers)
            {
                if (timer.name.Trim() == name.Trim())
                    return true;
            }
            return false;
        }

        public Timer GetTimer(string name)
        {
            foreach (Timer timer in listOfTimers)
            {
                if (timer.name.Trim() == name.Trim())
                    return timer;
            }
            return null;
        }

        public List<Timer> listOfTimers;

        [System.Serializable]
        public class Timer
        {
            public string name;
            public float remainingTime;
            public float startTime;
            public Sequence sequenceWhenZero;
            public bool ended = false;

            public Timer(string _name, float _time, Sequence seq)
            {
                name = _name.Trim();
                startTime = _time;
                remainingTime = _time;
                sequenceWhenZero = seq;
            }

            public void Countdown(float timePerFrame)
            {
                if (ended)
                    return;
                remainingTime -= timePerFrame;
                if(remainingTime <= 0)
                {
                    ended = true;
                    SequenceHandler.Instance.StartNewSequence(sequenceWhenZero);
                }
            }

            public float GetPercent()
            {
                return remainingTime / startTime;
            }
        }

    }
}
