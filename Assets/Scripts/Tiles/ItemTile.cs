using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemTile : Tile {

    public GameObject itemContainer;
    public GameObject powerupContainer;
    public GameObject weaponContainer;
    public ItemList itemList;
    public float spawnRateMin;
    public float spawnRateMax;
    public LayerMask ItemContainerLayer;

    private float lastSpawnTime;
    private float cooldown;
    private float cooldownPercent {
        get { return (Time.time - lastSpawnTime) / cooldown; }
    }
    [SerializeField]
    private float updateRate = 0.25f;

    // Set tile Type
    private void Awake() {
        type = Type.Item;
    }

    private void Start() {
        if (itemList == null) {
            Debug.Log(name + " is missing an Item list for it's drop table, This spawner will be disabled");
            gameObject.SetActive(false);
        }
        ResetCooldown();
        StartCoroutine(SpawnItems(updateRate));
    }

    // Continuously spawn items with updates happening at the specified rate
    private IEnumerator SpawnItems(float updateRate) {
        yield return new WaitForSeconds(Random.Range(0, updateRate)); // This just slightly offsets it from other spawners with the same update rate
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        while (true) { // Once the GameManager is implemented change this to run while a game is active
            if (cooldownPercent >= 1) { // This can be changed to make spawner faster by making it spawn at lower percents
                SpawnItem(itemList.RandomItem());
                ResetCooldown();
            }
            yield return wait;
        }
    }

    // Spawn the provided item with the correct Item Container
    private void SpawnItem(ItemData data) {
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
    private float ResetCooldown() {
        lastSpawnTime = Time.time;
        return cooldown = Random.Range(spawnRateMin, spawnRateMax);
    }

    // Destroy the Spawner as soon as the ground under it is decaying
    protected override void BreakingEffect(float duration) {
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other) {
        if (other.gameObject.GetComponent<ItemContainer>() != null)
            ResetCooldown();
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 1f, 1f, 0.75f);
        Gizmos.DrawWireCube(transform.position, transform.lossyScale);
        Gizmos.color = new Color(0.75f, 0.75f, 0.75f, 0.5f);
        Gizmos.DrawCube(transform.position, transform.lossyScale);
    }
}
