using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionCheckInventory))]
[CanEditMultipleObjects]

public class ActionCheckInventoryEditor : Editor
{
    ActionCheckInventory action;
    InventoryData invData;
    string[] invItemNames;
    string[] operations = { "=", ">", "> =", "<", "< =" };

    public override void OnInspectorGUI()
    {
        action = target as ActionCheckInventory;
        SerializedObject so = new SerializedObject(target);
        if (invData == null)
            LoadData();

        if (invData == null || invData.invItems == null || invData.invItems.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Inventory Items erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter SimpleGame > Inventory im Menü");
            return;
        }
        int currentIndex = 0;

        for (int i = 0; i < invItemNames.Length; i++)
        {
            if (invItemNames[i] == action.invItemName)
                currentIndex = i;
        }

        EditorGUILayout.LabelField("IF");
        EditorGUILayout.BeginHorizontal();
        so.FindProperty("invItemName").stringValue = invItemNames[EditorGUILayout.Popup(currentIndex, invItemNames)];
        action.vergleich = (ActionCheckInventory.Comparison)EditorGUILayout.Popup((int)action.vergleich, operations);
        so.FindProperty("vergleich").enumValueIndex = (int)action.vergleich;
        so.FindProperty("amount").intValue = EditorGUILayout.IntField(action.amount);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.LabelField("TRUE -> Sequence ");
        EditorGUILayout.BeginHorizontal();
        so.FindProperty("sequenceIfTrue").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField(action.sequenceIfTrue, typeof(Sequence), true);
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

    void LoadData()
    {
        invData = Resources.Load<InventoryData>("InventoryData");
        if (invData != null)
            invItemNames = invData.GetNames().ToArray();
    }

}
