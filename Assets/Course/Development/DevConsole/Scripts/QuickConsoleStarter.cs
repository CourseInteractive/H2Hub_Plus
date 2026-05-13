using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickConsoleStarter : MonoBehaviour
{
    public InputActionReference quickConsoleStart;
    public InputActionReference quickConsoleClose;
    public float timeBetweenHits = 0.3f;
    public int hitAmount = 3;
    float timer = 0;
    int hitCounter = 0;
    public QuickConsoleDisplay display;

    // Start is called before the first frame update
    void Start()
    {
        quickConsoleStart.action.Enable();
        quickConsoleClose.action.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
           

        if (quickConsoleClose.action.triggered)
        {
            CloseConsole();
            return;
        }

        if (quickConsoleStart.action.triggered)
        {
            

           hitCounter++;
           if (hitCounter >= hitAmount)
            {
                hitCounter = 0;
                OpenConsole();
            }

           
           timer = timeBetweenHits;
            
        }
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
            {
                hitCounter = 0;
            }
        }
    }

    void OpenConsole()
    {
      /*  if (!BuildVersionSetup_Ingame.developmentAccess)
        {
            Debug.Log("[Quick Console] Blocked by BuildVersion DevAccess");
            return;
        }*/
        display.Show();
        Cursor.lockState = CursorLockMode.None;
        if (QuickConsole.instance)
            QuickConsole.instance.ResetToRoot();
       
    }

    public void CloseConsole()
    {
        display.Close();
    }
}
