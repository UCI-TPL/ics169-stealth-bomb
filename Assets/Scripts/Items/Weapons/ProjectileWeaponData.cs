using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Projectile Weapon", menuName = "Item/Weapon/ProjectileWeapon")]
public class ProjectileWeaponData : WeaponData<ProjectileWeapon> {

    public ProjectileData projectile;
    public int numProj = 1;
    public float projSpeed = 20;
    public string SoundName;

}
