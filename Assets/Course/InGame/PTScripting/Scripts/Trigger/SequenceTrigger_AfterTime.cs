using UnityEngine;
using UnityEngine.UI;
using Course.PrototypeScripting;
public class SequenceTrigger_AfterTime : MonoBehaviour
{
    [Header("Erkennung")]
    [Tooltip("Welche Layer dürfen den Timer auslösen?")]
    public LayerMask targetLayers;

    [Header("Timer Einstellungen")]
    [Tooltip("Zeit in Sekunden, bis Execute ausgelöst wird.")]
    public float maxTime = 3f;

    [Tooltip("Zeit in Sekunden, die der Timer braucht, um wieder auf 0 zu fallen, wenn kein Objekt im Trigger ist.")]
    public float resetDuration = 1f;

    [Header("UI")]
    [Tooltip("Image, dessen FillAmount den Fortschritt zeigt.")]
    public Image fillImage;

    private float timer = 0f;
    private bool objectInside = false;
    private bool hasExecuted = false;

    public Sequence seqOnExecute;

    private void Update()
    {
        // Wenn ein gültiges Objekt im Trigger ist -> Timer hochzählen
        if (objectInside)
        {
            if (timer < maxTime)
            {
                timer += Time.deltaTime;
                if (timer >= maxTime)
                {
                    timer = maxTime;

                    if (!hasExecuted)
                    {
                        Execute();
                        hasExecuted = true;
                    }
                }
            }
        }
        else
        {
            // Kein Objekt mehr im Trigger -> Timer wieder runterlaufen lassen
            if (timer > 0f && resetDuration > 0f)
            {
                float resetSpeed = maxTime / resetDuration; // pro Sekunde
                timer -= resetSpeed * Time.deltaTime;

                if (timer <= 0f)
                {
                    timer = 0f;
                    hasExecuted = false; // bereit, beim nächsten Mal wieder auszulösen
                }
            }
        }

        // UI updaten
        if (fillImage != null && maxTime > 0f)
        {
            fillImage.fillAmount = timer / maxTime;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsInLayerMask(other.gameObject, targetLayers))
        {
            objectInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsInLayerMask(other.gameObject, targetLayers))
        {
            objectInside = false;
        }
    }

    /// <summary>
    /// Prüft, ob das Objekt in einem der gewünschten Layer liegt.
    /// </summary>
    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        int objLayerMask = 1 << obj.layer;
        return (layerMask.value & objLayerMask) != 0;
    }

    /// <summary>
    /// Diese Funktion wird aufgerufen, sobald der Timer maxTime erreicht.
    /// </summary>
    private void Execute()
    {
        // TODO: Hier deine gewünschte Logik einbauen
        Debug.Log("Execute ausgelöst!");
        seqOnExecute.ExecuteCompleteSequence();
    }
}
