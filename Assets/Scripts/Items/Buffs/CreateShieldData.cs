using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CreateShieldData", menuName = "Item/Weapon/PowerupTriggers/CreateShieldData")]
public class CreateShieldData : WeaponData<CreateShield> {
    public ShieldContainer shieldContainer;
    public Material[] PlayerColorMaterials;
}
