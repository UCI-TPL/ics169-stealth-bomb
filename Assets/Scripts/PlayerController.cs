using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {

    private Player _player;

    [HideInInspector]
    public InputManager input;
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

    [Tooltip("The sets Dodge to the teleportation-like AddForce with ForceMode.Impuse")]
    public bool ImpulseDodge = false;

    [Tooltip("Turn on to use lerping for player rotation instead of instant snapping")]
    public bool enableLerping = false;
    public float lerpfrac = 0.5f;
    private bool attackDown; // This should be handled in InputManager
    float rollTime = 0.0f; //to check if the player is currently rolling
    //float holdEnd = 0.0f;
    float speed;

    //bool isGrounded = true;
    bool movementAllowed = true; //used to lock movement while dodging
    bool rotationInput = false;
    bool updatingPhysics = false;
    bool moveCalled = false;
    bool dodging = false;

    private Vector3 _inputs = Vector3.zero;
    private Vector3 mostRecentLeftStickInput = Vector3.forward; //the most recent left stick input, used for dodging
    private Vector2 inputMoveVector = Vector2.zero;
    private Vector2 inputRotationVector = Vector2.zero;
    Vector3 forward, right;
    Vector3 rotationDirection; //used for Dodging

    public GameObject ShootPoint;
    public Transform GroundCheck; //this is used for linecasting for Jump
    public Rigidbody rb;
    [Tooltip("Represents which player this is. Only put in 1-4. Do not put 0!!! This attribute must have a value in order to work or take in input properly!!! ")]
    public int playerNum;

    // private variables used to store xbox controller input info
    private PlayerIndex playerIdx;
    private GamePadState currentState;
    private GamePadState prevState;

    private string playerPrefix;


    //void Start() {
    //    input.connectedControllers[playerNum - 1].LT_Pressed.AddListener(Jump);
    //    input.connectedControllers[playerNum - 1].A_Pressed.AddListener(Jump); 
    //    input.connectedControllers[playerNum - 1].B_Pressed.AddListener(Dodge);
    //    input.connectedControllers[playerNum - 1].LB_Pressed.AddListener(Dodge);
    //    input.connectedControllers[playerNum - 1].RT_Pressed.AddListener(InitiateAttack);
    //    input.connectedControllers[playerNum - 1].RT_Released.AddListener(FinishAttack);
    //    // input.connectedControllers[playerNum - 1].LStick.AddListener(GetMoveData);
    //    input.connectedControllers[playerNum - 1].RStick.AddListener(RotatePlayer);

    //    speed = player.stats.moveSpeed;
    //    rb = GetComponent<Rigidbody>();
    //    playerIdx = (PlayerIndex) (playerNum - 1);
    //    forward = Camera.main.transform.forward;
    //    forward.y = 0;
    //    forward = Vector3.Normalize(forward);
    //    right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // This right vector is -45 degrees from the world X axis 
    //}

    //// void MoveWASD(string horizontal, string vertical) {
    ////     Vector3 rightMovement = right * Input.GetAxis(horizontal);
    ////     Vector3 upMovement = forward * Input.GetAxis(vertical);
    ////     Vector3 direction = rightMovement + upMovement;
    ////     transform.position += direction * player.stats.moveSpeed * Time.fixedDeltaTime;
    //// }

    //void GetMoveData() {
    //    inputMoveVector = input.getLeftStickData(playerNum - 1);
    //    moveCalled = true;
    //}

    //void Move()
    //{
    //    GetMoveData();
    //    Vector3 rightMovement = right * inputMoveVector.x;
    //    Vector3 upMovement = forward * inputMoveVector.y;
    //    _inputs = (rightMovement + upMovement);

    //    if (_inputs.magnitude > 1f) {
    //        _inputs = _inputs.normalized;
    //    }
    //    if(_inputs != Vector3.zero)
    //    {
    //        mostRecentLeftStickInput = _inputs.normalized;
    //    }

    //    // this now handles default player rotation when there is no player input for rotation
    //    if (!rotationInput) {
    //        rightMovement = right * Time.deltaTime * inputMoveVector.x;
    //        upMovement = forward * Time.deltaTime * inputMoveVector.y;

    //        computeRotation(rightMovement, upMovement);
    //    }

    //    moveCalled = false;
    //}

    //bool IsGrounded() //uses raycasting to decide whether the layer can jump or not
    //{
    //    return Physics.Raycast(GroundCheck.position, GroundCheck.TransformDirection(Vector3.down),0.05f); //GroundCheck is set to the bottom of the player
    //    //return Physics.Linecast(transform.position, GroundCheck.position); //Linecast was used first but it got harder once there were two colliders (it was detecting the sphere)
    //}

    //void Jump() {
    //    if (movementAllowed && IsGrounded()) //Checking if on the ground
    //    { 
    //        speed = player.stats.airSpeed;
    //        rb.AddForce(Vector3.up * player.stats.jumpForce, ForceMode.Impulse);
    //        //isGrounded = false;
    //    }
    //}

    //void Dodge()
    //{
    //    if (movementAllowed) 
    //        StartCoroutine("Dodging");
    //}

    //IEnumerator Dodging()
    //{
    //    if(rollTime <= Time.time)
    //    {
    //        rollTime = Time.time + player.stats.dodgeTime + player.stats.dodgeRecharge; //Player has to wait for the dodge to recharge to avoid spamming
    //        if(!ImpulseDodge) //move at a faster speed for a short while
    //        {
    //            dodging = true;
    //            movementAllowed = false;
    //            rotationInput = false;
    //            speed = player.stats.moveSpeed * 2f;
    //            yield return new WaitForSeconds(player.stats.dodgeTime / 2); //Just incase we want to make this a roll like Enter the Gungeon where the player is inviceable for half the roll
    //            yield return new WaitForSeconds(player.stats.dodgeTime / 2); 
    //            movementAllowed = true;
    //            dodging = false;
    //            rotationInput = true;
    //            speed = player.stats.moveSpeed;
    //        }
    //        else //Add a force and zoom forward 
    //        {
    //            rb.AddForce( mostRecentLeftStickInput * 100, ForceMode.Impulse);
    //        }
    //    }

    //}

    //void InitiateAttack() {
    //    // do i need to add a condition for !movementAllowed ?
    //    if (!attackDown) {
    //        player.weapon.Activate();
    //        //Debug.Log(!attackDown);
    //        attackDown = true;
    //    }
    //}

    //void FinishAttack() {
    //    // do i need to add a condition for !movementAllowed ?
    //    if (attackDown) {
    //        player.weapon.Release();
    //        attackDown = false;
    //    }
    //}
    //private void OnCollisionEnter(Collision col) {
    //    /*
    //    foreach (ContactPoint contact in col.contacts)
    //    {
    //        Debug.Log(contact.thisCollider.name);
    //        Debug.Log(contact.otherCollider.name);
    //    }
    //    */
    //}

    //void GetRotationData() {
    //    inputRotationVector = input.getRightStickData(playerNum - 1);
    //}

    ///* Helper function that does the end computations for player rotation. Parameters are the vectors for the new rotation. */
    //void computeRotation(Vector3 rightMovement, Vector3 upMovement) {
    //    if(Vector3.Normalize(rightMovement + upMovement) != Vector3.zero)
    //        {
    //            Vector3 newRotation = Vector3.Normalize(rightMovement + upMovement);
    //            if (!enableLerping)
    //                transform.forward = newRotation;
    //            else 
    //                transform.forward = Vector3.Lerp(transform.forward, newRotation, lerpfrac);
    //        }
    //}

    //void RotatePlayer() {
    //    if (movementAllowed) {
    //        rotationInput = true;
    //        Vector3 rightMovement;
    //        Vector3 upMovement;
    //        Vector2 RStickInput = input.getRightStickData(playerNum - 1);
    //        rightMovement = right * Time.deltaTime * RStickInput.x;
    //        upMovement = -(forward * Time.deltaTime * -RStickInput.y);

    //        computeRotation(rightMovement, upMovement);
    //    }
    //}


    //private void FixedUpdate() //Physics things are supposed to be in FixedUpdate
    //{
    //    updatingPhysics = true;
    //    _inputs = _inputs * speed;
    //    rb.velocity = new Vector3(_inputs.x, rb.velocity.y, _inputs.z);
    //    rotationInput = false;
    //    updatingPhysics = false;      
    //}

    //void Update() {
    //    //Checking for Movement
    //    // only for testing a single player with keyboard if you dont have an Xbox controller!
    //    // Otherwise, comment out the first if-else block.
    //    // if (Input.GetAxis("Horizontal") != 0.0 | Input.GetAxis("Vertical") != 0.0) //WASD only for the first player
    //    //     if (playerNum == 1)
    //    //         MoveWASD("Horizontal", "Vertical");

    //    prevState = currentState;
    //    currentState = GamePad.GetState(playerIdx);

    //    if (currentState.IsConnected && prevState.IsConnected) 
    //    {
    //        if(movementAllowed)
    //        {
    //            _inputs = Vector3.zero;
    //            Move();
    //        }
    //        else //this could be used to stop movement during dodges and stuns 
    //        {
    //            if (dodging)
    //            {
    //                _inputs = transform.forward;
    //            }
    //            else //if movement is not allowed and the player is not dodging, don't move
    //            {
    //                _inputs = Vector3.zero;
    //            }
    //        }
    //    }

    //}
}
