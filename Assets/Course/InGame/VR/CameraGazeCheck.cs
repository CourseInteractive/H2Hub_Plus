using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Prüft, ob dieses GameObject (z.B. eine Kamera oder ein NPC-Kopf) innerhalb
/// eines maximalen Winkels auf ein Ziel-Transform schaut. Optional kann dabei
/// eine Achse ignoriert werden (z.B. Y, um Höhenunterschiede zu ignorieren).
///
/// Aufruf von außen:
///   - CheckGaze() startet die Prüfung und aktiviert das Script.
///   - TargetLookedAt() wird automatisch ausgelöst, sobald das Ziel im
///     erlaubten Winkel angeschaut wird. Danach deaktiviert sich das Script
///     wieder selbst (enabled = false).
/// </summary>
public class CameraGazeCheck : MonoBehaviour
{
    [Header("Ziel")]
    [Tooltip("Das Transform, das angeschaut werden soll.")]
    public Transform target;

    [Header("Winkel-Einstellungen")]
    [Tooltip("Maximaler Winkel in Grad zwischen Blickrichtung und Ziel, der noch als 'angeschaut' zählt.")]
    [Range(0f, 180f)]
    public float maxAngle = 15f;

    public enum IgnoreAxis { None, X, Y, Z }

    [Tooltip("Welche Achse bei der Winkelberechnung ignoriert werden soll (z.B. Y, um Höhenunterschiede zu ignorieren).")]
    public IgnoreAxis ignoreAxis = IgnoreAxis.None;

    [Header("Events")]
    [Tooltip("Wird ausgelöst, sobald das Ziel im erlaubten Winkel angeschaut wird.")]
    public UnityEvent onTargetLookedAt;

    private bool isChecking = false;

    private void Update()
    {
        if (!isChecking || target == null)
            return;

        Vector3 directionToTarget = target.position - transform.position;
        Vector3 forward = transform.forward;

        // Gewählte Achse für die Winkelberechnung neutralisieren
        switch (ignoreAxis)
        {
            case IgnoreAxis.X:
                directionToTarget.x = 0f;
                forward.x = 0f;
                break;
            case IgnoreAxis.Y:
                directionToTarget.y = 0f;
                forward.y = 0f;
                break;
            case IgnoreAxis.Z:
                directionToTarget.z = 0f;
                forward.z = 0f;
                break;
        }

        float angle = Vector3.Angle(forward, directionToTarget);

        if (angle <= maxAngle)
        {
            TargetLookedAt();
        }
    }

    /// <summary>
    /// Startet die Gaze-Prüfung und aktiviert das Script.
    /// </summary>
    public void CheckGaze()
    {
        enabled = true;
        isChecking = true;
    }

    public void SetTarget(GameObject obj)
    {
        enabled = true;
        isChecking = true;
        target = obj.transform;
    }

    /// <summary>
    /// Wird intern aufgerufen, sobald das Ziel angeschaut wird.
    /// Löst das Event aus und deaktiviert das Script danach wieder.
    /// </summary>
    private void TargetLookedAt()
    {
        isChecking = false;
        onTargetLookedAt?.Invoke();
        enabled = false;
    }
}
