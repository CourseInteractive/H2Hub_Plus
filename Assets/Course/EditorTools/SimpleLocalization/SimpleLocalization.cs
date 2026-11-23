using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimpleLocalization
{

    public static SimpleLocalization_InGame inGame;

    public static List<LokaElement> knownElements;
    public static int activeLanguage = 0;

    /// <summary>Wird nach jedem Sprachwechsel ausgelöst – für Scripts, die kein LokaElement sind.</summary>
    public static event System.Action OnLanguageChanged;

    public static string GetLocalization(string token)
    {
        if (inGame == null)
            return token;
        return inGame.kit.GetLocaStringByToken(token, activeLanguage);
        
    }

    public static void LanguageChanged(int newLanguage)
    {
        activeLanguage = newLanguage;
        if (knownElements != null)
        {
            foreach(LokaElement element in knownElements)
            {
                if (element != null)
                    element.LocalizeElement();
            }
        }
        OnLanguageChanged?.Invoke();
    }


}
