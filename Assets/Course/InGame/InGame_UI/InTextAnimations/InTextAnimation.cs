using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InTextAnimation : MonoBehaviour
{
    [System.Serializable]
    public class InTextDelay
    {
        public float delay;
        //public float reps;
    }

    int currentStep = 0;
    public InTextDelay[] delays;
    public List<string> textParts;
    public List<string> textTasks;
    string startText;

    string lastFallBack;

    Text textElement;
    // Start is called before the first frame update
    void Start()
    {
        textElement = GetComponent<Text>();
        startText = textElement.text;
        textElement.text = "";
        ParseText();
        FirstStep();
    }

    void ParseText()
    {
        textParts = new List<string>();
        textTasks = new List<string>();
        string currentPart = "";
        bool skipNext = false;
        for (int i = 0; i < startText.Length; i++)
        {
            if (skipNext)
            {
                skipNext = false;
            }
            else
            if(startText[i] == '|')
            {
                if (currentPart.Length != 0)
                    textParts.Add(currentPart);
                currentPart = "";
                textTasks.Add(startText[i + 1].ToString());
                skipNext = true;
            }
            else
                currentPart += startText[i];
        }
        textParts.Add(currentPart);
    }

    // Update is called once per frame
    void Update()
    {
        HandleTimer();
    }
    float timer = -1;
    float timeToNextStep;
    void HandleTimer()
    {
        if (stopped)
            return;
        timer += Time.deltaTime;
        if (timer > timeToNextStep)
            NextStep();
    }

    void FirstStep()
    {
        lastFallBack = textParts[0];
        Fallback();
    }

    void Fallback()
    {
        textElement.text = lastFallBack;
        timer = 0;
        timeToNextStep = delays[currentStep].delay;
    }
    void NextStep()
    {
        bool endReached = ExecuteTask();
        if (endReached)
            return;
        timer = 0;
        timeToNextStep = delays[currentStep].delay;
        currentStep++;
    }

    bool ExecuteTask()
    {
        string result = "";
        bool end = InterpreteTask(textElement.text, textParts[currentStep+1], textTasks[currentStep], out result);
        if (end)
            return true;
        textElement.text = result;
        return false;
    }

    bool InterpreteTask(string currentText, string nextText, string task,  out string result)
    {
        switch(task)
        {
            case "+":
                result = currentText + nextText;
                return false;
            case "@":
                currentStep = 0;
                lastFallBack = textParts[0];
                Fallback();
                result = currentText;
                return true;
            case ">":
                result = currentText + nextText;
                lastFallBack = result;
                return false;
            case "<":
                Fallback();
                result = lastFallBack + nextText;
                return false;
            case "!":
                stopped = true;
                result = currentText;
                return true;
            case "~":
                lastFallBack = "";
                Fallback();
                result = nextText;
                return false;
        }
        result = currentText;
        return false;
    }

    bool stopped;
}
