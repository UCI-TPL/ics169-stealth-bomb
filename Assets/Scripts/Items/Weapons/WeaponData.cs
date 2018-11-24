using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponData<WeaponType> : WeaponData where WeaponType : Weapon, new() {

    public WeaponType instance;

    private void OnEnable() {
        type = ItemData.Type.Weapon;
        if (instance == null)
            instance = new WeaponType();
        instance.weaponData = this;
    }

    public override Weapon NewInstance(Player player) {
        Weapon copy = instance.DeepCopy(this, player);
        return copy;
    }
}

public abstract class WeaponData : ItemData {

    public float cooldown = 0;
    public float knockbackStrength = 0;
    public float damage = 0;
    public BuffData[] buffs;

    public abstract Weapon NewInstance(Player player);

    public override void Use(Player player) {
        player.ChangeWeapon(this);
    }
}