﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour {

    public GameObject itemContainer;
    public GameObject powerupContainer;
    public GameObject weaponContainer;
    public ItemList itemList;
    public float spawnRateMin;
    public float spawnRateMax;

    private float cooldown;

    void Start() {
        ResetCooldown();
    }

    // Update is called once per frame
    void Update () {
		if (cooldown <= Time.time) {
            ItemData item = itemList.RandomItem();
            GameObject g = Instantiate(item.type == ItemData.Type.Powerup ? powerupContainer : item.type == ItemData.Type.Weapon ? weaponContainer : itemContainer, new Vector3(Mathf.Round(Random.Range(TerrainManager.terrainManager.mapArea.min.x, TerrainManager.terrainManager.mapArea.max.x)) +0.5f, transform.position.y + 1.5f, Mathf.Round(Random.Range(TerrainManager.terrainManager.mapArea.min.y, TerrainManager.terrainManager.mapArea.max.y)) +0.5f), Quaternion.identity);
            g.GetComponent<ItemContainer>().SetItemData(item);
            ResetCooldown();
        }
	}

    private float ResetCooldown() {
        return cooldown = Time.time + Random.Range(spawnRateMin, spawnRateMax);
    }
}
