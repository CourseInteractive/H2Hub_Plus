using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ActionTeleport))]
[CanEditMultipleObjects]
public class ActionTeleportEditor : Editor
{
    ActionTeleport action;
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        action = target as ActionTeleport;
        SerializedObject so = new SerializedObject(target);

        // so.FindProperty("objIsPlayer").boolValue = EditorGUILayout.Toggle("Move Player: ", action.objIsPlayer);
        //  if(!action.objIsPlayer)
        EditorGUI.BeginChangeCheck();

        so.FindProperty("objectToTeleport").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("Obj To Teleport: ", action.objectToTeleport, typeof(GameObject), true);
        so.FindProperty("teleportPoint").objectReferenceValue = (GameObject)EditorGUILayout.ObjectField("TeleportPoint: ", action.teleportPoint, typeof(GameObject), true);
        so.FindProperty("copyRotation").boolValue = EditorGUILayout.Toggle("Copy Target Y Rotation: ", action.copyRotation);
        
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }

}
