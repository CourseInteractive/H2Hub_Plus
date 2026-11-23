using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LokaElement : MonoBehaviour
{
    bool initialized = false;
    int lineID = -1;

    // Neues Feld. Das Migrations-Tool überträgt das alte 'token' hierher.
    [LocaMigrateFrom("token")]
    public LocaString loca = new LocaString();

    // Altes Feld – bleibt erhalten, bis das Migrations-Tool es übertragen hat (danach leer).
    // Solange 'loca' leer ist, wird weiterhin dieses Feld benutzt -> nichts geht verloren.
    [HideInInspector] public string token;

    // Start is called before the first frame update
    public void Start()
    {
        if(!initialized)
        {
            if (SimpleLocalization.knownElements != null)
                SimpleLocalization.knownElements.Add(this);
            initialized = true;
        }
        LocalizeElement();
    }

    public virtual void LocalizeElement()
    {
        var kit = SimpleLocalization.inGame != null ? SimpleLocalization.inGame.kit : null;
        if (kit != null)
            lineID = kit.GetLineID(loca.HasToken ? loca.Token : token);
    }

    public void OnEnable()
    {
        LocalizeElement();
    }

    /// <summary>
    /// Lokalisierter Text dieses Elements.
    /// Neues Feld 'loca' hat Vorrang; ist es leer (noch nicht migriert), gilt das alte Verhalten mit 'token'.
    /// </summary>
    public string GetLocaEntry()
    {
        if (!loca.IsEmpty)
            return loca.Value;
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;
        return GetLocaEntry(token);
    }

    public string GetLocaEntry(string token)
    {
        token = token.Trim().ToLower();
        if (token[0] == '$')
            return SimpleLocalization.GetLocalization(token.Substring(1));
        else
            return SimpleLocalization.GetLocalization(token);
    }
}
