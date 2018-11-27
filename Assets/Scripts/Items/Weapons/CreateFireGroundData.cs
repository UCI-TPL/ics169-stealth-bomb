using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CreateFireGroundData", menuName = "Item/Weapon/PowerupTriggers/CreateFireGroundData")]
public class CreateFireGroundData : WeaponData<CreateFireGround> {

    public GameObject firePrefab;
    public float hitCooldown = 0.1f;

}
