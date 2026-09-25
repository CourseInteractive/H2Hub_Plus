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
    
    public Sprite[] portraits;
    public GameObject[] resourceIcons;
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


    public void ShowMessage(MessageInfo info)
    {
        ClearResourceInfo();
        messageText.text = SimpleLocalization.GetLocalization(info.message);
        callerText.text = SimpleLocalization.GetLocalization(info.caller);
        icon.sprite = portraits[info.iconIndex];
        buttonText.text = buttonTexts[info.buttonTextIndex];
        if (!info.noResource)
            SetResourceInfo(info.resType);
        gameObject.SetActive(true);
    }

    public void ShowMessage(string caller, string message)
    {
        ShowMessage(caller,message, 0, 0);
    }

    public void ShowMessage(string caller, string message, int iconIndex, int buttonTextIndex)
    {
        ClearResourceInfo();
        messageText.text = SimpleLocalization.GetLocalization(message);
        callerText.text = SimpleLocalization.GetLocalization(caller);
        icon.sprite = portraits[iconIndex];
        buttonText.text = buttonTexts[buttonTextIndex];
        gameObject.SetActive(true);
    }

    public void SetResourceInfo(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.H:
                resourceIcons[0].SetActive(true);
                break;
            case ResourceType.O:
                resourceIcons[1].SetActive(true);
                break;
        }
    }

    void ClearResourceInfo()
    {
        foreach(GameObject resIcon in resourceIcons)
        {
            resIcon.SetActive(false);
        }
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

public class MessageInfo
{
    public string caller;
    public string message;
    public int iconIndex;
    public int buttonTextIndex;
    public ResourceType resType;
    public bool noResource;
    public MessageType mesType;
    /// <summary>Wenn true: kein Button, nur Sound, Nachricht wird sofort geöffnet.</summary>
    public bool openImmediately;
    public enum MessageType
    {
        Commission,
        Warning,
        Important
    }

    // Parameterloser Konstruktor bleibt erhalten (für Object-Initializer & Serialisierung)
    public MessageInfo() { }

    /// <summary>Nachricht mit Ressource.</summary>
    public MessageInfo(string caller, string message, MessageType mesType, ResourceType resType,
                       int iconIndex = 0, int buttonTextIndex = 0, bool openImmediately = false)

    {
        this.caller = caller;
        this.message = message;
        this.mesType = mesType;
        this.resType = resType;
        this.iconIndex = iconIndex;
        this.buttonTextIndex = buttonTextIndex;
        this.openImmediately = openImmediately;
        noResource = false;
    }

    /// <summary>Nachricht ohne Ressource.</summary>
    public MessageInfo(string caller, string message, MessageType mesType,
                       int iconIndex = 0, int buttonTextIndex = 0, bool openImmediately = false)

    {
        this.caller = caller;
        this.message = message;
        this.mesType = mesType;
        this.iconIndex = iconIndex;
        this.buttonTextIndex = buttonTextIndex;
        this.openImmediately = openImmediately;
        noResource = true;
    }

    /// <summary>
    /// Markiert die Nachricht zum sofortigen Öffnen. Verkettbar:
    /// MessageInfo.Warning("Technik", "Alarm!").Immediate()
    /// </summary>
    public MessageInfo Immediate(bool value = true)
    {
        openImmediately = value;
        return this;
    }

    // ------------------------------------------------ Kurzformen pro Typ

    public static MessageInfo Commission(string caller, string message, ResourceType res,
                                          int icon = 0, int buttonText = 0, bool openImmediately = false)
         => new MessageInfo(caller, message, MessageType.Commission, res, icon, buttonText, openImmediately);

    public static MessageInfo Warning(string caller, string message,
                                      int icon = 0, int buttonText = 0, bool openImmediately = false)
        => new MessageInfo(caller, message, MessageType.Warning, icon, buttonText, openImmediately);

    public static MessageInfo Important(string caller, string message,
                                        int icon = 0, int buttonText = 0, bool openImmediately = false)
        => new MessageInfo(caller, message, MessageType.Important, icon, buttonText, openImmediately);



}
/*
public enum ResourceType
{
    H2O,    // Wasser (Input)
    E,      // Energie (Input)
    H,      // Wasserstoff (Output)
    O,      // Sauerstoff (Output)
    W       // Wärme (Output)
}*/
