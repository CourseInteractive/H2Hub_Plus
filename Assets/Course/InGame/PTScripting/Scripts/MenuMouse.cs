using UnityEngine;

namespace Course.PrototypeScripting
{
    public class MenuMouse : MonoBehaviour
    {

        public bool activateOnStart = false;
        // Start is called before the first frame update
        void Start()
        {
            if (activateOnStart)
                ActivateMouse();
        }

        public void ActivateMouse()
        {
            Cursor.lockState = CursorLockMode.None;
        }

        public void DeactivateMouse()
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
