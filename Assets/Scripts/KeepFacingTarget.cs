using UnityEngine;

/// <summary>
/// Richtet dieses GameObject auf ein Ziel aus.
/// Einzelne lokale Rotationsachsen können gesperrt werden,
/// z. B. nur Y aktiv lassen, damit sich das Objekt nur horizontal dreht.
/// </summary>
public class KeepFacingTarget : MonoBehaviour
{
    [Header("Ziel")]
    [Tooltip("Das Objekt, auf das ausgerichtet werden soll.")]
    public Transform target;

    [Header("Aktive Achsen (lokal)")]
    [Tooltip("Neigen nach oben/unten (Pitch).")]
    public bool rotateX = false;

    [Tooltip("Drehen nach links/rechts (Yaw).")]
    public bool rotateY = true;

    [Tooltip("Rollen um die Blickrichtung (Roll).")]
    public bool rotateZ = false;

    [Header("Bewegung")]
    [Tooltip("Drehgeschwindigkeit in Grad pro Sekunde. 0 = sofort ausrichten.")]
    [Min(0f)]
    public float rotationSpeed = 0f;

    [Header("Korrektur")]
    [Tooltip("Zusätzliche Rotation, falls die 'Vorderseite' des Modells nicht in +Z zeigt " +
             "(z. B. (0, -90, 0), wenn das Modell nach +X schaut).")]
    public Vector3 rotationOffset = Vector3.zero;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        if (direction.sqrMagnitude < 0.0001f) return;

        // Richtung in den Raum des Parents umrechnen, damit die Achsen lokal gelten
        Transform parent = transform.parent;
        Vector3 localDirection = parent != null
            ? parent.InverseTransformDirection(direction)
            : direction;

        Quaternion desired = Quaternion.LookRotation(localDirection, Vector3.up)
                             * Quaternion.Euler(rotationOffset);

        // Gesperrte Achsen behalten ihren aktuellen Wert
        Vector3 desiredEuler = desired.eulerAngles;
        Vector3 currentEuler = transform.localEulerAngles;

        if (!rotateX) desiredEuler.x = currentEuler.x;
        if (!rotateY) desiredEuler.y = currentEuler.y;
        if (!rotateZ) desiredEuler.z = currentEuler.z;

        Quaternion targetRotation = Quaternion.Euler(desiredEuler);

        if (rotationSpeed <= 0f)
        {
            transform.localRotation = targetRotation;
        }
        else
        {
            transform.localRotation = Quaternion.RotateTowards(
                transform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, target.position);
    }
}
