using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeatherManager : MonoBehaviour {

    [SerializeField]
    private WeatherPreset[] weatherPresets;

    private Camera mainCamera;
    [SerializeField]
    private Light directionLight;

    public void ChangeWeather() {
        RenderSettings.ambientLight = weatherPresets[0].ambientLighting;
    }

    [System.Serializable]
    private struct WeatherPreset {
        public Vector3 lightDirection;
        [ColorUsage(false)]
        public Color lightColor;
        public float lightIntensity;
        [ColorUsage(false, true)]
        public Color ambientLighting;
        [ColorUsage(false)]
        public Color backgroundColor;
        public Color fogColor;
    }
}
