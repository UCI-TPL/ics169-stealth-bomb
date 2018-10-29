using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleTile : Tile {

    private Material crumbleMaterial;
    public float destroyEffDuration = 0.5f;
    public float destroyEffSpeed = 5f;
    public GameObject particles;

    private void Start() {
        crumbleMaterial = GetComponent<Renderer>().material;
        if (crumbleMaterial.shader.name != "Crumble")
            Debug.Log(name + " has incorrect shader, Crumble shader required.");
    }

    protected override void BreakingEffect(float duration) {
        StartCoroutine(CrumbleEffect(duration));
        Instantiate<GameObject>(particles, transform.position, Quaternion.identity);
    }

    private IEnumerator CrumbleEffect(float duration) {
        float startTime = Time.time;
        float endTime = Time.time + duration;
        while (endTime >= Time.time) {
            float scale = Random.Range(0.95f, 1.05f);
            transform.localScale = Vector3.one * scale;
            crumbleMaterial.SetFloat("Vector1_B581EF45", (Time.time - startTime)/duration);
            yield return null;
        }
        crumbleMaterial.SetFloat("Vector1_B581EF45", 1);
    }

    protected override void DestroyEffect() {
        StartCoroutine(DisolveEffect(destroyEffDuration));
    }

    private IEnumerator DisolveEffect(float duration) {
        GetComponent<Collider>().enabled = false;
        float startTime = Time.time;
        float endTime = Time.time + duration;
        while (endTime >= Time.time) {
            crumbleMaterial.SetFloat("Vector1_C76A37C5", (Time.time - startTime) / duration);
            transform.Translate(Vector3.down * destroyEffSpeed * Time.deltaTime);
            yield return null;
        }
        crumbleMaterial.SetFloat("Vector1_C76A37C5", 1);
        Destroy(gameObject);
    }
}
