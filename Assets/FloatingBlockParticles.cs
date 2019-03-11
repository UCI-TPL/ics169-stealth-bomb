using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
[RequireComponent(typeof(ParticleSystemForceField))]
public class FloatingBlockParticles : MonoBehaviour {

    private new ParticleSystem particleSystem;
    private ParticleSystemForceField particleSystemForceField;
    [SerializeField]
    private Transform crumbleIndicator;

    private void Awake() {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemForceField = GetComponent<ParticleSystemForceField>();
    }

    public void PlayParticles() {
        StopAllCoroutines();
        particleSystemForceField.endRange = 0;
        crumbleIndicator.localScale = Vector3.up * 20;
        particleSystem.Play(true);
    }

    public void DisperseParticles() {
        StartCoroutine(timer());

        IEnumerator timer() {
            StartCoroutine(GrowIndicator(new Vector3(100, 20, 100), 1f));
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(GrowForceField(50, 1f));
        }

        IEnumerator GrowIndicator(Vector3 targetScale, float duration) {
            float endTime = Time.time + duration;
            while (endTime > Time.time) {
                crumbleIndicator.localScale = Vector3.Lerp(targetScale, Vector3.up * 20, (endTime - Time.time) / duration);
                yield return null;
            }
            crumbleIndicator.localScale = Vector3.up * 20;
        }

        IEnumerator GrowForceField(float radius, float duration) {
            float endTime = Time.time + duration;
            while (endTime > Time.time) {
                particleSystemForceField.endRange = Mathf.Lerp(radius, 0, (endTime - Time.time) / duration);
                yield return null;
            }
            particleSystemForceField.endRange = radius;
            yield return new WaitForSeconds(5);
            particleSystemForceField.endRange = 0;
        }
    }
}
