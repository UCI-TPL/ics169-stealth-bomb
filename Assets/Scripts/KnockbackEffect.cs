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
    private ParticleSystem[] impactPS;
    private bool isActive;

    [SerializeField]
    private KnockbackColor[] knockbackColors;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    public void Activate(float time, int? playerNumber = null) {
        var trailmain = trailPS.main;
        trailmain.startColor = knockbackColors[playerNumber % (knockbackColors.Length - 1) ?? knockbackColors.Length - 1].trailColor;
        foreach (ParticleSystem p in impactPS) {
            p.Clear();
            var impactmain = p.main;
            impactmain.startColor = knockbackColors[playerNumber % (knockbackColors.Length - 1) ?? knockbackColors.Length - 1].impactColor;
            impactmain.startLifetime = time - 0.01f;
        }
        ps.Play(true);
        StopAllCoroutines();
        StartCoroutine(DelayedStop(time));
    }

    private IEnumerator DelayedStop(float time) {
        float endTime = Time.time + time;
        while (endTime > Time.time)
            yield return null;
        ps.Stop();
        //foreach (ParticleSystem p in impactPS)
        //    p.Clear();
    }

    [System.Serializable]
    private struct KnockbackColor {
        public Color trailColor;
        public Color impactColor;
    }
}
