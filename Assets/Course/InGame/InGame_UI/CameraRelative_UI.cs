using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRelative_UI : MonoBehaviour
{
    public Camera camera;
    public float distance = 10;
    public float angle = 0;
    public float height = 0;
    public float delay = 3f;
    float delayCounter;

    public float movementThresholdAngle = 5;

    Vector3 targetPosition;
    public float moveSpeed = 1f;

    bool moving;
    bool needsMoving;
    // Start is called before the first frame update
    void Start()
    {
       
        HardPositioning();
    }

    // Update is called once per frame
    void Update()
    {
        Positioning();
    }

    private void OnEnable()
    {
        HardPositioning();
    }

    void HardPositioning()
    {
        if (camera == null)
            camera = Camera.main;
        GetOffsetAngle(true);
        transform.position = Vector3.Lerp(transform.position, targetPosition, 1f) + Vector3.up * height;
        transform.forward = transform.position - camera.transform.position;
    }

    void Positioning()
    {
        GetOffsetAngle();

        if (moving)
        { HandleMovement(); }
    }

    void HandleMovement()
    {
        
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed / 100f) + Vector3.up * height;

        transform.forward = transform.position - camera.transform.position;
        if (Vector3.Distance(transform.position, targetPosition) < 1f)
        {
            moving = false;
        }
    }

    public void GetOffsetAngle(bool force = false)
    {

        Vector3 directionToObject = transform.position - camera.transform.position;
        float angleToCamDirection = Vector3.SignedAngle(directionToObject, camera.transform.forward, camera.transform.up);
       // Debug.Log(angleToCamDirection - angle + "   " + angleToCamDirection + angle);
        if (angleToCamDirection - angle > movementThresholdAngle || angleToCamDirection + angle < -movementThresholdAngle || force)
       // if (Mathf.Abs(angleToCamDirection - angle) > movementThresholdAngle) // || angleToCamDirection + angle < -movementThresholdAngle)
        {
            
            Quaternion myRotation = Quaternion.AngleAxis(angle, camera.transform.up);
            Vector3 startingDirection = camera.transform.forward;
            startingDirection.y = 0;
            Vector3 result = myRotation * startingDirection;
            needsMoving = true;
            delayCounter += Time.deltaTime;
            Vector3 camOnGround = new Vector3(camera.transform.position.x, 0, camera.transform.position.z);
            targetPosition = camOnGround + (result.normalized * distance) + Vector3.up * height;
            if (delayCounter > delay || moving)
                moving = true;
           // transform.position = camera.transform.position + (result.normalized * distance);
           // transform.forward = transform.position  - camera.transform.position;
        }
        else
        {
            needsMoving = false;
            delayCounter = 0;
        }
    }
}
