using UnityEngine;
using UnityEngine.UI;

public class UI_ProgressBar_Juice : MonoBehaviour
{
    public Image bar;

    public float startFilling;
    public float targetFilling;

    public float timeDefault;
    float fullTime;
    float timer = -1;

    public bool useCurve;
    public AnimationCurve curve;


    public bool startAutomatically;

    bool isActive;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(startAutomatically)
        {
            MoveToFilling(startFilling, targetFilling, timeDefault);
        }
    }

    public void MoveToFilling(float start, float target, float time)
    {
        startFilling = start;
        targetFilling = target;
        isActive = true;
        timer = 0;
        fullTime = time;
        bar.fillAmount = start;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
            return;
        timer += Time.deltaTime;
        if(timer > fullTime)
        {
            timer = fullTime;
            isActive = false;
        }
        float value = timer / fullTime;
        if (useCurve)
            value = curve.Evaluate(value);
        value = Mathf.Lerp(startFilling, targetFilling, value);
        bar.fillAmount = value;
        
    }
}
