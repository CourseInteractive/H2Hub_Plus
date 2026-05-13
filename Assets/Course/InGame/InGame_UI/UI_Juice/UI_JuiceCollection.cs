using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_JuiceCollection : MonoBehaviour
{
    public UI_Juice[] elements;
    public bool useElementDelays;
    public float time;
    public void ToSmall()
    {
        foreach (UI_Juice juice in elements)
            juice.ToSmall();
    }

    public void ToBig()
    {
        foreach (UI_Juice juice in elements)
            juice.ToBig(useElementDelays);
    }
}
