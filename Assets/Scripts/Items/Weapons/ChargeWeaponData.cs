using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charged Weapon", menuName = "Item/Weapon/ChargeWeapon", order = 0)]
public class ChargeWeaponData : WeaponData<ChargeWeapon> {

    public ProjectileData projectile;
    public float colorAddition = 25f;
    public float chargeTime = 0.5f; //time between charge levels
    public float chargeLevels = 3;
    public int numProj = 1;
    public float projSpeed = 4; //this is the base speed, it will be multiplied by charge level
    public int damage = 35;
}
