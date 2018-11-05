using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
    
    public Player player { get; private set; }

    [HideInInspector]
    public InputManager input;

    private Rigidbody rb;
    public GameObject ShootPoint;
    public Collider floorCollider;
    public Collider wallCollider;
    public float friction = 10f;
    
    public float jumpGravityMultiplier = 0.5f;

    // Used to scale movement to the camera's direction
    private Vector3 forward;
    private Vector3 right;

    // For disabling movement while performing a dodge or potentially while stunned
    private bool allowMovement = true;

    // Required variables for jumping and detecting ground collisions
    private float jumpCooldown = 0;
    private bool jumped = false;
    private bool jumpedReleased = true;
    public static readonly int groundLayer = 11;
    private static readonly int groundLayerMask = 1 << groundLayer;
    private static readonly float maxGroundDistance = 0.5f;
    private bool touchedGround;
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

    // Initialize referances
    private void Awake() {
        player = GetComponent<Player>();
        if (player == null) // Check to ensure Player component is present, since PlayerStats is a dependency of Player this will never happen, but just in case
            Debug.LogError(gameObject.name + " missing Player Component");
        input = InputManager.inputManager;
        if (input == null)
            Debug.LogError("Input Manager does not exist");
        rb = GetComponent<Rigidbody>();
    }

    // Set up controllers
    private void Start() {
        forward = Camera.main.transform.forward;
        forward.Scale(new Vector3(1, 0, 1));
        forward.Normalize();
        right = Camera.main.transform.right;
        right.Scale(new Vector3(1, 0, 1));
        right.Normalize();

        input.controllers[player.playerNumber].attack.OnDown.AddListener(ActivateAttack);
        input.controllers[player.playerNumber].attack.OnUp.AddListener(ReleaseAttack);
        input.controllers[player.playerNumber].jump.OnUp.AddListener(ReleaseJump);
    }

    // Perform movement every physics update
    private void FixedUpdate() {
        Move(isGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
        if (input.controllers[player.playerNumber].jump.Pressed) {
            Jump();
            if (jumped && rb.velocity.y > 0) {
                rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
            }
        }
    }

    // Rotate the player's facing direction
    private void Update() {
        Vector2 horizontalVector = input.controllers[player.playerNumber].AimVector();
        Debug.DrawRay(transform.position, transform.forward*100, Color.white);
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        if (scaledVector != Vector3.zero)
            transform.forward = scaledVector;
    }

    // Move the player using the the controller's move input scaled by the provided speed
    private void Move(float speed) {
        Vector2 horizontalVector = input.controllers[player.playerNumber].MoveVector() * speed;
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
            Vector3 newVelocity = rb.velocity + scaledVector * Time.fixedDeltaTime * 5; // Clamp velocity to either max speed or current speed(if player was launched)
            newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0, newVelocity.z), Mathf.Max(oldVelocity.magnitude, speed * input.controllers[player.playerNumber].MoveVector().magnitude + 0.1f) - 0.1f);
            rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
        }
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
        player.weapon.Activate();
    }

    // On Attack up
    private void ReleaseAttack() {
        player.weapon.Release();
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
