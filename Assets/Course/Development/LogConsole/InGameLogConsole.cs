using UnityEngine;
using UnityEngine.UI;

public class InGameLogConsole : MonoBehaviour
{
    private Text uiText;

    void Awake()
    {
        uiText = GetComponent<Text>();
        InGameLog.OnLogChanged += UpdateDisplay;
        UpdateDisplay(); // initialer Aufbau
    }

    void OnDestroy()
    {
        InGameLog.OnLogChanged -= UpdateDisplay;
    }

    private void UpdateDisplay()
    {
        if (uiText == null) return;

        var allLogs = InGameLog.GetLogs();
        uiText.text = string.Join("\n", allLogs);
    }

    public void AddLog(string data)
    {
        InGameLog.Log(data);
    }

    public void OnDisable()
    {
        InGameLog.Clear();
    }
}
