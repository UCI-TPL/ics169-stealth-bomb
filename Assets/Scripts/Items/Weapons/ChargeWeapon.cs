using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeWeapon : Weapon {

    private ChargeWeaponData data;

    private float startChargeTime;
    public float chargeLevel {
        get { return Mathf.Min((Time.time - startChargeTime) / data.chargeTime, 1); }
    }

    Renderer rend;
    Color startColor;

    public ChargeWeapon() : base() { }

    public ChargeWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (ChargeWeaponData)weaponData;
        rend = player.GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        startChargeTime = Time.time;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        rend.material.color = rend.material.color + (Color.red / data.colorAddition);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        rend.material.color = startColor;
            data.projectile.Shoot(player, data.projSpeed * chargeLevel, data.numProj);
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        ChargeWeapon copy = new ChargeWeapon(weaponData, player);
        return copy;
    }
}
