using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEngine.ParticleSystemModule;

public class BlackHole : MonoBehaviour {

	public float blackHoleGravity;

	[HideInInspector]
	public int ignorePlayerIdx = -1;

	private Color particleColor;

	private SphereCollider collider;

    [SerializeField]
    private GameObject[] BlackholeEffects;
    [SerializeField]
    private Color vortexColor;
    [SerializeField]
    private Color backgroundColor;
    [SerializeField]
    [ColorUsage(true, true)]
    private Color sparksColor;

	private PlayerController[] caughtPlayers;

	public float blackHoleDuration = 3f;

	private float time;

	public GameObject center;

	// these variables are just to remember old position of black hole prototype.
	private float x = 13.55f;

	private float y = 6.52f;

	private float z = 14.72f;

	private float defaultCenterScale = 0.25f;
	private float defaultParticleSize = 0.1f;

    private float radius;

	void Awake() {
		// material = gameObject.GetComponent<MeshRenderer>().materials[0];
		// Material[] materials = gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().materials;
		// particleColor = gameObject.GetComponent<ParticleSystem>().main.startColor.;
		// Material[] materials = gameObject.GetComponent<ParticleSystemRenderer>().GetComponent<Renderer>().materials;
		// particleColor = gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial;
		// Debug.Log("trail material: " + particleColor.name);
		// Debug.Log("materials: " + "length = " + materials.Length + ", " + materials[0].name /*+ ", " + materials[1].name*/);
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
		//Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
		if (time >= blackHoleDuration)
			StartCoroutine(DestroyDelayed(3));
		time += 1.0f * Time.deltaTime;
	}

    IEnumerator DestroyDelayed(float duration) {
        //ParticleSystem p = gameObject.GetComponent<ParticleSystem>();
        //ParticleSystem.EmissionModule e = p.emission;
        //e.enabled = false;
        Vector3 baseScale = transform.localScale/duration;
        while (transform.localScale.x > 0) {
            transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, baseScale.x * Time.deltaTime);
            yield return null;
        }
        Destroy(gameObject);
    }

	void FixedUpdate() {
        // Debug.Log("time: " + time);
        if (time < blackHoleDuration) {
            for (int i = 0; i < caughtPlayers.Length; i++) {
                if (caughtPlayers[i] != null && i != ignorePlayerIdx)
                    PullPlayer(caughtPlayers[i]);
            }
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
			if (Vector3.Distance(transform.position, other.gameObject.transform.position) >= collider.radius + ((SphereCollider) other).radius) {
				PlayerController pc = other.gameObject.GetComponent<PlayerController>();
				caughtPlayers[pc.player.playerNumber] = null;
			}
		}
	}

	// helper method that defines how players are pulled towards the center of the black hole
	private void PullPlayer(PlayerController player) {
        Vector3 pullVector = Vector3.Scale(transform.position - player.transform.position, new Vector3(1, 0, 1));
		player.Knockback(pullVector.normalized * (1 - pullVector.magnitude / radius) * blackHoleGravity, showEffect: false);
	}

	// call to set parameters of black hole object
	public void SetupBlackHole(float radius, int playerNum, float gravity, float duration, Color color) {
        // gameObject.transform.localScale = new Vector3(radius*2, radius*2, radius*2);
        // Apply radius to all concerned components and children of black hole: particle system, black hole (main) collider, and black hole center
        //ParticleSystem.ShapeModule shape = gameObject.GetComponent<ParticleSystem>().shape;
        //shape.radius = radius;
        this.radius = radius;
        foreach (GameObject g in BlackholeEffects)
            g.transform.localScale = Vector3.one * radius * 2;
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
        Color.RGBToHSV(color, out float targetH, out float targetS, out float targetV);

        Color.RGBToHSV(vortexColor, out float H, out float S, out float V);
        BlackholeEffects[0].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(targetH, S * targetS, V * targetV);

        Color.RGBToHSV(backgroundColor, out H, out S, out V);
        BlackholeEffects[1].GetComponent<MeshRenderer>().material.color = Color.HSVToRGB(targetH, S * targetS, V * targetV);

        Color.RGBToHSV(sparksColor, out H, out S, out V);
        BlackholeEffects[2].GetComponent<MeshRenderer>().material.SetColor("_SparkColor", Color.HSVToRGB(targetH, S * targetS, V * targetV, true));

		// Debug.Log("has material = " + (particleColor != null));
        // particleColor.SetColor("Color_E025656E", HDRColor);
		// main.startColor = color;
		// trail.colorOverTrail = Color.red;
		// main.startColor = HDRColor;
		// trail.colorOverLifetime = HDRColor;
	}

	// public void RevertToPrefab()
}
