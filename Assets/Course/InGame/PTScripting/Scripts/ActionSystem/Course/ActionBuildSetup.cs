using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Course.PrototypeScripting;

public class ActionBuildSetup : Action
{
    public enum Type { ActivateDevAccess }
    public Type type;

    // Start is called before the first frame update
    override public void ExecuteAction()
    {
        switch(type)
        {
            case Type.ActivateDevAccess:
                BuildVersionSetup_Ingame.OverwriteDevelopmentAccess();
                break;

        }
    }

    // Update is called once per frame
    override public string GetAdditionalInfo()
    {
        return "";
    }
}
