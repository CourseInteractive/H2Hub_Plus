using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;

using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(ActionNavigationArea))]
[CanEditMultipleObjects]

public class ActionNavigationAreaEditor : Editor
{
    ActionNavigationArea action;

    public override void OnInspectorGUI()
    {
        action = target as ActionNavigationArea;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();

        so.FindProperty("navAgent").objectReferenceValue = (NavMeshAgent)EditorGUILayout.ObjectField("Nav Agent: ", action.navAgent, typeof(NavMeshAgent), true);
        string[] areas = NavMesh.GetAreaNames();
        so.FindProperty("areaIndex").intValue = (int)EditorGUILayout.Popup("Nav Area: ", action.areaIndex, areas);
        so.FindProperty("walkable").boolValue = EditorGUILayout.Toggle("Walkable:", action.walkable);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();

    }
}
