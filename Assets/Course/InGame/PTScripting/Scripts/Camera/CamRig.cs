using UnityEngine;

namespace Course.PrototypeScripting
{
    public class CamRig : MonoBehaviour
    {

        Transform followObject;
        bool followBlocked;

        public void Initialize(GameObject target)
        {
            followObject = target.transform;
        }

        // Update is called once per frame
        void Update()
        {
            if(!followBlocked)
            transform.position = followObject.position;
        }

       /* 
        * Gedacht um den Spieler während einer Cutscene bewegen zu können, ohne dass die Kamera ihm folgt.
        * public void SetFollowBlock(bool value)
        {
            followBlocked = value;
        }*/
    }
}
