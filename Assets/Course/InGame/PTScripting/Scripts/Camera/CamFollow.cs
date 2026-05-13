using UnityEngine;

namespace Course.PrototypeScripting
{
    public class CamFollow : CamBase
    {
        public GameObject player;
        Vector3 offset;
        GameObject targetCam;

        public float speed = 1;

        public enum State {  Follow, MoveToStatic, Static, MoveToFollow}
        public State state;

        Vector3 followForward;

        public void Awake()
        {
            offset = player.transform.position - transform.position;
            followForward = transform.forward;
        }

        // Update is called once per frame
        void Update()
        {
            switch(state)
            {
                case State.Follow:
                    Vector3 target = player.transform.position - offset;
                    transform.position = Vector3.Lerp(transform.position, target, speed);
                    break;
                case State.MoveToStatic:
                    MoveToStatic();
                    break;
                case State.MoveToFollow:
                    MoveToFollow();
                    break;
                case State.Static:
                    break;
            }

        }

        void MoveToStatic() 
        {
            switchTimer += Time.deltaTime;
            if (switchTimer > switchTime)
            {
                switchTimer = switchTime;
                state = State.Static;
            }
            transform.position = Vector3.Lerp(startPosition, targetCam.transform.position, switchTimer/switchTime);
            transform.forward = Vector3.Lerp(startForward, targetCam.transform.forward, switchTimer / switchTime);
        }

        void MoveToFollow()
        {
            switchTimer += Time.deltaTime;
            if (switchTimer > switchTime)
            {
                switchTimer = switchTime;
                state = State.Follow;
            }
            transform.position = Vector3.Lerp(startPosition, player.transform.position - offset, switchTimer / switchTime);
            transform.forward = Vector3.Lerp(startForward, followForward, switchTimer / switchTime);
        }

        public override void SwitchToStatic(GameObject _targetCam, float time)
        {
            state = State.MoveToStatic;
            startPosition = transform.position;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
            targetCam = _targetCam;
        
        }

        public override void SwitchToStaticInstant(GameObject _targetCam)
        {
            targetCam = _targetCam;
            transform.position = targetCam.transform.position;
            transform.forward = targetCam.transform.forward;
            state = State.Static;
        }

        public void SwitchToFollow(float time = 0)
        {
            if(time <= 0)
            {
                state = State.Follow;
                return;
            }

            state = State.MoveToFollow;
            startPosition = transform.position;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
        }

    }
}
