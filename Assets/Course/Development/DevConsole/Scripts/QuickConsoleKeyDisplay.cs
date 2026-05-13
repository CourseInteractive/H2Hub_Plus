using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuickConsoleKeyDisplay : MonoBehaviour
{
    public Text keyText;
    public Text labelDisplay;
    public Image img;
    QuickConsole console;
    int index;

    public void SetUp(bool active, string text, int _index, QuickConsole _console, Sprite icon = null)
    {
        ResetFromPress();
        index = _index;
        console = _console;
        if (!active)
            gameObject.SetActive(false);
        else
        {
            gameObject.SetActive(true);
            labelDisplay.text = text.Replace("_", "\n");
        }

        if (icon == null)
            img.gameObject.SetActive(false);
        else
        {
            img.sprite = icon;
            img.gameObject.SetActive(true);
        }
    }

    public void ButtonHit()
    {
        GotPressed();
        Invoke("ExecutePress", console.display.pressDelay);
    }

    void ExecutePress()
    {
        console.ExecuteEntryByButton(index);
    }

    public void GotPressed()
    {
        pressedFeedbackTimer = console.display.gotPressedFeedbackTime;
        transform.localScale = Vector3.one * console.display.gotPressedScaling;
    }

    float pressedFeedbackTimer = -1;
    private void Update()
    {
        if(pressedFeedbackTimer > 0)
        {
            pressedFeedbackTimer -= Time.deltaTime;
         
            if(pressedFeedbackTimer <= 0)
                transform.localScale = Vector3.one;
        }
    }

    void ResetFromPress()
    {
        pressedFeedbackTimer = -1;
        transform.localScale = Vector3.one;
    }
}
