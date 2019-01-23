using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class KnockbackEffect : MonoBehaviour {

    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystemRenderer impact;
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
            impact.lengthScale = Mathf.Max(2, Mathf.Sqrt(direction.magnitude)*2);
            ps.Play(true);
        }
        else {
            ps.Stop(true);
        }
    }
}
