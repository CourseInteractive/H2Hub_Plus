using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    public enum FaceDirection
    {
        Forward,
        Back,
        Left,
        Right,
        Up,
        Down
    }

    [Header("Settings")]
    [Tooltip("Which local axis of this object points toward the camera.")]
    public FaceDirection faceDirection = FaceDirection.Forward;

    [Tooltip("The Up vector used for LookRotation. Defaults to Vector3.up.")]
    public Vector3 upVector = Vector3.up;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (_cam == null) return;

        // Direction FROM this object TO the camera
        Vector3 toCamera = _cam.transform.position - transform.position;

        // Depending on which local axis should face the camera,
        // we rotate that direction accordingly before passing to LookRotation.
        // LookRotation always aligns the local FORWARD (+Z) axis.
        // So we transform toCamera so that local +Z ends up being our desired face axis.
        Vector3 lookDir = AdjustDirectionForFaceAxis(toCamera);

        if (lookDir.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(lookDir, upVector);
    }

    /// <summary>
    /// Remaps toCamera so that Quaternion.LookRotation (which aligns +Z)
    /// effectively aligns the chosen faceDirection axis toward the camera instead.
    /// </summary>
    private Vector3 AdjustDirectionForFaceAxis(Vector3 toCamera)
    {
        switch (faceDirection)
        {
            // +Z should face camera → pass toCamera directly
            case FaceDirection.Forward:
                return toCamera;

            // -Z should face camera → flip direction
            case FaceDirection.Back:
                return -toCamera;

            // +X should face camera → rotate -90° around up so +Z maps to +X
            case FaceDirection.Right:
                return Quaternion.AngleAxis(-90f, upVector) * toCamera;

            // -X should face camera → rotate +90° around up so +Z maps to -X
            case FaceDirection.Left:
                return Quaternion.AngleAxis(90f, upVector) * toCamera;

            // +Y should face camera → rotate +90° around right so +Z maps to +Y
            case FaceDirection.Up:
                return Quaternion.AngleAxis(90f, Vector3.Cross(upVector, toCamera).normalized) * toCamera;

            // -Y should face camera → rotate -90° around right so +Z maps to -Y
            case FaceDirection.Down:
                return Quaternion.AngleAxis(-90f, Vector3.Cross(upVector, toCamera).normalized) * toCamera;

            default:
                return toCamera;
        }
    }
}
