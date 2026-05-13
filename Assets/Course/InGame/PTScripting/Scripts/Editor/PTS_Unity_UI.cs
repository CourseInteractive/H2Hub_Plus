using System.Collections;
using System.Collections.Generic;
using Course.PrototypeScripting;
using UnityEngine;
using UnityEditor;

public class PTS_Unity_UI
{
    [MenuItem("GameObject/Create Sequence as Child", false, 0)]
    public static void CreateSequenceAsChild()
    {
        GameObject newGO = new GameObject("NewSequence");
        newGO.AddComponent<Sequence>();
        if (Selection.activeObject)
            newGO.transform.parent = ((GameObject)Selection.activeObject).transform;
        Selection.activeObject = newGO;
    }
}
