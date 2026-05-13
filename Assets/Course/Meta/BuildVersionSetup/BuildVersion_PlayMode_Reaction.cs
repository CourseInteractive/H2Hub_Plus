using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildVersion_PlayMode_Reaction : MonoBehaviour
{
    public GameObject UI;
    //public ExampleScript_1 script1;

    // Start is called before the first frame update
    void Start()
    {
        switch(BuildVersionSetup_Ingame.savedData.GetPlayMode())
        {
            case BuildSetup.MainPlayModeType.Test:
                break;
            case BuildSetup.MainPlayModeType.Marketing:
                UI.SetActive(false); 
                //script1.subtitlesOn = false;
                break;
            case BuildSetup.MainPlayModeType.Custom:
                if (BuildVersionSetup_Ingame.IsCustomSettingActive("NoUI"))
                    UI.SetActive(false);
                //if (BuildVersionSetup_Ingame.savedData.GetStateOfCustom("NoSubtitles") == 1)
                //    script1.subtitlesOn = false;
                break;
        }

        
    }


}
