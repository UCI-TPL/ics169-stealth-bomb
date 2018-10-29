using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {
    
    private Player _player;
    private InputManager input;
    public Player player {
        get { return _player; }
        private set { _player = value; }
    }

    private void Awake() {
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
        
        input = GameObject.FindGameObjectWithTag("game-manager").GetComponentInChildren<InputManager>();
        if (input == null) 
            Debug.LogError("Input Manager does not exist");
    }
    [SerializeField]
    //float shootRate = 0.2f; //put these stats in playerStats soon 
    //float chargeTime = 0.5f;
    //float shootTime = 0.0f;
    //float holdTime = 0.0f;
    //float holdStart = 0.0f;
    private bool attackDown; // This should be handled in InputManager
    float rollTime = 0.0f; //to check if the player is currently rolling
    float holdEnd = 0.0f;
    float speed;

    bool isGrounded;
    bool movementAllowed = true; //used to lock movement while dodging
    bool rotationInput = false;
    bool updatingPhysics = false;
    bool moveCalled = false;
    bool dodging = false; 
    
    private Vector3 _inputs = Vector3.zero;
    private Vector2 inputMoveVector = Vector2.zero;
    private Vector2 inputRotationVector = Vector2.zero;
    Vector3 forward, right;
    Vector3 rotationDirection; //used for Dodging

    public GameObject ShootPoint;
    public Rigidbody rb;
    //public Projectile arrow; //this is used for the Basic Attack
    [Tooltip("Represents which player this is. Only put in 1-4. Do not put 0!!! This attribute must have a value in order to work or take in input properly!!! ")]
    public int playerNum;

    // private variables used to store xbox controller input info
    private PlayerIndex playerIdx;
    private GamePadState currentState;
    private GamePadState prevState;

    private string playerPrefix;


    //Renderer rend;
    //Color startColor;
    //[SerializeField]
    //Color changeColor;

    //public float colorAddition = 0.1f;

    // public float DpadX()
    // {
    //     return Input.GetAxis(playerPrefix + "DpadX");
    // }

    // public float DpadY()
    // {
    //     return Input.GetAxis(playerPrefix + "DpadY");
    // }

    // public float LeftStickX()
    // {
    //     return Input.GetAxis(playerPrefix + left_Joystick_X_Axis);
    // }

    // public float LeftStickY()
    // {
    //     return Input.GetAxis(playerPrefix + left_Joystick_Y_Axis);
    // }

    // public float RightStickX()
    // {
    //     return Input.GetAxis(playerPrefix + right_Joystick_X_Axis);
    // }

    // public float RightStickY()
    // {
    //     return Input.GetAxis(playerPrefix + right_Joystick_Y_Axis);
    // }

    // public float RightTrigger()
    // {
    //     return Input.GetAxis(playerPrefix + rightTrigger);
    // }


    void Start() {
        input.connectedControllers[playerNum - 1].RB_Pressed.AddListener(Jump);
        input.connectedControllers[playerNum - 1].B_Pressed.AddListener(Dodge);
        input.connectedControllers[playerNum - 1].RT_Pressed.AddListener(InitiateAttack);
        input.connectedControllers[playerNum - 1].RT_Released.AddListener(FinishAttack);
        // input.connectedControllers[playerNum - 1].LStick.AddListener(GetMoveData);
        input.connectedControllers[playerNum - 1].RStick.AddListener(RotatePlayer);
        //shootRate = player.stats.shootTime;
        //rend = GetComponent<Renderer>();
        //startColor = rend.material.color;
        speed = player.stats.moveSpeed;
        rb = GetComponent<Rigidbody>();
       
        // playerPrefix = "";
        // Decides which player to take input from if the correct input is given.
        // switch (playerNum) {
        //     case 0:
        //         Debug.Log("Either the player number (playerNum) was not assigned or you put in 0. Both are in invalid");
        //         break;
        //     case 1:
        //         playerPrefix = Player_1_Str;
        //         break;
        //     case 2:
        //         playerPrefix = Player_2_Str;
        //         break;
        //     case 3:
        //         playerPrefix = Player_3_Str;
        //         break;
        //     case 4:
        //         playerPrefix = Player_4_Str;
        //         break;
        //     default:
        //         Debug.Log("This game does not support more than 4 players.");
        //         break;
        // }

        playerIdx = (PlayerIndex) (playerNum - 1);
        //test lines
        //Debug.Log((int) ButtonState.Pressed);
        //Debug.Log((int) ButtonState.Released);

        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // This right vector is -45 degrees from the world X axis 
    }

    // void MoveWASD(string horizontal, string vertical) {
    //     Vector3 rightMovement = right * Input.GetAxis(horizontal);
    //     Vector3 upMovement = forward * Input.GetAxis(vertical);
    //     Vector3 direction = rightMovement + upMovement;
    //     transform.position += direction * player.stats.moveSpeed * Time.fixedDeltaTime;
    // }

    void GetMoveData() {
        inputMoveVector = input.getLeftStickData(playerNum - 1);
        moveCalled = true;
    }

    void Move()
    {
        GetMoveData();
        Vector3 rightMovement = right * inputMoveVector.x;
        Vector3 upMovement = forward * inputMoveVector.y;
        _inputs = (rightMovement + upMovement);

        if (_inputs.magnitude > 1f) {
            _inputs = _inputs.normalized;
        }

        if (!rotationInput) {
            rightMovement = right * Time.deltaTime * inputMoveVector.x;
            upMovement = forward * Time.deltaTime * inputMoveVector.y;

            if(Vector3.Normalize(rightMovement + upMovement) != Vector3.zero)
            {
                transform.forward = Vector3.Normalize(rightMovement + upMovement);
            }
        }

        moveCalled = false;

        // if (currentState.ThumbSticks.Left.Y != 0.0f || currentState.ThumbSticks.Left.X != 0.0f) 
        // {
        //     Vector3 rightMovement = right * currentState.ThumbSticks.Left.X;
        //     Vector3 upMovement = forward * currentState.ThumbSticks.Left.Y;
        //     _inputs = (rightMovement + upMovement);

        //     if (_inputs.magnitude > 1f) {
        //         _inputs = _inputs.normalized;
        //     }
        // }
        // else if (currentState.DPad.Left == ButtonState.Pressed || currentState.DPad.Right == ButtonState.Pressed || 
        //     currentState.DPad.Up == ButtonState.Pressed || currentState.DPad.Down == ButtonState.Pressed)
        // {
        //     Vector3 rightMovement = (right * (float) currentState.DPad.Left) + -(right * (float) currentState.DPad.Right);
        //     Vector3 upMovement = -(forward * (float) currentState.DPad.Up) + (forward * (float) currentState.DPad.Down);
        //     _inputs = (rightMovement + upMovement);

        //     if (_inputs.magnitude > 1f){
        //         _inputs = _inputs.normalized;
        //     }
        // }
    }

    void Jump() {
        if (movementAllowed && isGrounded) //Checking for jumping
        // if (currentState.Buttons.RightShoulder == ButtonState.Pressed && isGrounded) //Checking for jumping
        { 
            speed = player.stats.airSpeed;
            rb.AddForce(Vector3.up * player.stats.jumpForce, ForceMode.Impulse);
            //rb.AddForce(Vector3.up  * player.stats.jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
        //Debug.Log("up!");
        /*if (Input.GetButtonDown(playerPrefix + rightBumper) && isGrounded) //Checking for jumping
        { 
            speed = player.stats.airSpeed;
            rb.AddForce(Vector3.up * player.stats.jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }*/
    }

    void Dodge()
    {
        // if (currentState.Buttons.B == ButtonState.Pressed /*&& prevState.Buttons.B == ButtonState.Released*/)
        // //if (Input.GetButtonDown(playerPrefix + "B"))
        // {
        //     StartCoroutine("Dodging");
        // }

        if (movementAllowed) 
            StartCoroutine("Dodging");
    }

    IEnumerator Dodging()
    {
        if(rollTime <= Time.time)
        {
            rollTime = Time.time + player.stats.dodgeTime; //Dodge Begins
            dodging = true;
            movementAllowed = false;
            speed = player.stats.moveSpeed * 1.6f;
            Debug.Log("Dodge begins"+Time.time);
            //make movement stuff happen 
            yield return new WaitForSeconds(player.stats.dodgeTime/2); //Halfway through invincibility ends
            yield return new WaitForSeconds(player.stats.dodgeTime/2); //Dodging ends
            movementAllowed = true;
            dodging = false;
            Debug.Log("Dodge ends"+Time.time);
            speed = player.stats.moveSpeed;
        }
        
    }
    
    void InitiateAttack() {
        // do i need to add a condition for !movementAllowed ?
        if (!attackDown) {
            player.weapon.Activate();
            Debug.Log(!attackDown);
            attackDown = true;
        }
        

        // if (currentState.Triggers.Right != 0.0f) {
        //     if (!attackDown) {
        //         player.weapon.Activate();
        //         Debug.Log(!attackDown);
        //         attackDown = true;
        //     }
        // } else {
        //     if (attackDown) {
        //         player.weapon.Release();
        //         attackDown = false;
        //     }
        // }

        ////if (RightTrigger() != 0.0)
        //{
        //    if(holdStart == 0.0)
        //    {
        //        holdStart = Time.time;
        //    }
        //    StartCoroutine("IsTriggerBeingHeldDown");
        //}
        
    }

    void FinishAttack() {
        // do i need to add a condition for !movementAllowed ?
        if (attackDown) {
            player.weapon.Release();
            attackDown = false;
        }
    }

    //IEnumerator IsTriggerBeingHeldDown() //making Input.GetButtonDown/Up functionality for the Axis
    //{
    //    if(holdTime <= Time.time)
    //    {
    //        holdTime = Time.time + 0.01f;
    //        yield return null;
    //        if (currentState.Triggers.Right == 0.0f) //stop being held down
    //        //if(RightTrigger() == 0.0) //stop being held down
    //        {
    //            holdEnd = Time.time - holdStart;
    //            rend.material.color = startColor;
    //            if (holdEnd >= chargeTime) 
    //            {
                   
    //                StartCoroutine("Shoot");
    //            }
    //            holdStart = 0.0f;
    //        }
    //        else //is being held down
    //        {
    //            rend.material.color = rend.material.color + (Color.red / colorAddition);
    //        }
    //    }


    //}

    ////test to see if you can replace shootTime with player.stats.shootTime 
    //IEnumerator Shoot() {
    //    if (shootTime <= Time.time) {
    //        Instantiate(arrow, ShootPoint.transform.position, transform.rotation, null); //this instantiates the arrow as an attack
    //        shootTime = Time.time + shootRate;
    //        yield return null;
    //    }
    //}

    //change the way isGrounded is implemented 
    private void OnCollisionEnter(Collision collision) {

        if(isGrounded == false)
        {
            isGrounded = true;
            speed = player.stats.moveSpeed;
        }

    }

    void GetRotationData() {
        inputRotationVector = input.getRightStickData(playerNum - 1);
    }

    void RotatePlayer() {
        rotationInput = true;
        Vector3 rightMovement;
        Vector3 upMovement;
        Vector2 RStickInput = input.getRightStickData(playerNum - 1);
        rightMovement = right * Time.deltaTime * RStickInput.x;
        upMovement = -(forward * Time.deltaTime * -RStickInput.y);
        if(Vector3.Normalize(rightMovement + upMovement) != Vector3.zero)
        {
            transform.forward = Vector3.Normalize(rightMovement + upMovement);
        }
        
        // if (currentState.ThumbSticks.Right.X != 0.0f || currentState.ThumbSticks.Right.Y != 0.0f)
        // //if(RightStickX() != 0.0 || RightStickY() != 0.0)
        // {
        //     rightMovement = right * Time.deltaTime * currentState.ThumbSticks.Right.X;
        //     upMovement = -(forward * Time.deltaTime * -currentState.ThumbSticks.Right.Y);
        //     // rightMovement = right * Time.deltaTime * RightStickX();
        //     // upMovement = -(forward * Time.deltaTime * RightStickY());
        // }
        // else
        // {
        //     rightMovement = right * Time.deltaTime * currentState.ThumbSticks.Left.X;
        //     upMovement = forward * Time.deltaTime * currentState.ThumbSticks.Left.Y;
        //     // rightMovement = right * Time.deltaTime * LeftStickX();
        //     // upMovement = forward * Time.deltaTime * LeftStickY();
        // }
        // if(Vector3.Normalize(rightMovement + upMovement) != Vector3.zero)
        // {
        //     transform.forward = Vector3.Normalize(rightMovement + upMovement);
        // }
        
    }


    private void FixedUpdate() //Physics things are supposed to be in FixedUpdate
    {
        updatingPhysics = true;
        _inputs = _inputs * speed;
        rb.velocity = new Vector3(_inputs.x, rb.velocity.y, _inputs.z);
        rotationInput = false;
        updatingPhysics = false;
        //rb.velocity = _inputs * speed; //player has a mass of 1 
        //rb.AddForce((_inputs * speed * 900 * Time.fixedDeltaTime)); //The player moves forward forever just choose the Inputs (not sure if this is best)      
    }



    // Update is called once per frame
    void Update() {
        //Checking for Movement
        // only for testing a single player with keyboard if you dont have an Xbox controller!
        // Otherwise, comment out the first if-else block.
        // if (Input.GetAxis("Horizontal") != 0.0 | Input.GetAxis("Vertical") != 0.0) //WASD only for the first player
        //     if (playerNum == 1)
        //         MoveWASD("Horizontal", "Vertical");

        prevState = currentState;
        currentState = GamePad.GetState(playerIdx);

        if (currentState.IsConnected && prevState.IsConnected) 
        {
            if(movementAllowed)
            {
                
                _inputs = Vector3.zero;
                Move();
                // RotatePlayer();
                // Jump();
                // Attack();
                // Dodge();
            }
            else //this could be used to stop movement during dodges and stuns 
            {
                if (dodging)
                {
                    _inputs = transform.forward;
                }
                else
                {
                    _inputs = Vector3.zero;
                }
                //return;
            }
        }

    }
}
