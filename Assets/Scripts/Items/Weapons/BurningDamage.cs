using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurningDamage : Weapon {

	private BurningDamageData data;

	public BurningDamage(): base() {}

	public BurningDamage(WeaponData weaponData, Player player) : base(weaponData, player, Type.Instant) {
        data = (BurningDamageData) weaponData;
    }

	private int[] burningPlayers;

	protected override void Start() {}

	protected override void OnActivate(Vector3 start, Vector3 direction, PlayerController targetController = null)
	{
		if (targetController != null)
		{
			// if (targetController.player)
			// Debug.Log(start+direction);
			// Debug.Log(targetController.gameObject.transform.position);
			Burning b = GameObject.Instantiate(data.burningPrefab, targetController.gameObject.transform.position, Quaternion.identity).GetComponent<Burning>();
			b.OnHit.AddListener((Vector3 origin, Vector3 contactPoint, GameObject target) => { Hit(origin, contactPoint, target, null, false); });
		}
	}

	// protected override void OnHit(Vector3 origin, Vector3 contactPoint, PlayerController targetPlayerController, object extraData)
	// {
	// 	Debug.Log("hit in OnHit");
	// }

	public override Weapon DeepCopy(WeaponData weaponData, Player player)
	{
		BurningDamage copy = new BurningDamage(weaponData, player);
		return copy;
	}
}
