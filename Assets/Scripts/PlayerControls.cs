using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vector3Extensions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour {
    
    public Player player { get; private set; }

    [HideInInspector]
    public InputManager input;

    private Rigidbody rb;
    public GameObject ShootPoint;
    public Collider floorCollider;
    public Collider wallCollider;
    
    public float jumpGravityMultiplier = 0.5f;

    // Used to scale movement to the camera's direction
    private Vector3 forward;
    private Vector3 right;
    private Vector2 cameraScale;

    // For disabling movement while performing a dodge or potentially while stunned
    private bool allowMovement = true;

    // Required variables for jumping and detecting ground collisions
    private float jumpCooldown = 0;
    private bool jumped = false;
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
        cameraScale = new Vector2(Mathf.Sin(Mathf.Deg2Rad * Camera.main.transform.eulerAngles.x), 1);

        input.controllers[player.playerNumber].attack.OnDown.AddListener(ActivateAttack);
        input.controllers[player.playerNumber].attack.OnUp.AddListener(ReleaseAttack);
        input.controllers[player.playerNumber].jump.OnDown.AddListener(Jump);
    }

    // Perform movement every physics update
    private void FixedUpdate() {
        Move(isGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
        if (jumped && input.controllers[player.playerNumber].jump.Pressed && rb.velocity.y > 0) {
            rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
        }
    }

    // Rotate the player's facing direction
    private void Update() {
        Vector2 horizontalVector = (input.controllers[player.playerNumber].AimVector() * cameraScale).normalized;
        Debug.DrawRay(transform.position, transform.forward*100, Color.white);
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        if (scaledVector != Vector3.zero)
            transform.forward = scaledVector;
    }

    // Move the player using the the controller's move input scaled by the provided speed
    private void Move(float speed) {
        Vector2 horizontalVector = (input.controllers[player.playerNumber].MoveVector() * cameraScale).normalized * speed * input.controllers[player.playerNumber].MoveVector().magnitude;
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        rb.velocity = new Vector3(scaledVector.x, rb.velocity.y, scaledVector.z);
    }

    // Attempt to perform a jump
    public void Jump() {
        if (allowMovement && isGrounded) { //Checking if on the ground and movement is allowed
            jumped = true;
            rb.velocity = (2 * -Physics.gravity * (player.stats.jumpForce + 0.2f) * jumpGravityMultiplier).Sqrt();
            touchedGround = false;
            jumpCooldown = 0.1f + Time.time;
        }
    }

    // On Attack down
    public void ActivateAttack() {
        player.weapon.Activate();
    }

    // On Attack up
    public void ReleaseAttack() {
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
