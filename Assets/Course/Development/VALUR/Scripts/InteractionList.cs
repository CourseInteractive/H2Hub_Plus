using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VALUR
{
    public delegate void InteractionExecution(int index = 0);

    public class Interaction
    {
        public string name;
        public int value;
        public event InteractionExecution OnExecuted;

        public void Execute()
        {
            OnExecuted.Invoke(value);
        }

    }


    public class InteractionList : MonoBehaviour
    {
        public Transform listParent;
        public GameObject buttonPrefab;
        public List<Interaction> interactions;
        public List<GameObject> buttons;

        // Start is called before the first frame update
        void Start()
        {
            buttonPrefab.SetActive(false);
            buttons = new List<GameObject>();
            interactions = new List<Interaction>();
            gameObject.SetActive(false);
        }

        private void OnLevelWasLoaded(int level)
        {
            Close();
        }
        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void Reset()
        {
            foreach(GameObject obj in buttons)
            {
                Destroy(obj);
            }
            interactions = new List<Interaction>();
            buttons = new List<GameObject>();
        }

        public void AddInteraction(Interaction action)
        {
            GameObject button = CreateButton();
            button.GetComponent<InteractionList_Button>().Init(interactions.Count, this, action.name);
            interactions.Add(action);
            buttons.Add(button);
            gameObject.SetActive(true);
        }

        GameObject CreateButton()
        {
            GameObject obj = (GameObject)Instantiate(buttonPrefab, listParent);
            return obj;
        }

        public void ReportClick(int index)
        {
            Interaction action = interactions[index];
            Reset();
            gameObject.SetActive(false);
            action.Execute();
        }
    }
}


