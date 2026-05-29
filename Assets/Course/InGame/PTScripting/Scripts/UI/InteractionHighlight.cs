using UnityEngine;
using TMPro;

namespace Course.PrototypeScripting
{
    public class InteractionHighlight : MonoBehaviour
    {
        public UnityEngine.UI.Text textUI;
        public TMP_Text tmp_textUI;
        public int selectionHighlightUI_Index = 0;
        public bool updateDirection;
        public bool updatePosition;
        SelectableObject targetObj;

        // Start is called before the first frame update
        void Start()
        {
            CheckMainCamera();
            RuntimeGlobal.RegisterInteractionHighlightUI(this, selectionHighlightUI_Index);
            Hide();
        }

        public void Set(SelectableObject obj)
        {
            targetObj = obj;
            CheckMainCamera();
            transform.position = obj.transform.position + obj.highlightUI_offset;

            //Vector3 direction = transform.position - GameSpotManager.instance.activeSpot.spotCamera.transform.position;
            Vector3 direction = transform.position - Camera.main.transform.position;
            transform.forward = new Vector3(direction.x, 0, direction.z);
            if (tmp_textUI)
                tmp_textUI.text = obj.GetTitle();
            else
                textUI.text = obj.GetTitle();
            gameObject.SetActive(true);
        }

        void Update()
        {
            if (updatePosition && targetObj)
                transform.position = targetObj.transform.position + targetObj.highlightUI_offset;
            if (updateDirection)
            {
                Vector3 direction = transform.position - UnityEngine.Camera.main.transform.position;
                transform.forward = new Vector3(direction.x, 0, direction.z);
            }
        }

        void CheckMainCamera()
        {
            //if (UnityEngine.Camera.main == null)
            //    Debug.LogError("Keine MainCamera gefunden. Setze den Tag 'MainCamera' für das Objekt deiner Hauptkamera.");
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            targetObj = null;
        }
    }
}
