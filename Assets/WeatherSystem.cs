using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DigitalRuby.RainMaker;
using UnityStandardAssets.ImageEffects;

public class WeatherSystem : MonoBehaviour {
    public WindZone wind;

    public AutoIntensity sun;
    public Light sunSrc;

    public RainScript rain;
    public ParticleSystem rawRainParticles;
    public ParticleSystem rawRainExplosionParticles;
    public ParticleSystem rawRainMistParticles;

    public CloudsToy clouds;

    public Bloom bloom;

    private float prevSolarAngle;
    private float prevTimeOfDay;
    public float timeOfDay;

    public int dayNumber;

    public bool isRaining;

    private Gradient savedSunNightDayColor;
    public Gradient rainSunNightDayColor;

    private Color originalCloudColor;

    private bool firstUpdate;

    private Hashtable toClearRainIntensity;
    private Hashtable toClearCloudsAlpha;
    private Hashtable toRainRainIntensity;
    private Hashtable toRainCloudsAlpha;

    public float transitionTime = 2f;

    // Use this for initialization
    void Start () {
        dayNumber = 0;
        firstUpdate = true;
        originalCloudColor = clouds.CloudColor;

        toClearRainIntensity = iTween.Hash("from", 0.5f, "to", 0f, "time", transitionTime, "onupdate", "ChangeRainIntenisty", "oncomplete", "OnRainEnded");
        toClearCloudsAlpha = iTween.Hash("from", 0.5f, "to", 0f, "time", transitionTime, "eastype", "linear", "onupdate", "ChangeCloudAlphaDown");
        toRainRainIntensity = iTween.Hash("from", 0f, "to", 0.5f, "time", transitionTime, "onupdate", "ChangeRainIntenisty", "oncomplete", "OnRainBegan");
        toRainCloudsAlpha = iTween.Hash("from", 0f, "to", 0.5f, "time", transitionTime, "eastype", "linear", "onupdate", "ChangeCloudAlphaUp");

        BeginRain();
    }
	
	// Update is called once per frame
	void Update () {
        float solarAngle = (sun.transform.rotation.eulerAngles.x + 90f) % 360f - 90f; // Scale rotation from -90 to 90 (day is >0, night is <0)

        if (solarAngle > prevSolarAngle) // Increasing convert to AM
            timeOfDay = (((solarAngle *  1f) / 180f) + 0.5f) * 12f;
        else // Decreasing convert to PM
            timeOfDay = (((solarAngle * -1f) / 180f) + 0.5f) * 12f + 12f;

        if (firstUpdate)
        {
            prevTimeOfDay = timeOfDay;
            firstUpdate = false;
        }

        if (prevTimeOfDay < 8f && timeOfDay >= 8f) // If we cross 6am, iterate the day counter
        {
            dayNumber += 1;

            if (dayNumber % 2 == 1)
            {
                EndRain();
            }
            else if (dayNumber % 2 == 0)
            {
                BeginRain();
            }
        }

        if (isRaining)
            clouds.CloudColor = new Color(sun.currentAtmosphericIntenity, sun.currentAtmosphericIntenity, sun.currentAtmosphericIntenity, originalCloudColor.a);

        prevTimeOfDay = timeOfDay;
        prevSolarAngle = solarAngle;
    }

    void BeginRain()
    {
        isRaining = true;

        clouds.gameObject.SetActive(true);
        rain.gameObject.SetActive(true);

        var rainShape = rawRainParticles.shape;
        rainShape.scale = new Vector3(100f, 100f, 100f);

        iTween.ValueTo(gameObject, toRainRainIntensity);
        iTween.ValueTo(gameObject, toRainCloudsAlpha);
    }

    void OnRainBegan()
    {
        sunSrc.enabled = false;
        sunSrc.shadows = LightShadows.None;
        savedSunNightDayColor = sun.nightDayColor;
        sun.nightDayColor = rainSunNightDayColor;
    }

    void EndRain()
    {
        isRaining = false;

        iTween.ValueTo(gameObject, toClearRainIntensity);
        iTween.ValueTo(gameObject, toClearCloudsAlpha);
    }

    void OnRainEnded()
    {
        sunSrc.enabled = true;
        clouds.CloudColor.a = 0f;
        clouds.gameObject.SetActive(false);
        rain.gameObject.SetActive(false);
        sunSrc.shadows = LightShadows.Soft;
        sun.nightDayColor = savedSunNightDayColor;
    }

    void ChangeRainIntenisty(float intensity)
    {
        rain.RainIntensity = intensity;
    }

    void ChangeCloudAlphaUp(float alpha)
    {
        originalCloudColor.a = alpha;
        clouds.MainColor.a = alpha;
    }

    void ChangeCloudAlphaDown(float alpha)
    {
        originalCloudColor.a = alpha;
        clouds.MainColor.a = alpha;
    }
}
