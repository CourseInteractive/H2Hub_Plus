using UnityEngine;

namespace Course.PrototypeScripting
{
    public class NoDestroy : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
