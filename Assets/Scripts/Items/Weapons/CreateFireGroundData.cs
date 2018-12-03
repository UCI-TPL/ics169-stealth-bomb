using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CreateFireGroundData", menuName = "Item/Weapon/PowerupTriggers/CreateFireGroundData")]
public class CreateFireGroundData : WeaponData<CreateFireGround> {

    public GroundEffect burningGroundPrefab;
    public float hitCooldown = 0.1f;
    public float duration = 2f;
    public string counterName;

}
