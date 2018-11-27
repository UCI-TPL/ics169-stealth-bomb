using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleWeapon : Weapon {

	private BlackHoleWeaponData data;

	private SphereCollider collider;

	private PlayerController[] caughtPlayers;

	public BlackHoleWeapon(): base() { }

	public BlackHoleWeapon(WeaponData weaponData, Player player): base(weaponData, player, Type.Instant) {
		this.data = (BlackHoleWeaponData) weaponData;
	}

	// Use this for initialization
	protected override void Start () {
		
	}

	protected override void OnActivate() {
		data.projectile.Shoot(player, data.projSpeed, data.numProj, (Vector3 origin, Vector3 contactPoint, GameObject target) => { CreateBlackHole(contactPoint); });
	}

	protected override void OnHit(Vector3 origin, PlayerController targetPlayerController, object extraData) {
		GameObject.Instantiate(data.blackHolePrefab, (Vector3) extraData, Quaternion.identity);
	}

	protected void CreateBlackHole(Vector3 spawnPoint) {
		BlackHole blackHole = GameObject.Instantiate(data.blackHolePrefab, spawnPoint, Quaternion.identity).GetComponent<BlackHole>();
		blackHole.SetupBlackHole(data.blackHoleRadius, player.playerNumber, data.blackHoleGravity, data.blackHoleDuration, player.controller.playerColor);
	}
	
	// Update is called once per frame
	void Update () {
		// Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
	}

	// void FixedUpdate() {
	// 	for (int i = 0; i < caughtPlayers.Length; i++) {
	// 		if (caughtPlayers[i] != null) 
	// 			PullPlayer(caughtPlayers[i]); 
	// 	}
	// }

	public override Weapon DeepCopy(WeaponData weaponData, Player player) {
		BlackHoleWeapon copy = new BlackHoleWeapon(weaponData, player);
		return copy;
	}

	// private void PullPlayer(PlayerController player) {
	// 	float dirX = this.player.controller.transform.position.x - player.gameObject.transform.position.x;
	// 	float dirY = this.player.controller.transform.position.y - player.gameObject.transform.position.y;
	// 	float dirZ = this.player.controller.transform.position.z - player.gameObject.transform.position.z;
	// 	// float dirX = player.gameObject.transform.position.x - transform.position.x;
	// 	// float dirY = player.gameObject.transform.position.y - transform.position.y;
	// 	// float dirZ = player.gameObject.transform.position.z - transform.position.z;
	// 	player.Knockback((new Vector3(dirX, dirY, dirZ).normalized + Vector3.up*0.25f).normalized * data.blackHoleGravity);
	// 	// player.velocity = new Vector3(transform.position.x - player.position.x, transform.position.y - player.position.y, transform.position.z - player.position.z).normalized * blackHoleGravity;
	// }
}
