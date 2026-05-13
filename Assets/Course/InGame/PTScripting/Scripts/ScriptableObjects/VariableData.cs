using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    [System.Serializable]
    public class GenericVariable
    {
        public string name;
        public int value;
    }
    [System.Serializable]
    public class SceneLocalData
    {
        public List<GenericVariable> data;

        public SceneLocalData()
        {
            data = new List<GenericVariable>();
        }

        public SceneLocalData GetCopy()
        {
            SceneLocalData newData = new SceneLocalData();
            List<GenericVariable> copiedList = new List<GenericVariable>();
            foreach (GenericVariable variable in data)
            {
                GenericVariable genV = new GenericVariable();
                genV.name = variable.name;
                genV.value = variable.value;
                copiedList.Add(genV);
            }
            newData.data = copiedList;
            return newData;
        }
    }

    [CreateAssetMenu(fileName = "InventoryData", menuName = "COURSE/PT Scripting/Variable Data", order = 1)]
    public class VariableData : ScriptableObject
    {

        [SerializeField]
        public List<GenericVariable> variableInfos;

        [SerializeField]
        public List<string> sceneNameKeys;
        //[SerializeField]
        //public List<List<GenericVariable>> localVarLists;
        [SerializeField]
        public List<SceneLocalData> localVarData;

        //public SerializableDictionary<string, List<GenericVariable>> localVars;

        public List<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach(GenericVariable genVar in variableInfos)
            {
                names.Add(genVar.name);
            }
            return names;
        }

        public int GetSceneIndex(string name)
        {
            return sceneNameKeys.LastIndexOf(name);
        }

        public List<string> GetLocalNames(string sceneName)
        {

            List<GenericVariable> local = localVarData[GetSceneIndex(sceneName)].data;
            if(local == null || local.Count == 0)
                    return null;

            List<string> names = new List<string>();
            foreach (GenericVariable genVar in local)
            {
                names.Add(genVar.name);
            }
            return names;
        }

        public List<GenericVariable> GetLocalVars(string sceneName)
        {
            CheckInitialization();
            int sceneIndex = GetSceneIndex(sceneName);
            if (sceneIndex == -1)
                return AddLocalVarStack(sceneName);
            return localVarData[sceneIndex].data;
        }

        void CheckInitialization()
        {
            if (localVarData == null)
            {
                localVarData = new List<SceneLocalData>();
                sceneNameKeys = new List<string>();
            }
                
        }

        public List<GenericVariable> AddLocalVarStack(string sceneName)
        {
            CheckInitialization();
            int sceneIndex = GetSceneIndex(sceneName);
            if (sceneIndex != -1)
                return localVarData[GetSceneIndex(sceneName)].data;

            sceneNameKeys.Add(sceneName);
            SceneLocalData newList = new SceneLocalData();
            localVarData.Add(newList);
            return newList.data;
        }

        public void AddLocalVariable(string sceneName, GenericVariable variable)
        {
            int sceneIndex = GetSceneIndex(sceneName);
            localVarData[sceneIndex].data.Add(variable);
        }
    }
}