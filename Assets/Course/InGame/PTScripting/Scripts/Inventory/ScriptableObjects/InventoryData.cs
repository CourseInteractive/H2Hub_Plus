using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    [System.Serializable]
    public class InventoryItem
    {
        public string name;
        public int amount;
        public Sprite image;
        public GameObject sequencePrefab;
    }

    public class InventoryData : ScriptableObject
    {
        public List<InventoryItem> invItems;

        public List<string> GetNames()
        {
            List<string> names = new List<string>();
            foreach (InventoryItem item in invItems)
            {
                names.Add(item.name);
            }
            return names;
        }
    }
}