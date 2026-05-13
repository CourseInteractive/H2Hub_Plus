using UnityEngine;

namespace Course.PrototypeScripting
{
    public class DebugInfo : MonoBehaviour
    {
        public RuntimeGlobal.GameState gameState;


        // Update is called once per frame
        void Update()
        {
            gameState = RuntimeGlobal.gameState;
        }
    }
}
