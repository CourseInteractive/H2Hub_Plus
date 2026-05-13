using UnityEngine;

namespace Course.PrototypeScripting
{
    public static class RuntimeGlobal
    {

        public enum GameState { NormalGame, Cutscene, Conversation }
        public static GameState gameState;

        public static SelectableObject selectedObject;
        public static InteractionHighlight openSelectionHighlight;

        public static InteractionHighlight[] selectionHighlights;

        //public static MovementPerKeyboard keyboardMovement;
        public static MovementPerMouse mouseMovement;

        public static bool interactionBlockByInvCombination;

        public static void Select(SelectableObject obj)
        {
            ClearSelection();
            selectedObject = obj;
            obj.Select();
            SetSelectionHighlightUI(obj);
        
        }

        static void SetSelectionHighlightUI(SelectableObject obj)
        {
            HideOpenSelectionHighlightUI();
            openSelectionHighlight = selectionHighlights[obj.selectionHighlightUI_Index];
            openSelectionHighlight.Set(obj);
        }

        static void HideOpenSelectionHighlightUI()
        {
            if (openSelectionHighlight)
            {
                openSelectionHighlight.Hide();
                openSelectionHighlight = null;
            }
        
        }

        public static void ClearSelection()
        {
            if(selectedObject)
            {
                selectedObject.Deselect();
                selectedObject = null;
            }
            HideOpenSelectionHighlightUI();
        }

        public static void InteractWithSelectedObject()
        {
            SelectableObject lastObj = selectedObject;
            ClearSelection();
            lastObj.InteractWith();
        }

        public static void SwitchGameState(GameState newState)
        {
            if (newState == gameState)
                return;
            gameState = newState;
            switch(gameState)
            {
                case GameState.NormalGame:
                    if(UnityEngine.Camera.main.GetComponent<CamRotate>())
                        Cursor.lockState = CursorLockMode.Locked;
                    else
                        Cursor.lockState = CursorLockMode.None;
                    break;
                case GameState.Cutscene:
                    Cursor.lockState = CursorLockMode.Locked;
                    break;
                case GameState.Conversation:
                    Cursor.lockState = CursorLockMode.None;
                    break;
            }
        }

        public static void RegisterInteractionHighlightUI(InteractionHighlight ui, int index)
        {
            if (selectionHighlights == null || index >= selectionHighlights.Length)
                ExpandInteractionHighlighUIArray(index + 1);
            if (selectionHighlights[index] != null)
                Debug.LogWarning("Ein InteractionHighlightUI mit dem Index " + index + " ist bereits vorhanden und wurde jetzt überschrieben.");
            selectionHighlights[index] = ui;
        }

        static void ExpandInteractionHighlighUIArray(int newSize)
        {
            InteractionHighlight[] newArray = new InteractionHighlight[newSize];
            if(selectionHighlights != null && selectionHighlights.Length > 0)
            {
                for (int i = 0; i < selectionHighlights.Length; i++)
                    newArray[i] = selectionHighlights[i];
            }
            selectionHighlights = newArray;
        }
    }
}
