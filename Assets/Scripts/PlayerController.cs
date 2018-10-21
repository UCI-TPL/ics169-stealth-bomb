using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    private Player _player;
    public Player player {
        get { return _player; }
        private set { _player = value; }
    }

    private void Awake() {
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
    }
    [SerializeField]
    float shootRate = 0.2f;
    float chargeTime = 0.5f;
    float shootTime = 0.0f;
    float holdTime = 0.0f;
    float holdStart = 0.0f;
    float holdEnd = 0.0f;
    bool isGrounded;
    //bool buttonHeldDown = false ;
    //bool holdCheck = false;
    public bool newMovement = true; //this varialbe is temporary and for testing only
    public float speed;
    private Vector3 _inputs = Vector3.zero;
    Vector3 forward, right;
    public GameObject ShootPoint;
    public Rigidbody rb;
    public Projectile arrow; //this is used for the Basic Attack
    [Tooltip("Represents which player this is. Only put in 1-4. Do not put 0!!! This attribute must have a value in order to work or take in input properly!!! ")]
    public int playerNum;
   

    // These are the suffixes used to form a string that represents a specific input on a specific Xbox controller.
    // The suffixes represent the types of inputs found on an Xbox controller (NOTE: more might be added later in the script).
    [HideInInspector]
    public string left_Joystick_X_Axis = "LeftJoystickX";
    [HideInInspector]
    public string left_Joystick_Y_Axis = "LeftJoystickY";
    [HideInInspector]
    public string right_Joystick_X_Axis = "RightJoystickX";
    [HideInInspector]
    public string right_Joystick_Y_Axis = "RightJoystickY";
    [HideInInspector]
    public string leftBumper = "LeftBumper";
    [HideInInspector]
    public string rightBumper = "RightBumper";
    [HideInInspector]
    public string leftTrigger = "LeftTrigger";
    [HideInInspector]
    public string rightTrigger = "RightTrigger";
    [HideInInspector]
    public string Dpad_X_Axis = "DpadX";
    [HideInInspector]
    public string Dpad_Y_Axis = "DpadY";

    // These are the prefixes used to form a string that represents a specific input on a specific Xbox controller.
    // The prefixes represent which Xbox controller an instance of this script should take input from.
    [HideInInspector]
    public const string Player_1_Str = "P1_";
    [HideInInspector]
    public const string Player_2_Str = "P2_";
    [HideInInspector]
    public const string Player_3_Str = "P3_";
    [HideInInspector]
    public const string Player_4_Str = "P4_";

    private string playerPrefix;

    public float DpadX()
    {
        return Input.GetAxis(playerPrefix + "DpadX");
    }

    public float DpadY()
    {
        return Input.GetAxis(playerPrefix + "DpadY");
    }

    public float LeftStickX()
    {
        return Input.GetAxis(playerPrefix + left_Joystick_X_Axis);
    }

    public float LeftStickY()
    {
        return Input.GetAxis(playerPrefix + left_Joystick_Y_Axis);
    }

    public float RightStickX()
    {
        return Input.GetAxis(playerPrefix + right_Joystick_X_Axis);
    }

    public float RightStickY()
    {
        return Input.GetAxis(playerPrefix + right_Joystick_Y_Axis);
    }

    public float RightTrigger()
    {
        return Input.GetAxis(playerPrefix + rightTrigger);
    }


    void Start() {
        //shootRate = player.stats.shootTime;
        speed = player.stats.moveSpeed;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -60, 0);
        playerPrefix = "";
        // Decides which player to take input from if the correct input is given.
        switch (playerNum) {
            case 0:
                Debug.Log("Either the player number (playerNum) was not assigned or you put in 0. Both are in invalid");
                break;
            case 1:
                playerPrefix = Player_1_Str;
                break;
            case 2:
                playerPrefix = Player_2_Str;
                break;
            case 3:
                playerPrefix = Player_3_Str;
                break;
            case 4:
                playerPrefix = Player_4_Str;
                break;
            default:
                Debug.Log("This game does not support more than 4 players.");
                break;
        }

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // This right vector is -45 degrees from the world X axis 
    }

    void MoveWASD(string horizontal, string vertical) {
        Vector3 rightMovement = right * Input.GetAxis(horizontal);
        Vector3 upMovement = forward * Input.GetAxis(vertical);
        Vector3 direction = rightMovement + upMovement;
        transform.position += direction * player.stats.moveSpeed * Time.fixedDeltaTime;
    }

    void Move()
    {
        if (newMovement) //this is just for testing purposes. Set this to false in the inspector to get back to the previous movement system (and set drag to 0 like before)
        {
            if (LeftStickY() != 0.0 || LeftStickX() != 0.0)
            {
                Vector3 rightMovement = right * LeftStickX();
                Vector3 upMovement = forward * LeftStickY();
                _inputs = (rightMovement + upMovement);
            }
            else if (DpadX() != 0.0 || DpadY() != 0.0)
            {
                Vector3 rightMovement = right * DpadX();
                Vector3 upMovement = forward * DpadY();
                _inputs = (rightMovement + upMovement);
            }
        }
        else
        {
            if (LeftStickX() != 0.0 || LeftStickY() != 0.0) //Left Joystick
            {
                Vector3 rightMovement = right * LeftStickX();
                Vector3 upMovement = forward * LeftStickY();
                Vector3 direction = rightMovement + upMovement;
                transform.position += direction * player.stats.moveSpeed * Time.deltaTime;
            }
            else if (DpadX() != 0.0 | DpadY() != 0.0) //D-Pad
            {
                Vector3 rightMovement = right * DpadX();
                Vector3 upMovement = forward * DpadY();
                Vector3 direction = rightMovement + upMovement;
                transform.position += direction * player.stats.moveSpeed * Time.deltaTime;
            }
        }
    }

    void Jump() {
        if (Input.GetButtonDown(playerPrefix + rightBumper) && isGrounded) //Checking for jumping
        { 
            speed = player.stats.airSpeed;
            rb.AddForce(Vector3.up * player.stats.jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }


    void Attack() {
        if (RightTrigger() != 0.0)
        {
            if(holdStart == 0.0)
            {
                holdStart = Time.time;
            }
            StartCoroutine("IsTriggerBeingHeldDown");
        }
        
    }

    IEnumerator IsTriggerBeingHeldDown() //making Input.GetButtonDown/Up functionality for the Axis
    {
        if(holdTime <= Time.time)
        {
            holdTime = Time.time + 0.01f;
            yield return new WaitForSeconds(0.01f);
            if(RightTrigger() == 0.0)
            {
                holdEnd = Time.time - holdStart;
                if(holdEnd >= chargeTime) 
                {
                    StartCoroutine("Shoot");
                }
              
                holdStart = 0.0f;
            }
        }


    }

    //test to see if you can replace shootTime with player.stats.shootTime 
    IEnumerator Shoot() {
        if (shootTime <= Time.time) {
            Instantiate(arrow, ShootPoint.transform.position, transform.rotation, null); //this instantiates the arrow as an attack
            shootTime = Time.time + shootRate;
            yield return new WaitForSeconds(shootRate);
        }
        //yield return new WaitForSeconds(shootRate);

    }

    //change the way isGrounded is implemented 
    private void OnCollisionEnter(Collision collision) {
        isGrounded = true;
        speed = player.stats.moveSpeed;
    }

    void RotatePlayer() {
        if(RightStickX() != 0.0 || RightStickY() != 0.0)
        {
            Vector3 rightMovement = right * Time.deltaTime * RightStickX();
            Vector3 upMovement = forward * Time.deltaTime * RightStickY();
            transform.forward = Vector3.Normalize(rightMovement - upMovement);
        }
    }


    private void FixedUpdate() //Physics things are supposed to be in FixedUpdate
    {
       
        rb.AddForce((_inputs * speed * 900 * Time.fixedDeltaTime)); //using the Physics System to move 
    }



    // Update is called once per frame
    void Update() {
        //Checking for Movement
        // only for testing a single player with keyboard if you dont have an Xbox controller!
        // Otherwise, comment out the first if-else block.
        if (Input.GetAxis("Horizontal") != 0.0 | Input.GetAxis("Vertical") != 0.0) //WASD only for the first player
            if (playerNum == 1)
                MoveWASD("Horizontal", "Vertical");

        _inputs = Vector3.zero;
        Move();

        //if (RightStickX() != 0.0 || RightStickY() != 0.0) //rotation of player with right stick
        //{
            RotatePlayer();
        //}
        //if (Input.GetButtonDown(playerPrefix + rightBumper) && isGrounded) { //Checking for jumping
            Jump();
        //}
        //if (Input.GetAxis(playerPrefix + rightTrigger) != 0.0) { //Checking for attacking
            Attack();
        //}
    }
}
