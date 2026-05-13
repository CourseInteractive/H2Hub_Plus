using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class OTL_All : ObjectTransformLibrary
{
    public List<ObjectTransformState_All> states;
    protected ObjectTransformState_All savedState;

    public override void SetStateInstant(string name)
    {
        base.SetStateInstant(name);
        Debug.Log("OTL_All");
        if (useTransform)
        {

            transform.localPosition = states[currentIndex].target.position;
            transform.localEulerAngles = states[currentIndex].target.eulerAngles;
        }
        else
        {
            transform.localPosition = (states[currentIndex] as ObjectTransformState_All).position;
            transform.localEulerAngles = (states[currentIndex] as ObjectTransformState_All).rotation;
        }

    }

    public override void SetStateInstant(int index)
    {
        string n = states[index].name;
        SetStateInstant(n);


    }

    protected override void SaveState()
    {
        savedState = new ObjectTransformState_All();
        if (useTransform)
        {
            (savedState as ObjectTransformState_All).position = transform.position;
            (savedState as ObjectTransformState_All).rotation = KeepBetween180s(transform.eulerAngles);
        }
        else
        {
            (savedState as ObjectTransformState_All).position = transform.localPosition;
            (savedState as ObjectTransformState_All).rotation = KeepBetween180s(transform.localEulerAngles);
        }

        useSavedStateAsStart = true;
    }

    public override void MoveIntoState(string name, float time, AnimationCurve newCurve = null)
    {
        
        if (forceShortest)
            PrepareStateForShortestRotation(name);
        base.MoveIntoState(name, time, newCurve);

    }

    void PrepareStateForShortestRotation(string targetName)
    {
        SaveState();
        targetIndex = GetIndexByName(targetName);
        (states[targetIndex] as ObjectTransformState_All).AdjustForShortestRotationPath((savedState as ObjectTransformState_All).rotation);
    }

    protected override void SetValue(float value)
    {
        base.SetValue(value);
        if (useConstraint)
        {
            transform.localPosition = Vector3.Lerp(GetStartPosition(), GetTargetPositionWithConstraint(), value);
            transform.localEulerAngles = Vector3.Lerp(GetStartRotation(), GetTargetRotationWithConstraint(), value);
        }
        else
        {
            if (useTransform)
            {
                transform.localPosition = Vector3.Lerp(GetStartPosition(), states[targetIndex].target.position, value);
                transform.localEulerAngles = Vector3.Lerp(GetStartRotation(), states[targetIndex].target.eulerAngles, value);
            }
            else
            {
                transform.localPosition = Vector3.Lerp(GetStartPosition(), (states[targetIndex] as ObjectTransformState_All).position, value);
                transform.localEulerAngles = Vector3.Lerp(GetStartRotation(), (states[targetIndex] as ObjectTransformState_All).rotation, value);
            }

        }
    }

    public  override int GetIndexByName(string name)
    {
        for (int i = 0; i < states.Count; i++)
        {
            if (name == states[i].name)
                return i;
        }
        return -1;
    }

    Vector3 GetStartPosition()
        {
            if (useTransform)
            {
                return (savedState as ObjectTransformState_All).position;
            }
            else
            if (useSavedStateAsStart)
            {
                return (savedState as ObjectTransformState_All).position;
            }
            else
            {
                return (states[startIndex] as ObjectTransformState_All).position;
            }
        }

        Vector3 GetStartRotation()
        {
            if (useSavedStateAsStart)
            {
                return (savedState as ObjectTransformState_All).rotation;
            }
            else
            {
                return (states[startIndex] as ObjectTransformState_All).rotation;
            }
        }

        Vector3 GetTargetPositionWithConstraint()
        {
            Vector3 factor = constraint.GetPositionFactorVector();
            if (useTransform)
            {
                return LerpVectorByVector((states[startIndex] as ObjectTransformState_All).position, states[targetIndex].target.position, factor);
            }
            else
            {
                return LerpVectorByVector((states[startIndex] as ObjectTransformState_All).position, (states[targetIndex] as ObjectTransformState_All).position, factor);
            }

        }

        Vector3 GetTargetRotationWithConstraint()
        {
            Vector3 factor = constraint.GetRotationFactorVector();
            if (useTransform)
            {
                return LerpVectorByVector((states[startIndex] as ObjectTransformState_All).rotation, states[targetIndex].target.eulerAngles, factor);
            }
            else
            {
                return LerpVectorByVector((states[startIndex] as ObjectTransformState_All).rotation, (states[targetIndex] as ObjectTransformState_All).rotation, factor);
            }
        }

    }




[System.Serializable]
public class ObjectTransformState_All : ObjectTransformState
{
    public Vector3 position;
    public Vector3 rotation;
    [HideInInspector]
    public Quaternion quat;
    public void AdjustForShortestRotationPath(Vector3 rot)
    {
        Vector3 newRot = rot;
        float r = rotation.y - rot.y;

        if (r > 180)
        {
            rotation.y -= 360;
        }

        if (r < -180)
        {
            rotation.y += 360;
        }

        r = rotation.x - rot.x;

        if (r > 180)
        {
            rotation.x -= 360;
        }

        if (r < -180)
        {
            rotation.x += 360;
        }

        r = rotation.z - rot.z;

        if (r > 180)
        {
            rotation.z -= 360;
        }

        if (r < -180)
        {
            rotation.z += 360;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(OTL_All))]
[CanEditMultipleObjects]
public class ObjectTransformLibraryEditor : Editor
{
    OTL_All main;
    public override void OnInspectorGUI()
    {
        main = target as OTL_All;
        SerializedObject so = new SerializedObject(target);
        if (main.states == null)
            main.states = new List<ObjectTransformState_All>();
        main.useTransform = EditorGUILayout.Toggle("Use Transform:", main.useTransform);
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < main.states.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            main.states[i].name = EditorGUILayout.TextField(main.states[i].name);
            if (main.useTransform)
            {
                main.states[i].target = (Transform)EditorGUILayout.ObjectField(main.states[i].target, typeof(Transform), true);
            }
            else
            {
                if (GUILayout.Button("Save To " + i))
                {
                    (main.states[i] as ObjectTransformState_All).position = main.transform.localPosition;
                    (main.states[i] as ObjectTransformState_All).rotation = KeepBetween180s(main.transform.localEulerAngles);
                }
                if (GUILayout.Button("Move To " + i))
                {
                    main.SetStateInstant(main.states[i].name);
                    //    main.transform.localPosition = main.states[i].position;
                    //    main.transform.localEulerAngles = main.states[i].rotation;
                }
            }


            if (GUILayout.Button("X "))
            {
                main.transform.localPosition = (main.states[i] as ObjectTransformState_All).position;
                main.transform.localEulerAngles = (main.states[i] as ObjectTransformState_All).rotation;
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            (main.states[i] as ObjectTransformState_All).rotation = EditorGUILayout.Vector3Field("R", (main.states[i] as ObjectTransformState_All).rotation);
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+"))
        {
            main.states.Add(new ObjectTransformState_All());
        }
        main.useAnimationCurve = EditorGUILayout.Toggle("use Curve:", main.useAnimationCurve);
        if (main.useAnimationCurve)
        {
            if (main.curve == null)
                main.curve = new AnimationCurve();
            main.curve = EditorGUILayout.CurveField("Curve:", main.curve);
        }
        main.forceShortest = EditorGUILayout.Toggle("force Shortest Rotation:", main.forceShortest);

        main.useConstraint = EditorGUILayout.Toggle("Use Constraint:", main.useConstraint);
        if (main.useConstraint)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Position:", GUILayout.Width(80));
            EditorGUILayout.LabelField("X:", GUILayout.Width(15));
            main.constraint.posX = EditorGUILayout.Toggle(main.constraint.posX, GUILayout.Width(25));
            EditorGUILayout.LabelField("Y:", GUILayout.Width(15));
            main.constraint.posY = EditorGUILayout.Toggle(main.constraint.posY, GUILayout.Width(25));
            EditorGUILayout.LabelField("Z:", GUILayout.Width(15));
            main.constraint.posZ = EditorGUILayout.Toggle(main.constraint.posZ, GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Rotation:", GUILayout.Width(80));
            EditorGUILayout.LabelField("X:", GUILayout.Width(15));
            main.constraint.rotX = EditorGUILayout.Toggle(main.constraint.rotX, GUILayout.Width(25));
            EditorGUILayout.LabelField("Y:", GUILayout.Width(15));
            main.constraint.rotY = EditorGUILayout.Toggle(main.constraint.rotY, GUILayout.Width(25));
            EditorGUILayout.LabelField("Z:", GUILayout.Width(15));
            main.constraint.rotZ = EditorGUILayout.Toggle(main.constraint.rotZ, GUILayout.Width(25));
            EditorGUILayout.EndHorizontal();
        }

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