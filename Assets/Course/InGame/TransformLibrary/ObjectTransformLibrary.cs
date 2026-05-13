using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ObjectTransformState
{
    public string name;
    public Transform target;

}

[System.Serializable]
public class Constraint
{
    public bool posX;
    public bool posY;
    public bool posZ;
    public bool rotX;
    public bool rotY;
    public bool rotZ;

    public Vector3 GetPositionFactorVector()
    {
        Vector3 newV = new Vector3(0, 0, 0);
        if (posX)
            newV.x = 1;
        if (posY)
            newV.y = 1;
        if (posZ)
            newV.z = 1;
        return newV;
    }

    public Vector3 GetRotationFactorVector()
    {
        Vector3 newV = new Vector3(0, 0, 0);
        if (rotX)
            newV.x = 1;
        if (rotY)
            newV.y = 1;
        if (rotZ)
            newV.z = 1;
        return newV;
    }
}

public class ObjectTransformLibrary : MonoBehaviour
{

    protected int startIndex;
    protected int targetIndex;
    protected int currentIndex;
    public bool useAnimationCurve;
    public AnimationCurve curve = new AnimationCurve();

    public AnimationCurve usedCurve;

    public bool useConstraint;
    public Constraint constraint;


    protected bool useSavedStateAsStart;
    public bool forceShortest = false;

    public bool useTransform;
    protected void ChangeUseConstraint(int value)
    {
        ChangeUseConstraint(value == 1);
    }

    protected void ChangeUseConstraint(bool value)
    {
        useConstraint = value;
    }

    public virtual void SetStateInstant(string name)
    {
        currentIndex = GetIndexByName(name);
    }

    public virtual void SetStateInstant(int index)
    {
        currentIndex = index;
    }

    protected virtual void SaveState()
    {
        Debug.Log("SaveState");
        useSavedStateAsStart = true;
    }

    protected virtual void DismissSaveState()
    {
        useSavedStateAsStart = false;
    }

    public virtual int GetIndexByName(string name)
    {

        return -1;
    }

    private void FixedUpdate()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            float x = 1f - (timer / timeToMove);

                SetValue(Mathf.Clamp01(x));
                if (timer < 0)
                    EndMovement();

        }
    }

    float timer;
    float timeToMove;
    string targetState;

    public void MoveIntoState_Test(string name)
    {
        MoveIntoState(name, 2f);
    }

    public void MoveIntoState_0_5(string name)
    {
        MoveIntoState(name, 0.5f);
    }

    public void MoveIntoState_1(string name)
    {
        MoveIntoState(name, 1f);
    }

    public void MoveIntoState_1_5(string name)
    {
        MoveIntoState(name, 1.5f);
    }

    public void MoveIntoState_2(string name)
    {
        MoveIntoState(name, 2f);
    }

    public void MoveIntoState_4(string name)
    {
        MoveIntoState(name, 4f);
    }


    public virtual void MoveIntoState(string name, float time, AnimationCurve newCurve = null)
    {
        PrepareForMovement(name, newCurve);
        timer = time;
        timeToMove = time;
        targetState = name;

    }

    protected virtual void SetValue(float value)
    {
 
      
       
    }


    protected Vector3 LerpVectorByVector(Vector3 start, Vector3 target, Vector3 values)
    {
        Vector3 result = Vector3.zero;
        result.x = Mathf.Lerp(start.x, target.x, values.x);
        result.y = Mathf.Lerp(start.y, target.y, values.y);
        result.z = Mathf.Lerp(start.z, target.z, values.z);
        return result;
    }

    protected void PrepareForMovement(string targetName, AnimationCurve newCurve = null)
    {
       
        if (newCurve == null)
            usedCurve = curve;
        else
            usedCurve = newCurve;
        targetIndex = GetIndexByName(targetName);
        startIndex = currentIndex;
        if(forceShortest)
        {
            Debug.Log("PrepareForMovement FS");
               SaveState();
        }
     
    }


    protected void EndMovement()
    {
        currentIndex = targetIndex;
    }

    protected void EnableCurve()
    {
        useAnimationCurve = true;
    }

    protected void DisableCurve()
    {
        useAnimationCurve = false;
    }

    protected Vector3 KeepBetween180s(Vector3 input)
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

