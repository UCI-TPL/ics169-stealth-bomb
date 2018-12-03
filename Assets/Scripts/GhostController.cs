using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GhostController : PlayerController {

    //this inherits from PlayerController so the movement remains the same 

    /*
	// Use this for initialization
	void Start () {

        Debug.Log("A ghost controller was born, long live the ghooost");
		
	}
	
	// Update is called once per frame

	void Update () {
		
	}
    */


    Image CursorImage;

    protected float CheckGroundDistance() 
    {
        Debug.Log("Does this not ovverite?");
        return Mathf.Infinity;
    }

    private void Start()
    {
        
        //Debug.Log("Starting on the terms of the GHOST and ignoring all the input silliness");
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();
        CursorImage = GetComponent<Image>(); //used to change to player color
        CursorImage.color = playerColor;
        //rend.material.color = playerColor; //setting the player color based on playeNum 
        lastPosition = transform.position;

    }


    private void FixedUpdate()
    {
        Move(player.stats.moveSpeed);
        /*
        if (!dodging && !rolling)
            Move(IsGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
        else
            Move(dodgeSpeed); //hopefully this allows air dodges
        if (input.controllers[player.playerNumber].jump.Pressed)
        {
            Jump();
            if (jumped && rb.velocity.y > 0)
            {
                rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
            }
        }
        */
    }




}
