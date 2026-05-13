using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VALUR_QualityChange : MonoBehaviour
{
    void Start()
    {
        VALUR.ConsoleTopic topic = new VALUR.ConsoleTopic();
        topic.token = "quality";
        topic.name = "Quality";
        VALUR.Data.IntroduceTopic(topic);
        VALUR.Data.AddFetchFctToTopic("quality", FetchData1);

    }

	public void FetchData1()
	{

		VALUR.Data.AddConsoleInfo("Application", Application.productName);

		System.DateTime start = System.DateTime.FromFileTime(VALUR.Data.startTimestamp);
		System.DateTime now = System.DateTime.Now;
		VALUR.Data.AddConsoleInfo("Runtime", (now - start).ToString());

		//VALUR.Data.AddConsoleField("Pixel Light", PixelLight, QualitySettings.pixelLightCount.ToString());

		VALUR.Data.AddConsoleIntSlider("Pixel Light", PixelLightSlider, QualitySettings.pixelLightCount, new Vector2(0, 20));

		//VALUR.Data.AddConsoleButton("FPSCounter", ButtonOrder, "fps");

		string[] antiAliasOptions = new string[4] { "X", "2", "4", "8" };
		VALUR.Data.AddPopupMenu("AntiAliasing", AntiAlias, GetAliasIndex(QualitySettings.antiAliasing), antiAliasOptions);

		string[] textureQualityOptions = new string[4] { "Full", "1/2", "1/4", "1/8" };
		VALUR.Data.AddPopupMenu("Texture Quality", TextureQuality, QualitySettings.globalTextureMipmapLimit, textureQualityOptions);

		VALUR.Data.AddConsoleToggle("Soft Particles", "softParticle", QualitySettings.softParticles, Toggles);

		VALUR.Data.AddConsoleInfo("Shadow", "-----Shadow-----");

		//VALUR.Data.AddConsoleField("Shadow Distance", ShadowDist, QualitySettings.shadowDistance.ToString());
		string[] shadowType = { "Disable", "HardOnly", "Hard and Soft" };
		VALUR.Data.AddPopupMenu("Shadow Type", ShadowType, (int)QualitySettings.shadows, shadowType);

		string[] shadowResolution = { "Low", "Medium", "High", "Very High" };
		VALUR.Data.AddPopupMenu("Shadow Resolution", ShadowResolution, (int)QualitySettings.shadowResolution, shadowResolution);

		VALUR.Data.AddConsoleSlider("Shadow Distance", ShadowDistSlider, QualitySettings.shadowDistance, new Vector2(0,100));
		//VALUR.Data.AddConsoleToggle("Skip Texts", "skipTexts", GameTestHelper.skipTexts, ToggleOrder);

		string[] skinWeightOptions = { "One", "Two", "Four", "Unlimited" };
		VALUR.Data.AddPopupMenu("Skin Weights", SkinWeights, GetSkinWeights((int)QualitySettings.skinWeights), skinWeightOptions);
		
	}

	public void Toggles(string info, bool value)
	{
		switch(info)
        {
			case "softParticle":
				QualitySettings.softParticles = value;
				break;
		}
	}

	public void ShadowType(string info, int value)
	{
		Debug.Log(info + "  " + value);
		QualitySettings.shadows = (ShadowQuality)value;
	}

	public void ShadowResolution(string info, int value)
	{
		Debug.Log(info + "  " + value);
		QualitySettings.shadowResolution = (ShadowResolution)value;
	}


	public void PixelLightSlider(string data, int value)
	{
		QualitySettings.pixelLightCount = value;
	}

	public void PixelLight(string data)
	{
		Debug.Log("Pixel Light " + data);
		if (data.Trim() == "")
			return;
		int value = int.Parse(data);
		QualitySettings.pixelLightCount = value;
	}

	public void ShadowDistSlider(string data, float value)
	{
		QualitySettings.shadowDistance = value;
	}

	public void ShadowDist(string data)
	{
		Debug.Log("Shadow Dist " + data);
		if (data.Trim() == "")
			return;
		int value = int.Parse(data);
		QualitySettings.shadowDistance = value;
	}

	int GetAliasIndex(int value)
    {
		switch(value)
        {
			case 2:
				return 1;
			case 4:
				return 2;
			case 8:
				return 3;
        }
		return 0;
    }

	public void AntiAlias(string info, int value)
	{
		Debug.Log(info + "  " + value);
		switch (value)
		{
			case 0:
				QualitySettings.antiAliasing = 0;
				break;
			case 1:
				QualitySettings.antiAliasing = 2;
				break;
			case 2:
				QualitySettings.antiAliasing = 4;
				break;
			case 3:
				QualitySettings.antiAliasing = 8;
				break;
		}

		
	}

	public void TextureQuality(string info, int value)
	{
		Debug.Log(info + "  " + value);
		QualitySettings.globalTextureMipmapLimit = value;
	}

	int GetSkinWeights(int value)
	{
		switch (value)
		{
			case 1:
				return 0;
			case 2:
				return 1;
			case 4:
				return 2;
			case 255:
				return 3;
		}
		return 0;
	}

	public void SkinWeights(string info, int value)
	{

		Debug.Log(info + "  " + value);
		switch (value)
		{
			case 0:
				QualitySettings.skinWeights = UnityEngine.SkinWeights.OneBone;
				break;
			case 1:
				QualitySettings.skinWeights = UnityEngine.SkinWeights.TwoBones;
				break;
			case 2:
				QualitySettings.skinWeights = UnityEngine.SkinWeights.FourBones;
				break;
			case 3:
				QualitySettings.skinWeights = UnityEngine.SkinWeights.Unlimited;
				break;
		}

	}


}
