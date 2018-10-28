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
        instance.data = this;
    }

    public override Weapon NewInstance(Player player) {
        Weapon copy = instance.Clone(this, player);
        return copy;
    }
}

public abstract class WeaponData : ItemData {
    
    public abstract Weapon NewInstance(Player player);
}