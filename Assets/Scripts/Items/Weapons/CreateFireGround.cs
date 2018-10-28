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
        GameObject.Instantiate<GameObject>(data.firePrefab, player.transform.position, Quaternion.identity);
    }

    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        CreateFireGround copy = new CreateFireGround(weaponData, player);
        return copy;
    }
}
