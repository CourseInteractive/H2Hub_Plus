using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Course.PrototypeScripting
{
    [CustomEditor(typeof(ActionConditionSwitch))]
    [CanEditMultipleObjects]
    public class ActionConditionSwitchEditor : Editor
    {
        ActionConditionSwitch action;

        VariableData varData;
        string[] variableNames;
        string[] operations = { "=", ">", "> =", "<", "< =" };

        public override void OnInspectorGUI()
        {
            action = target as ActionConditionSwitch;
            SerializedObject so = new SerializedObject(target);

            if (varData == null)
                LoadData();

            if (GUILayout.Button("Update Data"))
                LoadData();

            if (varData == null || varData.variableInfos == null || varData.variableInfos.Count == 0)
            {
                EditorGUILayout.LabelField("Keine Variablen erstellt.");
                EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Variable Editor im Menü");
                return;
            }

            CheckSource();

            EditorGUILayout.PropertyField(so.FindProperty("source"));

            // --- Variable selection ---
            int currentIndex = 0;
            for (int i = 0; i < variableNames.Length; i++)
            {
                if (variableNames[i] == action.variableName)
                    currentIndex = i;
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("SWITCH ON", GUILayout.Width(80));
            so.FindProperty("variableName").stringValue =
                variableNames[EditorGUILayout.Popup(currentIndex, variableNames)];
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // --- Cases list ---
            SerializedProperty casesProp = so.FindProperty("cases");

            EditorGUI.BeginChangeCheck();

            for (int i = 0; i < casesProp.arraySize; i++)
            {
                SerializedProperty caseProp = casesProp.GetArrayElementAtIndex(i);
                SerializedProperty comparisonProp = caseProp.FindPropertyRelative("comparison");
                SerializedProperty valueProp = caseProp.FindPropertyRelative("value");
                SerializedProperty sequenceProp = caseProp.FindPropertyRelative("sequence");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("CASE " + (i + 1), EditorStyles.boldLabel, GUILayout.Width(60));

                // Comparison operator popup
                comparisonProp.enumValueIndex = EditorGUILayout.Popup(comparisonProp.enumValueIndex, operations, GUILayout.Width(50));

                // Value int field
                valueProp.intValue = EditorGUILayout.IntField(valueProp.intValue);

                // Remove button
                GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
                if (GUILayout.Button("✕", GUILayout.Width(24)))
                {
                    casesProp.DeleteArrayElementAtIndex(i);
                    so.ApplyModifiedProperties();
                    GUI.backgroundColor = Color.white;
                    break;
                }
                GUI.backgroundColor = Color.white;
                EditorGUILayout.EndHorizontal();

                // Sequence field
                EditorGUILayout.BeginHorizontal();
                sequenceProp.objectReferenceValue =
                    (Sequence)EditorGUILayout.ObjectField(
                        sequenceProp.objectReferenceValue, typeof(Sequence), true);

                if (sequenceProp.objectReferenceValue == null)
                {
                    if (GUILayout.Button("Create as Child", GUILayout.Width(120)))
                    {
                        sequenceProp.objectReferenceValue = CreateNewSequenceAsChild("Case" + (i + 1));
                        so.ApplyModifiedProperties();
                    }
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(2);
            }

            // --- Add Case button ---
            GUI.backgroundColor = new Color(0.4f, 0.8f, 0.4f);
            if (GUILayout.Button("+ Add Case"))
            {
                casesProp.InsertArrayElementAtIndex(casesProp.arraySize);
                // Reset newly added element
                SerializedProperty newCase = casesProp.GetArrayElementAtIndex(casesProp.arraySize - 1);
                newCase.FindPropertyRelative("comparison").enumValueIndex = 0;
                newCase.FindPropertyRelative("value").intValue = 0;
                newCase.FindPropertyRelative("sequence").objectReferenceValue = null;
                so.ApplyModifiedProperties();
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space();

            // --- Default / Else ---
            EditorGUILayout.LabelField("DEFAULT (no case matched)", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            so.FindProperty("defaultSequence").objectReferenceValue =
                (Sequence)EditorGUILayout.ObjectField(
                    action.defaultSequence, typeof(Sequence), true);

            if (action.defaultSequence == null)
            {
                if (GUILayout.Button("Create as Child", GUILayout.Width(120)))
                {
                    so.FindProperty("defaultSequence").objectReferenceValue =
                        CreateNewSequenceAsChild("Default");
                    so.ApplyModifiedProperties();
                }
            }
            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(action);
            }

            so.ApplyModifiedProperties();
            CheckSource();
        }

        Sequence CreateNewSequenceAsChild(string label)
        {
            GameObject newGO = new GameObject("NewSequence");
            Sequence s = newGO.AddComponent<Sequence>();
            newGO.name = action.gameObject.name + "_" + label;
            newGO.transform.parent = action.transform;
            Selection.activeObject = newGO;
            return s;
        }

        void LoadData()
        {
            varData = Resources.Load<VariableData>("VariableData");
        }

        void CheckSource()
        {
            if (action.source == ActionOnCondition.Source.Global)
            {
                if (varData != null)
                    variableNames = varData.GetNames().ToArray();
            }
            else
            {
                if (varData != null)
                    variableNames = varData.GetLocalNames(
                        UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).ToArray();
            }
        }
    }
}
