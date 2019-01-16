using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumbleTile : Tile {

    private const int MaxParticles = 150;
    private readonly static Queue<ParticleSystem> ParticlePool = new Queue<ParticleSystem>();
    private static Transform ParticlePoolParent;

    [Header("Destruction Properties")]
    [SerializeField]
    private float maxHealth;
    public float HealthPercent { get { return Health / maxHealth; } }
    public float Health { get; private set; }

    [Header("Material Properties")]
    public float shakeCooldown = 0.02f;
    public float destroyEffDuration = 0.5f;
    public float destroyEffSpeed = 5f;
    private Material crumbleMaterial;
    public GameObject particles;

    [HideInInspector]
    public Material BaseMaterial;
    private static readonly Dictionary<Material, Material[]> DamagedMaterials;
    [HideInInspector]
    public MeshFilter meshFilter;
    [HideInInspector]
    public MeshRenderer meshRenderer;
    [HideInInspector]
    public bool crumbling = false;

    private void Awake() {
        meshFilter = GetComponentInChildren<MeshFilter>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        BaseMaterial = meshRenderer.sharedMaterial;
        meshRenderer.enabled = false;
        if (ParticlePoolParent == null) {
            ParticlePoolParent = new GameObject("CrumbleParticlePool").transform;
            ParticlePoolParent.SetParent(GameManager.instance.PersistBetweenRounds);
            ParticlePool.Clear();
        }
        while (ParticlePool.Count < MaxParticles) { // Preload pool of particle systems during load time, so that play will be smoother(Instantiate is really slow)
            GameObject g = Instantiate(particles, ParticlePoolParent);
            g.SetActive(false);
            ParticlePool.Enqueue(g.GetComponent<ParticleSystem>());
        }

        // Instantiate health as max;
        ResetHealth();
    }

    /// <summary>
    /// Resets this tile's health to max health
    /// </summary>
    private void ResetHealth() {
        Health = maxHealth;
    }

    protected override void BreakingEffect(float duration) {
        crumbleMaterial = meshRenderer.material;
        if (crumbleMaterial.shader.name != "Crumble")
            Debug.Log(name + " has incorrect shader, Crumble shader required.", this);
        meshRenderer.enabled = crumbling = true;
        CrumbleEffect(duration);
        // Pull out the first particle system from the queue and reinsert it at the end
        ParticleSystem p = ParticlePool.Dequeue();
        ParticlePool.Enqueue(p);
        if (p != null) { // Check just to make sure there is a fall back if particle is for some reason deleted
            // Reuse old particle system taken from the pool
            p.transform.position = transform.position;
            p.gameObject.SetActive(true);
            p.Simulate(0, true, true);
            p.Play(true);
        }
    }

    private void CrumbleEffect(float duration) {
        crumbleMaterial.SetFloat("_CrumbleStartTime", Time.timeSinceLevelLoad); // Set shader crumble start time
        crumbleMaterial.SetFloat("_CrumbleDuration", duration); // Set shader crumble duration
    }

    protected override void DestroyEffect() {
        crumbleMaterial.SetFloat("_DestroyStartTime", Time.timeSinceLevelLoad); // Set shader destroy start time
        crumbleMaterial.SetFloat("_DestroyDuration", destroyEffDuration); // Set shader destroy duration
        GetComponentInChildren<Collider>().enabled = false;
        Destroy(gameObject, destroyEffDuration);
    }
}
