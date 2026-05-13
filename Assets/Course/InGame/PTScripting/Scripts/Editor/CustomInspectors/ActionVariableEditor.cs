using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionVariable))]
[CanEditMultipleObjects]
public class ActionVariableEditor : Editor
{
    ActionVariable action;

    VariableData varData;
    string[] variableNames;
    public override void OnInspectorGUI()
    {
        action = target as ActionVariable;
        
        SerializedObject so = new SerializedObject(target);
        if (varData == null)
            LoadData();
        CheckSource();
        if (GUILayout.Button("Update Data"))
            LoadData();
        if (varData == null || varData.variableInfos == null || varData.variableInfos.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Variablen erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Variable Editor im Menü");
            return;
        }
        EditorGUILayout.PropertyField(so.FindProperty("source"));
        int currentIndex = 0;
        for(int i = 0; i< variableNames.Length; i++)
        {
            if (variableNames[i] == action.variableName)
                currentIndex = i;
        }
        so.FindProperty("variableName").stringValue = variableNames[EditorGUILayout.Popup("Variable: ", currentIndex, variableNames)];
        EditorGUILayout.PropertyField(so.FindProperty("action"));
        SerializedProperty valProp = so.FindProperty("value");

        valProp.intValue = EditorGUILayout.IntField("Value: ", valProp.intValue);
        so.ApplyModifiedProperties();
        CheckSource();
    }

    void CheckSource()
    {
        if (action.source == ActionVariable.Source.Global)
        {
            if (varData != null)
                variableNames = varData.GetNames().ToArray();
        }
        else
        {
            if (varData != null)
                variableNames = varData.GetLocalNames(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).ToArray();
        }
    }

    void LoadData()
    {
        varData = Resources.Load<VariableData>("VariableData");
        
       
    }
}
