using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorExtensions;

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
    private KnockbackColor knockbackColor;
    [SerializeField]
    private KnockbackColor defaultColor;

    private void Awake() {
        ps = GetComponent<ParticleSystem>();
    }

    public void Activate(float time, Color? color = null) {
        var trailmain = trailPS.main;
        trailmain.startColor = color?.ScaleHSV(knockbackColor.trailColor) ?? defaultColor.trailColor;
        foreach (ParticleSystem p in impactPS) {
            p.Clear();
            var impactmain = p.main;
            impactmain.startColor = color?.ScaleHSV(knockbackColor.impactColor) ?? defaultColor.impactColor;
            impactmain.startLifetime = time - 0.05f;
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
