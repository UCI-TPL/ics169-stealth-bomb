using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    //This will control everything relating to the player 

   [SerializeField ]
   float speed;
   public bool lerp = false;

   Vector3 forward, right;
    
	void Start () {
        forward = Camera.main.transform.forward; 
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // This right vector is -45 degrees from the world X axis 
	}

    void Move(string horizontal, string vertical)
    {
        Debug.Log(horizontal);
        Vector3 direction = new Vector3(Input.GetAxis(horizontal), 0, Input.GetAxis(vertical));
        Vector3 rightMovement = right * speed * Time.deltaTime * Input.GetAxis(horizontal);
        Vector3 upMovement = forward * speed * Time.deltaTime * Input.GetAxis(vertical);
        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);
        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetAxis("Horizontal") != 0.0 | Input.GetAxis("Vertical") != 0.0) //Left Joystick and WASD
            Move("Horizontal","Vertical");
        else if (Input.GetAxis("HorizontalKey") != 0.0 | Input.GetAxis("VerticalKey") != 0.0) //D-Pad
            Move("HorizontalKey", "VerticalKey");
	}
}
