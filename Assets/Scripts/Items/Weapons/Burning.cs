using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : MonoBehaviour {

	public float burningDuration = 100f;
	public float timer;
	
	private GameObject target;
	private bool foundTarget = false;
	public Event_Vector3_GameObject OnHit = new Event_Vector3_GameObject();

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

	private void FixedUpdate()
	{
		burning();
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
		if (foundTarget && target != null)
		{
			Vector3 newPos = new Vector3(transform.position.x+2f, transform.position.y-.5f, transform.position.z);
			OnHit.Invoke(newPos, target.gameObject.transform.position, target.gameObject);
		}
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
