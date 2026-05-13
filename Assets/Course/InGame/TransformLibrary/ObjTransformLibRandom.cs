using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjTransformLibRandom : MonoBehaviour
{
    float timer;
    public Vector2 randomDelayLimits;
    public Vector2 randomDurationLimits;
    ObjectTransformLibrary library;

    bool timerIsDelay;

    // Start is called before the first frame update
    void Start()
    {
        library = GetComponent<ObjectTransformLibrary>();
        NextState();
        timerIsDelay = false;
    }

    // Update is called once per frame
    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if(timerIsDelay)
            {
                NextState();
                timerIsDelay = false;
            }
            else
            {
                timerIsDelay = true;
                timer = Random.Range(randomDelayLimits.x, randomDelayLimits.y);
            }
            
            
            
        }
    }

    void NextState()
    {
        StartTimer();
        // Todo Repair
        //int rState = Random.Range(0, library.states.Count);
        //library.MoveIntoState(library.states[rState].name, timer);
    }

    void StartTimer()
    {
        timer = Random.Range(randomDurationLimits.x, randomDurationLimits.y);
    }
}
