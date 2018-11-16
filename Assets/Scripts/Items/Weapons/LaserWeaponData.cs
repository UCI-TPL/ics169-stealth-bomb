using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Laser Weapon", menuName = "Item/Weapon/LaserWeapon")]
public class LaserWeaponData : WeaponData<LaserWeapon> {

    public float maxChargeTime = 0.5f;
    public float maxLength = 15;
    public float maxWidth = 1.5f;
    public float minWidth = 0.5f;
    public int damage = 35;
    public float DecayTimePerWidth = 0.5f;
    public LaserBeam LaserBeam;
}
