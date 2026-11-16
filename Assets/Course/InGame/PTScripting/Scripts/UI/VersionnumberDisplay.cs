using UnityEngine;
using TMPro;

namespace Course.PrototypeScripting
{
    public class VersionnumberDisplay : MonoBehaviour
    {
        public string prefix;
        public TMP_Text textDisplay;
        public string suffix;

        // Start is called before the first frame update
        void Start()
        {
            if (textDisplay == null)
                textDisplay = GetComponent<TMP_Text>();
            textDisplay.text = prefix + Application.version + suffix;

        }

    }
}
