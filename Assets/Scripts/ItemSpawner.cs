using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {
    
    private static ItemSpawner instance;
    public static ItemSpawner Instance {
        get {
            if (instance != null)
                return instance;
            instance = FindObjectOfType<ItemSpawner>();
            if (instance == null)
                Debug.LogWarning("ItemSpawner not found, Items will not spawn, please add an Item Spawner to GameManager");
            return instance;
        }
    }
    
    public ItemList itemList;
    public float spawnRateMin;
    public float spawnRateMax;

    private Dictionary<int, List<ItemTile>> SpawnersPerTier = new Dictionary<int, List<ItemTile>>(); 

    private float cooldown;

    void Start() {
        ResetCooldown();
    }

    public void UpdateSpawnPoints(TileMap tileMap) {
        SpawnersPerTier = new Dictionary<int, List<ItemTile>>();
        for (int i = 0; i < itemList.tiers.Length; ++i)
            SpawnersPerTier.Add(i, new List<ItemTile>());
        foreach (ItemTile itemTile in tileMap.ItemTiles) {
            for (int tier = itemTile.minTier; tier <= Mathf.Min(itemTile.maxTier, itemList.tiers.Length - 1); ++tier)
                SpawnersPerTier[tier].Add(itemTile);
        }
    }

    // Update is called once per frame
    void Update () {
		if (cooldown <= Time.time) {
            for (int tier = itemList.RandomTier(); tier >= 0; --tier) {
                List<ItemTile> availableTiles = GetAvailableItemTiles(tier);
                if (availableTiles.Count > 0) {
                    availableTiles[Random.Range(0, availableTiles.Count)].SpawnItem(itemList.RandomItem(tier));
                    break;
                }
            }
            ResetCooldown();
        }
	}

    private List<ItemTile> GetAvailableItemTiles(int tier) {
        List<ItemTile> resultList = new List<ItemTile>();
        if (!SpawnersPerTier.ContainsKey(tier))
            return resultList;
        for (int i = SpawnersPerTier[tier].Count - 1; i >= 0; --i) {
            if (SpawnersPerTier[tier][i] == null)
                SpawnersPerTier[tier].RemoveAt(i);
            else if (SpawnersPerTier[tier][i].Available)
                resultList.Add(SpawnersPerTier[tier][i]);
        }
        return resultList;
    }

    private float ResetCooldown() {
        return cooldown = Time.time + Random.Range(spawnRateMin, spawnRateMax);
    }
}
