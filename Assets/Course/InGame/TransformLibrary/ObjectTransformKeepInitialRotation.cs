using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransformKeepInitialRotation : MonoBehaviour
{
    Vector3 rot;
    // Start is called before the first frame update
    void Start()
    {
        rot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        transform.eulerAngles = rot;
    }
}
