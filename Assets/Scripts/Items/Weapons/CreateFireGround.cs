using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateFireGround : Weapon {

    private CreateFireGroundData data;

    public CreateFireGround() : base() { }

    public CreateFireGround(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (CreateFireGroundData)weaponData;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        fire burningGround = GameObject.Instantiate<GameObject>(data.firePrefab, player.controller.transform.position, Quaternion.identity).GetComponent<fire>();
        burningGround.hitCooldown = data.hitCooldown;
        burningGround.source = player;
        burningGround.IgnoreCollision = player.controller.gameObject;
        burningGround.OnHit = (Vector3 origin, GameObject target) => { Hit(origin, target); };
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateFireGround copy = new CreateFireGround(weaponData, player);
        return copy;
    }
}
