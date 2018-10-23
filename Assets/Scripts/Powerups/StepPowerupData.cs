using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Powerup", menuName = "Power-up/Step Power-up", order = 1)]
public class StepPowerupData : PowerupData {

    public GameObject spawnPrefab;
    public float cooldown;

    public override Powerup NewInstance(Player player, bool isPermenant = false) {
        return new StepPowerup(this, player, isPermenant);
    }
}

public class StepPowerup : Powerup {
    private StepPowerupData data;
    private float refreshTime;

    public override List<PlayerStats.Modifier> modifiers {
        get { return data.modifiers; }
    }

    public StepPowerup(StepPowerupData data, Player player, bool isPermenant = false) : base(player, isPermenant) {
        this.data = data;
        refreshTime = 0;
        AddUpdate(OnUpdate);
    }

    public void OnUpdate() {
        if (refreshTime <= Time.time && player.isMoving) {
            GameObject.Instantiate<GameObject>(data.spawnPrefab, player.transform.position, Quaternion.identity);
            refreshTime = Time.time + data.cooldown;
        }
    }
}
