using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    [RequireComponent(typeof(AudioSource))]
    public class ActionSound : Action
    {
        public AudioClip clip;
        public float volume;
        public float waitTimeInSeconds;

        override public void ExecuteAction()
        {

            GetComponent<AudioSource>().volume = volume;
            GetComponent<AudioSource>().clip = clip;
            GetComponent<AudioSource>().Play();

            if (waitTimeInSeconds > 0)
                Invoke("GoOn", waitTimeInSeconds);
            else
                ReportActionEnd();
        }

        void GoOn()
        {
            ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (clip == null)
                return "- No Clip set !";
            return clip.name;
        }
    }
}