using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildSetup;
#if COURSE_XR
using UnityEngine.XR.Management;
#endif
public static class BuildInformation
{

    public static SteamBuildKind ActiveBuildKind
    {
        get { return BuildVersionSetup_Ingame.savedData.activeBuildKind; }
    }

    public static MainPlayModeType ActivePlayMode
    {
        get { return BuildVersionSetup_Ingame.savedData.mainPlayMode; }
    }

    public static TestModeType ActiveTestMode
    {
        get { return BuildVersionSetup_Ingame.savedData.testModeType; }
    }

    public static MarketingModeType ActiveMarketingMode
    {
        get { return BuildVersionSetup_Ingame.savedData.marketingModeType; }
    }

    public static bool IsVR_Active
    {
        get {
#if COURSE_XR
            return XRGeneralSettings.Instance != null &&
            XRGeneralSettings.Instance.Manager != null &&
            XRGeneralSettings.Instance.Manager.activeLoader != null;
#else

            return false;
        #endif

        }
    }

}
