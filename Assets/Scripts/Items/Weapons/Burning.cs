using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : MonoBehaviour {

	public float burningDuration = 100f;
	public float timer;
	
	private GameObject target;
	private bool foundTarget = false;

	private void Start() {
		timer = burningDuration;
	}

	private void Update () {
		stickOnTarget();
		timer -= 1.0f * Time.deltaTime;
		if (timer <= 0f)
		{
			Destroy(gameObject);
		}
	}

	private void stickOnTarget()
	{
		if (target != null)
		{
			Transform playerPos = target.gameObject.transform;
			Vector3 newPos = new Vector3(playerPos.position.x, playerPos.position.y+.5f, playerPos.position.z);
			this.transform.position = newPos;
		}
	}

	private void burning()
	{
		
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!foundTarget && other.gameObject.tag == "Player")
		{
			target = other.gameObject;
			foundTarget = true;
		}
		//not found target but hit non-player object --> self-destruction
		else if (!foundTarget && other.gameObject.tag != "Player")
		{
			Destroy(gameObject);
		}
	}
}
