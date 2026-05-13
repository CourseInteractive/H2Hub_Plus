using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransformKeepLevel : MonoBehaviour
{
 
    // Update is called once per frame
    void Update()
    {
        Vector3 v = transform.parent.localEulerAngles;
        v.y = -v.y;
        transform.localEulerAngles = v;
    }
}
