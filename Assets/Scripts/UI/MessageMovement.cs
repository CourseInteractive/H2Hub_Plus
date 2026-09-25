using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lässt einen Nachrichten-Button zuerst an einem (beweglichen) Spawnpunkt verweilen
/// und danach zu einem (ebenfalls beweglichen) Zielpunkt fliegen.
/// Beide Punkte werden jeden Frame neu ausgelesen.
/// Während der Bewegung ignoriert der Button die LayoutGroup.
/// </summary>
[DisallowMultipleComponent]
public class MessageMovement : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Wie lange der Button am Spawnpunkt bleibt (Sekunden)")]
    [SerializeField] private float holdDuration = 2f;

    [Tooltip("Wie lange der Flug zum Container dauert (Sekunden)")]
    [SerializeField] private float flyDuration = 0.8f;

    [Tooltip("Verlauf des Flugs (0 = Spawnpunkt, 1 = Ziel)")]
    [SerializeField] private AnimationCurve flyCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    [Header("Optionen")]
    [Tooltip("Rotation vom Spawnpunkt übernehmen und zur Zielrotation drehen")]
    [SerializeField] private bool followRotation = true;

    [Tooltip("Darf der Button während der Bewegung angeklickt werden?")]
    [SerializeField] private bool interactableWhileMoving = false;

    private enum Phase { Idle, Holding, Flying }

    private Phase phase = Phase.Idle;
    private float timer;

    private Transform spawnPoint;
    private Transform targetPoint;
    private Action onArrived;

    private Vector3 lastSpawnPos;
    private Quaternion lastSpawnRot;

    private LayoutElement layoutElement;
    private Selectable selectable;
    private bool wasInteractable = true;

    public bool IsMoving => phase != Phase.Idle;

    /// <summary>Startet die Bewegung. onArrived wird aufgerufen, sobald das Ziel erreicht ist.</summary>
    public void Begin(Transform spawn, Transform target, Action arrivedCallback)
    {
        spawnPoint = spawn;
        targetPoint = target;
        onArrived = arrivedCallback;

        // Während der Bewegung darf die LayoutGroup den Button nicht positionieren
        if (!TryGetComponent(out layoutElement))
            layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.ignoreLayout = true;

        if (TryGetComponent(out selectable))
        {
            wasInteractable = selectable.interactable;
            if (!interactableWhileMoving) selectable.interactable = false;
        }

        lastSpawnPos = spawnPoint != null ? spawnPoint.position : transform.position;
        lastSpawnRot = spawnPoint != null ? spawnPoint.rotation : transform.rotation;

        timer = 0f;
        phase = Phase.Holding;
        Step(); // sofort positionieren, damit der Button nicht einen Frame falsch sitzt
    }

    // LateUpdate, damit bereits bewegte Punkte (Hand, Arm, Kopf) im selben Frame berücksichtigt werden
    private void LateUpdate()
    {
        if (phase == Phase.Idle) return;

        timer += Time.deltaTime;
        Step();
    }

    private void Step()
    {
        // Spawnpunkt jeden Frame neu auslesen (letzte bekannte Pose bleibt, falls er zerstört wird)
        if (spawnPoint != null)
        {
            lastSpawnPos = spawnPoint.position;
            lastSpawnRot = spawnPoint.rotation;
        }

        if (phase == Phase.Holding)
        {
            Apply(lastSpawnPos, lastSpawnRot);

            if (timer >= holdDuration)
            {
                timer -= holdDuration;
                phase = Phase.Flying;
            }
            return;
        }

        // Phase.Flying
        if (targetPoint == null)
        {
            Finish();
            return;
        }

        float n = flyDuration > 0f ? Mathf.Clamp01(timer / flyDuration) : 1f;
        float k = flyCurve.Evaluate(n);

        // Start und Ziel sind beide live, damit der Flug auch bei bewegtem Spieler sauber bleibt
        Vector3 pos = Vector3.LerpUnclamped(lastSpawnPos, targetPoint.position, k);
        Quaternion rot = Quaternion.SlerpUnclamped(lastSpawnRot, targetPoint.rotation, k);
        Apply(pos, rot);

        if (n >= 1f) Finish();
    }

    private void Apply(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        if (followRotation) transform.rotation = rotation;
    }

    private void Finish()
    {
        phase = Phase.Idle;

        if (layoutElement != null) layoutElement.ignoreLayout = false;
        if (selectable != null) selectable.interactable = wasInteractable;

        Action callback = onArrived;
        onArrived = null;
        callback?.Invoke();
    }
}
