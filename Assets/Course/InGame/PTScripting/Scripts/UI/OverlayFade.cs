using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayFade : MonoBehaviour
{
    public static OverlayFade instance;

    CanvasGroup canvas;

    private float timer;
    private float timeToFade;

    enum State { Static, ToInvisible, ToVisible}

    private State state;


    void Start()
    {
        canvas = GetComponent<CanvasGroup>();
        instance = this;
        InstantToInvisible();
    }

    public void FadeToVisible(float time)
    {
        timeToFade = time;
        timer = timeToFade;
        state = State.ToVisible;
    }

    public void FadeToInvisible(float time)
    {
        timeToFade = time;
        timer = timeToFade;
        state = State.ToInvisible;
    }

    public void InstantToInvisible()
    {
        canvas.alpha = 0f;
    }

    public void InstantToVisible()
    {
        canvas.alpha = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Static)
            return;
        timer -= Time.deltaTime;
        if (state == State.ToVisible)
        {
            canvas.alpha = 1f - (timer / timeToFade);
        }
        else
        {
            canvas.alpha = (timer / timeToFade);
        }

        if (timer < 0)
            state = State.Static;
    }
}
