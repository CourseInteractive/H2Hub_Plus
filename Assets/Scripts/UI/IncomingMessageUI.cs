using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IncomingMessageUI : MonoBehaviour
{
    public CameraRelative_UI movementToCam;
    public static IncomingMessageUI instance;
    public TMP_Text callerText;
    public TMP_Text messageText;
    public Image icon;
    public TMP_Text buttonText;
    public Sprite[] icons;
    public string[] buttonTexts;

    public delegate void AcceptingMessage();
    public event AcceptingMessage OnMessageAccepted;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        instance = this;
        Hide();
    }

    public void SetPositionToHold(Transform positionObject)
    {
        if (!movementToCam)
            return;
        movementToCam.SetPositionToHold(positionObject.transform.position);
    }

    public void FreeFromPosition()
    {
        if (!movementToCam)
            return;
        movementToCam.FreeFromPosition();
    }

    public void ShowMessage(string caller, string message)
    {
        ShowMessage(caller,message, 0, 0);
    }

    public void ShowMessage(string caller, string message, int iconIndex, int buttonTextIndex)
    {
        messageText.text = SimpleLocalization.GetLocalization(message);
        callerText.text = SimpleLocalization.GetLocalization(caller);
        icon.sprite = icons[iconIndex];
        buttonText.text = buttonTexts[buttonTextIndex];
        gameObject.SetActive(true);
    }

    public void ButtonClick()
    {
        OnMessageAccepted?.Invoke();
        GameEventManager.Instance.ReportGameEvent("MessageAccept");
        Hide();
    }

    public void Hide()
    {
        OnMessageAccepted = null;
        gameObject.SetActive(false);
    }
}
