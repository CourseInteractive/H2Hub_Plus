using UnityEngine;

namespace Course.PrototypeScripting
{
    public class CamBase : MonoBehaviour
    {
        [HideInInspector]
        public Vector3 startPosition;
        [HideInInspector]
        public Vector3 startForward;
        [HideInInspector]
        public float switchTime;
        [HideInInspector]
        public float switchTimer;

        public virtual void SwitchToStatic(GameObject _targetCam, float time)
        {


        }

        public virtual void SwitchToStaticInstant(GameObject _targetCam)
        {


        }



    }
}
