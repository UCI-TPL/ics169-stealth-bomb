using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Icicle Weapon", menuName = "Item/Weapon/IcicleWeapon", order = 7)]
public class IcicleWeaponData : WeaponData<IcicleWeapon> {

    public ProjectileData projectile;
    //public int numProj = 5;
    public float projSpeed = 4;
    public float targetDistance = 10;
    public float bulletSpacing = 0.25f;
    public float timeBetweenBullets = 0.1f;
    public int[] fireOrder;
}
