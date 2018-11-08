using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    private static CameraShake _instance;
    private static CameraShake instance {
        get {
            if (_instance != null)
                return _instance;
            _instance = FindObjectOfType<CameraShake>();
            if (_instance == null) {
                Debug.LogError("CameraShake not found in current scene");
            }
            return _instance;
        }
    }

    public float baseShakeRate = 0.1f;
    public float baseDisplacement = 0.1f;
    private Vector3 originalPosition;
    private Vector3 originalRight;

    private void Start() {
        originalPosition = transform.localPosition;
        originalRight = transform.right;
    }

    public static void Shake(float intensity, float duration) {
        instance.StartCoroutine(instance.ProcessShake(intensity, duration));
    }

    public static void ShakeDiminish(float intensity, float duration) {
        instance.StartCoroutine(instance.ProcessShakeDiminish(intensity, duration));
    }

    private IEnumerator ProcessShakeDiminish(float intensity, float duration) {
        float endTime = Time.time + duration;
        float newIntensity;
        while (endTime >= Time.time) {
            newIntensity = intensity * (-Mathf.Pow(1 - (endTime - Time.time) / duration, 2) + 1);
            Shake(newIntensity, 0.1f);
            yield return new WaitForSeconds(0.1f);
        }
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
            distance = transform.localPosition - targetPosition;
            targetRight = originalRight + new Vector3(0 , Random.Range(-intensity / 200, intensity / 200), Random.Range(-intensity / 200, intensity / 200));
            distanceF = transform.right - targetRight;
            shakeDuration = ((baseShakeRate * distance.magnitude / 2) / maxDisplacement);
            timer = Time.time + shakeDuration;
            while (timer >= Time.time) {
                transform.localPosition = targetPosition + distance * (Mathf.Sin(3*((timer - Time.time) / shakeDuration) - 1.5f) + 1 )/2;
                transform.right = targetRight + distanceF * (Mathf.Sin(3 * ((timer - Time.time) / shakeDuration) - 1.5f) + 1) / 2;
                yield return null;
            }
            transform.localPosition = targetPosition;
            transform.right = targetRight;
        }
        targetPosition = originalPosition;
        distance = transform.localPosition - targetPosition;
        shakeDuration = ((baseShakeRate * distance.magnitude / 2) / maxDisplacement);
        timer = Time.time + shakeDuration;
        while (timer >= Time.time) {
            transform.localPosition = targetPosition + distance * (Mathf.Sin(3 * ((timer - Time.time) / shakeDuration) - 1.5f) + 1) / 2;
            yield return null;
        }
        transform.localPosition = originalPosition;
        transform.right = originalRight;
    }
}
