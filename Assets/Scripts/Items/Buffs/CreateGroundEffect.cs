using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGroundEffect : Weapon {

    protected CreateGroundEffectData data;

    private float distanceMoved;
    private float distanceCounter;

    // Limit how quickly ground effects can be spammed
    private static readonly float minDistance = 0.33f;
    private static readonly float minTime = 0.5f;
    private Vector3 lastPlaceLocation = Vector3.zero;
    private float lastPlaceTime = 0;

    public CreateGroundEffect() : base() { }

    public CreateGroundEffect(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (CreateGroundEffectData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        float size = player.stats.GetStat(data.counterName);
        if (data.checkDistance) {
            distanceMoved += direction.magnitude;
            if (distanceMoved - distanceCounter >= 0.5f * size) {
                distanceCounter = distanceMoved;
                for (float dist = direction.magnitude; dist > 0; dist -= 0.5f * size)
                    data.groundEffectPrefab.Create((Vector3 origin, GameObject target) => { Hit(origin, target.transform.position, target, null, false); }, player, start + direction.normalized * dist, size, data.duration, data.hitCooldown, player.controller.HitBox);
            }
        }
        else {
            Vector3 location = start + direction; // Check to see if ground effect is being placed in a different location, or if enough time has passed
            if (Vector3.Distance(lastPlaceLocation, location) > minDistance || lastPlaceTime + minTime <= Time.time) {
                lastPlaceLocation = location;
                lastPlaceTime = Time.time;
                data.groundEffectPrefab.Create((Vector3 origin, GameObject target) => { Hit(origin, target.transform.position, target, null, false); }, player, start + direction, size, data.duration, data.hitCooldown, player.controller.HitBox);
            }
        }
    }

    protected override void OnHit(Vector3 origin, Vector3 contactPoint, PlayerController targetPlayerController, object extraData) {
        if (targetPlayerController != null && data.debuff != null)
            targetPlayerController.player.AddBuff(data.debuff.Instance(data.debuffDuration, player));
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateGroundEffect copy = new CreateGroundEffect(weaponData, player);
        return copy;
    }
}
