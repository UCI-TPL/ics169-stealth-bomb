using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ItemTile : Tile {
    
    public GameObject itemContainer;
    public GameObject powerupContainer;
    public GameObject weaponContainer;
    public float cooldown = 1;
    public float overlapRadius = 4;
    public int minTier = 0;
    public int maxTier = int.MaxValue;
    public LayerMask ItemContainerLayer;

    private float cooldownStartTime = 0;
    private float CooldownPercent {
        get { return (Time.time - cooldownStartTime) / cooldown; }
    }
    public bool Available { get { return CooldownPercent >= 1; } }
    [SerializeField]
    private float updateRate = 0.25f;

    // Set tile Type
    private void Awake() {
        type = Type.Item;
    }

    private void Start() {
        StartCoroutine(CheckOverlap(updateRate));
    }

    // Spawn the provided item with the correct Item Container
    public void SpawnItem(ItemData data) {
        GameObject container;
        switch (data.type) {
            case ItemData.Type.Powerup:
                container = powerupContainer;
                break;
            case ItemData.Type.Weapon:
                container = weaponContainer;
                break;
            default:
                container = itemContainer;
                break;
        }
        GameObject g = Instantiate(container, transform.position, Quaternion.identity);
        g.GetComponent<ItemContainer>().SetItemData(data);
    }

    // Reset the cooldown by setting required variables
    private void ResetCooldown() {
        cooldownStartTime = Time.time;
    }

    // Destroy the Spawner as soon as the ground under it is decaying
    protected override void BreakingEffect(float duration) {
        Destroy(gameObject);
    }

    // Continuously check if any items overlap this spawner's area and if so reset the cooldown
    private IEnumerator CheckOverlap(float updateRate) {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (true) {
            if (Physics.OverlapSphere(transform.position, overlapRadius, ItemContainerLayer).Length > 0)
                ResetCooldown();
            yield return wait;
        }
    }

    private void OnDrawGizmos() {
        if (Available) {
            Gizmos.color = new Color(0.5f, 1f, 0.5f, 0.75f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
            Gizmos.color = new Color(0f, 1f, 0f, 0.75f);
            Gizmos.DrawCube(transform.position, transform.lossyScale);
        } else {
            Gizmos.color = new Color(1f, 0.5f, 0.5f, 0.75f);
            Gizmos.DrawWireCube(transform.position, transform.lossyScale);
            Gizmos.color = new Color(1f, 0f, 0f, 0.75f);
            Gizmos.DrawCube(transform.position, transform.lossyScale);
        }

        Gizmos.color = new Color(1, 0.25f, 0.25f, 0.15f);
        Gizmos.DrawSphere(transform.position, overlapRadius);

#if UNITY_EDITOR
        Handles.Label(transform.position, "Cooldown Time: " + cooldown.ToString());
#endif
    }
}
