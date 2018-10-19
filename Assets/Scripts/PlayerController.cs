using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    
    private Player _player;
    public Player player {
        get { return _player; }
    }

    private void Awake() {
        _player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
    }

    [SerializeField]
    float shootRate = 0.2f;

    float shootTime = 0.0f;

    bool isGrounded;

    public GameObject ShootPoint;
    public Rigidbody rb;
    public Projectile arrow; //this is used for the Basic Attack

    Vector3 forward, right;

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



    void Start() {
        rb = GetComponent<Rigidbody>();
        //Physics.gravity = new Vector3(0, -15, 0);
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

    void Move(string horizontal, string vertical) {
        Vector3 rightMovement = right * player.playerStats.speed * Time.deltaTime * Input.GetAxis(horizontal);
        Vector3 upMovement = forward * player.playerStats.speed * Time.deltaTime * Input.GetAxis(vertical);
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    void Jump() {
        rb.AddForce(Vector3.up * player.playerStats.jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    void Attack() {
        StartCoroutine("Shoot");
    }

    IEnumerator Shoot() {
        if (shootTime <= Time.time) {
            Instantiate(arrow, ShootPoint.transform.position, transform.rotation, null); //this instantiates the arrow as an attack
            shootTime = Time.time + shootRate;
            yield return new WaitForSeconds(shootRate);
        }
        //yield return new WaitForSeconds(shootRate);

    }

    private void OnCollisionEnter(Collision collision) {
        isGrounded = true;
    }

    void RotatePlayer(string horizontal, string vertical) {
        Vector3 rightMovement = right * Time.deltaTime * Input.GetAxis(horizontal);
        Vector3 upMovement = forward * Time.deltaTime * Input.GetAxis(vertical);
        transform.forward = Vector3.Normalize(rightMovement - upMovement);
    }

    // Update is called once per frame
    void Update() {

        //Checking for Movement
        // only for testing a single player with keyboard if you dont have an Xbox controller!
        // Otherwise, comment out the first if-else block.
        if (Input.GetAxis("Horizontal") != 0.0 | Input.GetAxis("Vertical") != 0.0) //WASD only for the first player
            if (playerNum == 1)
                Move("Horizontal", "Vertical");
        if (Input.GetAxis(playerPrefix + left_Joystick_X_Axis) != 0.0 | Input.GetAxis(playerPrefix + left_Joystick_Y_Axis) != 0.0) //Left Joystick
            Move(playerPrefix + left_Joystick_X_Axis, playerPrefix + left_Joystick_Y_Axis);
        else if (Input.GetAxis(playerPrefix + Dpad_X_Axis) != 0.0 | Input.GetAxis(playerPrefix + Dpad_Y_Axis) != 0.0) //D-Pad
            Move(playerPrefix + Dpad_X_Axis, playerPrefix + Dpad_Y_Axis);

        if (Input.GetAxis(playerPrefix + right_Joystick_X_Axis) != 0.0 | Input.GetAxis(playerPrefix + right_Joystick_Y_Axis) != 0.0) //rotation of player with right stick
        {
            RotatePlayer(playerPrefix + right_Joystick_X_Axis, playerPrefix + right_Joystick_Y_Axis);
        }

        //Checking for jumping
        if (Input.GetButtonDown(playerPrefix + rightBumper) && isGrounded) {
            Jump();
        }

        //Checking for attacking
        if (Input.GetAxis(playerPrefix + rightTrigger) != 0.0) {
            Attack();
        }
    }
}
