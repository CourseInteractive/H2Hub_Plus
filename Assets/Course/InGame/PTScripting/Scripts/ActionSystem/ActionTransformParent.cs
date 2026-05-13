using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ActionTransformParent : Action
    {
        public Transform objToChange;
        public Transform parent;

        override public void ExecuteAction()
        {
            objToChange.parent = parent;
            SequenceHandler.Instance.ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (objToChange == null)
                return "! No Object set!";

            if (parent == null)
                return "Remove from parent (Free)";
            else
                return "Set new parent of " + objToChange.name + " to " + parent.name;
        }
    }
}
