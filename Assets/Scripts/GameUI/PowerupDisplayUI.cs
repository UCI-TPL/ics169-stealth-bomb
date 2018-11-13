using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupDisplayUI : MonoBehaviour {

    public PowerupData powerupData;
    public Buff buff;
    public Image powerupDiplay;
    private RectTransform rt;

    private void Awake() {
        if (powerupDiplay == null) // Check to ensure Image component has been set in inspector
            Debug.LogError("Image component not set in " + gameObject.name);
        rt = GetComponent<RectTransform>();
    }

    public void NewPowerup(PowerupData powerupData, Buff buff, Vector2 targetSize, float duration) {
        this.powerupData = powerupData;
        this.buff = buff;
        powerupDiplay.sprite = powerupData.image;
        StartCoroutine(ResetDisplay(targetSize, duration));
    }

    public IEnumerator ResetDisplay(Vector2 targetSize, float duration) {
        float endTime = Time.time + duration;
        while (endTime > Time.time) {
            float x = (endTime - Time.time) / duration;
            rt.sizeDelta = targetSize * (-Mathf.Pow(1.35f * x - 0.302f, 2) + 1.1f);
            yield return null;
        }
        rt.sizeDelta = targetSize;
    }
}
