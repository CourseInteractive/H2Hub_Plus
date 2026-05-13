using UnityEngine;

namespace Course.PrototypeScripting
{
    public class TextPanel_alternative : MonoBehaviour
    {
        public UnityEngine.UI.Text nameTextUI;
        public UnityEngine.UI.Text textUI;

        public static TextPanel_alternative _instance;
        public static TextPanel_alternative Instance
        {
            get
            {
                if (_instance == null)
                    throw new System.Exception("No TextPanel in the scene. Import the prefab SubtitleCanvas from the project folder.");
                return _instance;
            }
            private set { _instance = value; }
        }
        // Start is called before the first frame update
        void Awake()
        {
            if (_instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            Hide();
        }

        public void SetText(string name, string text)
        {
            nameTextUI.text = name;
            textUI.text = text;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            nameTextUI.text = "";
            textUI.text = "";
            gameObject.SetActive(false);
        }
    }
}