﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class KnockbackEffect : MonoBehaviour {

    private ParticleSystem ps;
    [SerializeField]
    private ParticleSystemRenderer impactRenderer;
    [SerializeField]
    private ParticleSystem trailPS;
    [SerializeField]
    private ParticleSystem impactPS;
    private bool isActive;

    [SerializeField]
    private KnockbackColor[] knockbackColors;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    public void SetActive(bool active, Vector3 direction, int? playerNumber = null) {
        if (active == isActive)
            return;
        isActive = active;
        if (active) {
            var trailmain = trailPS.main;
            trailmain.startColor = knockbackColors[playerNumber%(knockbackColors.Length-1) ?? knockbackColors.Length-1].trailColor;
            var impactmain = impactPS.main;
            impactmain.startColor = knockbackColors[playerNumber%(knockbackColors.Length-1) ?? knockbackColors.Length-1].impactColor;
            transform.forward = direction;
            impactRenderer.lengthScale = Mathf.Max(2, Mathf.Sqrt(direction.magnitude)*2);
            ps.Play(true);
        }
        else {
            ps.Stop(true);
        }
    }

    [System.Serializable]
    private struct KnockbackColor {
        public Color trailColor;
        public Color impactColor;
    }
}
