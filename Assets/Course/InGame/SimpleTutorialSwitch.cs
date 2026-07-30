using System.Collections.Generic;
using UnityEngine;

public class SimpleTutorialSwitch : MonoBehaviour
{
    [System.Serializable]
    public class TutorialDisplay
    {
        public string name;
        public GameObject gameObject;
    }

    [Header("Liste aller Tutorial-Displays")]
    public List<TutorialDisplay> displays = new List<TutorialDisplay>();


    public void Hide()
    {
        foreach (var display in displays)
        {
            if (display.gameObject != null)
                display.gameObject.SetActive(false);
        }

    }

    /// <summary>
    /// Blendet alle Displays aus und zeigt nur das an, dessen Name übereinstimmt.
    /// </summary>
    public void Show(string name)
    {
        Hide();
        foreach (var display in displays)
        {
            if (display.name == name && display.gameObject != null)
            {
                display.gameObject.SetActive(true);
                return;
            }
        }

        Debug.LogWarning($"SimpleTutorialSwitch: Kein Display mit Namen '{name}' gefunden.");
    }
}
