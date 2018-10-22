using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Power up/Speed Power up", order = 1)]
public class StepPowerup : Powerup {

    public GameObject spawnPrefab;
    public float cooldown;
    private float refreshTime;

    public override void OnUpdate(Player player) {
        if (refreshTime <= Time.time && player.isMoving) {
            GameObject.Instantiate<GameObject>(spawnPrefab, player.transform.position, Quaternion.identity);
            refreshTime = Time.time + cooldown;
        }
    }
}
