using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;
using UnityEngine.UI;

public class inputControl : MonoBehaviour {

	public Rigidbody rb;
	public GamePadState currentState;

	Vector3 forward, right;

	private Vector3 _inputs = Vector3.zero;
	float speed;


	/////////////////////////////////////////////////////////

	private float moveY = 0f;
	private float moveX = 0f;
	public string currentMove = "ThumbSticks.Left";

	public Dropdown moveOpDp;

	List<string> moveOpList = new List<string>{ "ThumbSticks.Left", "ThumbSticks.Right" };

	private Hashtable opKey = new Hashtable();

	public void changeMoveOp(int i)
	{
		opKey["Move"] = moveOpList[i];
	}
	
	private void getCurrentInput()
	{
		moveY = 0f;
		moveX = 0f;
		if ((string)opKey["Move"] == "ThumbSticks.Left")
		{
			moveY = currentState.ThumbSticks.Left.Y;
			moveX = currentState.ThumbSticks.Left.X;
		}
		else if ((string)opKey["Move"] == "ThumbSticks.Right")
		{
			moveY = currentState.ThumbSticks.Right.Y;
			moveX = currentState.ThumbSticks.Right.X;
		}
	}

	void Start() {
		moveOpDp.AddOptions(moveOpList);
		rb = GetComponent<Rigidbody>();
		forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; 
		speed = 5f;

	}

	void Update() {
		// opKey["Move"] = currentMove;
		currentState = GamePad.GetState(0);
		_inputs = Vector3.zero;
		getCurrentInput();
		Move();
		Debug.Log(opKey["Move"]);
	}

	void FixedUpdate() {
		rb.velocity = _inputs * speed;
	}


	void Move()
    {
		Vector3 rightMovement = right * moveX;
		Vector3 upMovement = forward * moveY;
		_inputs = (rightMovement + upMovement);

		if (_inputs.magnitude > 1f) {
			_inputs = _inputs.normalized;
		}     
		// if (currentState.ThumbSticks.Left.Y != 0.0f || currentState.ThumbSticks.Left.X != 0.0f) 
		// {
		// 	Vector3 rightMovement = right * currentState.ThumbSticks.Left.X;
		// 	Vector3 upMovement = forward * currentState.ThumbSticks.Left.Y;
		// 	_inputs = (rightMovement + upMovement);

		// 	if (_inputs.magnitude > 1f) {
		// 		_inputs = _inputs.normalized;
		// 	}
		// }
    }
}
