using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour {

    [SerializeField]
    private WeatherPreset[] weatherPresets;
    private int previousPreset = 1;

    public void ChangeWeather(float time) {
        int randomPreset = Random.Range(0, weatherPresets.Length);
        for (int i = 0; i < 10 && randomPreset == previousPreset; ++i) {
            randomPreset = Random.Range(0, weatherPresets.Length);
        }
        previousPreset = randomPreset;

        StopAllCoroutines();
        StartCoroutine(LerpWeather(weatherPresets[randomPreset], time));
    }

    private IEnumerator LerpWeather(WeatherPreset weatherPreset, float duration) {
        Light directionLight = GameObject.Find("Directional Light").GetComponent<Light>();
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;

        Quaternion directionLightRotation = directionLight.transform.rotation;
        Color directionLightColor = directionLight.color;
        float directionLightIntensity = directionLight.intensity;
        float directionLightShadowStrength = directionLight.shadowStrength;
        Color backgroundColor = Camera.main.backgroundColor;
        Color ambientLight = RenderSettings.ambientLight;

        float startTime = Time.time;
        while (Time.time - startTime < duration) {
            float timePassed = (Time.time - startTime) / duration;

            directionLight.transform.rotation = Quaternion.Slerp(directionLightRotation, Quaternion.Euler(weatherPreset.lightDirection), timePassed);
            directionLight.color = Color.Lerp(directionLightColor, weatherPreset.lightColor, timePassed);
            directionLight.intensity = Mathf.Lerp(directionLightIntensity, weatherPreset.lightIntensity, timePassed);
            directionLight.shadowStrength = Mathf.Lerp(directionLightShadowStrength, weatherPreset.shadowStrength, timePassed);
            Camera.main.backgroundColor = Color.Lerp(backgroundColor, weatherPreset.backgroundColor, timePassed);
            Shader.SetGlobalColor(Shader.PropertyToID("_BackgroundColor"), Camera.main.backgroundColor.linear);
            RenderSettings.ambientLight = Color.Lerp(ambientLight, weatherPreset.ambientLighting, timePassed);
            yield return null;
        }
        directionLight.transform.rotation = Quaternion.Euler(weatherPreset.lightDirection);
        directionLight.color = weatherPreset.lightColor;
        directionLight.intensity = weatherPreset.lightIntensity;
        directionLight.shadowStrength = weatherPreset.shadowStrength;
        Camera.main.backgroundColor = weatherPreset.backgroundColor;
        Shader.SetGlobalColor(Shader.PropertyToID("_BackgroundColor"), Camera.main.backgroundColor.linear);
        RenderSettings.ambientLight = weatherPreset.ambientLighting;
    }

    [System.Serializable]
    private struct WeatherPreset {
        public Vector3 lightDirection;
        [ColorUsage(false)]
        public Color lightColor;
        public float lightIntensity;
        public float shadowStrength;
        [ColorUsage(false, true)]
        public Color ambientLighting;
        [ColorUsage(false)]
        public Color backgroundColor;
        public Color fogColor;
    }
}
