using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour {

	public float blackHoleGravity;

	[HideInInspector]
	public int ignorePlayerIdx = -1;

	private Material material;

	private SphereCollider collider;

	private PlayerController[] caughtPlayers;

	public float blackHoleDuration = 3f;

	private float time;

	// these variables are just to remember old position of black hole prototype.
	private float x = 13.55f;

	private float y = 6.52f;

	private float z = 14.72f;

	void Awake() {
		material = gameObject.GetComponent<MeshRenderer>().materials[0];
	}

	// Use this for initialization
	void Start () {
		time = 0.0f;
		collider = gameObject.GetComponent<SphereCollider>();
		caughtPlayers = new PlayerController[4];
		// material = gameObject.GetComponent<MeshRenderer>().m;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
		if (time >= blackHoleDuration)
			Destroy(gameObject);
		time += 1.0f * Time.deltaTime;
	}

	void FixedUpdate() {
		// Debug.Log("time: " + time);
		for (int i = 0; i < caughtPlayers.Length; i++) {
			if (caughtPlayers[i] != null && i != ignorePlayerIdx) 
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

	public void SetupBlackHole(float radius, int playerNum, float gravity, float duration, Color color) {
		gameObject.transform.localScale = new Vector3(radius*2, radius*2, radius*2);
		ignorePlayerIdx = playerNum;
		blackHoleGravity = gravity;
		blackHoleDuration = duration;
		SetupColor(color);
	}

	public void SetupColor(Color color) {
		Color HDRColor = color * 2;
		Debug.Log("has material = " + (material != null));
        material.SetColor("Color_E025656E", HDRColor);
	}
}
