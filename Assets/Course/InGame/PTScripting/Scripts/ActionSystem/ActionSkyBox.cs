using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionSkyBox : Action
    {
        public Material skyBox;

        override public void ExecuteAction()
        {
            RenderSettings.skybox = skyBox;
            ReportActionEnd();
        }
    }
}