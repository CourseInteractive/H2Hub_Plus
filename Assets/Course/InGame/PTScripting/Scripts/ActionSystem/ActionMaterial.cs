using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionMaterial : Action
    {

        public MeshRenderer rendererToChange;
        public Material newMaterial;


        override public void ExecuteAction()
        {
            rendererToChange.material = newMaterial;
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (rendererToChange == null || newMaterial == null)
                return "- No Renderer or material set!";
            return rendererToChange.gameObject.name + " => " + newMaterial.name;
        }
    }

}