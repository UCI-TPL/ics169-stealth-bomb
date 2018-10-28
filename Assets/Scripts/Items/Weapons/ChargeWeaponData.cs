using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charged Weapon", menuName = "Item/Weapon/ChargeWeapon", order = 0)]
public class ChargeWeaponData : WeaponData<ChargeWeapon> {

    public ProjectileData projectile;
    public float colorAddition = 25f;
    public float chargeTime = 0.5f;
    public int numProj = 1;
    public float projSpeed = 100;
}
