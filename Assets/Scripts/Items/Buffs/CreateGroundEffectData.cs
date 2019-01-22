using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New CreateGroundEffectData", menuName = "Item/Weapon/PowerupTriggers/CreateGroundEffectData")]
public class CreateGroundEffectData : WeaponData<CreateGroundEffect> {

    public GroundEffect groundEffectPrefab;
    public float hitCooldown = 0.1f;
    public float duration = 2f;
    public bool checkDistance;
    public string counterName;

    public BuffData debuff;
    public float debuffDuration;

}
