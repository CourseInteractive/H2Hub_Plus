using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(InventoryCombination))]
[CanEditMultipleObjects]

public class InventoryCombinationEditor : Editor
{

    InventoryCombination main;

    InventoryData invData;
    string[] invItemNames;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        main = target as InventoryCombination;
        SerializedObject so = new SerializedObject(target);
        if (invData == null)
            LoadData();

        if (invData == null || invData.invItems == null || invData.invItems.Count == 0)
        {
            EditorGUILayout.LabelField("Keine Inventory Items erstellt.");
            EditorGUILayout.LabelField("Erstelle diese unter 'Simple Game' im Menü");
            return;
        }

        int currentIndex = 0;
        for (int i = 0; i < invItemNames.Length; i++)
        {
            if (invItemNames[i] == main.invItemName)
                currentIndex = i;
        }

        so.FindProperty("invItemName").stringValue = invItemNames[EditorGUILayout.Popup("Inventory Item: ", currentIndex, invItemNames)];
        so.FindProperty("sequenceOnCombination").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField("Sequence on combination: ", main.sequenceOnCombination, typeof(Sequence), true);
        so.ApplyModifiedProperties();
    }

    void LoadData()
    {
        invData = Resources.Load<InventoryData>("InventoryData");
        if (invData != null)
            invItemNames = invData.GetNames().ToArray();
    }
}
