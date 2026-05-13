using UnityEngine;
using UnityEngine.AI;

namespace Course.PrototypeScripting
{
    public class MovementIndirect : MonoBehaviour
    {
        NavMeshAgent navAgent;
        public enum FollowAction { None, Moving, Looking}
        public FollowAction currentFollowAction;
        [HideInInspector]
        public GameObject objectToFollowPermanent;
    
        // Start is called before the first frame update
        void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
            navAgent.enabled = false;


        }

        public void MoveTo(GameObject target)
        {
            Debug.Log("MOVE");
            if (navAgent == null)
                return;
            navAgent.enabled = true;
            navAgent.SetDestination(target.transform.position);
        }

        void LookAt(GameObject target)
        {
            Vector3 direction = target.transform.position - transform.position;
            direction.y = 0;
            transform.forward = direction;
        }

        private void Update()
        {
            switch(currentFollowAction)
            {
                case FollowAction.Moving:
                    MoveTo(objectToFollowPermanent);
                    break;
                case FollowAction.Looking:
                    LookAt(objectToFollowPermanent);
                    break;
            }
        
            
        }

        public void KeepLookAtObject(GameObject objToFollow)
        {
            objectToFollowPermanent = objToFollow;
            currentFollowAction = FollowAction.Looking;
        }

        public void FollowObject(GameObject objToFollow)
        {
            objectToFollowPermanent = objToFollow;
            currentFollowAction = FollowAction.Moving;
        }

        public void StopFollow()
        {
            currentFollowAction = FollowAction.None;
            objectToFollowPermanent = null;
            if(navAgent && !navAgent.isStopped)
                navAgent.isStopped = true;
        }

        public void StopLooking()
        {
            currentFollowAction = FollowAction.None;
            objectToFollowPermanent = null;
        }

    }
}
