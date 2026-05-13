// v1.1
// Access to active custom setup via one method

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildSetup;

namespace BuildSetup
{
    public enum SteamBuildKind { MAIN_GAME, Demo, ExternTest, InternTest }
    public enum MainPlayModeType { Normal, Test, Marketing, Custom }
    public enum TestModeType { Main }       // Extendable
    public enum MarketingModeType { Main }  // Extendable

    [System.Serializable]
    public class CustomBuildSetup
    {
        public string name;
        public bool active;
    }

    [System.Serializable]
    public class BuildVersionInfo
    {
        public BuildVersionInfo()
        { }


        public bool useMainInfo;
        public string title;
        public string identifier;
        public int steam_GameID;
        public bool splitBinary;
        public bool specialKeystore;

        // ToDo:
        /* Vielleicht einbauen:
         * PlayerSettings.Android.keystoreName = "Assets/mykeystore.keystore";
            PlayerSettings.Android.keystorePass = "meinPasswort";
            PlayerSettings.Android.keyaliasName = "meinAlias";
            PlayerSettings.Android.keyaliasPass = "aliasPasswort";
         */
        // Erweitern um Google Play Bundle Option`?
    }
}




public class BuildVersionSetup : ScriptableObject
{

    public int steam_releaseGameID;
    public int steam_demoGameID;
    public int steam_playTestGameID;
    public int iOS_appID;

    public BuildVersionInfo[] versionInfos;

    public SteamBuildKind activeBuildKind;


    public MainPlayModeType mainPlayMode;

    public TestModeType testModeType;
    public MarketingModeType marketingModeType;

    public CustomBuildSetup[] customSetups;


    public BuildVersionSetup GetClone()
    {
        BuildVersionSetup clone = new BuildVersionSetup();
        clone.steam_releaseGameID = steam_releaseGameID;
        clone.steam_demoGameID = steam_demoGameID;
        clone.steam_playTestGameID = steam_playTestGameID;
        clone.iOS_appID = iOS_appID;
        clone.activeBuildKind = activeBuildKind;
        clone.mainPlayMode = mainPlayMode;
        clone.testModeType = testModeType;
        clone.marketingModeType = marketingModeType;
        clone.customSetups = customSetups;
        clone.versionInfos = versionInfos;
        return clone;
    }

    public string GetCurrentAppID()
    {
        switch(activeBuildKind)
        {
            case SteamBuildKind.MAIN_GAME:
                return steam_releaseGameID.ToString();
            case SteamBuildKind.Demo:
                return steam_demoGameID.ToString();
            case SteamBuildKind.ExternTest:
                return steam_playTestGameID.ToString();
        }
        return "-1";
    }

    public MainPlayModeType GetPlayMode()
    {
        return mainPlayMode;
    }

    public int GetStateOfCustom(string name)
    {
        foreach (CustomBuildSetup setup in customSetups)
        {
            if(setup.name.ToLower().Trim() == name.ToLower().Trim())
            {
                if (setup.active)
                    return 1;
                else
                    return 0;
            }
        }
        return -1;
    }

    public int GetStateOfCustomIfStateActive(string name)
    {
        if (BuildVersionSetup_Ingame.savedData.GetPlayMode() != MainPlayModeType.Custom)
            return 0;
        foreach (CustomBuildSetup setup in customSetups)
        {
            if (setup.name.ToLower().Trim() == name.ToLower().Trim())
            {
                if (setup.active)
                    return 1;
                else
                    return 0;
            }
        }
        return 0;
    }

  
}
