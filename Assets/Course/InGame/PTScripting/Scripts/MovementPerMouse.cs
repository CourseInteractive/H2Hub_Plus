using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Course.PrototypeScripting
{
    public class MovementPerMouse : MonoBehaviour
    {

        public GameObject positionObject;
        [HideInInspector]
        public GameObject hitObject;
        public NavMeshAgent player;

        public bool blockedByUI;

        public InputActionReference clickAction;
        public InputActionReference mousePos;
        private void Awake()
        {
            RuntimeGlobal.mouseMovement = this;
            clickAction.action.Enable();
            mousePos.action.Enable();
        }
        void Update()
        {
            if (RuntimeGlobal.gameState != RuntimeGlobal.GameState.NormalGame)
                return;
            MouseInput();

        }

        public void SetMaxSpeed(float speed)
        {
            player.speed = speed;
        }

        void MouseInput()
        {
            if (blockedByUI)
                return;
            Ray charles = UnityEngine.Camera.main.ScreenPointToRay(mousePos.action.ReadValue<Vector2>());
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(charles, out hitInfo, 10000))
            {
                hitObject = hitInfo.collider.gameObject;
                if(positionObject)
                positionObject.transform.position = hitInfo.point;
           

                if (hitObject.GetComponent<SelectableObject>())
                    RuntimeGlobal.Select(hitObject.GetComponent<SelectableObject>());
                else
                    RuntimeGlobal.ClearSelection();
            }
            else
            {
                hitObject = null;
                RuntimeGlobal.ClearSelection();
            }




            if (clickAction.action.triggered) // UnityEngine.Input.GetMouseButtonUp(0))
            {
                if (InventoryManager.Instance && InventoryManager.Instance.itemInDrag != null)
                    return;

                if (hitObject && RuntimeGlobal.selectedObject != null)
                {
              
                    StartInteraction();
                }
                else
                {
                    player.enabled = true;
                    player.SetDestination(positionObject.transform.position);
                }

            }
        }

        void StartInteraction()
        {
            RuntimeGlobal.InteractWithSelectedObject();
        }

        public void SetBlockByUI(bool value)
        {
            blockedByUI = value;
        }
    }
}
