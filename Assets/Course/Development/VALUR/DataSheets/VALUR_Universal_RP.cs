using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VALUR_Universal_RP : MonoBehaviour
{
    RenderPipelineAsset currentPipelineAsset;
    UniversalRenderPipelineAsset universalRenderPipelineAsset;

    void Start()
    {
        VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
        topic.token = "universalRP";
        topic.name = "Universal Render Pipeline";
        VALUR.Data.IntroduceTopic(topic);
        VALUR.Data.AddFetchFctToTopic("universalRP", FetchData1);

    }

    public void FetchData1()
    {
        
        //VALUR.Data.AddConsoleInfo("Runtime", (now - start).ToString());

        //VALUR.Data.AddConsoleIntSlider("Pixel Light", PixelLightSlider, QualitySettings.pixelLightCount, new Vector2(0, 20));
        VALUR.Data.AddConsoleButton("Render Pipeline Switch", BtnAction, "rpAsset");
        VALUR.Data.AddConsoleInfo("Active RP Asset", RenderPipelineSwitcher.GetCurrentRPAssetName());
        GetActiveUrpAsset();
        // VALUR.Data.AddConsoleToggle("Soft Particles", "softParticle", QualitySettings.softParticles, Toggles);
        VALUR.Data.AddConsoleButton("Shadow Resolution: 512", BtnAction, "sh512");
        VALUR.Data.AddConsoleButton("Shadow Resolution: 2048", BtnAction, "sh2048");
        //VALUR.Data.AddPopupMenu("Texture Quality", TextureQuality, QualitySettings.globalTextureMipmapLimit, textureQualityOptions);
        //string[] resolutions = new string[3] { "512", "1024", "2048" };
        //VALUR.Data.AddPopupMenu("Shadow Resolution", ShadowResAction, universalRenderPipelineAsset.additionalLightsShadowmapResolution, resolutions);

        VALUR.Data.AddConsoleSlider("Shadow Distance", SliderAction, universalRenderPipelineAsset.shadowDistance, new Vector2(0, 100), "ShDist");

        VALUR.Data.AddConsoleInfo("Additional Light", "");
        VALUR.Data.AddConsoleIntSlider("Light Count", SliderIntAction, universalRenderPipelineAsset.maxAdditionalLightsCount, new Vector2(0, 20), "LightCount");

    }

    void BtnAction(string data)
    {
        switch (data)
        {
            case "sh512":
                SetShadowResolution(512);
                break;
            case "sh2048":
                SetShadowResolution(2048);
                break;
            case "rpAsset":
                RenderPipelineSwitcher.CycleRenderPipelineAsset();

                break;
        }
    }

    void SetShadowResolution(int value)
    {
#if UNITY_6000_0_OR_NEWER
        universalRenderPipelineAsset.mainLightShadowmapResolution = value;
#else
        System.Reflection.FieldInfo fieldInfo = typeof(UniversalRenderPipelineAsset).GetField(
            "m_MainLightShadowmapResolution",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic
        );

        if (fieldInfo == null)
        {
            return;
        }

        fieldInfo.SetValue(universalRenderPipelineAsset, value);
#endif
    }


    void ShadowResAction(string data, int value)
    {
        Debug.Log(data);
        int[] resolutions = new int[3] { 512, 1024, 2048 };
        SetShadowResolution(resolutions[value]);
       // universalRenderPipelineAsset.mainLightShadowmapResolution = resolutions[value];
    }

    void SliderAction(string data, float value)
    {
        Debug.Log(data);
        switch(data)
        {
            case "ShDist":
                universalRenderPipelineAsset.shadowDistance = value;
                break;
        }
      
    }

    void SliderIntAction(string data, int value)
    {
        Debug.Log(data + "  " + value);
        switch (data)
        {
            case "LightCount":
                universalRenderPipelineAsset.maxAdditionalLightsCount = value;
                break;
        }
    }

    public  void GetActiveUrpAsset()
    {
        currentPipelineAsset = QualitySettings.renderPipeline;
        universalRenderPipelineAsset = Instantiate(currentPipelineAsset as UniversalRenderPipelineAsset);
        universalRenderPipelineAsset.hideFlags = HideFlags.DontSave;
        GraphicsSettings.defaultRenderPipeline = universalRenderPipelineAsset;
        QualitySettings.renderPipeline = universalRenderPipelineAsset;
    //    return universalRenderPipelineAsset;
    }
}
