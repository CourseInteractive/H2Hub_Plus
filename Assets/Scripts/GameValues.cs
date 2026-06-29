using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "GameValues", menuName = "Game/Game Values")]
public class GameValues : ScriptableObject
{
    [Header("Camera Rotation Settings")]
    [Tooltip("Allgemeine Drehgeschwindigkeit der Kamera.")]
    public float rotationSpeed = 1.0f;

    [Tooltip("Maus-Sensitivität (X = horizontal, Y = vertikal).")]
    public Vector2 sensitivity = new Vector2(1f, 1f);

    [Tooltip("Y-Achse invertieren?")]
    public bool invertY = false;

    [Tooltip("Grenzen für Pitch (vertikale Rotation).")]
    public Vector2 pitchLimits = new Vector2(-80f, 80f);

    [Header("Input (neues Input System)")]
    [Tooltip("Look-Input (Vector2).")]
    public InputActionReference lookAction;

    [Tooltip("Wenn gedrückt, wird die Rotation unterbunden (z. B. Shift).")]
    public InputActionReference blockRotationAction;


    [Tooltip("Main Machine")]
    public Vector2 outputFactorLimitsByDial;

    [Header("Errors")]
    public Material errorMaterial;
    public AnimationCurve errorBlink;
    public float errorBlinkTime;


    [Header("Tooltips")]
    public string defaultGrabTooltip;
}
