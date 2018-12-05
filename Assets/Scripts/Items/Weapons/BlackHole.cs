using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.ParticleSystemModule;

public class BlackHole : MonoBehaviour {

	public float blackHoleGravity;

	[HideInInspector]
	public int ignorePlayerIdx = -1;

	private Color particleColor;
	private ParticleSystem.MainModule main;
	private ParticleSystem.TrailModule trail;

	private SphereCollider collider;

	private PlayerController[] caughtPlayers;

	public float blackHoleDuration = 3f;

	private float time;

	public GameObject center;

	// these variables are just to remember old position of black hole prototype.
	private float x = 13.55f;

	private float y = 6.52f;

	private float z = 14.72f;

	private float defaultCenterScale = 0.5f;
	private float defaultParticleSize = 0.1f;

	void Awake() {
		// material = gameObject.GetComponent<MeshRenderer>().materials[0];
		// Material[] materials = gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().materials;
		// particleColor = gameObject.GetComponent<ParticleSystem>().main.startColor.;
		// Material[] materials = gameObject.GetComponent<ParticleSystemRenderer>().GetComponent<Renderer>().materials;
		// particleColor = gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial;
		// Debug.Log("trail material: " + particleColor.name);
		// Debug.Log("materials: " + "length = " + materials.Length + ", " + materials[0].name /*+ ", " + materials[1].name*/);
		main = gameObject.GetComponent<ParticleSystem>().main;
		trail = gameObject.GetComponent<ParticleSystem>().trails;
		collider = gameObject.GetComponent<SphereCollider>();
	}

	// Use this for initialization
	void Start () {
		time = 0.0f;
		caughtPlayers = new PlayerController[4];
		// material = gameObject.GetComponent<MeshRenderer>().m;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
		if (time >= blackHoleDuration)
			StartCoroutine(DestroyDelayed(3));
		time += 1.0f * Time.deltaTime;
	}

    IEnumerator DestroyDelayed(float duration) {
        ParticleSystem p = gameObject.GetComponent<ParticleSystem>();
        ParticleSystem.EmissionModule e = p.emission;
        e.enabled = false;
        Vector3 baseScale = center.transform.localScale/duration;
        while (p.particleCount > 0) {
            if (center.transform.localScale.x > 0)
                center.transform.localScale = Vector3.MoveTowards(center.transform.localScale, Vector3.zero, baseScale.x * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
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
			if (Distance(transform.position, other.gameObject.transform.position) >= collider.radius + ((SphereCollider) other).radius) {
				PlayerController pc = other.gameObject.GetComponent<PlayerController>();
				caughtPlayers[pc.player.playerNumber] = null;
			}
		}
	}

	private float Distance(Vector3 p1, Vector3 p2) {
		return Mathf.Sqrt(Squared(p2.x - p1.x) + Squared(p2.y - p1.y) + Squared(p2.z - p1.z));
	}

	private float Squared(float value) {
		return value * value;
	}

	// helper method that defines how players are pulled towards the center of the black hole
	private void PullPlayer(PlayerController player) {
		float dirX = transform.position.x - player.gameObject.transform.position.x;
		float dirY = transform.position.y - player.gameObject.transform.position.y;
		float dirZ = transform.position.z - player.gameObject.transform.position.z;
		// float dirX = player.gameObject.transform.position.x - transform.position.x;
		// float dirY = player.gameObject.transform.position.y - transform.position.y;
		// float dirZ = player.gameObject.transform.position.z - transform.position.z;
		player.Knockback((new Vector3(dirX, 0.0f, dirZ).normalized /*+ Vector3.up*0.25f*/).normalized * blackHoleGravity);
		// player.velocity = new Vector3(transform.position.x - player.position.x, transform.position.y - player.position.y, transform.position.z - player.position.z).normalized * blackHoleGravity;
	}

	// call to set parameters of black hole object
	public void SetupBlackHole(float radius, int playerNum, float gravity, float duration, Color color) {
		// gameObject.transform.localScale = new Vector3(radius*2, radius*2, radius*2);
		// Apply radius to all concerned components and children of black hole: particle system, black hole (main) collider, and black hole center
		ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape;
		shape.radius = radius;
		main.startSizeMultiplier = radius * defaultParticleSize * 0.5f;
		trail.widthOverTrailMultiplier = radius;
		collider.radius = radius;
		center.transform.localScale = new Vector3(radius*defaultCenterScale, radius*defaultCenterScale, radius*defaultCenterScale);

		// assign the other parameters to black hole: player to ignore (player that spawned the black hole), gravity, duration, and color
		ignorePlayerIdx = playerNum;
		blackHoleGravity = gravity;
		blackHoleDuration = duration;
		SetupColor(color);
		transform.Translate(0, center.transform.lossyScale.y / 4.0f, 0, Space.World);
	}

	public void SetupColor(Color color) {
		Color HDRColor = color * 2.0f;
		// Debug.Log("has material = " + (particleColor != null));
        // particleColor.SetColor("Color_E025656E", HDRColor);
		main.startColor = color;
		// trail.colorOverTrail = Color.red;
		// main.startColor = HDRColor;
		// trail.colorOverLifetime = HDRColor;
	}

	// public void RevertToPrefab()
}
