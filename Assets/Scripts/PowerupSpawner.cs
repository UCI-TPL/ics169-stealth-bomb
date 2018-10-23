using System.Collections;
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
		if (cooldown <= Time.time) {
            GameObject g = Instantiate(powerupHolder, new Vector3(Random.Range(0, 15)+0.5f, 1.5f, Random.Range(0, 15)+0.5f), Quaternion.identity);
            g.GetComponent<PowerupBehavior>().SetPowerupData(powerupList.RandomPowerup());
            ResetCooldown();
        }
	}

    private float ResetCooldown() {
        return cooldown = Time.time + Random.Range(spawnRateMin, spawnRateMax);
    }
}
