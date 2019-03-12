using ColorExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ShotGunEffect : SkillEffect {

    private new ParticleSystem particleSystem;
    [SerializeField]
    private ParticleSystem[] bullets;

    private bool hasSetUp = false;

    public override void Play(Color color) {
        if (!hasSetUp) {
            foreach (ParticleSystem p in bullets) {
                var main = p.main;
                main.startColor = color.ScaleHSV(main.startColor.color, multiplyBaseValue: false);
            }
            hasSetUp = true;
        }
        particleSystem.Play(true);
    }

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
    }
}
