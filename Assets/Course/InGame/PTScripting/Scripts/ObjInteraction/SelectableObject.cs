using UnityEngine;

namespace Course.PrototypeScripting
{
    public class SelectableObject : MonoBehaviour
    {
        public string title;
        public Sequence sequenceOnInteraction;
        [HideInInspector]
        public bool selected = false;
        public int selectionHighlightUI_Index = 0;

        public Vector3 highlightUI_offset;

        public string GetTitle()
        {
            if (title.Trim() == "")
                return gameObject.name;
            else
                return title;
        }

        public void InteractWith()
        {
            if (sequenceOnInteraction == null)
                Debug.LogError(gameObject.name + " has no sequence on interaction set!");
            else
            {
                if(InventoryManager.Instance != null)
                {
                    if (InventoryManager.Instance.itemInDrag == null && RuntimeGlobal.interactionBlockByInvCombination == false)
                        SequenceHandler.Instance.StartNewSequence(sequenceOnInteraction);
                }
                else
                    SequenceHandler.Instance.StartNewSequence(sequenceOnInteraction);
            }
        }

        public void Select()
        {
            selected = true;
            RuntimeGlobal.selectedObject = this;
        }
        public void Deselect()
        {
            selected = false;
            RuntimeGlobal.selectedObject = null;
        }
    }
}
