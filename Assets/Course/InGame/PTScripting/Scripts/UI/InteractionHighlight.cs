using UnityEngine;

namespace Course.PrototypeScripting
{
    public class InteractionHighlight : MonoBehaviour
    {
        public UnityEngine.UI.Text textUI;
        public int selectionHighlightUI_Index = 0;
        public bool updateDirection;

        // Start is called before the first frame update
        void Start()
        {
            CheckMainCamera();
            RuntimeGlobal.RegisterInteractionHighlightUI(this, selectionHighlightUI_Index);
            Hide();
        }

        public void Set(SelectableObject obj)
        {
            CheckMainCamera();
            transform.position = obj.transform.position + obj.highlightUI_offset;
            Vector3 direction = transform.position - UnityEngine.Camera.main.transform.position;
            transform.forward = new Vector3(direction.x, 0, direction.z);
            textUI.text = obj.GetTitle();
            gameObject.SetActive(true);
        }

        void Update()
        {
            if(updateDirection)
            {
                Vector3 direction = transform.position - UnityEngine.Camera.main.transform.position;
                transform.forward = new Vector3(direction.x, 0, direction.z);
            }
        }

        void CheckMainCamera()
        {
            if (UnityEngine.Camera.main == null)
                Debug.LogError("Keine MainCamera gefunden. Setze den Tag 'MainCamera' für das Objekt deiner Hauptkamera.");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
