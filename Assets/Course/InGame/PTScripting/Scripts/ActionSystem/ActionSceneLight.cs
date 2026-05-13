using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Course.PrototypeScripting
{
    public class ActionSceneLight : Action
    {
        public string settingsName;

        public Material skyboxMaterial;
        public Light sunSource;

        public Color ambientLightColor;
        public float ambientIntensity;

        public bool fog;
        public float fogDensity;
        public float fogStartDistance;
        public float fogEndDistance;
        public FogMode fogMode;


        public override void ExecuteAction()
        {
            RenderSettings.skybox = skyboxMaterial;
            RenderSettings.sun = sunSource;

            RenderSettings.ambientLight = ambientLightColor;
            RenderSettings.ambientIntensity = ambientIntensity;

            if (fog)
                SetFog();
            else
                RenderSettings.fog = false;


            ReportActionEnd();
        }
        void SetFog()
        {

            RenderSettings.fogMode = fogMode;
            if (fogMode == FogMode.Linear)
            {
                RenderSettings.fogStartDistance = fogStartDistance;
                RenderSettings.fogEndDistance = fogEndDistance;
            }
            else
            {

                RenderSettings.fogDensity = fogDensity;
            }
            RenderSettings.fog = true;
        }

        // Update is called once per frame
        public override string GetAdditionalInfo()
        {
            return "=> " + settingsName;
        }
    }
}