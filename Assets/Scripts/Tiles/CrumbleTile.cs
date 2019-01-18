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
    public Texture[] DamagedTextures;

    [Header("Material Properties")]
    public float shakeCooldown = 0.02f;
    public float destroyEffDuration = 0.85f;
    public GameObject particles;
    
    [HideInInspector]
    public bool crumbling = false;

    private void Awake() {
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

    /// <summary>
    /// Inflicts damage to this and all tiles above equal to the specified amount. Destroys the tile if health reaches 0.
    /// </summary>
    /// <param name="amount"> Amount of damage taken </param>
    public override void Hurt(float amount) {
        if (crumbling) // If a block is already crumbling it cannot be hurt
            return;
        TileManager.tileManager.DamagePillar(position, amount);
    }

    /// <summary>
    /// (To be used only by TileManager) Applys damage to this tile
    /// </summary>
    /// <param name="amount"> Amount of damage taken</param>
    public override void ApplyDamage(float amount) {
        if (crumbling) // If a block is already crumbling it cannot be hurt
            return;
        Health = Mathf.Max(0, Health - amount);
        TileManager.tileManager.SetTileDamage(position, 0, 0, 1 - HealthPercent);
        if (Health <= 0)
            DestroyEffect();
    }

    protected override void BreakingEffect(float duration) {
        //crumbleMaterial = meshRenderer.material;
        //meshRenderer.enabled = crumbling = true;
        StartCoroutine(CrumbleEffect(duration));
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

    private IEnumerator CrumbleEffect(float duration) {
        float startTime = Time.time;
        float endTime = Time.time + duration;
        float shakeTimer = Time.time;
        while (endTime >= Time.time) {
            TileManager.tileManager.SetTileDamage(position, (Time.time - startTime) / duration, 0, 1-HealthPercent); // Set shader crumble level
            yield return null;
        }
        TileManager.tileManager.SetTileDamage(position, 1, 0, 1-HealthPercent); // Set shader crumble level
    }

    protected override void DestroyEffect() {
        StartCoroutine(DisolveEffect(destroyEffDuration));
    }

    private IEnumerator DisolveEffect(float duration) {
        GetComponentInChildren<Collider>().enabled = false;
        float startTime = Time.time;
        float endTime = Time.time + duration;
        while (endTime >= Time.time) {
            TileManager.tileManager.SetTileDamage(position, 1, (Time.time - startTime) / duration, 1-HealthPercent); // Set shader dissolve level
            yield return null;
        }
        TileManager.tileManager.SetTileDamage(position, 1, 1, 1-HealthPercent); // Set shader dissolve level
        Destroy(gameObject);
    }
}