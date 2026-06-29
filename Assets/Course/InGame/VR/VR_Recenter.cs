using UnityEngine;

public class VR_Recenter : MonoBehaviour
{
    public static VR_Recenter Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void RecenterPlayer()
    {
        // Aktuelle Kopfposition im Worldspace
        Transform cameraTransform = Camera.main.transform;
        Vector3 headPosition = cameraTransform.position;
        float headYaw = cameraTransform.eulerAngles.y;

        // XR Origin so verschieben, dass Kopf wieder am gewünschten Punkt landet
        Vector3 originPosition = transform.position;
        Vector3 offset = headPosition - originPosition;
        offset.y = 0; // Nur horizontal korrigieren

        transform.position -= offset;

        // Optional: Auch Rotation ausrichten
        /*transform.RotateAround(
            headPosition,
            Vector3.up,
            -headYaw // Spieler schaut wieder in "Vorwärts"-Richtung
        );*/
    }
}
