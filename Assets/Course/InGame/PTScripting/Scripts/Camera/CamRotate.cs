using UnityEngine;

namespace Course.PrototypeScripting
{
    public class CamRotate : CamBase
    {
        public GameObject player;
        Vector3 offset;
        GameObject targetCam;
        GameObject cameraRig;
        public bool rotateCameraItself;
        public bool rotateCharacterWithCamera;
        GameObject camRotateObject;
        public enum State { Follow, MoveToStatic, Static, MoveToFollow }
        public State state;

        //public float speed = 1;
        public float rotateSpeed = 1;
        public float verticalRotateSpeed = 1;
        Quaternion startLocalRotation;
        Quaternion offsetLocalRotation;
        Vector3 normalLocalPosition;
        public bool invertVerticalMouseRotation;

        public float vertRotationLimit_Up = 20;
        public float vertRotationLimit_Down = -20;
        [HideInInspector]
        public Vector2 verticalRotationLimits;
        float verticalAngleStart;

        public void Awake()
        {
            cameraRig = new GameObject();
            cameraRig.name = "CamRig";
            cameraRig.transform.position = player.transform.position;
            cameraRig.transform.rotation = player.transform.rotation;
            CamRig camRigScript = cameraRig.AddComponent<CamRig>();
            camRigScript.Initialize(player);
            transform.parent = cameraRig.transform;
            verticalRotationLimits = new Vector2(vertRotationLimit_Up, vertRotationLimit_Down);

            Cursor.lockState = CursorLockMode.Locked;

            offsetLocalRotation = transform.localRotation;
            normalLocalPosition = transform.localPosition;
            verticalAngleStart = cameraRig.transform.localEulerAngles.x;
        }

        // Update is called once per frame
        void Update()
        {
            switch (state)
            {
                case State.Follow:
                    TrackMouseMovement();
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
        Vector3 lastMousePosition;
        void TrackMouseMovement()
        {
            if (RuntimeGlobal.gameState != RuntimeGlobal.GameState.NormalGame)
                return;
            if (rotateCameraItself)
                camRotateObject = gameObject;
            else
                camRotateObject = cameraRig;
            camRotateObject.transform.Rotate(new Vector3(0, 1, 0), rotateSpeed * Time.deltaTime * UnityEngine.Input.GetAxis("Mouse X"), Space.World);
            float direction = 1f;
            if (invertVerticalMouseRotation)
                direction = -1f;

       

            float newVerticalAngle = verticalRotateSpeed * Time.deltaTime * UnityEngine.Input.GetAxis("Mouse Y") * direction + camRotateObject.transform.localEulerAngles.x;
            if (newVerticalAngle > 180)
                newVerticalAngle -= 360;
            newVerticalAngle = Mathf.Clamp(newVerticalAngle, verticalRotationLimits.x, verticalRotationLimits.y);
            camRotateObject.transform.localEulerAngles = new Vector3(newVerticalAngle, camRotateObject.transform.localEulerAngles.y, camRotateObject.transform.localEulerAngles.z);
            if (rotateCharacterWithCamera)
                SetPlayerookAtInCamDirection();
        }

        void SetPlayerookAtInCamDirection()
        {
            Vector3 direction = transform.forward;
            direction.y = 0;
            player.transform.forward = direction;

        }

        public void SetYRotationTarget(Transform obj)
        {
            Vector3 euler = offsetLocalRotation.eulerAngles;
            euler.y = obj.localEulerAngles.y;
            offsetLocalRotation = Quaternion.Euler(euler);
        }

        void SaveRotationOffset()
        {
            offsetLocalRotation = transform.localRotation;
        }

        void MoveToStatic()
        {
            
            switchTimer += Time.deltaTime;
            if (switchTimer > switchTime)
            {
                switchTimer = switchTime;
                state = State.Static;
            }
            transform.position = Vector3.Lerp(startPosition, targetCam.transform.position, switchTimer / switchTime);
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
            transform.localPosition = Vector3.Lerp(startPosition, normalLocalPosition, switchTimer / switchTime);
            transform.localRotation = Quaternion.Lerp(startLocalRotation, offsetLocalRotation, switchTimer / switchTime);
        }

        public override void SwitchToStatic(GameObject _targetCam, float time)
        {
            SaveRotationOffset();
            state = State.MoveToStatic;
            startPosition = transform.position;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
            targetCam = _targetCam;

        }

        public override void SwitchToStaticInstant(GameObject _targetCam)
        {
            SaveRotationOffset();
            targetCam = _targetCam;
            transform.position = targetCam.transform.position;
            transform.forward = targetCam.transform.forward;
            state = State.Static;
        }

        public void SwitchToFollow(float time = 0)
        {
            if (time <= 0)
            {
                state = State.Follow;
                return;
            }

            state = State.MoveToFollow;
            startPosition = transform.localPosition;
            startLocalRotation = transform.localRotation;
            startForward = transform.forward;
            switchTimer = 0;
            switchTime = time;
        }
    }
}
