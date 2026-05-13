using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionAnimatorParameter))]
[CanEditMultipleObjects]
public class ActionAnimatorEditor : Editor
{

    ActionAnimatorParameter main;
    public override void OnInspectorGUI()
    {
        main = target as ActionAnimatorParameter;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        so.FindProperty("animatedObject").objectReferenceValue = (Animator)EditorGUILayout.ObjectField("Animated Object:", main.animatedObject, typeof(Animator), true);

        so.FindProperty("parameterName").stringValue = EditorGUILayout.TextField("Parameter Name: ", main.parameterName);
        main.parametertype = (ActionAnimatorParameter.ParameterType)EditorGUILayout.EnumPopup("Type: ", main.parametertype);
        so.FindProperty("parametertype").enumValueIndex = (int)main.parametertype;
        switch (main.parametertype)
        {
            case ActionAnimatorParameter.ParameterType.Float:
                so.FindProperty("floatValue").floatValue = EditorGUILayout.FloatField("Value: ", main.floatValue);
                break;
            case ActionAnimatorParameter.ParameterType.Int:
                so.FindProperty("intValue").intValue = EditorGUILayout.IntField("Value: ", main.intValue);
                break;
            case ActionAnimatorParameter.ParameterType.Bool:
                so.FindProperty("boolValue").boolValue = EditorGUILayout.Toggle("Value: ", main.boolValue);
                break;
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(main);
        so.ApplyModifiedProperties();
    }
}
