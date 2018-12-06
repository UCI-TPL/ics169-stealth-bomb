using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burning : MonoBehaviour {

	public float burningDuration = 100f;
	public float timer;

	private static PlayerController[] caughtPlayers = new PlayerController[4];
	
	private GameObject target = null;
	private bool foundTarget = false;
	public Event_Vector3_GameObject OnHit = new Event_Vector3_GameObject();

	private void Start() {
		timer = burningDuration;
	}

	private void Update () {
		// Debug.Log("caught players: " + (caughtPlayers[0] != null) + ", " + (caughtPlayers[1] != null) + ", " + (caughtPlayers[2] != null) + ", " + (caughtPlayers[3] != null));
		timer -= 1.0f * Time.deltaTime;
		burning();
		extinguish();
	}

	private void FixedUpdate()
	{

		
	}

	private void burning()
	{
		if (target != null)
		{
			Transform playerPos = target.gameObject.transform;
			Vector3 newPos = new Vector3(playerPos.position.x, playerPos.position.y+.5f, playerPos.position.z);
			this.transform.position = newPos;
			OnHit.Invoke(this.transform.position, target.gameObject.transform.position, target.gameObject);
		}
	}

	private void extinguish()
	{
		if (timer <= 0f)
		{
			if (target != null)
			{
				PlayerController pc = target.gameObject.GetComponent<PlayerController>();
				caughtPlayers[pc.player.playerNumber] = null;
				target = null;
			}
			Destroy(gameObject);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (target == null && other.gameObject.tag == "Player")
		{	
			target = other.gameObject;
			PlayerController pc = other.gameObject.GetComponent<PlayerController>();
			if (caughtPlayers[pc.player.playerNumber] == null)
			{
				caughtPlayers[pc.player.playerNumber] = pc;
			}
			else
			{
				Destroy(gameObject);
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			extinguish();
		}
	}

}
