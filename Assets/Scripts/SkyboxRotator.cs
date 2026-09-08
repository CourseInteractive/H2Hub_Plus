using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    [Tooltip("Grad pro Sekunde")]
    [SerializeField] private float rotationSpeed = 2f;

    [Tooltip("1 = im Uhrzeigersinn, -1 = gegen den Uhrzeigersinn")]
    [SerializeField] private float direction = 1f;

    private Material skyboxMaterial;
    private float currentRotation;

    void Start()
    {
        // Kopie erzeugen, damit nicht das Original-Asset verändert wird
        skyboxMaterial = new Material(RenderSettings.skybox);
        RenderSettings.skybox = skyboxMaterial;

        currentRotation = skyboxMaterial.GetFloat("_Rotation");
    }

    void Update()
    {
        currentRotation += rotationSpeed * direction * Time.deltaTime;
        currentRotation %= 360f;

        skyboxMaterial.SetFloat("_Rotation", currentRotation);
    }
}
