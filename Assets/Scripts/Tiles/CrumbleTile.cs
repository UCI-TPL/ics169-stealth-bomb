using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleTile : Tile {

    private const int MaxParticles = 150;
    private readonly static Queue<ParticleSystem> ParticlePool = new Queue<ParticleSystem>();
    private static Transform ParticlePoolParent;

    private Material crumbleMaterial;
    public float shakeCooldown = 0.02f;
    public float destroyEffDuration = 0.5f;
    public float destroyEffSpeed = 5f;
    public GameObject particles;

    [HideInInspector]
    public Material BaseMaterial;
    [HideInInspector]
    public MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    [HideInInspector]
    public bool crumbling = false;

    private void Awake() {
        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();
        BaseMaterial = meshRenderer.sharedMaterial;
        if (ParticlePoolParent == null) {
            ParticlePoolParent = new GameObject("CrumbleParticlePool").transform;
            ParticlePoolParent.SetParent(GameManager.instance.PersistBetweenRounds);
        }
        while (ParticlePool.Count < MaxParticles) { // Preload pool of particle systems during load time, so that play will be smoother(Instantiate is really slow)
            GameObject g = Instantiate(particles, ParticlePoolParent);
            g.SetActive(false);
            ParticlePool.Enqueue(g.GetComponent<ParticleSystem>());
        }
        crumbleMaterial = GetComponent<Renderer>().material;
        if (crumbleMaterial.shader.name != "Crumble")
            Debug.Log(name + " has incorrect shader, Crumble shader required.");
        crumbleMaterial.SetFloat("Vector1_674F81FE", Random.Range(0, 100f)); // Set shader dissolve level
    }

    protected override void BreakingEffect(float duration) {
        meshRenderer.enabled = crumbling = true;
        StartCoroutine(CrumbleEffect(duration));
        // Pull out the first particle system from the queue and reinsert it at the end
        ParticleSystem p = ParticlePool.Dequeue();
        ParticlePool.Enqueue(p);
        // Reuse old particle system taken from the pool
        p.transform.position = transform.position;
        p.gameObject.SetActive(true);
        p.Simulate(0, true, true);
        p.Play(true);
    }

    private IEnumerator CrumbleEffect(float duration) {
        float startTime = Time.time;
        float endTime = Time.time + duration;
        float shakeTimer = Time.time;
        float targetScale = 1;
        float currentScale = 1;
        float scaleVelocity = 0;
        while (endTime >= Time.time) {
            if (shakeTimer <= Time.time) {
                targetScale = Random.Range(1.01f, 1.2f);
                shakeTimer = Mathf.Max(Time.time, shakeTimer + shakeCooldown);
            }
            currentScale = Mathf.SmoothDamp(currentScale, targetScale, ref scaleVelocity, shakeCooldown);
            crumbleMaterial.SetVector("Vector3_B38DBA48", Vector3.one * currentScale); // Set object scale with shader vertex offset
            crumbleMaterial.SetFloat("Vector1_B581EF45", (Time.time - startTime)/duration); // Set shader crumble level
            yield return null;
        }
        crumbleMaterial.SetFloat("Vector1_B581EF45", 1); // Set shader crumble level
    }

    protected override void DestroyEffect() {
        StartCoroutine(DisolveEffect(destroyEffDuration));
    }

    private IEnumerator DisolveEffect(float duration) {
        GetComponent<Collider>().enabled = false;
        float startTime = Time.time;
        float endTime = Time.time + duration;
        Vector3 offset = Vector3.zero;
        while (endTime >= Time.time) {
            crumbleMaterial.SetFloat("Vector1_C76A37C5", (Time.time - startTime) / duration); // Set shader dissolve level
            crumbleMaterial.SetVector("Vector3_5B57DCD6", (offset += Vector3.down * destroyEffSpeed * Time.deltaTime) * -offset.y); // Set object offset with shader vertex offset
            yield return null;
        }
        crumbleMaterial.SetFloat("Vector1_C76A37C5", 1); // Set shader dissolve level
        Destroy(gameObject);
    }
}
