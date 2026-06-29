using UnityEngine;

public class TeleportPoint_Visuals : MonoBehaviour
{
    public GameObject[] disableCloseToPlayer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        float distance = Vector3.Distance(transform.position, GameData.player.transform.position);
        if(distance < 0.5)
        {
            DeactivateVisuals();
        }
        else
        {
            ActivateVisuals();
        }
    }

    void ActivateVisuals()
    {
        foreach (GameObject go in disableCloseToPlayer)
        {
            go.SetActive(true);
        }
    }

    void DeactivateVisuals()
    {
        foreach (GameObject go in disableCloseToPlayer)
        {
            go.SetActive(false);
        }
    }

}
