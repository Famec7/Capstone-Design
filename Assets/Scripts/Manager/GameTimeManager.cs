using UnityEngine;
using TMPro;
using System;

public enum DayPhase { Day, Night }

[Serializable]
public class DifficultySettings
{
    public int day1Day = 1;
    public int day1Night = 2;
    public int day2Day = 3;
    public int day2Night = 4;
    public int day3Day = 5;
}

[Serializable]
public class SkyboxSettings
{
    public Color daySkyTint = new Color(0.6f, 0.8f, 1f);
    public Color nightSkyTint = new Color(0.01f, 0.01f, 0.1f);
    public Color sunriseSkyTint = new Color(1f, 0.5f, 0.3f);
    public float dayExposure = 1.3f;
    public float nightExposure = 0.05f;
    public float sunriseExposure = 0.5f;
    public float dayAtmosphereThickness = 1.3f;
    public float nightAtmosphereThickness = 0.15f;
    public float sunriseAtmosphereThickness = 1.8f;
    public float daySunSize = 0.04f;
    public float nightSunSize = 0f;
    public float sunriseSunSize = 0.03f;

    public float dayLightIntensity = 1f;
    public float nightLightIntensity = 0.05f;
    public float sunriseLightIntensity = 0.5f;
    public Color dayLightColor = Color.white;
    public Color nightLightColor = new Color(0.2f, 0.2f, 0.35f);
    public Color sunriseLightColor = new Color(1f, 0.6f, 0.4f);
}

public class GameTimeManager : MonoBehaviour
{
    [SerializeField] private float realSecondsPerGameDay = 720f;
    [SerializeField] private DifficultySettings difficultySettings;
    [SerializeField] private SkyboxSettings skyboxSettings;
    [SerializeField] private Material skyboxMaterial;

    [SerializeField] private Light sunLight;

    private const float secondsInGameDay = 86400f;
    private float currentGameTimeSeconds;
    private int currentDay = 1;
    private DayPhase currentPhase = DayPhase.Day;
    private int currentDifficulty;

    public event Action<int, DayPhase, int> OnPhaseChanged;
    public event Action OnGameOver;

    void Start()
    {
        currentGameTimeSeconds = 21600f;
        UpdateDifficulty();
        InvokeRepeating(nameof(UpdateGameTime), 1f, 1f);
    }

    void UpdateGameTime()
    {
        float gameSecondsPerRealSecond = secondsInGameDay / realSecondsPerGameDay;
        currentGameTimeSeconds += gameSecondsPerRealSecond * 10;

        RotateSkyboxAndLighting();
        CheckPhaseChange();

        if (currentDay == 3 && currentGameTimeSeconds >= 64800f)
        {
            GameOver();
        }

        if (currentGameTimeSeconds >= secondsInGameDay)
        {
            currentGameTimeSeconds -= secondsInGameDay;
            currentDay++;
            UpdateDifficulty();
        }
    }

    private void RotateSkyboxAndLighting()
    {
        float rotation = (currentGameTimeSeconds / secondsInGameDay) * 360f;
        skyboxMaterial.SetFloat("_Rotation", rotation);

        float hour = currentGameTimeSeconds / 3600f;

        Color tint;
        float exposure;
        float atmosphere;
        float sunSize;

        float lightIntensity;
        Color lightColor;

        if (hour < 5f)
        {
            tint = skyboxSettings.nightSkyTint;
            exposure = skyboxSettings.nightExposure;
            atmosphere = skyboxSettings.nightAtmosphereThickness;
            sunSize = skyboxSettings.nightSunSize;

            lightIntensity = skyboxSettings.nightLightIntensity;
            lightColor = skyboxSettings.nightLightColor;
        }
        else if (hour < 7f)
        {
            float t = (hour - 5f) / 2f;
            tint = Color.Lerp(skyboxSettings.nightSkyTint, skyboxSettings.sunriseSkyTint, t);
            exposure = Mathf.Lerp(skyboxSettings.nightExposure, skyboxSettings.sunriseExposure, t);
            atmosphere = Mathf.Lerp(skyboxSettings.nightAtmosphereThickness, skyboxSettings.sunriseAtmosphereThickness, t);
            sunSize = Mathf.Lerp(skyboxSettings.nightSunSize, skyboxSettings.sunriseSunSize, t);

            lightIntensity = Mathf.Lerp(skyboxSettings.nightLightIntensity, skyboxSettings.sunriseLightIntensity, t);
            lightColor = Color.Lerp(skyboxSettings.nightLightColor, skyboxSettings.sunriseLightColor, t);
        }
        else if (hour < 17f)
        {
            float t = (hour - 7f) / 10f;
            tint = Color.Lerp(skyboxSettings.sunriseSkyTint, skyboxSettings.daySkyTint, t);
            exposure = Mathf.Lerp(skyboxSettings.sunriseExposure, skyboxSettings.dayExposure, t);
            atmosphere = Mathf.Lerp(skyboxSettings.sunriseAtmosphereThickness, skyboxSettings.dayAtmosphereThickness, t);
            sunSize = Mathf.Lerp(skyboxSettings.sunriseSunSize, skyboxSettings.daySunSize, t);

            lightIntensity = Mathf.Lerp(skyboxSettings.sunriseLightIntensity, skyboxSettings.dayLightIntensity, t);
            lightColor = Color.Lerp(skyboxSettings.sunriseLightColor, skyboxSettings.dayLightColor, t);
        }
        else if (hour < 19f)
        {
            float t = (hour - 17f) / 2f;
            tint = Color.Lerp(skyboxSettings.daySkyTint, skyboxSettings.sunriseSkyTint, t);
            exposure = Mathf.Lerp(skyboxSettings.dayExposure, skyboxSettings.sunriseExposure, t);
            atmosphere = Mathf.Lerp(skyboxSettings.dayAtmosphereThickness, skyboxSettings.sunriseAtmosphereThickness, t);
            sunSize = Mathf.Lerp(skyboxSettings.daySunSize, skyboxSettings.sunriseSunSize, t);

            lightIntensity = Mathf.Lerp(skyboxSettings.dayLightIntensity, skyboxSettings.sunriseLightIntensity, t);
            lightColor = Color.Lerp(skyboxSettings.dayLightColor, skyboxSettings.sunriseLightColor, t);
        }
        else if (hour < 24f)
        {
            float t = (hour - 19f) / 5f;
            tint = Color.Lerp(skyboxSettings.sunriseSkyTint, skyboxSettings.nightSkyTint, t);
            exposure = Mathf.Lerp(skyboxSettings.sunriseExposure, skyboxSettings.nightExposure, t);
            atmosphere = Mathf.Lerp(skyboxSettings.sunriseAtmosphereThickness, skyboxSettings.nightAtmosphereThickness, t);
            sunSize = Mathf.Lerp(skyboxSettings.sunriseSunSize, skyboxSettings.nightSunSize, t);

            lightIntensity = Mathf.Lerp(skyboxSettings.sunriseLightIntensity, skyboxSettings.nightLightIntensity, t);
            lightColor = Color.Lerp(skyboxSettings.sunriseLightColor, skyboxSettings.nightLightColor, t);
        }
        else
        {
            tint = skyboxSettings.nightSkyTint;
            exposure = skyboxSettings.nightExposure;
            atmosphere = skyboxSettings.nightAtmosphereThickness;
            sunSize = skyboxSettings.nightSunSize;

            lightIntensity = skyboxSettings.nightLightIntensity;
            lightColor = skyboxSettings.nightLightColor;
        }

        skyboxMaterial.SetColor("_SkyTint", tint);
        skyboxMaterial.SetFloat("_Exposure", exposure);
        skyboxMaterial.SetFloat("_AtmosphereThickness", atmosphere);
        skyboxMaterial.SetFloat("_SunSize", sunSize);

        if (sunLight != null)
        {
            sunLight.intensity = lightIntensity;
            sunLight.color = lightColor;
        }
    }

    private void CheckPhaseChange()
    {
        DayPhase newPhase = (currentGameTimeSeconds >= 21600f && currentGameTimeSeconds < 64800f) ? DayPhase.Day : DayPhase.Night;
        if (newPhase != currentPhase)
        {
            currentPhase = newPhase;
            UpdateDifficulty();
            OnPhaseChanged?.Invoke(currentDay, currentPhase, currentDifficulty);
        }
    }

    private void UpdateDifficulty()
    {
        if (currentDay == 1 && currentPhase == DayPhase.Day) currentDifficulty = difficultySettings.day1Day;
        else if (currentDay == 1 && currentPhase == DayPhase.Night) currentDifficulty = difficultySettings.day1Night;
        else if (currentDay == 2 && currentPhase == DayPhase.Day) currentDifficulty = difficultySettings.day2Day;
        else if (currentDay == 2 && currentPhase == DayPhase.Night) currentDifficulty = difficultySettings.day2Night;
        else if (currentDay == 3 && currentPhase == DayPhase.Day) currentDifficulty = difficultySettings.day3Day;
    }

    private void GameOver()
    {
        CancelInvoke(nameof(UpdateGameTime));
        OnGameOver?.Invoke();
    }

    public string GetFormattedTime()
    {
        int hours = Mathf.FloorToInt(currentGameTimeSeconds / 3600f);
        int minutes = Mathf.FloorToInt((currentGameTimeSeconds % 3600f) / 60f);
        return $"{hours:00}:{minutes:00}";
    }

    public int GetCurrentDay() => currentDay;
    public DayPhase GetCurrentPhase() => currentPhase;
    public int GetCurrentDifficulty() => currentDifficulty;
}
