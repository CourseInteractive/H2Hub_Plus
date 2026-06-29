using UnityEngine;
using UnityEngine.InputSystem;

public class GameData : MonoBehaviour
{
    public static GameData Instance { get; private set; }

    public static Camera VR_Camera;
    public static GameObject player;
    [SerializeField] private GameValues values;
    public GameValues Values => values;

    [Header("Lifecycle")]
    [SerializeField] private bool dontDestroyOnLoad = true;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);

        // Fallback: Versuche automatisch aus Resources zu laden, falls nicht gesetzt
        if (values == null)
            values = Resources.Load<GameValues>("GameValues");

        EnableInputs(true);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            EnableInputs(false);
            Instance = null;
        }
    }

    private void OnValidate()
    {
        // Editor-Komfort: halte Limits konsistent
        if (values != null)
        {
            values.pitchLimits = new Vector2(
                Mathf.Min(values.pitchLimits.x, values.pitchLimits.y),
                Mathf.Max(values.pitchLimits.x, values.pitchLimits.y)
            );
        }
    }

    private void EnableInputs(bool enable)
    {
        if (values == null) return;

        if (values.lookAction != null && values.lookAction.action != null)
        {
            if (enable) values.lookAction.action.Enable();
            else values.lookAction.action.Disable();
        }

        if (values.blockRotationAction != null && values.blockRotationAction.action != null)
        {
            if (enable) values.blockRotationAction.action.Enable();
            else values.blockRotationAction.action.Disable();
        }
    }
}

public enum Scenario { Kfz, Sicherheit, Schweiﬂen, Wasser, Sanit‰ter, Elektrik, CAD}