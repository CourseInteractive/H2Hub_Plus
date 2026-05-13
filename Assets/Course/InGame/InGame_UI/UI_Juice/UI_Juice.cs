using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UI_JuiceState
{

    public Rect rect;
    public Vector3 scale = Vector3.one;
    public float alpha = 1;
    

    public void LerpFrom2States(UI_JuiceState s1, UI_JuiceState s2, float v)
    {
        scale = s1.scale + (s2.scale - s1.scale) * v;
        alpha = s1.alpha + (s2.alpha - s1.alpha) * v;
        Rect r = new Rect();
        r.width = s1.rect.width + (s2.rect.width - s1.rect.width) * v;
        r.height = s1.rect.height + (s2.rect.height - s1.rect.height) * v;
        r.position = s1.rect.position + (s2.rect.position - s1.rect.position) * v;

        rect = r;
    }

}

public class UI_Juice : MonoBehaviour
{
 
    public enum Style { Scale, Rect}
    public Style style;
    public bool toBigOnEnable;

    public AnimationCurve curve;
    public UI_JuiceState stateSmall;
    public UI_JuiceState stateBig;

    public float time;
    float timer = 0f;

    UI_JuiceState startState;
    UI_JuiceState targetState;

    public bool changeWidth;
    public bool changeHeight;

    public enum TransformState { None, Transforming, Wait }
    public TransformState state;

    public float delay = -1;

    private void OnEnable()
    {
       

        if(toBigOnEnable)
        {
            SetToState(stateSmall);
            if (delay > 0)
            {
                Wait();
            }
            else
                ToBig();
        }
            
    }

    void Wait()
    {
        timer = delay;
        state = TransformState.Wait;
    }

    public void ToBigInstant()
    {
        state = TransformState.None;
        SetToState(stateBig);
    }

    public void ToBig(bool useDelay = false)
    {
        if(useDelay)
        {
            Wait();
            return;
        }
        startState = stateSmall;
        targetState = stateBig;
        state = TransformState.Transforming;
        timer = time;
    }

    public void ToSmall()
    {
        startState = stateBig;
        targetState = stateSmall;
        state = TransformState.Transforming;
        timer = time;
    }

    public void ToSmallInstant()
    {
        state = TransformState.None;
        SetToState(stateSmall);
    }

    // Update is called once per frame
    void Update()
    {
        if (state == TransformState.Wait)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer < 0)
            {
                ToBig();
            }
        }
        else
        if (state == TransformState.Transforming)
        {
            timer -= Time.unscaledDeltaTime;
            if (timer < 0)
            {
                timer = 0;
                state = TransformState.None;
            }
            SetToPartialState(1f - (timer / time));
          
                
        }
    }

    void SetToPartialState(float percent)
    {
        float value = curve.Evaluate(percent);
        UI_JuiceState newState = new UI_JuiceState();
        newState.LerpFrom2States(startState, targetState, value);
        SetToState(newState);
    }

    void SetToState(UI_JuiceState jState)
    {
        if(style == Style.Rect)
        {
            RectTransform r = GetComponent<RectTransform>();
            r.anchoredPosition = jState.rect.position;
            if(changeWidth)
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, jState.rect.width);
            if (changeHeight) 
                r.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, jState.rect.height);
        }
        if(style == Style.Scale)
        {
            GetComponent<RectTransform>().localScale = jState.scale;
        }
       
        if(GetComponent<CanvasGroup>())
        {
            GetComponent<CanvasGroup>().alpha = jState.alpha;
        }
    }
}
