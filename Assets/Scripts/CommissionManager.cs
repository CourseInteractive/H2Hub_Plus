using UnityEngine;
using System.Collections.Generic;

public class CommissionManager : MonoBehaviour
{
    public static CommissionManager instance;
    public CommissionDatabase database;

    public Transform commissionList;
    public GameObject commissionPrefab;

    public List<Commission> currentCommissions;

    int commissionCounter = 0;
    public int maximumCommissions = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        commissionPrefab.SetActive(false);
        instance = this;
        currentCommissions = new List<Commission>();
        CreateNewCommission();
        CreateNewCommission();
        SetNewCommissionTimer();
    }

    [ContextMenu("Create New")]
    public void CreateNewCommission()
    {
        GameObject newCommissionEntry = (GameObject)Instantiate(commissionPrefab, commissionList);
        Commission c = newCommissionEntry.GetComponent<Commission>();
        c.Initialize(database.GetRandomCommission(currentCommissions), commissionCounter);
        commissionCounter++;
        currentCommissions.Add(c);
        newCommissionEntry.SetActive(true);
    }

    public void Remove(int internIndex)
    {
        int toRemove = -1;
        for(int i = 0; i < currentCommissions.Count; i++)
        {
            if (currentCommissions[i].internIndex == internIndex)
            {
                toRemove = i;
                break;
            }    
        }
        if (toRemove >= 0)
        {
            Destroy(currentCommissions[toRemove].gameObject);
            currentCommissions.RemoveAt(toRemove);
        }
        
    }
    float timer = -1;
    public float timeForUpdate = 30f;
    // Update is called once per frame
    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer < 0)
            {
                SetNewCommissionTimer();
                CreateNewCommission();
            }
        }
    }

    void SetNewCommissionTimer()
    {
        if (currentCommissions.Count == 0)
            timer = timeForUpdate * 0.4f;
        else if (currentCommissions.Count == 1)
            timer = timeForUpdate * 0.6f;
        else
            timer = timeForUpdate;
    }
}
