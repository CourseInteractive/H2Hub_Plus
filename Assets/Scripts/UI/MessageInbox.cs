using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Verwaltet eingehende Nachrichten: speichert sie, erstellt pro Nachricht einen Button,
/// lässt ihn vom Spawnpunkt zum Container fliegen und übernimmt ihn dann in die LayoutGroup.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class MessageInbox : MonoBehaviour
{
    public static MessageInbox instance;

    [Header("Button-Liste")]
    [Tooltip("Transform mit einer Vertical/Horizontal/Grid LayoutGroup")]
    [SerializeField] private Transform buttonContainer;

    [Tooltip("Ein Prefab pro MessageType, in derselben Reihenfolge wie das Enum: Commission, Warning, Important")]
    [SerializeField] private MessageButton[] buttonPrefabs;

    [Header("Einflug neuer Nachrichten")]
    [Tooltip("Beweglicher Punkt, an dem neue Buttons zuerst erscheinen")]
    [SerializeField] private Transform spawnPoint;

    [Tooltip("Zielpunkt, zu dem der Button fliegt, bevor er in die LayoutGroup übernommen wird")]
    [SerializeField] private Transform containerPosition;

    [Tooltip("Optional: Parent während des Flugs (z. B. Canvas-Root), falls der Container maskiert ist. Leer = buttonContainer")]
    [SerializeField] private Transform flyingParent;

    [Header("Sounds")]
    [Tooltip("Ein Sound pro MessageType, in derselben Reihenfolge wie das Enum: Commission, Warning, Important")]
    [SerializeField] private AudioClip[] newMessageSounds;

    [Range(0f, 1f)]
    [SerializeField] private float soundVolume = 1f;

    private AudioSource audioSource;

  /*  [Header("Detail-Ansicht")]
    [SerializeField] private GameObject messagePanel;
    [SerializeField] private TMP_Text callerText;
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button confirmButton;
    [SerializeField] private TMP_Text confirmButtonText;
    [SerializeField] private Button closeButton;

    [Header("Daten für die Indizes aus MessageInfo")]
    [SerializeField] private Sprite[] icons;          // über iconIndex
    [SerializeField] private string[] buttonTexts;    // über buttonTextIndex*/

    /// <summary>Wird ausgelöst, wenn eine Nachricht bestätigt (und damit gelöscht) wurde.</summary>
    public event Action<MessageInfo> OnMessageConfirmed;

    /// <summary>Wird ausgelöst, wenn eine neue Nachricht eingegangen ist (Button erscheint am Spawnpunkt).</summary>
    public event Action<MessageInfo> OnMessageReceived;

    /// <summary>Wird ausgelöst, wenn der Button im Container angekommen und in die LayoutGroup übernommen ist.</summary>
    public event Action<MessageInfo> OnMessageArrived;

    private readonly Dictionary<MessageInfo, MessageButton> messages = new Dictionary<MessageInfo, MessageButton>();
    private MessageInfo openMessage;

    public int Count => messages.Count;
    public IEnumerable<MessageInfo> Messages => messages.Keys;

    public Transform armTransformVR;

    private void Awake()
    {
        instance = this;
        Invoke("ClipToArm", 1f);

        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;   // reiner 2D-Sound

     /*   if (confirmButton != null) confirmButton.onClick.AddListener(ConfirmOpenMessage);
        if (closeButton != null) closeButton.onClick.AddListener(CloseMessage);
        if (messagePanel != null) messagePanel.SetActive(false);*/
    }

    public void ClipToArm()
    {
        if(armTransformVR != null)
        {
            transform.parent = armTransformVR;
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = new Vector3(0, 90, 0);
        }
    }

    private void OnValidate()
    {
        int typeCount = Enum.GetValues(typeof(MessageInfo.MessageType)).Length;
        if (buttonPrefabs != null && buttonPrefabs.Length != typeCount)
            Debug.LogWarning($"[MessageInbox] buttonPrefabs sollte {typeCount} Einträge haben (einen pro MessageType).", this);
        if (newMessageSounds != null && newMessageSounds.Length != typeCount)
            Debug.LogWarning($"[MessageInbox] newMessageSounds sollte {typeCount} Einträge haben (einen pro MessageType).", this);
    }

    // ---------------------------------------------------------------- Öffentliche API

    /// <summary>Fügt eine neue Nachricht hinzu, erstellt den passenden Button und startet den Einflug.</summary>
    public void AddMessage(MessageInfo info)
    {
        if (info == null || messages.ContainsKey(info)) return;

        // Sonderfall: kein Button, nur Sound und direkt öffnen
        if (info.openImmediately)
        {
            ShowImmediately(info);
            return;
        }

        int typeIndex = (int)info.mesType;
        if (buttonPrefabs == null || typeIndex >= buttonPrefabs.Length || buttonPrefabs[typeIndex] == null)
        {
            Debug.LogError($"[MessageInbox] Kein Prefab für MessageType '{info.mesType}' hinterlegt.", this);
            return;
        }

        MessageButton prefab = buttonPrefabs[typeIndex];
        Vector3 prefabScale = prefab.transform.localScale;

        bool animate = spawnPoint != null && containerPosition != null;
        Transform parent = (animate && flyingParent != null) ? flyingParent : buttonContainer;

        MessageButton button = Instantiate(prefab, parent);
        button.inbox = this;   // Prefabs können keine Szenen-Referenz halten, daher hier setzen
        button.Setup(info);
        messages.Add(info, button);

        PlayNewMessageSound(info.mesType);
        OnMessageReceived?.Invoke(info);

        if (animate)
        {
            if (!button.TryGetComponent(out MessageMovement movement))
                movement = button.gameObject.AddComponent<MessageMovement>();

            movement.Begin(spawnPoint, containerPosition, () => IntegrateIntoLayout(info, button, prefabScale));
        }
        else
        {
            IntegrateIntoLayout(info, button, prefabScale);
        }
    }

    /// <summary>
    /// Öffnet eine Nachricht sofort, ohne Button und ohne Speicherung in der Inbox.
    /// Verhält sich danach wie eine geöffnete und bestätigte Nachricht.
    /// </summary>
    private void ShowImmediately(MessageInfo info)
    {
        PlayNewMessageSound(info.mesType);
        OnMessageReceived?.Invoke(info);

        if (IncomingMessageUI.instance != null)
            IncomingMessageUI.instance.ShowMessage(info);
        else
            Debug.LogWarning("[MessageInbox] IncomingMessageUI.instance ist nicht vorhanden.", this);

        OnMessageConfirmed?.Invoke(info);
    }

    /// <summary>Spielt den Sound für den jeweiligen Nachrichtentyp (2D, überlappend möglich).</summary>
    private void PlayNewMessageSound(MessageInfo.MessageType type)
    {
        int index = (int)type;
        if (audioSource == null || newMessageSounds == null || index >= newMessageSounds.Length) return;

        AudioClip clip = newMessageSounds[index];
        if (clip != null) audioSource.PlayOneShot(clip, soundVolume);
    }

    /// <summary>Wird aufgerufen, wenn der Button am Container angekommen ist.</summary>
    private void IntegrateIntoLayout(MessageInfo info, MessageButton button, Vector3 scale)
    {
        // Nachricht wurde evtl. während des Flugs gelöscht
        if (button == null || !messages.ContainsKey(info)) return;

        Transform t = button.transform;
        if (t.parent != buttonContainer) t.SetParent(buttonContainer, false);

        t.localRotation = Quaternion.identity;
        t.localScale = scale;
        t.localPosition = Vector3.zero;   // x/y setzt gleich die LayoutGroup
        t.SetAsLastSibling();

        if (buttonContainer is RectTransform containerRect)
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRect);

        OnMessageArrived?.Invoke(info);
    }

    /// <summary>Öffnet eine Nachricht. Wird vom MessageButton aufgerufen.</summary>
    public void OpenMessage(MessageInfo info)
    {
        if (info == null || !messages.ContainsKey(info)) return;
        IncomingMessageUI.instance.ShowMessage(info);
        GameEventManager.Instance.ReportGameEvent("MessageOpen", "");
        openMessage = info;
        ConfirmOpenMessage();
   /*     if (callerText != null) callerText.text = info.caller;
        if (messageText != null) messageText.text = info.message;

        if (iconImage != null)
        {
            bool hasIcon = icons != null && info.iconIndex >= 0 && info.iconIndex < icons.Length;
            iconImage.gameObject.SetActive(hasIcon);
            if (hasIcon) iconImage.sprite = icons[info.iconIndex];
        }

        if (confirmButtonText != null)
        {
            bool hasText = buttonTexts != null && info.buttonTextIndex >= 0 && info.buttonTextIndex < buttonTexts.Length;
            confirmButtonText.text = hasText ? buttonTexts[info.buttonTextIndex] : "OK";
        }

        if (messagePanel != null) messagePanel.SetActive(true); */
    }

    /// <summary>Schließt das Detail-Panel, ohne die Nachricht zu löschen.</summary>
    public void CloseMessage()
    {
        openMessage = null;
     //   if (messagePanel != null) messagePanel.SetActive(false);
    }

    /// <summary>Bestätigt die aktuell geöffnete Nachricht, meldet sie per Event und löscht sie.</summary>
    public void ConfirmOpenMessage()
    {
        if (openMessage == null) return;

        MessageInfo confirmed = openMessage;
        DeleteMessage(confirmed);
        OnMessageConfirmed?.Invoke(confirmed);
    }

    /// <summary>Entfernt eine Nachricht samt Button aus der Inbox (auch während des Flugs).</summary>
    public void DeleteMessage(MessageInfo info)
    {
        if (info == null || !messages.TryGetValue(info, out MessageButton button)) return;

        messages.Remove(info);
        if (button != null) Destroy(button.gameObject);

        if (openMessage == info) CloseMessage();
    }

    /// <summary>Löscht alle Nachrichten.</summary>
    public void ClearAll()
    {
        foreach (MessageButton button in messages.Values)
            if (button != null) Destroy(button.gameObject);

        messages.Clear();
        CloseMessage();
    }
}
