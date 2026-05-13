using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Course.PrototypeScripting;

namespace Course.PrototypeScripting
{
    public class ActionCamFollow : Action
    {
        public GameObject objToFollow;
        public enum ActionType { SetFollowObject, NoFollow}
        public ActionType type;
        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            if(type == ActionType.SetFollowObject)
            {
                if (UnityEngine.Camera.main.GetComponent<CamStatic>())
                    UnityEngine.Camera.main.GetComponent<CamStatic>().ChangeBehaviourToFollow(objToFollow);
                else
                    Debug.LogError("Nur Cameras mit 'CamStatic' können bestimmten Objekten folgen. Die MainCamera hat kein solches Script.");
            }
            else
            {
                if (UnityEngine.Camera.main.GetComponent<CamStatic>())
                    UnityEngine.Camera.main.GetComponent<CamStatic>().ChangeBehaviourToStatic();
                else
                    Debug.LogError("Nur Cameras mit 'CamStatic' können bestimmten Objekten folgen. Die MainCamera hat kein solches Script.");
            }
            SequenceHandler.Instance.ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (type == ActionType.SetFollowObject)
            {
                if (objToFollow == null)
                    return "! No Object to follow set";
                else
                    return "Follow object: " + objToFollow.name;

            }
            else
            {
                return "Clear Follow";
            }
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(ActionCamFollow))]
[CanEditMultipleObjects]
public class ActionCamFollowEditor : Editor
{
    ActionCamFollow action;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        action = target as ActionCamFollow;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        action.type = (ActionCamFollow.ActionType)EditorGUILayout.EnumPopup("Type: ", action.type);
        so.FindProperty("type").enumValueIndex = (int)action.type;
        if (action.type == ActionCamFollow.ActionType.SetFollowObject)
        {
            so.FindProperty("objToFollow").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Obj To Follow: ", action.objToFollow, typeof(GameObject), true);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);

        so.ApplyModifiedProperties();
    }

}

#endif
