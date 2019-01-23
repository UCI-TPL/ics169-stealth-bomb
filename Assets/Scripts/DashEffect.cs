using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DashEffect : MonoBehaviour {

    private ParticleSystem ps;
    private bool isActive;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetActive(bool active, Vector3 direction) {
        if (active == isActive)
            return;
        isActive = active;
        if (active) {
            transform.forward = direction;
            ps.Play(true);
        }
        else {
            ps.Stop(true);
        }
    }
}
