using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

	public float blackHoleGravity;

	private SphereCollider collider;

	private PlayerController[] caughtPlayers;

	// Use this for initialization
	void Start () {
		collider = gameObject.GetComponent<SphereCollider>();
		caughtPlayers = new PlayerController[4];
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
	}

	void FixedUpdate() {
		for (int i = 0; i < caughtPlayers.Length; i++) {
			if (caughtPlayers[i] != null) 
				PullPlayer(caughtPlayers[i]); 
		}
	}

	void OnTriggerEnter(Collider other) {
		if (other.gameObject.tag.Equals("Player")) {
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			caughtPlayers[pc.player.playerNumber] = pc;
		}
	}

	void OnTriggerExit(Collider other) {
		if (other.gameObject.tag.Equals("Player")) {
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			caughtPlayers[pc.player.playerNumber] = null;
		}
	}

	private void PullPlayer(PlayerController player) {
		float dirX = transform.position.x - player.gameObject.transform.position.x;
		float dirY = transform.position.y - player.gameObject.transform.position.y;
		float dirZ = transform.position.z - player.gameObject.transform.position.z;
		// float dirX = player.gameObject.transform.position.x - transform.position.x;
		// float dirY = player.gameObject.transform.position.y - transform.position.y;
		// float dirZ = player.gameObject.transform.position.z - transform.position.z;
		player.Knockback((new Vector3(dirX, dirY, dirZ).normalized + Vector3.up*0.25f).normalized * blackHoleGravity);
		// player.velocity = new Vector3(transform.position.x - player.position.x, transform.position.y - player.position.y, transform.position.z - player.position.z).normalized * blackHoleGravity;
	}
}
