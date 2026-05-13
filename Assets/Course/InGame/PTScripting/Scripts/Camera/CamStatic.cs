using UnityEngine;

namespace Course.PrototypeScripting
{
    public class CamStatic : CamBase
    {
        public enum FctOnStart {  Static, RotateToFollow }
        public FctOnStart BehaviourOnStart;

        public enum State { Static, Moving, Follow }
        State state;
        bool moveTargetStateIsFollow;

        public GameObject followObject;

        Vector3 targetPosition;
        Vector3 targetForward;

        private void Awake()
        {
            if (BehaviourOnStart == FctOnStart.RotateToFollow)
                state = State.Follow;
            else
                state = State.Static;
        }

        void Update()
        {
            if(state == State.Moving)
                MoveTo();
            if (state == State.Follow)
                FollowRotate();

        }

        public void ChangeBehaviourToStatic()
        {
            Debug.Log("k");
            followObject = null;
            state = State.Static;
        }

        public void ChangeBehaviourToFollow(GameObject objToFollow)
        {
            Debug.Log("m");
            followObject = objToFollow;
            state = State.Follow;
        }

        void FollowRotate()
        {
            Vector3 direction = (followObject.transform.position - transform.position).normalized;
            transform.forward = direction;
        }

        public void SwitchToFollow(GameObject _targetCam, float time)
        {

            if (time <= 0)
            {
                SwitchToFollowInstant(_targetCam);
                return;
            }

            moveTargetStateIsFollow = true;
            startPosition = transform.position;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
            targetPosition = _targetCam.transform.position;
            targetForward = (followObject.transform.position - _targetCam.transform.position).normalized;
            state = State.Moving;
        }

        public override void SwitchToStatic(GameObject _targetCam, float time)
        {
            if (time <= 0)
            {
                SwitchToStaticInstant(_targetCam);
                return;
            }

            moveTargetStateIsFollow = false;
            startPosition = transform.position;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
            targetPosition = _targetCam.transform.position;
            targetForward = _targetCam.transform.forward;
            state = State.Moving;
        }

        public override void SwitchToStaticInstant(GameObject _targetCam)
        {
            moveTargetStateIsFollow = false;
            transform.position = _targetCam.transform.position;
            transform.forward = _targetCam.transform.forward;
            state = State.Static;
        }

        public void SwitchToFollowInstant(GameObject _targetCam)
        {
            moveTargetStateIsFollow = true;
            transform.position = _targetCam.transform.position;
            transform.forward = _targetCam.transform.forward;
            state = State.Follow;
        }

        void MoveTo()
        {
            switchTimer += Time.deltaTime;
            if (switchTimer > switchTime)
            {
                switchTimer = switchTime;
                if(moveTargetStateIsFollow)
                    state = State.Follow;
                else
                    state = State.Static;
            }
            transform.position = Vector3.Lerp(startPosition, targetPosition, switchTimer / switchTime);
            transform.forward = Vector3.Lerp(startForward, targetForward, switchTimer / switchTime);
        }

    }
}
