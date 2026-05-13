using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Course.PrototypeScripting;

public class InventoryEditor : EditorWindow
{

    static InventoryData data;
    static string dataName = "InventoryData";
    static List<InventoryItem> invItems;

   // [MenuItem("Simple Game/Inventory")]
    static void Open()
    {

        InventoryEditor window = (InventoryEditor)EditorWindow.GetWindow(typeof(InventoryEditor));
        Init();
    }

    static void Init()
    {
        data = (InventoryData)Resources.Load("InventoryData");
        if (data == null)
        {
            if (!Directory.Exists(Application.dataPath + "/SimpleGamePlugin/Resources/"))
                Directory.CreateDirectory(Application.dataPath + "/SimpleGamePlugin/Resources/");
            CreateAsset<InventoryData>("Assets/SimpleGamePlugin/Resources/" + dataName + ".asset");
        }
        data = (InventoryData)Resources.Load("InventoryData");
        invItems = new List<InventoryItem>();
        invItems = data.invItems;
    }

    Vector2 scroll = Vector2.zero;
    void OnGUI()
    {
        if (data == null || invItems == null)
            Init();

        if (invItems != null && invItems.Count > 0)
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            for (int i = 0; i < invItems.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: ", GUILayout.Width(50));
                invItems[i].name = EditorGUILayout.TextField(invItems[i].name, GUILayout.Width(120));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Amount on start: ", GUILayout.Width(120));
                invItems[i].amount = EditorGUILayout.IntField(invItems[i].amount, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                
                
                invItems[i].image = (Sprite)EditorGUILayout.ObjectField(invItems[i].image, typeof(Sprite), false, GUILayout.Height(80), GUILayout.Width(80));
                if (GUILayout.Button("X"))
                {
                    DeleteEntry(i);
                    EditorGUILayout.EndHorizontal();
                    return;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Examine Sequence: ", GUILayout.Width(120));
                invItems[i].sequencePrefab = (GameObject)EditorGUILayout.ObjectField(invItems[i].sequencePrefab, typeof(GameObject), false);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }


        if (GUILayout.Button("+ New Item Type"))
            CreateNewInvItem();

        if (GUILayout.Button("Save"))
            Save();
    }

    void CreateNewInvItem()
    {
        InventoryItem v = new InventoryItem();
        v.name = "New";
        if (invItems == null)
            invItems = new List<InventoryItem>();
        invItems.Add(v);
    }

    void DeleteEntry(int index)
    {
        invItems.RemoveAt(index);
    }


    public void Save()
    {
        InventoryData newData = new InventoryData();

        newData.invItems = invItems;

        ReplaceAsset<InventoryData>("Assets/SimpleGamePlugin/Resources/" + dataName + ".asset", newData);

        data = newData;
    }

    public static void CreateAsset<T>(string path) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();


        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
    }

    public static void ReplaceAsset<T>(string path, T data) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();
        asset = data;
        AssetDatabase.DeleteAsset(path);
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path);
        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }
}
