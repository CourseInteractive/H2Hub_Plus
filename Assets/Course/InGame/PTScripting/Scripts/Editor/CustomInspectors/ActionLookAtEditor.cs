using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionLookAt))]
[CanEditMultipleObjects]

public class ActionLookAtEditor : Editor
{

    ActionLookAt action;

    public override void OnInspectorGUI()
    {
        action = target as ActionLookAt;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        so.FindProperty("character").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Character: ", action.character, typeof(GameObject), true);
        action.type = (ActionLookAt.ActionType)EditorGUILayout.EnumPopup("Action Type: ", action.type);
        so.FindProperty("type").enumValueIndex = (int)action.type;
        if(action.type == ActionLookAt.ActionType.Once || action.type == ActionLookAt.ActionType.Follow)
        {
            so.FindProperty("target").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Target: ", action.target, typeof(GameObject), true);
            so.FindProperty("time").floatValue = EditorGUILayout.FloatField("Time: ", action.time);
            so.FindProperty("waitForCompletion").boolValue = EditorGUILayout.Toggle("Wait for completion: ", action.waitForCompletion);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }

}
