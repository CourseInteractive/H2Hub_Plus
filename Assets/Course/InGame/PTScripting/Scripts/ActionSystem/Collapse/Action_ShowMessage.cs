using UnityEngine;
using Course.PrototypeScripting;
public class Action_ShowMessage : Action
{
    public bool onlyClose;

    public string callerStringToken;
    public LocaString callerLabel;
    public string messageStringToken;
    public LocaString messageLabel;
    public int iconIndex;
    public int buttonTextIndex;
    public bool immediately;

    public Transform positionToHold;

    override public void ExecuteAction()
    {
        if(onlyClose)
        {
            IncomingMessageUI.instance.Hide();
            ReportActionEnd();
            return;
        }

        if (positionToHold)
            IncomingMessageUI.instance.SetPositionToHold(positionToHold);
        else
            IncomingMessageUI.instance.FreeFromPosition();
        //Direct Message??
        //IncomingMessageUI.instance.ShowMessage(callerStringToken, messageStringToken, iconIndex, buttonTextIndex);
        if(messageLabel.IsEmpty)
        {
            MessageInbox.instance.AddMessage(MessageInfo.Important(callerStringToken, messageStringToken, iconIndex, buttonTextIndex, immediately));

        }
        else
        {
            MessageInbox.instance.AddMessage(MessageInfo.Important(callerLabel.Token, messageLabel.Token, iconIndex, buttonTextIndex, immediately));

        }


        ReportActionEnd();
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {
        return "";
    }
}
