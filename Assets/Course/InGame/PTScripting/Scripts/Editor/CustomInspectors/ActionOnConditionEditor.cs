using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Course.PrototypeScripting
{

    [CustomEditor(typeof(ActionOnCondition))]
    [CanEditMultipleObjects]
    public class ActionOnConditionEditor : Editor
    {
        ActionOnCondition action;

        VariableData varData;
        string[] variableNames;
        string[] operations = {"=", ">", "> =", "<", "< ="};

        public override void OnInspectorGUI()
        {
            action = target as ActionOnCondition;
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
            int currentIndex = 0;
            CheckSource();
                for (int i = 0; i < variableNames.Length; i++)
                {
                    if (variableNames[i] == action.variableName)
                        currentIndex = i;
                }


            EditorGUILayout.PropertyField(so.FindProperty("source"));
            

            EditorGUILayout.LabelField("IF");
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            so.FindProperty("variableName").stringValue =
                variableNames[EditorGUILayout.Popup(currentIndex, variableNames)];
            action.vergleich = (ActionOnCondition.Comparison) EditorGUILayout.Popup((int) action.vergleich, operations);
            so.FindProperty("vergleich").enumValueIndex = (int) action.vergleich;
            so.FindProperty("value").intValue = EditorGUILayout.IntField(action.value);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("TRUE -> Sequence ");
            EditorGUILayout.BeginHorizontal();
            so.FindProperty("sequenceIfTrue").objectReferenceValue =
                (Sequence) EditorGUILayout.ObjectField(action.sequenceIfTrue, typeof(Sequence), true);
            if (action.sequenceIfTrue == null)
            {
                if (GUILayout.Button("Create as Child"))
                {
                    so.FindProperty("sequenceIfTrue").objectReferenceValue = CreateNewSequenceAsChild(true);
                }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.LabelField("FALSE -> Sequence ");
            EditorGUILayout.BeginHorizontal();
            so.FindProperty("sequenceIfFalse").objectReferenceValue =
                (Sequence) EditorGUILayout.ObjectField(action.sequenceIfFalse, typeof(Sequence), true);
            if (action.sequenceIfFalse == null)
            {
                if (GUILayout.Button("Create as Child"))
                {
                    so.FindProperty("sequenceIfFalse").objectReferenceValue = CreateNewSequenceAsChild(false);
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

        Sequence CreateNewSequenceAsChild(bool ifTrue)
        {
            GameObject newGO = new GameObject("NewSequence");
            Sequence s = newGO.AddComponent<Sequence>();
            newGO.name = action.gameObject.name + "_" + ifTrue;
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
                    variableNames = varData.GetLocalNames(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name).ToArray();
            }
        }
    }
}
