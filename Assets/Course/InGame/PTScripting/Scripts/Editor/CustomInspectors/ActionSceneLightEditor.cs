using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Course.PrototypeScripting;

[CustomEditor(typeof(ActionSceneLight))]
[CanEditMultipleObjects]
public class ActionSceneLightEditor : Editor
{

    ActionSceneLight action;

    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        action = target as ActionSceneLight;
        SerializedObject so = new SerializedObject(target);
        EditorGUI.BeginChangeCheck();
        if (GUILayout.Button("Fetch current Scene Light Settings"))
            CopyCurrent();
        so.FindProperty("skyboxMaterial").objectReferenceValue = (Material)EditorGUILayout.ObjectField("Skybox Material:", action.skyboxMaterial, typeof(Material), false);
        so.FindProperty("sunSource").objectReferenceValue = (Light)EditorGUILayout.ObjectField("Sun Source:", action.sunSource, typeof(Light), true);
        so.FindProperty("ambientLightColor").colorValue = EditorGUILayout.ColorField("Ambient Color:", action.ambientLightColor);
        so.FindProperty("ambientIntensity").floatValue = EditorGUILayout.FloatField("Ambient Intensity:" ,action.ambientIntensity);
        bool fog = EditorGUILayout.Toggle("Activate Fog:", action.fog);
        so.FindProperty("fog").boolValue = fog;
        if(fog)
            ShowFogSettings(so);
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(action);
        so.ApplyModifiedProperties();
    }

    private void ShowFogSettings(SerializedObject so)
    {
        if (action.fogMode == 0)
            action.fogMode = FogMode.Linear;
        action.fogMode = (FogMode)EditorGUILayout.EnumPopup("FogMode: ", action.fogMode);
        so.FindProperty("fogMode").enumValueIndex = (int)action.fogMode - 1;
        if (action.fogMode == FogMode.Linear)
        {
            so.FindProperty("fogStartDistance").floatValue = EditorGUILayout.FloatField("Fog Start Distance:", action.fogStartDistance);
            so.FindProperty("fogEndDistance").floatValue = EditorGUILayout.FloatField("Fog End Distance:", action.fogEndDistance);
        }
        else
        {
            so.FindProperty("fogDensity").floatValue = EditorGUILayout.FloatField("Fog Density:", action.fogDensity);
        }

    }

    void CopyCurrent()
    {
        action.fog = RenderSettings.fog;
        action.fogStartDistance = RenderSettings.fogStartDistance;
        action.fogEndDistance = RenderSettings.fogEndDistance;
        action.skyboxMaterial = RenderSettings.skybox;
        action.sunSource = RenderSettings.sun;
        action.ambientLightColor = RenderSettings.ambientLight;
        action.ambientIntensity = RenderSettings.ambientIntensity;
        action.fogDensity = RenderSettings.fogDensity;
        action.fogMode = RenderSettings.fogMode;
    }


}
