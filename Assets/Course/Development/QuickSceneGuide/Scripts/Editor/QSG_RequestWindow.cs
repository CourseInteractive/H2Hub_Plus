using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;

//-----------------------------------------------------------------------------
// Copyright 2016 Oliver Ziegler - Course Interactive.  All rights reserverd.
//-----------------------------------------------------------------------------
namespace Course.QuickSceneGuide
{
	public class QSG_RequestWindow : EditorWindow
	{
		public QuickSceneGuide parent;
		public string newScenePath;

		public void OnGUI()
		{
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Do you want to save the changes you made in the scene?", GUILayout.MaxWidth(380));
			EditorGUILayout.Separator();
			EditorGUILayout.LabelField("Your changes will be lost if you don't save them.", GUILayout.MaxWidth(380));
			EditorGUILayout.Separator();
			EditorGUILayout.BeginHorizontal();
			if (GUILayout.Button("Save"))
			{
				parent.SaveScene(newScenePath);
				this.Close();
			}

			if (GUILayout.Button("Don't Save"))
			{
				parent.DontSaveScene(newScenePath);
				this.Close();
			}


			if (GUILayout.Button("Cancel"))
			{
				this.Close();
			}
			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Separator();
			EditorGUILayout.Separator();

		}
	}
}