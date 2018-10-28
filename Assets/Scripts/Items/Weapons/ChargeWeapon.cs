using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChargeWeapon : Weapon {
    
    private float startChargeTime;
    public float chargeLevel {
        get { return Mathf.Min((Time.time - startChargeTime) / ((ChargeWeaponData)data).chargeTime, 1); }
    }

    Renderer rend;
    Color startColor;

    public ChargeWeapon() : base() { }

    public ChargeWeapon(WeaponData data, Player player) : base(data, player, Type.Charge) {
        rend = player.GetComponent<Renderer>();
        startColor = rend.material.color;
    }

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        startChargeTime = Time.time;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        rend.material.color = rend.material.color + (Color.red / ((ChargeWeaponData)data).colorAddition);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        rend.material.color = startColor;
        if (chargeLevel == 1)
            GameObject.Instantiate(((ChargeWeaponData)data).arrow, player.controller.ShootPoint.transform.position, player.transform.rotation, null); //this instantiates the arrow as an attack
    }

    // Create a deep copy of this weapon instance
    public override Weapon Clone(WeaponData data, Player player) {
        ChargeWeapon copy = new ChargeWeapon(data, player);
        return copy;
    }
}
