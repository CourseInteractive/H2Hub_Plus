using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Course.PrototypeScripting;

namespace Course.PrototypeScripting
{
    public class OnSceneLoad_StartSequence : MonoBehaviour
    {

        public Sequence sequenceToStart;

        public void Start()
        {
            if(sequenceToStart != null)
                SequenceHandler.Instance.StartNewSequence(sequenceToStart);
        }

    }
}

#if UNITY_EDITOR
namespace Course.PrototypeScripting
{
    [CustomEditor(typeof(OnSceneLoad_StartSequence))]
    public class OnSceneLoad_StartSequenceEditor : Editor
    {
        OnSceneLoad_StartSequence action;
        // Start is called before the first frame update
        public override void OnInspectorGUI()
        {
            action = target as OnSceneLoad_StartSequence;
            SerializedObject so = new SerializedObject(target);
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Sequence:", GUILayout.Width(65));
            so.FindProperty("sequenceToStart").objectReferenceValue = (Sequence)EditorGUILayout.ObjectField(action.sequenceToStart, typeof(Sequence), true);
            if (action.sequenceToStart == null)
            {
                if (GUILayout.Button("Create as Child", GUILayout.Width(110)))
                {
                    so.FindProperty("sequenceToStart").objectReferenceValue = CreateNewSequenceAsChild();
                }
                if (GUILayout.Button("Create", GUILayout.Width(60)))
                {
                    so.FindProperty("sequenceToStart").objectReferenceValue = CreateNewSequenceHere();
                }
            }


            EditorGUILayout.EndHorizontal();
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(action);

            so.ApplyModifiedProperties();
        }

        Sequence CreateNewSequenceAsChild()
        {
            GameObject newGO = new GameObject("OnStartSequence");
            Sequence s = newGO.AddComponent<Sequence>();
            newGO.name = "OnStartSequence";
            newGO.transform.parent = action.transform;
            Selection.activeObject = newGO;
            return s;
        }

        Sequence CreateNewSequenceHere()
        {
            Sequence s = action.gameObject.AddComponent<Sequence>();
            return s;
        }
    }

}

#endif
