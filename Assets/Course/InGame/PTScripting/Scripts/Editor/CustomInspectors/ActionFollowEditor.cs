using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionFollow))]
[CanEditMultipleObjects]

public class ActionFollowEditor : Editor
{
    ActionFollow action;

    public override void OnInspectorGUI()
    {
        action = target as ActionFollow;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        action.type = (ActionFollow.Type)EditorGUILayout.EnumPopup("Action Type: ", action.type);
        so.FindProperty("type").enumValueIndex = (int)action.type;
        so.FindProperty("character").objectReferenceValue = (MovementIndirect)EditorGUILayout.ObjectField("Character: ", action.character, typeof(MovementIndirect), true);
        if(action.type == ActionFollow.Type.Start)
        {
            so.FindProperty("objectToFollow").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Object to Follow: ", action.objectToFollow, typeof(GameObject), true);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }
}
