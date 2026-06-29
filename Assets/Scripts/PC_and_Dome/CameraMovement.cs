using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class CameraMovement : MonoBehaviour
{
    public float panSpeed;
    public float zoomSpeed;
    public float rotationSpeed;
    Vector3 lastMousePos;
    Camera cam;
    public float panRadius = 1;
    public Vector2 zoomLimit;
    Vector3 camStartPosition;
    float startFOV = 1;

    public float keyboardPower = 1f;

    public InputActionReference leftKey;
    public InputActionReference rightKey;

    [Header("WASD Settings")]
    public bool rotateByKeys = false;

    // Start is called before the first frame update
    void Start()
    {
        leftKey.action.Enable();
        rightKey.action.Enable();
        cam = GetComponent<Camera>();
        camStartPosition = cam.transform.position;
        startFOV = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
       // if (RuntimeGlobal.gameState == RuntimeGlobal.GameState.Conversation)
       //     return;

        HandleMousePan();
        HandleMouseRotation();
      
        HandleKeyboardMovement();
       // if (RuntimeGlobal.gameState != RuntimeGlobal.GameState.NormalGame)
       //     return;
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        HandleZoom();
    }

    void HandleMousePan()
    {
     /*   if (clickAction.action.triggered)
        {
            lastMousePos = Input.mousePosition;
        }

        if (clickAction.action.IsPressed())
        {
            Vector3 deltaMouse = Input.mousePosition - lastMousePos;
            Vector3 direction = transform.forward;
            direction.y = 0;
            Vector3 movement = (-transform.right * deltaMouse.x * panSpeed
                               + -direction * deltaMouse.y * panSpeed) * Time.deltaTime;

            if (PanAllowed(movement))
                transform.position += movement;

            lastMousePos = Input.mousePosition;
        }*/
    }

    void HandleMouseRotation()
    {
      /*  if (clickAction.action.triggered)
        {
            lastMousePos = mousePos.action.ReadValue<Vector2>();
        }

        if (clickAction.action.IsPressed())
        {
            Vector3 rotatePoint = GetPointOnPlane();
            Vector3 deltaMouse = Input.mousePosition - lastMousePos;
            transform.RotateAround(rotatePoint, Vector3.up, deltaMouse.x * rotationSpeed * Time.deltaTime);
            lastMousePos = Input.mousePosition;
        }*/
    }

    void HandleZoom()
    {
        /*    if (CaptainTeague.buildManager && !CaptainTeague.buildManager.previewBuild)
            {

            }*/
        /*float zoom = zoomSpeed * -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime;
        if (ZoomAllowed(zoom))
            cam.fieldOfView += zoom;*/
    }

    void HandleKeyboardMovement()
    {
        Vector3 forward = transform.forward;
        forward.y = 0;
        forward.Normalize();

        // W / S → immer vorwärts/rückwärts bewegen
        float vertical = 0f;
      /*  if (Input.GetKey(KeyCode.W)) vertical = 1f;
        if (Input.GetKey(KeyCode.S)) vertical = -1f;

        if (vertical != 0f)
        {
            Vector3 movement = forward * vertical * panSpeed * keyboardPower * Time.deltaTime;
            if (PanAllowed(movement))
                transform.position += movement;
        }
      */
        // A / D → abhängig von rotateByKeys: drehen oder schieben
        float horizontal = 0f;
        if (leftKey.action.IsPressed()) horizontal = -1f;
        if (rightKey.action.IsPressed()) horizontal = 1f;

        if (horizontal != 0f)
        {
            if (rotateByKeys)
            {
                // Rotation um den Punkt auf der Ebene (wie rechte Maustaste)
                Vector3 rotatePoint = GetPointOnPlane();
                transform.RotateAround(rotatePoint, Vector3.up, horizontal * rotationSpeed * Time.deltaTime * keyboardPower);
            }
            else
            {
                // Seitliches Verschieben (Pan)
                Vector3 movement = transform.right * horizontal * panSpeed * Time.deltaTime * keyboardPower;
                if (PanAllowed(movement))
                    transform.position += movement;
            }
        }
    }

    public bool PanAllowed(Vector3 move)
    {
        Vector3 desiredPos = transform.position + move;
        float currentDistance = Vector3.Distance(transform.position, camStartPosition);
        float desiredDistance = Vector3.Distance(desiredPos, camStartPosition);
        if (desiredDistance < currentDistance)
            return true;
        else
            return desiredDistance < panRadius;
    }

    public bool ZoomAllowed(float zoom)
    {
        float desiredZoom = cam.fieldOfView + zoom;
        return desiredZoom > startFOV + zoomLimit.x && desiredZoom < startFOV + zoomLimit.y;
    }

    public Vector3 GetPointOnPlane()
    {
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        float d = 0;
        Ray charles = new Ray(transform.position, transform.forward);
        plane.Raycast(charles, out d);
        return charles.origin + charles.direction.normalized * d;
    }
}