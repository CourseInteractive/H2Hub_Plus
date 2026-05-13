using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickConsole_Local : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

        if(QuickConsole_LocalConnector.instance)
        {
            QuickConsole_LocalConnector.instance.ConnectNewLocal(gameObject);
        }
    }

}
