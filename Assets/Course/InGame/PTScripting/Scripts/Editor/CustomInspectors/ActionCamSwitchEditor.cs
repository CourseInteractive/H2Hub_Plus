using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionCamSwitch))]
[CanEditMultipleObjects]

public class ActionCamSwitchEditor : Editor
{
    ActionCamSwitch action;

    public override void OnInspectorGUI()
    {
        action = target as ActionCamSwitch;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        action.type = (ActionCamSwitch.Type)EditorGUILayout.EnumPopup("Type: ", action.type);
        so.FindProperty("type").enumValueIndex = (int)action.type;
        so.FindProperty("time").floatValue = EditorGUILayout.FloatField("Time : ", action.time);
        if (action.type != ActionCamSwitch.Type.BackToMovingCam)
        {
            so.FindProperty("camTarget").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Cam Target: ", action.camTarget, typeof(GameObject), true);
        }
        else
        {
           // so.FindProperty("copyYRotationOf").objectReferenceValue = (Transform)EditorGUILayout.ObjectField("Copy Y Rotation of: ", action.copyYRotationOf, typeof(Transform), true);
        }
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(action);
        }
        so.ApplyModifiedProperties();
    }
}
