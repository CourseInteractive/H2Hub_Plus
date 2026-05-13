using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Course.PrototypeScripting
{
    public class InventoryUI : MonoBehaviour
    {
        public GameObject listPanel;
        public GameObject itemReference;

        List<GameObject> currentItems;
        public ScrollRect scrollBar;

        public enum Direction { None, Horizontal, Vertical}
        public Direction adjustSizeAutomaticallyIn = Direction.Horizontal;

        // Start is called before the first frame update
        void Start()
        {
            itemReference.SetActive(false);
            InventoryManager.Instance.OnInventoryChange += OnInventoryChanged;
            UpdateList();
        }

        void OnInventoryChanged()
        {
            UpdateList();
        }

        void UpdateList()
        {
            DeleteCurrentItems();
            List<InventoryItem> list = InventoryManager.Instance.GetCurrentItems();
            if (list.Count == 0)
                return;
            foreach(InventoryItem item in list)
            {
                GameObject go = CreateItemUI(item);
                currentItems.Add(go);
            }
            if(scrollBar)
                scrollBar.enabled = true;
            AdjustListPanelToItemList();
        }

        private GameObject CreateItemUI(InventoryItem item)
        {
            GameObject go = (GameObject)Instantiate(itemReference);
            InventoryUI_Item _item = go.GetComponent<InventoryUI_Item>();
            _item.Initialize(item, this);
            go.transform.parent = listPanel.transform;
            go.transform.localScale = Vector3.one;
            go.SetActive(true);
            return go;
        }

        void DeleteCurrentItems()
        {
            if(currentItems != null && currentItems.Count > 0)
            {
                foreach (GameObject go in currentItems)
                {
                    DestroyImmediate(go);
                }
            }
            currentItems = new List<GameObject>();

        }

        void AdjustListPanelToItemList()
        {
            if (currentItems == null || currentItems.Count == 0)
                return;
            float width = currentItems[0].GetComponent<RectTransform>().rect.width;
            float height = currentItems[0].GetComponent<RectTransform>().rect.height;
            if (adjustSizeAutomaticallyIn == Direction.Horizontal)
                listPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * currentItems.Count);
            else if (adjustSizeAutomaticallyIn == Direction.Vertical)
                listPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height * currentItems.Count);


        }

        public void DeactivateScrollingTemporarily()
        {
            if (scrollBar)
                scrollBar.enabled = false;
        }
    }
}
