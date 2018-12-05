using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningDamage : Weapon {

	private BurningDamageData data;

	public BurningDamage(): base() {}

	public BurningDamage(WeaponData weaponData, Player player) : base(weaponData, player, Type.Instant) {
        data = (BurningDamageData) weaponData;
    }

	protected override void Start() {}

	protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null)
	{
		Burning b = GameObject.Instantiate(data.burningPrefab, 	start + direction, Quaternion.identity).GetComponent<Burning>();
		b.OnHit.AddListener((Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target, ChargeLevel); });
	}

	protected override void OnHit(Vector3 origin, Vector3 contactPoint, PlayerController targetPlayerController, object extraData)
	{
	}

	public override Weapon DeepCopy(WeaponData weaponData, Player player)
	{
		BurningDamage copy = new BurningDamage(weaponData, player);
		return copy;
	}
}
