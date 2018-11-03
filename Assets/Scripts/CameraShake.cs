using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

    public float baseShakeRate = 0.1f;
    public float baseDisplacement = 0.1f;
    public float intensity;
    private Vector3 originalPosition;
    private Vector3 originalForward;

    private void Start() {
        originalPosition = transform.position;
        originalForward = transform.forward;
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
        Vector3 targetForward;
        Vector3 distanceF;
        float maxDisplacement = intensity * baseDisplacement;
        float timer;
        float shakeDuration;
        while (endTime > Time.time) {
            targetPosition = originalPosition + new Vector3(Random.Range(-maxDisplacement, maxDisplacement), Random.Range(-maxDisplacement, maxDisplacement), Random.Range(-maxDisplacement, maxDisplacement));
            distance = transform.position - targetPosition;
            targetForward = originalForward + new Vector3(Random.Range(-intensity / 100, intensity / 100), Random.Range(-intensity / 400, intensity / 400), 0);
            distanceF = transform.forward - targetForward;
            shakeDuration = ((baseShakeRate * distance.magnitude / 2) / maxDisplacement);
            timer = Time.time + shakeDuration;
            while (timer >= Time.time) {
                transform.position = targetPosition + distance * (Mathf.Sin(3*((timer - Time.time) / shakeDuration) - 1.5f) + 1 )/2;
                transform.forward = targetForward + distanceF * (Mathf.Sin(3 * ((timer - Time.time) / shakeDuration) - 1.5f) + 1) / 2;
                yield return null;
            }
            transform.position = targetPosition;
            transform.forward = targetForward;
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
        transform.forward = originalForward;
    }
}
