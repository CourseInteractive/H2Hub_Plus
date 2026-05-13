using UnityEngine;
using UnityEngine.AI;

namespace Course.PrototypeScripting
{
    public class SpeedToAnimator : MonoBehaviour
    {
        public Rigidbody character;
        public string parameterName = "Speed";
        public float internSpeed;
        public Animator animator;

        private void Awake()
        {
            if(animator == null)
                animator = character.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            internSpeed = character.linearVelocity.magnitude;
            if (GetComponent<NavMeshAgent>())
                internSpeed = GetComponent<NavMeshAgent>().velocity.magnitude;
            if (animator)
                animator.SetFloat(parameterName, internSpeed);
        }
    }
}
