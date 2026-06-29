using UnityEngine;

public class TeleportPointContainer : MonoBehaviour
{
    public static TeleportPointContainer Instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instance = this;
        Deactivate();
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

}
