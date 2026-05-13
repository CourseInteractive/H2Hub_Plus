using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VALUR_Init : MonoBehaviour
{
    public string valurPrefabName;

    // Start is called before the first frame update
    void Start()
    {
        if(VALUR.Data.displayManager == null)
        {
            InstantiateValur();
        }
    }

    // Update is called once per frame
    void InstantiateValur()
    {
        GameObject go = (GameObject)Resources.Load<GameObject>(valurPrefabName);
        Instantiate(go);
    }
}
