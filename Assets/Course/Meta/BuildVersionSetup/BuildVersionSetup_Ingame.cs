#if UNITY_ANDROID || UNITY_IOS || UNITY_TIZEN || UNITY_TVOS || UNITY_WEBGL || UNITY_WSA || UNITY_PS4 || UNITY_WII || UNITY_XBOXONE || UNITY_SWITCH
#define DISABLESTEAMWORKS
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

#if !DISABLESTEAMWORKS
using Steamworks;
#endif

public class BuildVersionSetup_Ingame : MonoBehaviour
{

    string dataName = "BuildSetup";
    public static BuildVersionSetup savedData;
    public static bool RunningOnSteamDeck = false;

    public static bool developmentAccess;
    // Start is called before the first frame update
    void Awake()
    {
        BuildVersionSetup originalData = (BuildVersionSetup)Resources.Load(dataName);
        savedData = originalData.GetClone();
        CheckIfOnSteamDeck();
        SetDevelopmentAccess();
    }

    private void Update()
    {
      /*  if(Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log(savedData.activeBuildKind.ToString());
        }*/
    }

    void CheckIfOnSteamDeck()
    {
        #if !DISABLESTEAMWORKS
        try
        {
            if (Steamworks.SteamUtils.IsSteamRunningOnSteamDeck())
            {
                RunningOnSteamDeck = true;
                return;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
        #endif
    }

    void SetDevelopmentAccess()
    {
        switch(savedData.activeBuildKind)
        {
            case BuildSetup.SteamBuildKind.MAIN_GAME:
            case BuildSetup.SteamBuildKind.Demo:
                developmentAccess = PlayerPrefs.GetInt("BV_DEV_ACCESS", 0) == 1;
                break;
            case BuildSetup.SteamBuildKind.ExternTest:
            case BuildSetup.SteamBuildKind.InternTest:
                developmentAccess = true;
                break;
        }
        Debug.Log("[Build Version] developmentAccess " + developmentAccess);
    }

    public static void OverwriteDevelopmentAccess(bool value = true)
    {
        PlayerPrefs.SetInt("BV_DEV_ACCESS", value ? 1 : 0);
        developmentAccess = value;
        Debug.Log("[Build Version] developmentAccess " + developmentAccess);
    }

    public static void ForceInternSteamDeckValue(bool value)
    {
        RunningOnSteamDeck = value;
    }

    public static BuildSetup.MainPlayModeType GetPlayMode()
    {
        return savedData.GetPlayMode();
    }

    public static bool IsCustomSettingActive(string name)
    {
        return savedData.GetStateOfCustom(name) == 1;
    }

    public static int GetStateOfCustom(string name)
    {
        return savedData.GetStateOfCustom(name);
    }

    public static int GetStateOfCustomIfStateActive(string name)
    {
        return savedData.GetStateOfCustomIfStateActive(name);
    }

    public static void SetStateOfCustom(string name, bool state)
    {
        int index = System.Array.FindIndex(savedData.customSetups, x => x.name == name);
        savedData.customSetups[index].active = state;
        Debug.Log("Set State of " + name + " to " + state);
    }

    public static void ToggleStateOfCustom(string name)
    {
        int index = System.Array.FindIndex(savedData.customSetups, x => x.name == name);
        savedData.customSetups[index].active = !savedData.customSetups[index].active;
        Debug.Log("Set State of " + name + " to " + savedData.customSetups[index].active);
    }


}
