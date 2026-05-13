using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Course.PrototypeScripting;

public class VariableEditor : EditorWindow
{
	static VariableData data;
	static string dataName = "VariableData";

	static List<GenericVariable> variableInfos;
	static List<GenericVariable> currentLocalVars;
	static string currentSceneName;

	[MenuItem("Tools/Course/PTS/Variable Editor")]
	static void Open()
	{

		VariableEditor window = (VariableEditor)EditorWindow.GetWindow(typeof(VariableEditor));
		Init();
	}

	static void Init()
	{
		currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
		data = (VariableData)Resources.Load("VariableData");
		if (data == null)
		{
			return;
			//if (!Directory.Exists(Application.dataPath + "/SimpleGamePlugin/Resources/"))
			//	Directory.CreateDirectory(Application.dataPath + "/SimpleGamePlugin/Resources/");
			//CreateAsset<VariableData>("Assets/SimpleGamePlugin/Resources/" + dataName + ".asset");
		}
		variableInfos = new List<GenericVariable>();
		variableInfos = data.variableInfos;
		FetchLocalVars();
	}
	VariableManager manager;
    private void Update()
    {
		//if (Application.isPlaying)
		//	OnGUI();
		if (currentSceneName != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name || currentLocalVars == null)
		{
			currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
			FetchLocalVars();
		}
		Repaint();
    }

	public void OnLevelWasLoaded(int level)
	{
		FetchLocalVars();
	}

	static void FetchLocalVars()
    {
		currentLocalVars = data.GetLocalVars(currentSceneName);
		if (currentLocalVars == null)
			currentLocalVars = data.AddLocalVarStack(currentSceneName);
	}

	void OnGUI()
	{
		
		if (data == null || variableInfos == null)
		{
			data = (VariableData)Resources.Load("VariableData");
			variableInfos = new List<GenericVariable>();
			variableInfos = data.variableInfos;
			data = (VariableData)EditorGUILayout.ObjectField("Variable Data", data, typeof(VariableData), false);
			return;
		}
		if (Application.isPlaying)
		{
			EditorGUILayout.LabelField("In Playmode");
			if (manager == null)
				manager = FindObjectOfType<VariableManager>();
			if (manager == null)
				return;
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("- - - Variables - - -");

			for (int i = 0; i < manager.variableSavedData.variableInfos.Count; i++)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(manager.variableSavedData.variableInfos[i].name, GUILayout.Width(150));
				manager.variableSavedData.variableInfos[i].value = EditorGUILayout.IntField(manager.variableSavedData.variableInfos[i].value, GUILayout.Width(40));
				EditorGUILayout.EndHorizontal();
				if(EditorGUI.EndChangeCheck())
                {
					manager.ExecuteVariableChangedEvent();
				}
			}
			EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();

			EditorGUILayout.LabelField("- - - Local Variables - - -");
			int localVarIndex = manager.variableSavedData.GetSceneIndex(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
			for (int i = 0; i < manager.variableSavedData.localVarData[localVarIndex].data.Count; i++)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(manager.variableSavedData.localVarData[localVarIndex].data[i].name, GUILayout.Width(150));
				manager.variableSavedData.localVarData[localVarIndex].data[i].value = EditorGUILayout.IntField(manager.variableSavedData.localVarData[localVarIndex].data[i].value, GUILayout.Width(40));
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					manager.ExecuteVariableChangedEvent();
				}
			}
			EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();


			EditorGUILayout.LabelField("- - - Timers - - -");
			for (int i = 0; i < manager.listOfTimers.Count; i++)
			{
				EditorGUI.BeginChangeCheck();
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.LabelField(manager.listOfTimers[i].name, GUILayout.Width(150));
				manager.listOfTimers[i].remainingTime = EditorGUILayout.FloatField(manager.listOfTimers[i].remainingTime, GUILayout.Width(40));
				EditorGUILayout.EndHorizontal();
				if (EditorGUI.EndChangeCheck())
				{
					manager.ExecuteVariableChangedEvent();
				}
			}

			return;
		}


		EditorGUILayout.LabelField("- - - GLOBAL Variables - - -");
		if (variableInfos != null && variableInfos.Count > 0)
		{
			for (int i = 0; i < variableInfos.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				variableInfos[i].name = EditorGUILayout.TextField(variableInfos[i].name, GUILayout.Width(150));
				variableInfos[i].value = EditorGUILayout.IntField(variableInfos[i].value, GUILayout.Width(40));
				if (GUILayout.Button("X"))
				{
					DeleteEntry(i);
					EditorGUILayout.EndHorizontal();
					return;
				}
				EditorGUILayout.EndHorizontal();
			}

		}
		if (GUILayout.Button("+ New Global Variable"))
			AddVariable();

		EditorGUILayout.Space(); EditorGUILayout.Space();
		EditorGUILayout.LabelField("- - - LOCAL Variables - - -");
		if (currentLocalVars != null && currentLocalVars.Count > 0)
		{
			for (int i = 0; i < currentLocalVars.Count; i++)
			{
				EditorGUILayout.BeginHorizontal();
				currentLocalVars[i].name = EditorGUILayout.TextField(currentLocalVars[i].name, GUILayout.Width(150));
				currentLocalVars[i].value = EditorGUILayout.IntField(currentLocalVars[i].value, GUILayout.Width(40));
				/*		if (GUILayout.Button("X"))
						{
							DeleteEntry(i);
							EditorGUILayout.EndHorizontal();
							return;
						}*/
				EditorGUILayout.EndHorizontal();
			}

		}
		if (GUILayout.Button("+ New Local Variable"))
			AddLocalVariable();


		if (GUILayout.Button("Save"))
			Save();
	}

	void DeleteEntry(int index)
	{
		variableInfos.RemoveAt(index);
	}

	void AddVariable()
	{
		GenericVariable v = new GenericVariable();
		v.name = "New";
		if (variableInfos == null)
			variableInfos = new List<GenericVariable>();
		variableInfos.Add(v);
	}

	void AddLocalVariable()
    {
		GenericVariable v = new GenericVariable();
		v.name = "New";
		data.AddLocalVarStack(currentSceneName);
		data.AddLocalVariable(currentSceneName, v);
		currentLocalVars = data.GetLocalVars(currentSceneName);
	}

	public void Save()
	{
		EditorUtility.SetDirty(data);
		AssetDatabase.SaveAssets();
	}
}
