using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charged Weapon", menuName = "Item/Weapon/ChargeWeapon", order = 1)]
public class ChargeWeaponData : WeaponData<ChargeWeapon> {

    public Projectile arrow;
    public float colorAddition = 25f;
    public float chargeTime = 0.5f;
    public float numProj = 1;
    public float projSpeed = 100;
}
