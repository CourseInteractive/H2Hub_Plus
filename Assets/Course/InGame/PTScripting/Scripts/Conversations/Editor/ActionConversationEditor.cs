using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionConversation))]
[CanEditMultipleObjects]
public class ActionConversationEditor : Editor
{
    ActionConversation main;

    public override void OnInspectorGUI()
    {
        main = target as ActionConversation;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        so.FindProperty("conversation").objectReferenceValue = (Conversation)EditorGUILayout.ObjectField("Conversation: ", main.conversation, typeof(Conversation), true);
        if (main.conversation == null)
        {
            if (GUILayout.Button("Create as Child"))
            {
                so.FindProperty("conversation").objectReferenceValue = CreateNewConversationAsChild(true);
            }
        }
        EditorGUILayout.EndHorizontal();
        main.type = (ActionConversation.Type)EditorGUILayout.EnumPopup("Type: ", main.type);
        so.FindProperty("type").enumValueIndex = (int)main.type;
        if(main.type == ActionConversation.Type.SetOption)
        {
            if(main.conversation == null)
            {
                EditorGUILayout.LabelField("No Conversation Set!");
                so.ApplyModifiedProperties();
                return;
            }
            string[] optionNames = main.conversation.GetOptionNames();
            so.FindProperty("index").intValue = (int)EditorGUILayout.Popup("Option: ", main.index, optionNames);
            so.FindProperty("enableValue").boolValue = EditorGUILayout.Toggle("Enable?: ", main.enableValue);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(main);
        so.ApplyModifiedProperties();
    }

    Conversation CreateNewConversationAsChild(bool ifTrue)
    {
        GameObject newGO = new GameObject("NewConversation");
        Conversation s = newGO.AddComponent<Conversation>();
        newGO.name = main.gameObject.name + "_Conversation";
        newGO.transform.parent = main.transform;
        Selection.activeObject = newGO;
        return s;
    }
}
