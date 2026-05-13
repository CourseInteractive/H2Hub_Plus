using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class InventoryManager : MonoBehaviour
    {
        public delegate void InventoryChange();
        public event InventoryChange OnInventoryChange;

        public InventoryUI_Item itemInDrag;

        public static InventoryManager _instance;
        public static InventoryManager Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No InventoryManager in the scene. Import the prefab InventoryManager from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }

        public InventoryData inventorySavedData;

        void Awake()
        {
            if (_instance == null)
            {
                Instance = this;
                LoadData();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadData()
        {
            InventoryData loadedData = Resources.Load<InventoryData>("InventoryData");
            if (loadedData == null)
                return;
            inventorySavedData = new InventoryData();
            inventorySavedData.invItems = new List<InventoryItem>();
            foreach (InventoryItem item in loadedData.invItems)
            {
                InventoryItem newItem = new InventoryItem();
                newItem.name = item.name;
                newItem.amount = item.amount;
                newItem.image = item.image;
                newItem.sequencePrefab = item.sequencePrefab;
                inventorySavedData.invItems.Add(newItem);
            }

        }

        public void AddInvItem(string name, int amount = 1)
        {
            ChangeInvItemAmount(name, amount);
        }

        public void SubInvItem(string name, int amount = 1)
        {
            ChangeInvItemAmount(name, -amount);
        }

        public void SetExplicitInvItem(string name, int amount = 1)
        {
            foreach (InventoryItem item in inventorySavedData.invItems)
            {
                if (item.name == name)
                    item.amount = Mathf.Clamp(amount, 0, 99999); ;
            }
            if(OnInventoryChange != null)
                OnInventoryChange.Invoke();
        }

        public void ChangeInvItemAmount(string name, int amount)
        {
            foreach (InventoryItem item in inventorySavedData.invItems)
            {
                if (item.name == name)
                {
                    item.amount = Mathf.Clamp(item.amount + amount, 0, 99999);
                }
                
            }
            if (OnInventoryChange != null)
                OnInventoryChange.Invoke();
        }

        public List<InventoryItem> GetCurrentItems()
        {
            List<InventoryItem> list = new List<InventoryItem>();
            foreach(InventoryItem item in inventorySavedData.invItems)
            {
                if (item.amount > 0)
                    list.Add(item);
            }
            return list;
        }

        public int GetAmount(string invItemName)
        {
            foreach (InventoryItem item in inventorySavedData.invItems)
            {
                if (item.name == invItemName)
                    return item.amount;
            }
            Debug.LogError("Item with name " + invItemName + " not defined in Editor!");
            return 0;
        }

        private void Update()
        {
            RuntimeGlobal.interactionBlockByInvCombination = false;
            if (resetUIBlockNextFrame)
                ResetUIBlock();
            /*if (UnityEngine.Input.GetMouseButtonUp(0) && itemInDrag != null)
            {
                InventoryCombination combi = InvCombinationIfPossible(itemInDrag.item);
                if (combi != null)
                {
                    Reset();
                    RuntimeGlobal.interactionBlockByInvCombination = true;
                    SequenceHandler.Instance.StartNewSequence(combi.sequenceOnCombination);
                }
                else
                {
                    DestroyImmediate(itemInDrag.gameObject);
                    Reset();
                }
         
            }*/
        }

        InventoryCombination InvCombinationIfPossible(InventoryItem item)
        {
            if (RuntimeGlobal.selectedObject == null)
                return null;
            InventoryCombination[] combis = RuntimeGlobal.selectedObject.GetComponents<InventoryCombination>();
            if (combis == null || combis.Length == 0)
                return null;
            foreach (InventoryCombination c in combis)
            {
                if (c.invItemName == item.name)
                {
                    return c;
                }
                
            }
            return null;
        }

        public void Reset()
        {
            if(itemInDrag)
                itemInDrag.Reset();
            resetUIBlockNextFrame = true;
        
            itemInDrag = null;
            if (OnInventoryChange != null)
                OnInventoryChange.Invoke();
        }
        bool resetUIBlockNextFrame;

        void ResetUIBlock()
        {
            resetUIBlockNextFrame = false;
        //    RuntimeGlobal.mouseMovement.SetBlockByUI(false);
        }
    }
}
