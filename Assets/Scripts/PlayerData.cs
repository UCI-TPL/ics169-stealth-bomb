using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Data", menuName = "Player")]
public class PlayerData : ScriptableObject {
    
    public WeaponData defaultWeapon;
    public SpecialMoveData defaultSpecialMove; 
}
