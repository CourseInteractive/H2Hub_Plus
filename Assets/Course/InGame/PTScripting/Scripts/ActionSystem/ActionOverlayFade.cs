using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{

    public class ActionOverlayFade : Action
    {
        public enum FadeAction { ShowOverlay, HideOverlay}

        public FadeAction fadeAction;

        public float time;

        public bool waitUntilFinished;
        // Start is called before the first frame update
        override public void ExecuteAction()
        {
            if (time <= 0)
            {
                if (fadeAction == FadeAction.ShowOverlay)
                {
                   OverlayFade.instance.InstantToVisible();
                }
                else
                {
                    OverlayFade.instance.InstantToVisible();
                }

                GoOn();
            }
            else
            {
                if (fadeAction == FadeAction.ShowOverlay)
                {
                    OverlayFade.instance.FadeToVisible(time);
                }
                else
                {
                    OverlayFade.instance.FadeToInvisible(time);
                }
                if(waitUntilFinished)
                    Invoke("GoOn", time);
                else
                {
                    GoOn();
                }
            }
             
        }

        void GoOn()
        {
            SequenceHandler.Instance.ReportActionEnd();
        }

        override public string GetAdditionalInfo()
        {
            if (fadeAction == FadeAction.ShowOverlay)
                return "Fade in Overlay in " + time + " seconds";
            else 
                return "Fade out Overlay in " + time + " seconds";
        }
    }
}