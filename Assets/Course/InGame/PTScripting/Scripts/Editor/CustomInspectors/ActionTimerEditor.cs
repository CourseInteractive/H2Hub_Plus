using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionTimer))]
[CanEditMultipleObjects]

public class ActionTimerEditor : Editor
{
    ActionTimer action;

    public override void OnInspectorGUI()
    {
        action = target as ActionTimer;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();

        action.type = (ActionTimer.Type)EditorGUILayout.EnumPopup("Action Type: ", action.type);
        so.FindProperty("type").enumValueIndex = (int)action.type;
        so.FindProperty("timerName").stringValue = EditorGUILayout.TextField("Timer Name: ", action.timerName);
        if (action.type == ActionTimer.Type.Start)
        {
            so.FindProperty("timeToRun").floatValue = EditorGUILayout.FloatField("Time to run: ", action.timeToRun);
            so.FindProperty("sequenceWhenRunOut").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField("Sequence: ", action.sequenceWhenRunOut, typeof(Sequence), true);
        }
        else if (action.type == ActionTimer.Type.StartRandom)
        {
            Vector2 limits = new Vector2(action.randomLimits.x, action.randomLimits.y);
            limits.x = EditorGUILayout.FloatField("Minimum time: ", limits.x);
            limits.y = EditorGUILayout.FloatField("Maximum time: ", limits.y);
            so.FindProperty("randomLimits").vector2Value = limits;
            so.FindProperty("sequenceWhenRunOut").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField("Sequence: ", action.sequenceWhenRunOut, typeof(Sequence), true);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }
}
