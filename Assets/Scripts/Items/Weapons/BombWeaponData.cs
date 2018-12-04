using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Bomb Weapon", menuName = "Item/Weapon/BombWeapon")]
public class BombWeaponData : WeaponData<BombWeapon> {
    [Header("Projectile Variables")]
    public ProjectileData projectile;
    public int projNum = 1;
    public float projSpeed = 25f;

    [Header("Bomb Variables")]
    public GameObject bombPrefab;
    public float explosionSize = 2f;                    // Used to set the scale of a Bomb GameObject.
    [Range(0f, 1f)]                                     // This value is going to be put into a lerp equation, so it only needs to go from 0 to 1.
    public float growthRate = .05f;
    public int numberOfUses = 1;
}
