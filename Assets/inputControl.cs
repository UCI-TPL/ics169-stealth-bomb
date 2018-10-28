﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class inputControl : MonoBehaviour {

	public Rigidbody rb;
	public GamePadState currentState;

	Vector3 forward, right;

	private Vector3 _inputs = Vector3.zero;
	float speed;

	void Start() {
		rb = GetComponent<Rigidbody>();
		forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; 
		speed = 5f;

	}

	void Update() {
		currentState = GamePad.GetState(0);
		_inputs = Vector3.zero;
		Move();
	}

	void FixedUpdate() {
		rb.velocity = _inputs * speed;
	}


	void Move()
    {     
		if (currentState.ThumbSticks.Left.Y != 0.0f || currentState.ThumbSticks.Left.X != 0.0f) 
		{
			Vector3 rightMovement = right * currentState.ThumbSticks.Left.X;
			Vector3 upMovement = forward * currentState.ThumbSticks.Left.Y;
			_inputs = (rightMovement + upMovement);

			if (_inputs.magnitude > 1f) {
				_inputs = _inputs.normalized;
			}
		}
    }
}
