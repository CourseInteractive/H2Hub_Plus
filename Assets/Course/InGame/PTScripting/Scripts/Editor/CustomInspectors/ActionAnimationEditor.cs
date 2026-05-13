using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionAnimation))]
[CanEditMultipleObjects]

public class ActionAnimationEditor : Editor
{
    ActionAnimation action;

    public override void OnInspectorGUI()
    {
        action = target as ActionAnimation;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();

        action.actionType = (ActionAnimation.ActionType)EditorGUILayout.EnumPopup("Action Type: ", action.actionType);
        so.FindProperty("actionType").enumValueIndex = (int)action.actionType;
        so.FindProperty("animationComponent").objectReferenceValue = (Animation)EditorGUILayout.ObjectField("Animated Object: ", action.animationComponent, typeof(Animation), true); 
        if (action.actionType == ActionAnimation.ActionType.Play)
        {
            so.FindProperty("animationName").stringValue = EditorGUILayout.TextField("Animation Name: ", action.animationName);
            so.FindProperty("waitUntilEnded").boolValue = EditorGUILayout.Toggle("Wait until ended: ", action.waitUntilEnded);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }
}
