using UnityEngine;

namespace Course.PrototypeScripting
{
    public class TextPanel : MonoBehaviour
    {
        public UnityEngine.UI.Text nameTextUI;
        public UnityEngine.UI.Text textUI;

        public GameObject speakerObject;


        public static TextPanel _instance;
        public static TextPanel Instance
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

        public void SetText(string name, string text, GameObject objToFollow = null)
        {
            
            SetText(name, text);
            speakerObject = objToFollow;
        }

        public void SetText(string name, string text)
        {
            speakerObject = null;
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

        public void Update()
        {
            if(speakerObject)
            {
                GetComponent<RectTransform>().anchoredPosition = Camera.main.WorldToScreenPoint(speakerObject.transform.position);
            }
        }
    }
}
