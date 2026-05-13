using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ObjectTransformState_OneAxisRotation : ObjectTransformState
{
    public float angle;
    public Quaternion q;
  
}


public class OTL_OneAxisRotation : ObjectTransformLibrary
{
    public Vector3 fixRotation;
    public Vector3 axis;
    public Vector3 localAxis;
    public List<ObjectTransformState_OneAxisRotation> states;
    protected ObjectTransformState_OneAxisRotation savedState;

    private void Awake()
    {
        InitLocalAxis();
    }

    public void InitLocalAxis()
    {
        localAxis = transform.InverseTransformDirection(axis);
    }

    public override void SetStateInstant(string name)
    {
        base.SetStateInstant(name);
        //Vector3 worldAxis = transform.TransformDirection(axis);
        transform.localRotation = Quaternion.AngleAxis(states[currentIndex].angle, axis);
    }

    public override void SetStateInstant(int index)
    {
        string n = states[index].name;
        SetStateInstant(n);
    }

    protected override void SaveState()
    {
        savedState = new ObjectTransformState_OneAxisRotation();
        savedState.q = transform.localRotation;
        useSavedStateAsStart = true;
    }

    public override void MoveIntoState(string name, float time, AnimationCurve newCurve = null)
    {
        SaveState();
        base.MoveIntoState(name, time, newCurve);
    }

    protected override void SetValue(float value)
    {
        if (useAnimationCurve)
        {
            value = usedCurve.Evaluate(value);
        }
        //Vector3 worldAxis = transform.TransformDirection(axis);
        Quaternion targetRot = Quaternion.AngleAxis(states[targetIndex].angle, axis);
        transform.localRotation = Quaternion.LerpUnclamped(savedState.q, targetRot, value);
    }

    public override int GetIndexByName(string name)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (name == states[i].name)
                return i;
        }
        return -1;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OTL_OneAxisRotation))]
[CanEditMultipleObjects]
public class OTL_OneAxisEditor : Editor
{
    OTL_OneAxisRotation main;
    public override void OnInspectorGUI()
    {
        main = target as OTL_OneAxisRotation;
        SerializedObject so = new SerializedObject(target);
        if (main.states == null)
            main.states = new List<ObjectTransformState_OneAxisRotation>();
        EditorGUI.BeginChangeCheck();

        //Todo -> Umwandeln in Enum und dann abspeichern
        main.axis = EditorGUILayout.Vector3Field("Achse", main.axis);
        for (int i = 0; i < main.states.Count; i++)
        {
            
            EditorGUILayout.BeginHorizontal();
            main.states[i].name = EditorGUILayout.TextField(main.states[i].name);
            if (GUILayout.Button("Save To " + i))
                {

                }
                if (GUILayout.Button("Move To " + i))
                {
                    main.InitLocalAxis();
                    main.SetStateInstant(main.states[i].name);
                }
            main.states[i].angle = EditorGUILayout.FloatField(main.states[i].angle);
            EditorGUILayout.LabelField(main.states[i].angle.ToString());
            EditorGUILayout.EndHorizontal();

        }
        if (GUILayout.Button("+"))
        {
            main.states.Add(new ObjectTransformState_OneAxisRotation());
        }
        main.useAnimationCurve = EditorGUILayout.Toggle("use Curve:", main.useAnimationCurve);
        if (main.useAnimationCurve)
        {
            if (main.curve == null)
                main.curve = new AnimationCurve();
            main.curve = EditorGUILayout.CurveField("Curve:", main.curve);
        }
        main.forceShortest = EditorGUILayout.Toggle("force Shortest Rotation:", main.forceShortest);

     
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(main);
        }

    }

    Vector3 KeepBetween180s(Vector3 input)
    {
        if (input.x > 180)
            input.x -= 360;
        if (input.x < -180)
            input.x += 360;
        if (input.y > 180)
            input.y -= 360;
        if (input.y < -180)
            input.y += 360;
        if (input.z > 180)
            input.z -= 360;
        if (input.z < -180)
            input.z += 360;
        return input;

    }
}


#endif
