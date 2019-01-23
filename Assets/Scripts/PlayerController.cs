using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Vector3Extensions;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IHurtable {

    public Weapon specialMove;
    // public SpecialMove specialMove; // Dodge, Ice Wall, etc. 
    public Weapon Weapon { get; private set; }
    public Weapon PreviousWeapon;

    private Player _player;
    public Player player {
        get { return _player; }
        set {
            _player = value;
        }
    }

    [HideInInspector]
    public InputManager input;

    protected Rigidbody rb;
    public Renderer rend;
    public Transform PlayerHitbox;
    public GameObject ShootPoint;
    public Collider floorCollider;
    public List<Collider> HitBox;
    public GameObject crown;

    public float friction = 1f;
    public float airResistance = 1f;
    public float terminalVelocity = 1f;
    public float DeathAnimationTime = 0.75f;
    [Tooltip("time it takes to go from 0 to max speed is 1 second / acceleration")]
    public float acceleration = 10f;

    public Color[] Colors = new Color[4]; //The player's color is based on thier player number
    public Color playerColor { get { return Colors[player.playerNumber]; } set { Colors[player.playerNumber] = value; } }//used for the bullettrail

    public float jumpGravityMultiplier = 0.5f;

    // Used to scale movement to the camera's direction
    protected Vector3 forward;
    protected Vector3 right;

    public Vector2 lastForwardMovement = Vector2.zero; //used to dodge forward when not moving 
    public Vector3 lastScaledVector = Vector2.zero;

    // For disabling movement while performing a dodge or potentially while stunned
    private bool allowMovement = true;
    private bool allowAttack = true; //used to disable movement during the countdown
    public bool dodging = false;
    public bool rolling = false;
    public bool braking;
    public float dodgeSpeed = 0f;
    [SerializeField]
    private DashEffect dashEffect;
    [SerializeField]
    private KnockbackEffect knockbackEffect;

    // Required variables for jumping and detecting ground collisions
    private float jumpCooldown = 0;
    protected bool jumped = false;
    private bool jumpedReleased = true;
    public static readonly int groundLayer = 11;
    private static readonly int groundLayerMask = 1 << groundLayer;
    private static readonly float maxGroundDistance = 0.5f;
    private bool touchedGround;

    private float lastGroundCheck = 0;
    private bool isGrounded = false;
    public bool IsGrounded {
        get {
            if (lastGroundCheck == Time.fixedTime)
                return isGrounded;
            else {
                lastGroundCheck = Time.fixedTime;
                if (touchedGround) {
                    if (CheckGroundDistance() < maxGroundDistance)
                        return isGrounded = true;
                    touchedGround = false;
                }
                return isGrounded = false;
            }
        }
    }

    // Used for detecting when a player has been knocked back, for vertical dampening
    private bool isKnockedBack = false;

    protected Vector3 lastPosition;

    public bool IsMoving {
        get { return rb.velocity.magnitude > 0.1f; }
    }



    // Initialize referances
    private void Awake() {
        input = InputManager.inputManager;
        if (input == null)
            Debug.LogError("Input Manager does not exist");
        rb = GetComponent<Rigidbody>();
        rend = rend == null ? GetComponent<Renderer>() : rend;
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

        // rend.material.color = playerColor; //setting the player color based on playeNum 
        rend.material.SetColor("Color_91A455EE", playerColor); //this is how shader properties are changed 

        lastPosition = transform.position;

        input.controllers[player.playerNumber].attack.OnDown.AddListener(ActivateAttack);
        input.controllers[player.playerNumber].attack.OnUp.AddListener(ReleaseAttack);
        input.controllers[player.playerNumber].jump.OnUp.AddListener(ReleaseJump);
        input.controllers[player.playerNumber].dodge.OnDown.AddListener(ActivateSpecialMove);
        input.controllers[player.playerNumber].dodge.OnUp.AddListener(ReleaseSpecialMove);
        input.controllers[player.playerNumber].Switch.OnDown.AddListener(SwitchWeapon);

        StartCoroutine(StartAnimation());
    }

    public void Destroy() {
        if (Weapon != null)
            Weapon.UnequipWeapon();
        StartCoroutine(DeathAnimation());
    }

    public void CheckHeight() //if the player is falling remove them from the Camera 
    {
        if (transform.position.y <= 1) //if the player is falling, remove them from the camera because they will die =(
        {
            Camera.main.GetComponentInParent<FollowTargetsCamera>().targets.Remove(gameObject); //perhaps not the most efficent 
        }
    }

    public IEnumerator StartAnimation() //basically the death animation in reverse
    {
        float _startTime = Time.time + DeathAnimationTime + 0.275f;
        float count = Time.time;
        while (Time.time <= _startTime)
        {
            float dissolveValue = 1 - (Time.time - (count * Time.deltaTime)) * 1.5f; //-1 is not dissolved, 1 is fully disolved
            if (rend)
                rend.material.SetFloat("Vector1_F96347CF", dissolveValue);
            yield return null; //the game crashes super hard if you remove this
        }

        if (rend)
            rend.material.SetFloat("Vector1_F96347CF", -1f);
        yield return null;
    }

    public IEnumerator DeathAnimation() //after the player dies, change a shader property in a while loop to make the player dissovle 
    {
        float _deathTime = Time.time + DeathAnimationTime;
        float count = Time.time;
        while (Time.time <= _deathTime)
        {
            float dissolveValue = Time.time - count - 0.75f; //-1 is not dissolved, 1 is fully disolved
            if (rend)
                rend.material.SetFloat("Vector1_F96347CF", dissolveValue);
            yield return null; //the game crashes super hard if you remove this
        }
        //yield return new WaitForSeconds(DeathAnimationTime); //in this time an animation or something can happen 
        if (gameObject)
            GameManager.Destroy(gameObject);
        yield return null;
    }

    // Perform movement every physics update
    private void FixedUpdate() {
        if (!dodging && !rolling)
            Move(IsGrounded ? player.stats.moveSpeed : player.stats.airSpeed);
        else
        {
            Move(dodgeSpeed); //hopefully this allows air dodges
        }
        if (input.controllers[player.playerNumber].jump.Pressed) {
            Jump();
            //if (jumped && rb.velocity.y > 0) {
            //    rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
            //}
        }
        if (jumped && rb.velocity.y > 0) {
            rb.AddForce(Physics.gravity * jumpGravityMultiplier - Physics.gravity, ForceMode.Acceleration);
        }

        // Show smoke when dodging
        if (dashEffect != null)
            dashEffect.SetActive(dodging, lastScaledVector);
    }

    // Rotate the player's facing direction
    private void Update() {
        player.InGameUpdate();
        PlayerHitbox.localScale = player == GameManager.instance.leader ? Vector3.one * 1.35f : Vector3.one;
        if (crown != null)
            crown.SetActive(player == GameManager.instance.leader);


        Vector2 horizontalVector = input.controllers[player.playerNumber].AimVector();

        //Debug.DrawRay(transform.position, transform.forward*100, Color.white);
        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);

        if (scaledVector != Vector3.zero)
            transform.forward = Vector3.RotateTowards(transform.forward, scaledVector, player.stats.TurnSpeed * Mathf.Deg2Rad * Time.deltaTime, 0);

        UpdateTriggers();
        CheckHeight();
        lastPosition = transform.position;
    }

    // Move the player using the the controller's move input scaled by the provided speed
    protected void Move(float speed) {
        Vector2 horizontalVector = input.controllers[player.playerNumber].MoveVector() * speed;
        if (dodging)
        {
            if (horizontalVector == Vector2.zero)
                horizontalVector = lastForwardMovement * speed; //if the user is not entering any input
            else
                horizontalVector = horizontalVector.normalized * speed; //in the case the magnitude is below 1

            if (rolling)
                horizontalVector = lastForwardMovement * speed;
        }

        Vector3 scaledVector = (horizontalVector.y * forward) + (horizontalVector.x * right);

        if (!dodging && horizontalVector != Vector2.zero) { //if not dodging, remember the movement and scaled vector so the can player can dodge even when standing still
            lastForwardMovement = horizontalVector.normalized;
            lastScaledVector = scaledVector.normalized;
        }

        Vector3 frictionVector;
        if (IsGrounded) {// If grounded apply friction
            isKnockedBack = false;
            frictionVector = -friction * rb.velocity; // Friction is a negative percentage of current velocity
        }
        else { // In air apply Air Resistance
            Vector3 upVector = Vector3.Project(rb.velocity, Physics.gravity); // get upwardVelocity with respect to gravity, if for some reason gravity is not straight down this will still work
            Vector3 terminalVector = -Physics.gravity.normalized * terminalVelocity;
            Vector3 scaledVelocity = rb.velocity;

            if (Vector3.Distance(upVector, terminalVector) < Vector3.Distance(upVector, -terminalVector) && (upVector.magnitude > terminalVelocity || isKnockedBack)) {
                //scaledVelocity -= terminalVector; // Calculate y velocity greater than terminal velocity
                isKnockedBack = true; // Vertical velocity over threshold
            }
            else
                scaledVelocity -= upVector; // Ignore y velocity if falling or not over terminal velocity
            frictionVector = -airResistance * scaledVelocity; // Friction is a negative percentage of current velocity
        }
        // Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, frictionVector, Color.blue);
        // Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, scaledVector, Color.green);

        // Remove friction in the desired move direction if moving slower than max speed in that direction
        Vector3 velocityInDirection = Vector3.Project(rb.velocity, scaledVector); // Get velocity in the desired move direction
        if (Vector3.Distance(velocityInDirection, scaledVector) < Vector3.Distance(velocityInDirection, -scaledVector) && velocityInDirection.magnitude < input.controllers[player.playerNumber].MoveVector().magnitude * speed) // Check if we are moving slower than our maximum speed
            frictionVector -= Vector3.Project(frictionVector, scaledVector); // Scale friction to remove the forward direction, so friction doesnt slow player in moving direction
        // Debug.DrawRay(floorCollider.transform.position + Vector3.down * floorCollider.bounds.extents.y, frictionVector, Color.red);
        
        rb.velocity += frictionVector * Time.fixedDeltaTime; // Add friction to velocity, friction is acceleration, so multiply by delta time to get change in velocity
        if (allowMovement) {
            Vector3 oldVelocity = rb.velocity.Scaled(new Vector3(1, 0, 1));
            Vector3 newVelocity = rb.velocity + scaledVector * Time.fixedDeltaTime * acceleration; // Clamp velocity to either max speed or current speed(if player was launched)
            if (dodging) //don't take the old velocity into account at all when dodging
                newVelocity = scaledVector * acceleration; //scaled Vector is the direction of movement
            if (!dodging) //don't clamp while dodging
                newVelocity = Vector3.ClampMagnitude(new Vector3(newVelocity.x, 0, newVelocity.z), Mathf.Max(oldVelocity.magnitude, speed * input.controllers[player.playerNumber].MoveVector().magnitude + 0.1f) - 0.1f);
            if (braking)
                rb.velocity = new Vector3(0f, newVelocity.y, 0f);
            else
                rb.velocity = new Vector3(newVelocity.x, rb.velocity.y, newVelocity.z);
            
        }
        //if (rb.velocity != Vector3.zero && dodging)
        //  Debug.Log("Velocity is " + rb.velocity+" and more importantly "+rb.velocity.magnitude); 
    }

    private void UpdateTriggers() {
        if (IsMoving && IsGrounded) {
            player.OnMove.Invoke(lastPosition, transform.position - lastPosition, null);
        }
    }

    private void ActivateSpecialMove() {
        if (allowAttack)
            player.specialMove.Activate(transform.position, transform.forward);
    }

    private void ReleaseSpecialMove() {
        player.specialMove.Release();
    }

    public void DashVelocity() //make sure the player starts the dash at full velocity and then slows down
    {

    }

    public void ResetVelocity()
    {
        rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
    }

    // Attempt to perform a jump
    protected void Jump() {
        if (jumpedReleased && allowMovement && IsGrounded) { //Checking if on the ground and movement is allowed
            jumpedReleased = false;
            jumped = true;
            rb.velocity = rb.velocity += (2 * -Physics.gravity * player.stats.jumpForce * jumpGravityMultiplier).Sqrt();
            touchedGround = false;
            jumpCooldown = 0.1f + Time.time;
        }
    }

    private void ReleaseJump() {
            jumpedReleased = true;
        //if (jumped && rb.velocity.y > 0) {
        //    rb.velocity -= Vector3.Project(rb.velocity, Physics.gravity);
        //    jumped = false;
        //}
    }

    // On Attack down
    private void ActivateAttack() {
        if(allowAttack)
            Weapon.Activate(ShootPoint.transform.position, ShootPoint.transform.forward);   
    }

    // On Attack up
    private void ReleaseAttack() {
        if(allowAttack)
            Weapon.Release();
    }

    public void ResetCharge()
    {
        Weapon.ResetCharge();
    }

    public void EquipWeapon(Weapon weapon) {
        if (Weapon != null)
        {
            PreviousWeapon = this.Weapon;
            Weapon.UnequipWeapon();
        }
        Weapon = weapon;
        Weapon.EquipWeapon();

        if (input.controllers[player.playerNumber].attack.Pressed && allowAttack) // If the attack button was held down at the time of equipting new weapon activate the new weapon
            Weapon.Activate(ShootPoint.transform.position, ShootPoint.transform.forward);
    }

    public void SwitchWeapon()    {
        if(PreviousWeapon != null)
        {
            EquipWeapon(PreviousWeapon);
        }
    }

    private IEnumerator lastKnockback;
    public void Knockback(Vector3 direction) {
        rb.AddForce(direction, ForceMode.VelocityChange); //move back in the direction of the projectile 
        
        // Show smoke when knockback
        if (lastKnockback != null) {
            StopCoroutine(lastKnockback);
            knockbackEffect.SetActive(false, Vector3.zero);
        }
        knockbackEffect.SetActive(true, direction);
        lastKnockback = StopKnockback(direction.magnitude/friction);
        StartCoroutine(lastKnockback);

        IEnumerator StopKnockback(float maxDuration) {
            float stopTime = Time.time + maxDuration;
            while(Time.time < stopTime) {
                yield return null;
            }
            knockbackEffect.SetActive(false, Vector3.zero);
        }
    }

    public float Hurt(Player damageDealer, float amount) {
        input.controllers[player.playerNumber].Vibrate(1.0f, 0.1f);
        StartCoroutine("HurtIndicator");
        return player.Hurt(damageDealer, amount);
    }

    IEnumerator HurtIndicator() //show the player that it is hurt 
    {
        rend.material.SetColor("Color_91A455EE", Color.white);
        yield return new WaitForSeconds(0.04f); //the player flashes white 
        rend.material.SetColor("Color_91A455EE", playerColor);
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

    public void DisableUI() //when the player dies and the death animation is going on
    {
        transform.Find("PlayerUI").gameObject.SetActive(false); // deleted the UI 
    }

    protected float CheckGroundDistance() {
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
