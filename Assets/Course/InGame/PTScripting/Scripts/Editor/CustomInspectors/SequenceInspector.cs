using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Sequence))]
[CanEditMultipleObjects]


public class SequenceInspector : Editor
{
    Sequence main;

    public override void OnInspectorGUI()
    {
        main = target as Sequence;
        SerializedObject so = new SerializedObject(target);
        
        string[] gameStateNames = System.Enum.GetNames(typeof(RuntimeGlobal.GameState));
        string[] modifiedNames = new string[gameStateNames.Length + 1];
        modifiedNames[0] = "All";
        for (int i = 0; i < gameStateNames.Length; i++)
            modifiedNames[i + 1] = gameStateNames[i];

        main.allowedGameState = EditorGUILayout.Popup("Execute only in Game State: ", main.allowedGameState+1, modifiedNames)-1;
        EditorGUI.BeginChangeCheck();
        so.FindProperty("allowedGameState").intValue = main.allowedGameState;
        if (main.priority == 0)
            main.priority = 1;
    
        main.priority = EditorGUILayout.Popup("Priority:", main.priority - 1, new string[5] { "1", "2", "3", "4", "5" }) + 1;
        so.FindProperty("priority").intValue = main.priority;
        if(EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(main);
        }

            Component[] list = main.gameObject.transform.GetComponents(typeof(Action));
        main.actions = new Action[list.Length];
        for (int i = 0; i < list.Length; i++)
            main.actions[i] = (Action)list[i];
        GUIStyle headerStyle = new GUIStyle();
        headerStyle.fontSize = 15;
        headerStyle.fontStyle = FontStyle.Bold;
        headerStyle.normal.textColor = Color.white;
        headerStyle.alignment = TextAnchor.MiddleCenter;
        EditorGUILayout.LabelField("List of actions in order", headerStyle );
        EditorGUILayout.Space();
        for(int i = 0; i < list.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(i + ": " + main.actions[i].GetType().Name.ToString(), GUILayout.Width(135));
            EditorGUILayout.LabelField(main.actions[i].GetAdditionalInfo());
            EditorGUILayout.EndHorizontal();
        }
           

    }
}
