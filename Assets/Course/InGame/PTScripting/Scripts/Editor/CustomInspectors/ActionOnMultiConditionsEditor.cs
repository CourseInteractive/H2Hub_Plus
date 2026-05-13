using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionOnMultiConditions))]
[CanEditMultipleObjects]
public class ActionOnMultiConditionsEditor : Editor
{
    ActionOnMultiConditions action;

    VariableData varData;
    string[] variableNames;
    string[] operations = { "=", ">", ">=", "<", "<=", "!=" };

    public override void OnInspectorGUI()
    {
        action = target as ActionOnMultiConditions;
        SerializedObject so = new SerializedObject(target);
        
        if (varData == null)
            LoadData();
        CheckEntryToDelete();
        if (GUILayout.Button("Update Data"))
            LoadData();

        if (varData == null || varData.variableInfos == null || varData.variableInfos.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Variablen erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Variable Editor im Menü");
            return;
        }
        
        EditorGUILayout.LabelField("IF");
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
     //   so.FindProperty("variableName").stringValue = variableNames[EditorGUILayout.Popup(currentIndex, variableNames)];
     //   action.compType = (ActionOnMultiConditions.Comparison)EditorGUILayout.Popup((int)action.compType, operations);
     //   so.FindProperty("compType").enumValueIndex = (int)action.compType;
       
        EditorGUILayout.EndHorizontal();
        if (action.comparisons == null || action.comparisons.Length == 0)
            AddEmptyToList();
        for (int pairIndex = 0; pairIndex < action.comparisons.Length; pairIndex++)
        {
            DisplayComparisonPair(pairIndex);
        }
        if(GUILayout.Button("+"))
        {
            AddEmptyToList();
        }
        EditorGUILayout.LabelField("TRUE -> Sequence ");
        EditorGUILayout.BeginHorizontal();
        so.FindProperty("sequenceIfTrue").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField(action.sequenceIfTrue, typeof(Sequence),true);
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
        so.FindProperty("sequenceIfFalse").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField(action.sequenceIfFalse, typeof(Sequence), true);
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

    void AddEmptyToList()
    {
        if (action.comparisons == null)
        {
            action.comparisons = new ActionOnMultiConditions.ComparisonPair[1];
            action.comparisons[0] = new ActionOnMultiConditions.ComparisonPair();
            return;
        }
            
        ActionOnMultiConditions.ComparisonPair[] newArray = new ActionOnMultiConditions.ComparisonPair[action.comparisons.Length + 1];
        for(int i = 0; i < action.comparisons.Length; i++)
        {
            newArray[i] = action.comparisons[i];
        }
        newArray[newArray.Length-1] = new ActionOnMultiConditions.ComparisonPair();
        action.comparisons = newArray;
    }

    void DisplayComparisonPair(int index)
    {
        int currentIndex = 0;
        for (int i = 0; i < variableNames.Length; i++)
        {
            if (variableNames[i] == action.comparisons[index].varName)
                currentIndex = i;
        }
        EditorGUILayout.BeginHorizontal();
        action.comparisons[index].varName = variableNames[EditorGUILayout.Popup(currentIndex, variableNames, GUILayout.Width(150))];
        //EditorGUILayout.LabelField(GetStringForComparisonType(), GUILayout.Width(50));
        action.comparisons[index].comp = (ActionOnMultiConditions.ExtComparison)EditorGUILayout.Popup((int)action.comparisons[index].comp, operations);
        action.comparisons[index].varValue = EditorGUILayout.IntField(action.comparisons[index].varValue);
        if (GUILayout.Button("X"))
            MarkEntryForDelete(index);
        EditorGUILayout.EndHorizontal();
    }
    int entryToDelete = -1;
    void MarkEntryForDelete(int index)
    {
       
        entryToDelete = index;
    }

    void CheckEntryToDelete()
    {
        if (entryToDelete == -1)
            return;

        if (action.comparisons.Length == 1)
        {
            action.comparisons = new ActionOnMultiConditions.ComparisonPair[1];
            action.comparisons[0] = new ActionOnMultiConditions.ComparisonPair();
            entryToDelete = -1;
            return;
        }

        ActionOnMultiConditions.ComparisonPair[] newArray = new ActionOnMultiConditions.ComparisonPair[action.comparisons.Length - 1];
        for (int i = 0; i < action.comparisons.Length; i++)
        {
            if (i == entryToDelete)
                continue;
            else if (i > entryToDelete)
                newArray[i - 1] = action.comparisons[i];
            else
                newArray[i] = action.comparisons[i];
        }
        action.comparisons = newArray;
        //   
        //       action.comparisons.RemoveAt(entryToDelete);
           entryToDelete = -1;
    }

    void LoadData()
    {
        varData = Resources.Load<VariableData>("VariableData");
        if(varData != null)
             variableNames = varData.GetNames().ToArray();
    }
}

