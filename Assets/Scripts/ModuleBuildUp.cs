using UnityEngine;

public class ModuleBuildUp : MonoBehaviour
{
    [Tooltip("Die Objekte, die nach und nach aktiviert werden sollen")]
    public GameObject[] objects;

    [Tooltip("Wartezeit in Sekunden zwischen dem Aktivieren der Objekte")]
    public float delay = 1f;

    private float timer = 0f;
    private int nextIndex = 0;

    private void Start()
    {
        // Sicherstellen, dass alle Objekte zu Beginn deaktiviert sind
        foreach (GameObject obj in objects)
        {
            if (obj != null)
                obj.SetActive(false);
        }

        timer = delay; // erstes Objekt sofort nach "delay" Sekunden aktivieren
    }

    private void Update()
    {
        // Wenn schon alle Objekte aktiviert sind, nichts mehr tun
        if (nextIndex >= objects.Length)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            ActivateNext();
            timer = delay; // Timer für das nächste Objekt zurücksetzen
        }
    }

    private void ActivateNext()
    {
        if (objects[nextIndex] != null)
            objects[nextIndex].SetActive(true);

        nextIndex++;
    }
}
