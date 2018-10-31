using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControls : MonoBehaviour {

    private Player _player;
    public Player player {
        get { return _player; }
        private set { _player = value; }
    }

    [HideInInspector]
    public InputManager input;

    private Rigidbody rb;
    public GameObject ShootPoint;
    public Collider floorCollider;
    public Collider wallCollider;

    // Used to scale movement to the camera's direction
    private Vector3 forward;
    private Vector3 right;

    // For disabling movement while performing a dodge or potentially while stunned
    private bool allowMovement = true;

    // Required variables for detecting ground collisions
    private static readonly int groundLayer = 11;
    private static readonly int groundLayerMask = 1 << groundLayer;
    private bool touchedGround;
    public bool isGrounded {
        get {
            if (touchedGround) {
                Vector3 startPos = floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y;
                if (Physics.Linecast(transform.position, startPos + Vector3.down * 0.1f, groundLayerMask)) {
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
    }

    // Perform movement every physics update
    private void FixedUpdate() {
        Move(isGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
    }

    // Rotate the player's facing direction
    private void Update() {
        Vector2 horizontalVector = input.controllers[player.playerNumber].AimVector();
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        if (scaledVector != Vector3.zero)
            transform.forward = scaledVector;
        if (input.controllers[player.playerNumber].jump.Pressed)
            Jump();
    }

    // Move the player using the the controller's move input scaled by the provided speed
    private void Move(float speed) {
        Vector2 horizontalVector = input.controllers[player.playerNumber].MoveVector() * speed;
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);
        rb.velocity = new Vector3(scaledVector.x, rb.velocity.y, scaledVector.z);
    }

    // Attempt to perform a jump
    public void Jump() {
        if (allowMovement && isGrounded) { //Checking if on the ground and movement is allowed
            rb.AddForce(Vector3.up * player.stats.jumpForce, ForceMode.Impulse);
            touchedGround = false;
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

    // Check the the player has collided with the ground
    private void OnCollisionStay(Collision collision) {
        if (collision.gameObject.layer == groundLayer)
            touchedGround = true;
    }
}
