using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGroundEffect : Weapon {

    protected CreateGroundEffectData data;

    private float distanceMoved;
    private float distanceCounter;

    public CreateGroundEffect() : base() { }

    public CreateGroundEffect(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (CreateGroundEffectData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        distanceMoved += direction.magnitude;
        float size = player.stats.GetStat(data.counterName);
        if (distanceMoved - distanceCounter >= 0.5f * size) {
            distanceCounter = distanceMoved;
            for (float dist = direction.magnitude; dist > 0; dist -= 0.5f * size)
                data.groundEffectPrefab.Create((Vector3 origin, GameObject target) => { Hit(origin, target); }, player, start + direction.normalized * dist, size, data.duration, data.hitCooldown, player.controller.HitBox);
        }
    }

    protected override void OnHit(Vector3 origin, PlayerController targetPlayerController, object extraData) {
        if (data.debuff != null)
            targetPlayerController.player.AddBuff(data.debuff.Instance(data.debuffDuration, player));
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateGroundEffect copy = new CreateGroundEffect(weaponData, player);
        return copy;
    }
}
