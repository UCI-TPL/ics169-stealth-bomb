﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : Weapon {

    private ProjectileWeaponData data;

    public ProjectileWeapon() : base() { }
    
    public ProjectileWeapon(WeaponData weaponData, Player player) : base(weaponData, player, Type.Instant) {
        data = (ProjectileWeaponData)weaponData;
    }

    protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null) {
        data.projectile.Shoot(player, data.projSpeed, data.numProj, (Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target); });
        if(data.SoundName != "")
            GameManager.instance.audioManager.Play(data.SoundName);
    }

    // Create a deep copy of this weapon instance
    public override Weapon DeepCopy(WeaponData weaponData, Player player) {
        ProjectileWeapon copy = new ProjectileWeapon(weaponData, player);
        return copy;
    }
}
