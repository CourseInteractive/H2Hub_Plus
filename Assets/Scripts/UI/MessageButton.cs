using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sitzt auf dem Button-Prefab. Merkt sich seine Nachricht und leitet Klicks an die Inbox weiter.
/// </summary>
[RequireComponent(typeof(Button))]
public class MessageButton : MonoBehaviour
{
    [Tooltip("Optional: zeigt den Absender auf dem Button an")]
 //   [SerializeField] private TMP_Text callerLabel;

    private Button button;
    private MessageInfo message;
    public MessageInbox inbox;

    public MessageInfo Message => message;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleClick);
    }

    /// <summary>Wird von der Inbox direkt nach dem Instantiieren aufgerufen.</summary>
    public void Setup(MessageInfo info)
    {
        message = info;
        gameObject.SetActive(true);
     //   if (callerLabel != null) callerLabel.text = info.caller;
    }

    private void HandleClick()
    {
        if (inbox != null && message != null)
            inbox.OpenMessage(message);
    }

    private void OnDestroy()
    {
        if (button != null) button.onClick.RemoveListener(HandleClick);
    }
}
