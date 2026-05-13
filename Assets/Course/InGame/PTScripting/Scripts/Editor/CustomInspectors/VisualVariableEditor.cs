using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VariableAsImageDisplay))]
[CanEditMultipleObjects]
public class VisualVariableEditor : Editor
{
    VariableAsImageDisplay main;
    VariableData varData;
    string[] variableNames;

    public override void OnInspectorGUI()
    {
        main = target as VariableAsImageDisplay;
        SerializedObject so = new SerializedObject(target);
        if (varData == null)
            LoadData();

        if (varData.variableInfos == null || varData.variableInfos.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Variablen erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Variable Editor im Menü");
            return;
        }
        EditorGUI.BeginChangeCheck();
        int currentIndex = 0;
        for (int i = 0; i < variableNames.Length; i++)
        {
            if (variableNames[i] == main.variableName)
                currentIndex = i;
        }

        so.FindProperty("variableName").stringValue = variableNames[EditorGUILayout.Popup(currentIndex, variableNames)];
        so.FindProperty("maxAmount").intValue = EditorGUILayout.IntField("Maximum Amount:", main.maxAmount);

        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(main);
        so.ApplyModifiedProperties();
    }

    void LoadData()
    {
        varData = Resources.Load<VariableData>("VariableData");
        variableNames = varData.GetNames().ToArray();
    }
}
