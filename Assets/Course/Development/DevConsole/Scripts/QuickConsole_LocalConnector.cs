using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuickConsole_LocalConnector : MonoBehaviour
{
    public static QuickConsole_LocalConnector instance;
    public QuickConsoleDisplay console;
    
    public void ConnectNewLocal(GameObject local)
    {
        Debug.Log("QCLOCAL Mit neuer Szene verbunden: " + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        ReparentChildren(local, gameObject);
        GetComponent<QuickConsole_Entry>().IdentifyType();
        console.UpdateDisplay();
    }

    void DeleteAllChildren()
    {
        Debug.Log("QCLOCAL DeleteAllChildren " + gameObject.name);
        if (transform == null || gameObject == null)
            return;
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        gameObject.transform.DetachChildren();
    }

    void ReparentChildren(GameObject sourceParent, GameObject targetParent)
    {
        // Temporäre Liste, um Probleme bei der Iteration zu vermeiden
        List<Transform> children = new List<Transform>();

        foreach (Transform child in sourceParent.transform)
        {
            children.Add(child);
        }

        foreach (Transform child in children)
        {
            child.SetParent(targetParent.transform);
        }
    }

    public void DeleteEntries()
    {
        DeleteAllChildren();
    }

    void Awake()
    {
        if (instance != null)
            return;
        instance = this;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        Debug.Log("QCLOCAL Register LOCAL CONNECT");
    }

  /*  void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }*/

    void OnSceneUnloaded(Scene scene)
    {
        DeleteEntries();
        Debug.Log("QCLOCAL Szene wurde entladen: " + scene.name);
        // Hier kannst du deine Logik einfügen
    }
}
