using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Header("Components")]
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator anim;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask CCLayer; //the corner correction layer

    [Header("Movement")]
    [SerializeField] private float acceleration = 70f; //movement speed acceleration of the player
    [SerializeField] private float maxSpeed = 12f; //maximum speed of the player
    [SerializeField] private float groundLinDrag = 7f; //linear drag when not moving <= decceleration
    private float horizontalDir;
    private bool changingDir {
        get {
            return (rb.velocity.x > 0f && horizontalDir < 0f) || (rb.velocity.x < 0f && horizontalDir > 0f);
        }
    }

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f; //jump force of the player
    [SerializeField] private float airLinDrag = 2.5f; //linear drag when in air
    [SerializeField] private float fallMultiplier = 8f; //gravity multiplier when in air
    [SerializeField] private float lowJumpFallMultiplier = 5f; //gravity multiplier when jumping and not pressing the jump button <= short jump 
    [SerializeField] private int additionalJumps = 1; //number of extra jumps the player can make after his first
    [SerializeField] private float coyoteTime = .1f; //time window in which the player can jump after walking over an edge
    [SerializeField] private float jumpBufferLength = .1f;
    [SerializeField] [Tooltip("The maximum jump height for one jump in normal tiles")] private float maxJumpHeight;
    private int additionalJumpsCounted;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private Vector2 lastJumpPos;

    public float jumpHeight;

    private bool canJump {
        get {
            return jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || additionalJumpsCounted > 0);
        }
    }

    [Header("Corner Correction")]
    [SerializeField] private float CCRayLength = 1f;
    [SerializeField] private Vector3 CCedgeRayOffset;
    [SerializeField] private Vector3 CCinnerRayOffset;
    private bool canCornerCorrect;

    [Header("Ground Collision")]
    [SerializeField] private float groundRayLength = 1f;
    [SerializeField] private Vector3 groundRayOffset;
    private bool isGrounded;

    private Vector2 startPos;
    private bool isGliding;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    private Vector2 GetInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    void Update() 
    {
        horizontalDir = GetInput().x;

        if (Input.GetButtonDown("Jump")) {
            jumpBufferCounter = jumpBufferLength;
        }
        else {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl)) {
            if (!isGrounded) {
                Glide();
            }
        }
        else if (isGliding) {
            isGliding = false;
        }

        if (Input.GetKey(KeyCode.R)) {
            transform.position = startPos;
        }

        HandleRotation();
        //HandleAnimations();
    }

    private void FixedUpdate() {
        CollisionCheck();
        Move();

        if (isGrounded) {
            ApplyGroundLinearDrag();
            additionalJumpsCounted = additionalJumps; //reset jumps counter
            coyoteTimeCounter = coyoteTime; //reset coyote time counter
        }
        else {
            ApplyAirLinearDrag();
            ApplyFallGravity();
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }
        if (canJump) {
            Jump();
        }
        if (canCornerCorrect) {
            ApplyCornerCorrection(rb.velocity.y);
        }
    }

#region Animations and Sprite Management
    private void HandleRotation() {
        if (GetInput().x > 0) {
            playerSprite.flipX = false;
        }
        else if (GetInput().x < 0) {
            playerSprite.flipX = true;
        }
    }

    //private void HandleAnimations() {
    //    if (isJumping) {
    //        anim.SetBool("jump", true);
    //    }
    //    else {
    //        anim.SetBool("jump", false);
    //    }
    //    if (isRunning && isGrounded) {
    //        anim.SetBool("running", true);
    //    }
    //    else {
    //        anim.SetBool("running", false);
    //    }
    //    if (isFalling) {
    //        anim.SetBool("falling", true);
    //    }
    //    else {
    //        anim.SetBool("falling", false);
    //    }
    //}
#endregion

#region Input Movement
    private void Move() {
        rb.AddForce(new Vector2(horizontalDir, 0f) * acceleration);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed) {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //Clamp velocity when max speed is reached!
        }
    }
    private void Jump() {
        lastJumpPos = transform.position;
        
        if (!isGrounded) {
            additionalJumpsCounted--;
        }

        ApplyAirLinearDrag();
        rb.gravityScale = 1f;
        rb.velocity = new Vector2(rb.velocity.x, 0f); //set y velocity to 0
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        jumpHeight = (rb.velocity.y * rb.velocity.y) / ((2 * -Physics.gravity.y + rb.drag * rb.velocity.y)* -Physics.gravity.y*0.1f);

        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }
    private void Glide() {
        if (!isGliding) {
            isGliding = true;
        }

        float fallSpeed = Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 0);

        rb.AddForce(-transform.up * fallSpeed * Time.deltaTime * 75);//upforce by fallspeed

        rb.AddForce(transform.up * Mathf.Abs(rb.velocity.x) * Time.deltaTime * 75);//upforce by horizontal speed

        rb.AddForce(-transform.right * horizontalDir * fallSpeed * Time.deltaTime * 50);//horizontal speed by fallspeed * x input
    }
#endregion

#region LinearDrag & Gravity Management
    private void ApplyGroundLinearDrag() {
        if(Mathf.Abs(horizontalDir) < .4f || changingDir) {
            rb.drag = groundLinDrag;
        }
        else {
            rb.drag = 0f;
        }
    }
    private void ApplyAirLinearDrag() {
        rb.drag = airLinDrag;
    }
    private void ApplyFallGravity() {
        if (rb.velocity.y < 0f || transform.position.y - lastJumpPos.y > maxJumpHeight)
        {
            rb.gravityScale = fallMultiplier;                   
        }
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump")) {
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else {
            rb.gravityScale = 1f;
        }
    }
#endregion

#region Collision Checks
    private void CollisionCheck() {
        isGrounded = Physics2D.Raycast(transform.position + groundRayOffset, Vector2.down, groundRayLength, groundLayer) ||
                     Physics2D.Raycast(transform.position - groundRayOffset, Vector2.down, groundRayLength, groundLayer);

        canCornerCorrect = Physics2D.Raycast(transform.position + CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer) &&
                           !Physics2D.Raycast(transform.position + CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer) ||
                           Physics2D.Raycast(transform.position - CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer) &&
                           !Physics2D.Raycast(transform.position - CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer);
    }

    private void ApplyCornerCorrection(float _yVelo) {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.left, CCRayLength, CCLayer);

        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * CCRayLength,
                transform.position - CCinnerRayOffset + Vector3.up * CCRayLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, _yVelo);
            return;
        }

        hit = Physics2D.Raycast(transform.position + CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.right, CCRayLength, CCLayer);
        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * CCRayLength,
                transform.position + CCedgeRayOffset + Vector3.up * CCRayLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, _yVelo);
        }
    }
#endregion

#region Debugging
    private void OnDrawGizmos() {
        DrawGroundRays();
        DrawCornerCheckRays();
        DrawCornerCheckRays();
        
    }
    private void DrawGroundRays() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundRayOffset, transform.position + groundRayOffset + Vector3.down * groundRayLength);
        Gizmos.DrawLine(transform.position - groundRayOffset, transform.position - groundRayOffset + Vector3.down * groundRayLength);
    }
    private void DrawCornerCheckRays() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + CCedgeRayOffset, transform.position + CCedgeRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position - CCedgeRayOffset, transform.position - CCedgeRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position + CCinnerRayOffset, transform.position + CCinnerRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position - CCinnerRayOffset, transform.position - CCinnerRayOffset + Vector3.up * CCRayLength);
    }
    private void DrawCornerDistanceRays() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - CCinnerRayOffset + Vector3.up * CCRayLength,
                        transform.position - CCinnerRayOffset + Vector3.up * CCRayLength + Vector3.left * CCRayLength);
        Gizmos.DrawLine(transform.position + CCinnerRayOffset + Vector3.up * CCRayLength,
                        transform.position + CCinnerRayOffset + Vector3.up * CCRayLength + Vector3.right * CCRayLength);
    }
#endregion
}
