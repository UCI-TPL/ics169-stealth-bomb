using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour {

    [SerializeField]
    private WeatherPreset[] weatherPresets;
    private int previousPreset = 1;

    public void ChangeWeather() {
        try {
            int randomPreset = Random.Range(0, weatherPresets.Length);
            for (int i = 0; i < 10 && randomPreset == previousPreset; ++i) {
                randomPreset = Random.Range(0, weatherPresets.Length);
            }
            Light directionLight = GameObject.Find("Directional Light").GetComponent<Light>();
            directionLight.transform.rotation = Quaternion.Euler(weatherPresets[randomPreset].lightDirection);
            directionLight.color = weatherPresets[randomPreset].lightColor;
            directionLight.intensity = weatherPresets[randomPreset].lightIntensity;
            directionLight.shadowStrength = weatherPresets[randomPreset].shadowStrength;
            Camera.main.backgroundColor = weatherPresets[randomPreset].backgroundColor;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = weatherPresets[randomPreset].ambientLighting;
            previousPreset = randomPreset;
        }
        catch (System.Exception e) { }
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
