using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Charged Weapon", menuName = "Item/Weapon/ChargeWeapon", order = 0)]
public class ChargeWeaponData : WeaponData<ChargeWeapon> {

    public ProjectileData projectile;
    [Tooltip("The lower the number, the faster the color change")]
    public float colorAddition = 0.5f;
    [Tooltip("Limits the size of the aura/glow")]
    public float GlowLimit = 3; //where the aura/glow stops growing 
    public float chargeTime = 0.5f; //time between charge levels
    public float chargeLevels = 3;
    public int numProj = 1;
    public float projSpeed = 4; //this is the base speed, it will be multiplied by charge level
}
