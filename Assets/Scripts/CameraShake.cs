using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public float baseShakeRate = 0.1f;
    public float baseDisplacement = 0.1f;
    public float intensity;
    private Vector3 originalPosition;
    private Vector3 originalRight;

    private void Start() {
        originalPosition = transform.position;
        originalRight = transform.right;
        StartCoroutine(Shaky());
    }

    private IEnumerator Shaky() {
        while (intensity > 0) {
            Shake(intensity, 0.1f);
            intensity -= 0.2f;
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Shake(float intensity, float duration) {
        //this.intensity = 1f;
        StartCoroutine(ProcessShake(intensity, duration));
    }

    private IEnumerator ProcessShake(float intensity, float duration) {
        float endTime = Time.time + duration;
        Vector3 targetPosition;
        Vector3 distance;
        Vector3 targetRight;
        Vector3 distanceF;
        float maxDisplacement = intensity * baseDisplacement;
        float timer;
        float shakeDuration;
        while (endTime > Time.time) {
            targetPosition = originalPosition + new Vector3(Random.Range(-maxDisplacement, maxDisplacement), Random.Range(-maxDisplacement, maxDisplacement), Random.Range(-maxDisplacement, maxDisplacement));
            distance = transform.position - targetPosition;
            targetRight = originalRight + new Vector3(0 , Random.Range(-intensity / 200, intensity / 200), Random.Range(-intensity / 200, intensity / 200));
            distanceF = transform.right - targetRight;
            shakeDuration = ((baseShakeRate * distance.magnitude / 2) / maxDisplacement);
            timer = Time.time + shakeDuration;
            while (timer >= Time.time) {
                transform.position = targetPosition + distance * (Mathf.Sin(3*((timer - Time.time) / shakeDuration) - 1.5f) + 1 )/2;
                transform.right = targetRight + distanceF * (Mathf.Sin(3 * ((timer - Time.time) / shakeDuration) - 1.5f) + 1) / 2;
                yield return null;
            }
            transform.position = targetPosition;
            transform.right = targetRight;
        }
        targetPosition = originalPosition;
        distance = transform.position - targetPosition;
        shakeDuration = ((baseShakeRate * distance.magnitude / 2) / maxDisplacement);
        timer = Time.time + shakeDuration;
        while (timer >= Time.time) {
            transform.position = targetPosition + distance * (Mathf.Sin(3 * ((timer - Time.time) / shakeDuration) - 1.5f) + 1) / 2;
            yield return null;
        }
        transform.position = originalPosition;
        transform.right = originalRight;
    }
}
