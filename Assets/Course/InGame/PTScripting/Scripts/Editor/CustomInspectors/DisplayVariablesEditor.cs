using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VariableAsTextDisplay))]
[CanEditMultipleObjects]

public class DisplayVariablesEditor : Editor
{


    VariableAsTextDisplay main;
    VariableData varData;
    string[] variableNames;

    public override void OnInspectorGUI()
    {
        main = target as VariableAsTextDisplay;
        SerializedObject so = new SerializedObject(target);
        if (varData == null)
            LoadData();

        //if (GUILayout.Button("Update Data"))
         //   LoadData();

        if (varData.variableInfos == null || varData.variableInfos.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Variablen erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Variable Editor im Menü");
            return;
        }
        int currentIndex = 0;
        for (int i = 0; i < variableNames.Length; i++)
        {
            if (variableNames[i] == main.variableName)
                currentIndex = i;
        }
        so.FindProperty("permanentUpdate").boolValue = EditorGUILayout.ToggleLeft("Update permanent (for testing): ", main.permanentUpdate);
        main.style = (VariableAsTextDisplay.Style)EditorGUILayout.EnumPopup("Style: ",main.style);
        so.FindProperty("style").enumValueIndex = (int)main.style;
        switch(main.style)
        {
            case VariableAsTextDisplay.Style.Simple:
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Text: ");
                so.FindProperty("prefix").stringValue = EditorGUILayout.TextField(main.prefix);
                so.FindProperty("variableName").stringValue = variableNames[EditorGUILayout.Popup(currentIndex, variableNames)];
                EditorGUILayout.EndHorizontal();
                break;
            case VariableAsTextDisplay.Style.Complex:
                EditorGUILayout.Space(); EditorGUILayout.Space();

                EditorGUILayout.LabelField("Variables: ");
                SerializedProperty displayedVariableNames = so.FindProperty("variableNames");
                int length = displayedVariableNames.arraySize;
                for (int i = 0; i < length; i++)
                {
                    string currentValue = displayedVariableNames.GetArrayElementAtIndex(i).stringValue;
                    int internIndex = 0;
                    for (int j = 0; j < variableNames.Length; j++)
                    {
                        if (variableNames[j] == currentValue)
                            internIndex = j;
                    }
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("{" + i + "} :");
                    displayedVariableNames.GetArrayElementAtIndex(i).stringValue = variableNames[EditorGUILayout.Popup(internIndex, variableNames)];
                    EditorGUILayout.EndHorizontal();
                }

                if (GUILayout.Button("Add Variable to show"))
                {
                    SerializedProperty _displayedVariableNames = so.FindProperty("variableNames");
                    int _length = _displayedVariableNames.arraySize;
                    _displayedVariableNames.InsertArrayElementAtIndex(length);
                    _displayedVariableNames.GetArrayElementAtIndex(length).stringValue = variableNames[0];
                }
                EditorGUILayout.Space(); EditorGUILayout.Space();
                EditorGUILayout.LabelField("Text: ");
                so.FindProperty("prefix").stringValue = EditorGUILayout.TextArea(main.prefix);

                break;
        }
   
      //  so.FindProperty("textUI").objectReferenceValue = (UnityEngine.UI.Text)EditorGUILayout.ObjectField("UI Text element:", main.textUI, typeof(UnityEngine.UI.Text), true);
        so.ApplyModifiedProperties();

    }

    void LoadData()
    {
        varData = Resources.Load<VariableData>("VariableData");
        variableNames = varData.GetNames().ToArray();
    }
}
