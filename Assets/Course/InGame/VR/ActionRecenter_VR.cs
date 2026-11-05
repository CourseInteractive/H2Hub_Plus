using UnityEngine;
using Course.PrototypeScripting;
public class ActionRecenter_VR : Action
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    override public void ExecuteAction()
    {
        if(VR_Recenter.Instance)
            VR_Recenter.Instance.RecenterPlayer();
        ReportActionEnd();
    }

    override public string GetAdditionalInfo()
    {
        return "";
    }
}
