using UnityEngine;

/// <summary>
/// Rotiert die Skybox kontinuierlich um eine frei wählbare Achse.
/// Voraussetzung: Das Skybox-Material verwendet den Shader "Custom/SkyboxAxisRotate".
/// </summary>
public class SkyboxAxisRotator : MonoBehaviour
{
    [Tooltip("Grad pro Sekunde")]
    [SerializeField] private float rotationSpeed = 2f;

    [Tooltip("Achse, um die sich die Skybox dreht (wird automatisch normalisiert)")]
    [SerializeField] private Vector3 rotationAxis = Vector3.up;

    private Material skyboxMaterial;
    private float currentAngle;

    private static readonly int RotationMatrixID = Shader.PropertyToID("_SkyRotationMatrix");

    void Start()
    {
        // Kopie erzeugen, damit nicht das Original-Asset verändert wird
        skyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxMaterial;
    }

    void Update()
    {
        currentAngle += rotationSpeed * Time.deltaTime;
        currentAngle %= 360f;

        Quaternion rotation = Quaternion.AngleAxis(currentAngle, rotationAxis.normalized);
        Matrix4x4 rotationMatrix = Matrix4x4.Rotate(rotation);

        skyboxMaterial.SetMatrix(RotationMatrixID, rotationMatrix);
    }
}
