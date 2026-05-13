using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionSetActivity : Action
    {
        public GameObject obj;
        public bool state;
        override public void ExecuteAction()
        {
            obj.SetActive(state);
            // NUR IN RZZ LLK4 VR
         //   if (ObjectInfoManager._instance)
          //      ObjectInfoManager.Instance.UpdateDisplayedPanels();
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (obj == null)
                return "- No Object set!";
            return obj.name + " => Set " + state.ToString();
        }

    }
}