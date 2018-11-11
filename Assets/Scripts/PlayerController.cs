using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public SpecialMove specialMove; // Dodge, Ice Wall, etc. 

    private Player _player;
    public Player player {
        get { return _player; }
        set {
            RemoveListeners();
            _player = value;
            AddListeners();
        }
    }

    [HideInInspector]
    public InputManager input;

    private Rigidbody rb;
    public Renderer rend;
    public GameObject ShootPoint;
    public Collider floorCollider;
    public Collider wallCollider;
    public GameObject crown;

    public float friction = 1f;
    [Tooltip("time it takes to go from 0 to max speed is 1 second / acceleration")]
    public float acceleration = 10f;

    public Color[] Colors = new Color[4]; //The player's color is based on thier player number
    public Color playerColor { get { return Colors[player.playerNumber]; } set { Colors[player.playerNumber] = value; } }//used for the bullettrail

    public float jumpGravityMultiplier = 0.5f;

    // Used to scale movement to the camera's direction
    private Vector3 forward;
    private Vector3 right;

    private Vector2 lastForwardMovement = Vector2.zero; //used to dodge forward when not moving 

    // For disabling movement while performing a dodge or potentially while stunned
    private bool allowMovement = true;
    private bool allowAttack = true; //used to disable movement during the countdown
    public bool dodging;
    public float dodgeSpeed  = 0f;

    // Required variables for jumping and detecting ground collisions
    private float jumpCooldown = 0;
    private bool jumped = false;
    private bool jumpedReleased = true;
    public static readonly int groundLayer = 11;
    private static readonly int groundLayerMask = 1 << groundLayer;
    private static readonly float maxGroundDistance = 0.5f;
    private bool touchedGround;

    // Required Variables for the Player's UI stuff.
    public RectTransform playerUI_HPCanvas;
    public RectTransform playerUI_HPMaskCanvas;
    public UnityEngine.UI.Image playerUI_healthBar;

    // flags for rendering player UI
    private bool HP_CoroutineActive = false;

    public bool isGrounded {
        get {
            if (touchedGround) {
                if (CheckGroundDistance() < maxGroundDistance) {
                    return true;
                }
                touchedGround = false;
            }
            return false;
        }
    }

    public bool isMoving {
        get { return rb.velocity.magnitude > 0.1f; }
    }

    // Initialize referances
    private void Awake() {
        input = InputManager.inputManager;
        if (input == null)
            Debug.LogError("Input Manager does not exist");
        rb = GetComponent<Rigidbody>();
        rend = rend == null ? GetComponent<Renderer>() : rend;

        // To have HP bar render all the time remove this code, as well as code in the HurtPlayer method in Player.cs
        playerUI_HPCanvas.gameObject.SetActive(false);
        playerUI_HPMaskCanvas.gameObject.SetActive(false);   
    }

    // Set up controllers
    private void Start() {
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        //lastForwardMovement = forward * dodgeSpeed;
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();
        rend.material.color = playerColor; //setting the player color based on playeNum 

        input.controllers[player.playerNumber].attack.OnDown.AddListener(ActivateAttack);
        input.controllers[player.playerNumber].attack.OnUp.AddListener(ReleaseAttack);
        input.controllers[player.playerNumber].jump.OnUp.AddListener(ReleaseJump);
        input.controllers[player.playerNumber].dodge.OnDown.AddListener(SpecialMove);
    }

    private void AddListeners() {
        player.onHeal.AddListener(Heal);
        player.onHurt += Hurt;
    }

    private void RemoveListeners() {
        if (player != null) {
            player.onHeal.RemoveListener(Heal);
            player.onHurt -= Hurt;
        }
    }

    private void OnDestroy() {
        RemoveListeners();
    }

    // Perform movement every physics update
    private void FixedUpdate() {
        if(!dodging)
            Move(isGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
        else
            Move(dodgeSpeed); //hopefully this allows air dodges
        if (input.controllers[player.playerNumber].jump.Pressed) {
            Jump();
            if (jumped && rb.velocity.y > 0) {
                rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
            }
        }
    }

    // Rotate the player's facing direction
    private void Update() {
        player.InGameUpdate();
        transform.localScale = player == GameManager.instance.leader ? Vector3.one * 1.35f : Vector3.one;
        crown.SetActive(player == GameManager.instance.leader);

        Vector2 horizontalVector = input.controllers[player.playerNumber].AimVector();
        Debug.DrawRay(transform.position, transform.forward*100, Color.white);
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        if (scaledVector != Vector3.zero)
            transform.forward = scaledVector;

        // MOVE THIS CRAP OUT INTO A SEPARATE SCRIPT EVENTUALLY.
        if (player.health < player.stats.maxHealth)
        {
            playerUI_HPCanvas.gameObject.SetActive(true);
            playerUI_HPMaskCanvas.gameObject.SetActive(true);
        }
        else
        {
            playerUI_HPCanvas.gameObject.SetActive(false);
            playerUI_HPMaskCanvas.gameObject.SetActive(false);
        }
        playerUI_HPCanvas.rotation = Quaternion.Euler(90f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        playerUI_HPMaskCanvas.rotation = Quaternion.Euler(90f, Camera.main.transform.rotation.eulerAngles.y, 0f);
        playerUI_healthBar.fillAmount = player.health / player.stats.maxHealth;
    }

    // Move the player using the the controller's move input scaled by the provided speed
    private void Move(float speed) {
        Vector2 horizontalVector = input.controllers[player.playerNumber].MoveVector() * speed;
        if (!dodging && horizontalVector != Vector2.zero)
            lastForwardMovement = horizontalVector.normalized;          
        if(dodging)
            horizontalVector = lastForwardMovement * speed;
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
    
        if (isGrounded) { // If grounded apply friction
            Vector3 frictionVector = -friction * rb.velocity; // Friction is a negative percentage of current velocity
            Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, frictionVector, Color.blue);
            Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, scaledVector, Color.green);
            frictionVector -= Vector3.Project(frictionVector, scaledVector); // Scale friction to remove the forward direction, so friction doesnt slow player in moving direction
            Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, frictionVector, Color.red);
            rb.velocity += frictionVector * Time.fixedDeltaTime; // Add friction to velocity
        }
        if (allowMovement) {
            Vector3 oldVelocity = rb.velocity.Scaled(new Vector3(1, 0, 1));
            Vector3 newVelocity = rb.velocity + scaledVector * Time.fixedDeltaTime * acceleration; // Clamp velocity to either max speed or current speed(if player was launched)
            if(!dodging) //don't clamp while dodging
                newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0, newVelocity.z), Mathf.Max(oldVelocity.magnitude, speed * input.controllers[player.playerNumber].MoveVector().magnitude + 0.1f) - 0.1f);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
        
    }

    public void AttachComponent(SpecialMoveData moveData)
    {
        //specialMove = new DodgeDash(); //coroutines gave errors when I used 'new' so now it is a component

        if (moveData.type == SpecialMoveData.MoveType.DodgeDash) //make one for every type of SpecialMove
            specialMove = gameObject.AddComponent<DodgeDash>();
        specialMove.SetData(this, moveData);
    }

    private void SpecialMove() {
        specialMove.Activate();
    }

    // Attempt to perform a jump
    private void Jump() {
        if (jumpedReleased && allowMovement && isGrounded) { //Checking if on the ground and movement is allowed
            jumpedReleased = false;
            jumped = true;
            rb.velocity = rb.velocity += (2 * -Physics.gravity * player.stats.jumpForce * jumpGravityMultiplier).Sqrt();
            touchedGround = false;
            jumpCooldown = 0.1f + Time.time;
        }
    }

    private void ReleaseJump() {
            jumpedReleased = true;
        if (jumped && rb.velocity.y > 0) {
            rb.velocity -= Vector3.Project(rb.velocity, Physics.gravity);
            jumped = false;
        }
    }

    // On Attack down
    private void ActivateAttack() {
        if(allowAttack)
            player.weapon.Activate();   
    }

    // On Attack up
    private void ReleaseAttack() {
        if(allowAttack)
            player.weapon.Release();
    }

    public void Knockback(Vector3 direction) {
        rb.AddForce(direction, ForceMode.VelocityChange); //move back in the direction of the projectile 
    }

    private void Hurt(Player damageDealer, Player reciever, float percentDealt) {
        input.controllers[player.playerNumber].Vibrate(1.0f, 0.1f);
        StartCoroutine("HurtIndicator");

        // Remove this code to render HP bars all the time, as well as code in the Awake method in PlayerController.cs

    }

    private void Heal() {
        // render HP on heal

    }

    // Renders the players HP bar for a second.

    IEnumerator HurtIndicator() //show the player that it is hurt 
    {
        Color test = playerColor;
        rend.material.color = Color.white;
        yield return new WaitForSeconds(0.025f); //the player flashes white 
        rend.material.color = playerColor;
    }

    // Restrict the player's movement for a duration
    public void DisableMovement(float duration) {
        StartCoroutine(DisableMovementTimer(duration));
    }

    private IEnumerator DisableMovementTimer(float duration) {
        allowMovement = false;
        yield return new WaitForSeconds(duration);
        allowMovement = true;
    }

    public void DisableAttack(float duraion){
        StartCoroutine(DisableAttackTimer(duraion));
    }

    private IEnumerator DisableAttackTimer(float duration) {
        allowAttack = false;
        yield return new WaitForSeconds(duration);
        allowAttack = true;
    }

    private float CheckGroundDistance() {
        RaycastHit hit; // Create a SphereCast below the player and check the distance to the ground, if none return infinity;
        return Physics.SphereCast(transform.position, floorCollider.bounds.extents.y, Vector3.down, out hit, 5f, groundLayerMask, QueryTriggerInteraction.Ignore) ? hit.distance : float.PositiveInfinity;
    }

    // Check the the player has collided with the ground
    private void OnCollisionStay(Collision collision) {
        if (jumpCooldown < Time.time && collision.gameObject.layer == groundLayer && collision.contacts[0].thisCollider == floorCollider) {
            touchedGround = true;
            jumped = false;
        }
    }

#if UNITY_EDITOR //Editor only tag
    // Draw the groundcheck spherecast under the player
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + Vector3.down * CheckGroundDistance(), floorCollider.bounds.extents.y);
    }
#endif
}
