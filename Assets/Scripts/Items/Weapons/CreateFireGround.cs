using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFireGround : Weapon {

    private CreateFireGroundData data;

    private float distanceMoved;
    private float distanceCounter;

    public CreateFireGround() : base() { }

    public CreateFireGround(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (CreateFireGroundData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        distanceMoved += direction.magnitude;
        float size = player.stats.GetStat(data.counterName);
        if (distanceMoved - distanceCounter >= 0.5f * size) {
            distanceCounter = distanceMoved;
            for (float dist = direction.magnitude; dist > 0; dist -= 0.5f * size)
                data.burningGroundPrefab.Create((Vector3 origin, GameObject target) => { Hit(origin, target); }, player, start + direction.normalized * dist, size, data.duration, data.hitCooldown, player.controller.HitBox);
        }
    }
    
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateFireGround copy = new CreateFireGround(weaponData, player);
        return copy;
    }
}
