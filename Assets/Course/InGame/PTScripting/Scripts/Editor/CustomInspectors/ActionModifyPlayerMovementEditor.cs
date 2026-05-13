using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionModifyPlayerMovement))]
[CanEditMultipleObjects]

public class ActionModifyPlayerMovementEditor : Editor
{

    ActionModifyPlayerMovement action;

    public override void OnInspectorGUI()
    {
        action = target as ActionModifyPlayerMovement;
        SerializedObject so = new SerializedObject(target);
        action.type = (ActionModifyPlayerMovement.MovementType)EditorGUILayout.EnumPopup("Type: ", action.type);
        EditorGUI.BeginChangeCheck();

        so.FindProperty("type").enumValueIndex = (int)action.type;
        if (action.type == ActionModifyPlayerMovement.MovementType.Direct)
        {
            so.FindProperty("changeSpeed").boolValue = EditorGUILayout.Toggle("Change speed:", action.changeSpeed);
            if (so.FindProperty("changeSpeed").boolValue)
                so.FindProperty("speed").floatValue = EditorGUILayout.FloatField("New Speed: ", action.speed);
            so.FindProperty("changeJumpForce").boolValue = EditorGUILayout.Toggle("Change Jump Force:", action.changeJumpForce);
            if (so.FindProperty("changeJumpForce").boolValue)
                so.FindProperty("jumpForce").floatValue = EditorGUILayout.FloatField("New Jump Force: ", action.jumpForce);
        }
        else
        {
            so.FindProperty("charIsPlayer").boolValue = EditorGUILayout.Toggle("Char is player:", action.charIsPlayer);
            if (!so.FindProperty("charIsPlayer").boolValue)
                so.FindProperty("character").objectReferenceValue = (UnityEngine.AI.NavMeshAgent)EditorGUILayout.ObjectField("Character: ", action.character, typeof(UnityEngine.AI.NavMeshAgent), true);
            so.FindProperty("speed").floatValue = EditorGUILayout.FloatField("New Speed: ", action.speed);
        }
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }


}
