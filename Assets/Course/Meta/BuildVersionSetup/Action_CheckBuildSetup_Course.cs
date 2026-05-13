using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

/*namespace AC
{
    public class Action_CheckBuildVersion_Course : ActionCheck
    {
        public enum CheckTarget { Version, PlayMode}
        public CheckTarget checkTarget;

        public BuildSetup.SteamBuildKind buildKind;

        public BuildSetup.MainPlayModeType playMode;

        public Action_CheckBuildVersion_Course()
        {
            this.isDisplayed = true;
            category = ActionCategory.Custom;
            title = "Check Build Version";
            description = "Queries the value of parameters defined in the parent ActionList.";
        }

        override public bool CheckCondition()
        {
            if (checkTarget == CheckTarget.Version)
            {
                if (BuildVersionSetup_Ingame.savedData.activeBuildKind == buildKind)
                {
                    return true;
                }
            }
            else
            {
                if (BuildVersionSetup_Ingame.savedData.mainPlayMode == playMode)
                {
                    return true;
                }
            }

            
            return false;
        }

        #if UNITY_EDITOR

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            checkTarget = (CheckTarget)EditorGUILayout.EnumPopup("Check Target:", checkTarget);
            if(checkTarget == CheckTarget.Version)
                buildKind = (BuildSetup.SteamBuildKind)EditorGUILayout.EnumPopup("Build Version:", buildKind);
            else
                playMode = (BuildSetup.MainPlayModeType)EditorGUILayout.EnumPopup("Play Mode:", playMode);
        }

        #endif
    }
}*/
