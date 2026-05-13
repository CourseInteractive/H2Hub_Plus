using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Course.PrototypeScripting
{
    public class InventoryUI_Item : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
    {
        InventoryUI mainUI;
        public Image ui_image;
        public Text ui_amount;
    
    
        public bool draggable = true;
        public bool clickableForExamination;
        [HideInInspector]
        public InventoryItem item;

        public enum DragState { None, Down, Drag }
        public DragState dragState;

        public void Initialize(InventoryItem _item, InventoryUI _main)
        {
            item = _item;
            mainUI = _main;
            ui_image.sprite = item.image;
            if (ui_amount)
                ui_amount.text = item.amount.ToString();
        }

        void Update()
        {
            if(dragState == DragState.Drag)
            {
                transform.position = UnityEngine.Input.mousePosition;
            }
        }


        public void OnPointerDown(PointerEventData eventData)
        {   
            if(draggable)
                dragState = DragState.Down;
            else if (clickableForExamination)
            {
                SequenceHandler.Instance.InstantiateAssetSequence(item.sequencePrefab);
            }
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //if (RuntimeGlobal.mouseMovement)
            //    RuntimeGlobal.mouseMovement.SetBlockByUI(false);
            if (dragState != DragState.Down || !draggable)
                return;
            dragState = DragState.Drag;
            GetComponent<Image>().raycastTarget = false;
            InventoryManager.Instance.itemInDrag = this;
            mainUI.DeactivateScrollingTemporarily();
            transform.parent = mainUI.transform;
        
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //if (RuntimeGlobal.mouseMovement)
           //     RuntimeGlobal.mouseMovement.SetBlockByUI(true);
        }

        public void Reset()
        {
            //GetComponent<Image>().raycastTarget = true;
        }
    }
}
