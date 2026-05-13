using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Conversation))]
[CanEditMultipleObjects]

public class ConversationEditor : Editor
{

    Conversation main;
    public override void OnInspectorGUI()
    {
        

        main = target as Conversation;
        SerializedObject so = new SerializedObject(target);
        EditorGUILayout.BeginHorizontal();
        so.FindProperty("welcomeSequence").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField("Welcome Seq: ", main.welcomeSequence, typeof(Sequence), true);
        if (main.welcomeSequence == null)
        {
            if (GUILayout.Button("Create as Child"))
            {
                so.FindProperty("welcomeSequence").objectReferenceValue = CreateNewSequenceAsChild("welcome");
            }
        }

        EditorGUILayout.EndHorizontal();
        if (main.options == null)
            main.Init();
        EditorGUILayout.Space();
        DrawLine(Color.grey);
        EditorGUILayout.LabelField("Conversation Options:");
        DrawLine(Color.grey);
        SerializedProperty _optionsListProperty = so.FindProperty("options");
        int _length = _optionsListProperty.arraySize;
        for (int i = 0; i < _length; i++)
            {
            SerializedProperty op = _optionsListProperty.GetArrayElementAtIndex(i);
            if(!op.FindPropertyRelative("open").boolValue)
            {
                EditorGUILayout.BeginHorizontal();
                op.FindPropertyRelative("open").boolValue = EditorGUILayout.Foldout(op.FindPropertyRelative("open").boolValue, op.FindPropertyRelative("name").stringValue, true, EditorStyles.foldout);
                if(GUILayout.Button("Delete"))
                {
                    DeleteEntry(so, i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                EditorGUILayout.EndHorizontal();
                DrawLine(Color.grey);
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                op.FindPropertyRelative("open").boolValue = EditorGUILayout.Foldout(op.FindPropertyRelative("open").boolValue, op.FindPropertyRelative("name").stringValue, true, EditorStyles.foldout);
                op.FindPropertyRelative("name").stringValue = EditorGUILayout.TextField("Change Name: ", op.FindPropertyRelative("name").stringValue);
                EditorGUILayout.EndHorizontal();
                   
                EditorGUILayout.LabelField("Text in Conversation:");
                op.FindPropertyRelative("text").stringValue = EditorGUILayout.TextArea(op.FindPropertyRelative("text").stringValue);
                EditorGUILayout.BeginHorizontal();
                op.FindPropertyRelative("sequence").objectReferenceValue = EditorGUILayout.ObjectField("Sequence:", op.FindPropertyRelative("sequence").objectReferenceValue, typeof(Sequence), true);
                if (op.FindPropertyRelative("sequence").objectReferenceValue == null)
                {
                    if (GUILayout.Button("Create as Child"))
                    {
                        op.FindPropertyRelative("sequence").objectReferenceValue = CreateNewSequenceAsChild(op.FindPropertyRelative("name").stringValue);
                    }
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                op.FindPropertyRelative("enabled").boolValue = EditorGUILayout.Toggle("Enabled: ", op.FindPropertyRelative("enabled").boolValue);
                op.FindPropertyRelative("endConversationAfterwards").boolValue = EditorGUILayout.Toggle("End Conv. after this: ", op.FindPropertyRelative("endConversationAfterwards").boolValue);
                EditorGUILayout.EndHorizontal();
                SerializedProperty conditionProp = op.FindPropertyRelative("condition");
                EditorGUILayout.PropertyField(conditionProp, new GUIContent("Option Condition"), true);
                EditorGUILayout.Space();

                DrawLine(Color.grey);
            }

        }

        if(GUILayout.Button("Add Option"))
        {
            SerializedProperty optionsListProperty = so.FindProperty("options");
            int length = optionsListProperty.arraySize;
            optionsListProperty.InsertArrayElementAtIndex(length);
            optionsListProperty.GetArrayElementAtIndex(length).FindPropertyRelative("name").stringValue = "NEW";
        }

        so.ApplyModifiedProperties();
    }

    void DrawLine(Color color)
    {
        GUIStyle horizontalLine;
        horizontalLine = new GUIStyle();
        horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
        horizontalLine.margin = new RectOffset(0, 0, 4, 4);
        horizontalLine.fixedHeight = 1;
        var c = GUI.color;
        GUI.color = color;
        GUILayout.Box(GUIContent.none, horizontalLine);
        GUI.color = c;
    }

    void DeleteEntry(SerializedObject so,int index)
    {
        SerializedProperty optionsListProperty = so.FindProperty("options");
        optionsListProperty.DeleteArrayElementAtIndex(index);
    }

    Sequence CreateNewSequenceAsChild(string optionName)
    {
        GameObject newGO = new GameObject(optionName);
        Sequence s = newGO.AddComponent<Sequence>();
        newGO.name = main.gameObject.name + "_" + optionName;
        newGO.transform.parent = main.transform;
        Selection.activeObject = newGO;
        return s;
    }


}
