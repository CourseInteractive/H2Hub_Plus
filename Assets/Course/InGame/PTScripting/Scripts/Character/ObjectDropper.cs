using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    [System.Serializable]
    public class ObjectType
    {
        public GameObject referenceObject;
        public string variableName;
    }

    public class ObjectDropper : MonoBehaviour
    {
        public GameObject dropPoint;
        public ObjectType[] objectsTypes;
        public bool limitedAmount;

        bool ObjectsLeft(int index)
        {
            string variableName = objectsTypes[index].variableName;
            return VariableManager.Instance.GetVariable(variableName) > 0;
        }

        void ReduceObjectAmount(int index)
        {
            string variableName = objectsTypes[index].variableName;
            VariableManager.Instance.SetVariable(variableName, VariableManager.Instance.GetVariable(variableName) - 1);
        }

        public void CreateAndDropObject(int index)
        {
            if (limitedAmount && !ObjectsLeft(index))
                return;

            GameObject dropObject = Instantiate(objectsTypes[index].referenceObject);
            dropObject.transform.position = dropPoint.transform.position;
            dropObject.transform.parent = null;
            dropObject.SetActive(true);
            if (limitedAmount)
                ReduceObjectAmount(index);

        }
    }
}