using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionMusic : Action
    {
        public AudioClip clip;
        public float timeToFadeIn;
        public float volume;

        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            MusicManager.Instance.SwitchMusic(clip, timeToFadeIn, volume);
            ReportActionEnd();
        }

        // Update is called once per frame
        override public string GetAdditionalInfo()
        {
            if (clip == null)
                return "- No Music set!";
            return "";
        }
    }
}
