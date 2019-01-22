using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour, IHurtable {

    [SerializeField]
    private float maxHealth;
    public float HealthPercent { get { return Health / maxHealth; } }
    public float Health { get; private set; }
    [Tooltip("passive health regeneration rate")]
    public float regenRate;
    [Tooltip("A multiplier to health regeneration rate while the shield is disabled")]
    public float disabledRegenMultiplier = 2;

    [Header("Do not Change")]
    [SerializeField]
    private GameObject container;
    public Collider[] colliders;
    [SerializeField]
    private MeshRenderer meshRenderer;

    private void Start() {
        ResetHealth();
    }

    /// <summary>
    /// Reset shield back to full health
    /// </summary>
    public void ResetHealth() {
        Health = maxHealth;
    }

    public float Hurt(Player damageDealer, float amount) {
        Health = Mathf.Max(Health - amount, 0);
        if (Health <= 0)
            container.SetActive(false);
        return 0;
    }

    void Update() {
        Health = Mathf.Min(Health + regenRate * Time.deltaTime * (container.activeSelf ? 1 : disabledRegenMultiplier), maxHealth);
        if (HealthPercent >= 1)
            container.SetActive(true);
    }

    public void SetMaterial(Material material) {
        meshRenderer.sharedMaterial = material;
    }
}
