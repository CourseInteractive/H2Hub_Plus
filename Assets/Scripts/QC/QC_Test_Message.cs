using UnityEngine;

public class QC_Test_Message : QuickConsole_Entry
{
    public MessageInfo.MessageType msgType;

    new private void Awake()
    {
        base.Awake();
        type = EntryType.Action;
    }

    public override void ExecuteAction()
    {
        switch(msgType)
        {
            case MessageInfo.MessageType.Commission:
                MessageInbox.instance.AddMessage(MessageInfo.Commission("Teststelle", "Wir brauchen Wasserstoff!", ResourceType.H, icon: 0, buttonText: 0));
                break;
            case MessageInfo.MessageType.Warning:
                MessageInbox.instance.AddMessage(MessageInfo.Warning("Technik-Test", "Druck ist da!", icon: 2, buttonText: 1));
                break;
            case MessageInfo.MessageType.Important:
                MessageInbox.instance.AddMessage(MessageInfo.Important("TestChef", "Meeting um 10 Uhr", icon: 3, buttonText: 2));
                break;
        }
    }
}
