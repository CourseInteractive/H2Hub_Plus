/// <summary>
/// Sprachen des Projekts. Der Zahlenwert entspricht dem Index in
/// LokaKitEntry.translationText bzw. SimpleLocalization.activeLanguage.
/// Index 0 ist die Ausgangssprache (wird im Inspector angezeigt und als Fallback genutzt).
/// </summary>
public enum LocaLanguage
{
    Deutsch = 0,
    English = 1
}

public static class LocaLanguageUtility
{
    public static LocaLanguage Current => (LocaLanguage)SimpleLocalization.activeLanguage;

    public static void Set(LocaLanguage language) => SimpleLocalization.LanguageChanged((int)language);

    public static int Count => System.Enum.GetValues(typeof(LocaLanguage)).Length;
}
