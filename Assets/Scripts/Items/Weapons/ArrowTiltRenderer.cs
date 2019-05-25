using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ArrowTiltRenderer : MonoBehaviour
{
    [SerializeField]
    private Transform renderer;

    private Rigidbody _rigidbody;
    private Rigidbody rigidbody { get { return _rigidbody ?? GetComponent<Rigidbody>(); } }

    [SerializeField]
    [Tooltip("Rotations per sec")]
    private float rotationSpeed;

    private void FixedUpdate() {
        Vector3 right = Vector3.Cross(Vector3.up, rigidbody.velocity);
        renderer.rotation = Quaternion.LookRotation(right, Vector3.Cross(rigidbody.velocity, right));
        renderer.Rotate(rigidbody.velocity, Time.time * rotationSpeed * 360, Space.World);
    }
}
