using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : Weapon {

    private LaserWeaponData data;
    private LaserBeam laserBeam;

    private float startChargeTime;
    protected override float GetChargeLevel() {
        return ((Time.time - startChargeTime) / data.maxChargeTime); // Charge Level is automatically clamped between 0 and 1
    }

    public LaserWeapon() : base() { }

    public LaserWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Charge) {
        data = (LaserWeaponData)weaponData;
    }

    //protected override void OnEquip() {
    //    laserBeam = GameObject.Instantiate<GameObject>(data.LaserBeam.gameObject, player.controller.transform).GetComponent<LaserBeam>();
    //    laserBeam.transform.forward = player.controller.transform.forward;
    //    laserBeam.transform.position += laserBeam.transform.forward * 0.5f;
    //}

    // OnActivate is called once when the weapon is activated
    protected override void OnActivate() {
        if (laserBeam == null) {
            laserBeam = GameObject.Instantiate<GameObject>(data.LaserBeam.gameObject, player.controller.transform).GetComponent<LaserBeam>();
            laserBeam.transform.forward = player.controller.transform.forward;
            laserBeam.transform.position = player.controller.transform.position + laserBeam.transform.forward * 0.5f;
        }
        startChargeTime = Time.time;
        laserBeam.gameObject.SetActive(true);
        laserBeam.MaxLength = 0;
        laserBeam.Width = data.minWidth;
    }

    // OnChargingUpdate is called once per frame while the weapon is charging
    protected override void OnChargingUpdate() {
        laserBeam.MaxLength = Mathf.Lerp(0, data.maxLength, ChargeLevel);
        laserBeam.Width = Mathf.Lerp(data.minWidth, data.maxWidth, ChargeLevel);
    }

    // OnRelease is called once when the weapon is released
    protected override void OnRelease() {
        laserBeam.gameObject.SetActive(false);
    }

    protected override void OnRemove() {
        GameObject.Destroy(laserBeam.gameObject);
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        LaserWeapon copy = new LaserWeapon(weaponData, player);
        return copy;
    }
}
