using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Black Hole Weapon", menuName = "Item/Weapon/BlackHoleWeapon")]
public class BlackHoleWeaponData : WeaponData<BlackHoleWeapon> {

	public ProjectileData projectile;

	public GameObject blackHolePrefab;

	public float blackHoleGravity = 1.5f;

	public float blackHoleRadius = 3f;
	public float blackHoleDuration = 3f;

	public float projSpeed = 4;

	public int numProj = 1;

	public int numOfUses = 1;

	public bool debugOn;

}
