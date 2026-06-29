using System;
using System.Collections.Generic;
using UnityEngine;

public static class InGameLog
{
    public static event Action OnLogChanged;

    private static List<string> logs = new List<string>();
    private static int maxLogs = 100;

    public static void Log(string message)
    {
        logs.Add(message);
        Debug.Log(message);
        // Anzahl begrenzen
        if (logs.Count > maxLogs)
            logs.RemoveAt(0);

        OnLogChanged?.Invoke();
    }

    public static IReadOnlyList<string> GetLogs()
    {
        return logs;
    }

    public static void Clear()
    {
        logs.Clear();
        OnLogChanged?.Invoke();
    }
}
