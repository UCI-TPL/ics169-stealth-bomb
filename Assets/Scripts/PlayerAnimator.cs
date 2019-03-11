using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {

    private Vector3 previousPosition;
    private Vector3 avgVelocity = Vector3.zero;
    private Vector3 acceleration;
    [SerializeField]
    private float smoothTime = 0.1f;
    [SerializeField]
    private float maxAngle = 75f;
    [SerializeField]
    private float maxVelocity = 10f;
    [SerializeField]
    private float velocityPower = 2f;

    private void Start() {
        previousPosition = transform.position;
    }

    private void LateUpdate() {
        Vector3 curVelocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        avgVelocity = Vector3.SmoothDamp(avgVelocity, curVelocity, ref acceleration, smoothTime);

        float angle = Mathf.Lerp(0, maxAngle, Mathf.Pow(avgVelocity.magnitude / maxVelocity, velocityPower));

        transform.rotation = Quaternion.AngleAxis(angle, Vector3.Cross(Vector3.up, avgVelocity)) * transform.parent.rotation;
    }
}
