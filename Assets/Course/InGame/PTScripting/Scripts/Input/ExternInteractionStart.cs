using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ExternInteractionStart : MonoBehaviour
    {

        public KeyCode mainInteractionKey;

        // Update is called once per frame
        void Update()
        {
            if (RuntimeGlobal.gameState != RuntimeGlobal.GameState.NormalGame)
                return;

            if (UnityEngine.Input.GetKeyUp(mainInteractionKey) && RuntimeGlobal.selectedObject != null)
                StartInteraction();
        }

        void StartInteraction()
        {
            RuntimeGlobal.InteractWithSelectedObject();
        }
    }

  
}
