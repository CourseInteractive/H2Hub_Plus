using UnityEngine;

namespace Course.PrototypeScripting
{
    public class VersionnumberDisplay : MonoBehaviour
    {
        public string prefix;
        public UnityEngine.UI.Text textDisplay;
        public string suffix;

        // Start is called before the first frame update
        void Start()
        {
            if (textDisplay == null)
                textDisplay = GetComponent<UnityEngine.UI.Text>();
            textDisplay.text = prefix + Application.version + suffix;

        }

    }
}
