using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Course.PrototypeScripting
{
    public class ActionParticles : Action
    {
        public ParticleSystem particles;
        public enum ParticleAction {  Start, Stop }
        public ParticleAction action;

        override public void ExecuteAction()
        {
            if (particles == null)
            {
                SequenceHandler.Instance.ReportActionEnd();
                return;
            }
            if (action == ParticleAction.Start)
                particles.Play();
            else
                particles.Stop();
                

        }


        override public string GetAdditionalInfo()
        {
            if(particles == null)
                return "! No particleSystem set!";
            if (action == ParticleAction.Start)
                return "Start particles " + particles.name;
            else
                return "Stop particles " + particles.name;
        }
    }
}
