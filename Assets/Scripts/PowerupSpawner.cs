﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour {

    public GameObject powerupHolder;
    public PowerupList powerupList;
    public float spawnRateMin;
    public float spawnRateMax;

    private float cooldown;

    void Start() {
        ResetCooldown();
    }

    // Update is called once per frame
    void Update () {
        print(TerrainManager.terrainManager.mapArea);
		if (cooldown <= Time.time) {
            GameObject g = Instantiate(powerupHolder, new Vector3(Mathf.Round(Random.Range(TerrainManager.terrainManager.mapArea.min.x, TerrainManager.terrainManager.mapArea.max.x)) +0.5f, 1.5f, Mathf.Round(Random.Range(TerrainManager.terrainManager.mapArea.min.y, TerrainManager.terrainManager.mapArea.max.y)) +0.5f), Quaternion.identity);
            g.GetComponent<PowerupBehavior>().SetPowerupData(powerupList.RandomPowerup());
            ResetCooldown();
        }
	}

    private float ResetCooldown() {
        return cooldown = Time.time + Random.Range(spawnRateMin, spawnRateMax);
    }
}