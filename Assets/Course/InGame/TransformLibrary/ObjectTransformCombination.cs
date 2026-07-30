using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ObjectTransformCombinationState
{
    public string name;
    public string[] stateNames;
}

public class ObjectTransformCombination : MonoBehaviour
{
    public ObjectTransformLibrary[] objects;

    public ObjectTransformCombinationState[] states;
    ObjectTransformCombinationState currentState;
    public bool test;
    public string startState;

    private void Start()
    {
        if (startState != "")
            SetStateInstant(GetStateByName(startState));
    }

    // Update is called once per frame
    void Update()
    {
   /*   

        if(timer > 0)
        {
            timer -= Time.deltaTime;
            float x = 1f - (timer / timeToMove);
            for (int i = 0; i < objects.Length; i++)
            {
                objects[i].SetValue(Mathf.Clamp01(x));
                if (timer < 0)
                    objects[i].EndMovement();
            }
  
        }*/
   //Todo: Repair
    }

    public void MoveIntoState_10(string name)
    {
        MoveIntoState(name, 10);
    }
    public void MoveIntoState(string name, float time)
    {
        currentState = GetStateByName(name);
        for (int i = 0; i < objects.Length; i++)
        {
           // objects[i].PrepareForMovement(currentState.stateNames[i]);
          // Todo repair
        }
        timer = time;
        timeToMove = time;
        targetState = name;
    }

    float timer;
    float timeToMove;
    string targetState;


    public void SetStateInstant(string name)
    {
        currentState = GetStateByName(name);
        SetStateInstant(currentState);
    }

    ObjectTransformCombinationState GetStateByName(string name)
    {
        for (int i = 0; i < states.Length; i++)
        {
            if (name == states[i].name)
                return states[i];
        }

        return null;
    }

    void SetStateInstant(ObjectTransformCombinationState state)
    {
        for(int i = 0; i < objects.Length; i++)
        {
            objects[i].SetStateInstant(state.stateNames[i]);
        }
    }
}
