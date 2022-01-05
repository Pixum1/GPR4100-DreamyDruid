using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator anim;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask cornerCorrectLayer;

    [Header("Movement")]
    [SerializeField] private float acceleration = 70f; //movement speed acceleration of the player
    [SerializeField] private float maxSpeed = 12f; //maximum speed of the player
    [SerializeField] private float groundLinDrag = 7f; //linear drag when not moving <= decceleration
    private float horizontalDir;
    private bool changingDir => (rb.velocity.x > 0f && horizontalDir < 0f) || (rb.velocity.x < 0f && horizontalDir > 0f); //returns true when switching from left to right and vice versa

    [Header("Jump")]
    [SerializeField] private float jumpForce = 20f; //jump force of the player
    [SerializeField] private float airLinDrag = 2.5f; //linear drag when in air
    [SerializeField] private float fallMultiplier = 8f; //gravity multiplier when in air
    [SerializeField] private float lowJumpFallMultiplier = 5f; //gravity multiplier when jumping and not pressing the jump button <= short jump 
    [SerializeField] private int extraJumps = 1; //number of extra jumps the player can make after his first
    [SerializeField] private float coyoteTime = .1f; //time window in which the player can jump after walking over an edge
    [SerializeField] private float jumpBufferLength = .1f; 
    private int extraJumpsValue;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool canJump => jumpBufferCounter > 0f && (coyoteTimeCounter > 0f || extraJumpsValue > 0);

    [Header("Corner Correction")]
    [SerializeField] private float topRayLength = 1f;
    [SerializeField] private Vector3 edgeRayOffset;
    [SerializeField] private Vector3 innerRayOffset;
    private bool canCornerCorrect;

    [Header("Ground Collision")]
    [SerializeField] private float groundRayLength = 1f;
    [SerializeField] private Vector3 groundRayOffset;
    private bool isGrounded;

    private Vector2 startPos;
    private bool isGliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
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

        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isGrounded)
            {
                Glide();
            }
        }
        else if (isGliding)
        {
            isGliding = false;
        }

        //reset player position
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = startPos;
        }

        HandleRotation();
        HandleAnimations();
    }

    private void FixedUpdate() {
        CheckCollision();
        Move();

        if (canJump) {
            Jump();
        }

        if (isGrounded) {
            ApplyGroundLinearDrag();
            extraJumpsValue = extraJumps;
            coyoteTimeCounter = coyoteTime;
        }
        else {
            ApplyAirLinearDrag();
            ApplyFallGravity();
            coyoteTimeCounter -= Time.deltaTime;
        }
        if (canCornerCorrect) {
            CornerCorrect(rb.velocity.y);
        }
    }
    #region Animations and Sprite Management
    private void HandleRotation() {
        if(rb.velocity.x > 0) {
            playerSprite.flipX = false;
        }
        else if(rb.velocity.x < 0){
            playerSprite.flipX = true;
        }
    }

    private void HandleAnimations() {
        if (rb.velocity.y > 0) {
            anim.SetBool("jump", true);
        }
        else if (rb.velocity.y < 0) {
            anim.SetBool("falling", true);
        }
        else {
            anim.SetBool("jump", false);
            anim.SetBool("falling", false);
        }
    }
    #endregion

    private Vector2 GetInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void CheckCollision() {
        isGrounded = Physics2D.Raycast(transform.position +groundRayOffset, Vector2.down, groundRayLength, groundLayer) ||
                     Physics2D.Raycast(transform.position - groundRayOffset, Vector2.down, groundRayLength, groundLayer);

        canCornerCorrect = Physics2D.Raycast(transform.position + edgeRayOffset, Vector2.up, topRayLength, cornerCorrectLayer) &&
                           !Physics2D.Raycast(transform.position + innerRayOffset, Vector2.up, topRayLength, cornerCorrectLayer) ||
                           Physics2D.Raycast(transform.position - edgeRayOffset, Vector2.up, topRayLength, cornerCorrectLayer) &&
                           !Physics2D.Raycast(transform.position - innerRayOffset, Vector2.up, topRayLength, cornerCorrectLayer);
    }

    private void Move() {
        rb.AddForce(new Vector2(horizontalDir, 0f) * acceleration);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed) {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //Clamp velocity when max speed is reached!
        }
    }

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

    private void Jump() {
        if (!isGrounded) {
            extraJumpsValue--;
        }
        rb.velocity = new Vector2(rb.velocity.x, 0f); //set y velocity to 0
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        coyoteTimeCounter = 0f;
        jumpBufferCounter = 0f;
    }
    private void ApplyFallGravity() {
        if (rb.velocity.y < 0f) {
            rb.gravityScale = fallMultiplier;
        }
        else if (rb.velocity.y > 0f && !Input.GetButton("Jump")) {
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else {
            rb.gravityScale = 1f;
        }
    }

    private void CornerCorrect(float _yVelocity) {
        //Push player to the right
        RaycastHit2D hit = Physics2D.Raycast(transform.position - innerRayOffset + Vector3.up * topRayLength, Vector3.left, topRayLength, cornerCorrectLayer);
        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayLength,
                transform.position - innerRayOffset + Vector3.up * topRayLength);
            transform.position = new Vector3(transform.position.x + newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, _yVelocity);

            Debug.Log("Corner Correct: Right!");
            return;
        }

        //Push player to the left
        hit = Physics2D.Raycast(transform.position + innerRayOffset + Vector3.up * topRayLength, Vector3.right, topRayLength, cornerCorrectLayer);
        if (hit.collider != null) {
            float newPos = Vector3.Distance(new Vector3(hit.point.x, transform.position.y, 0f) + Vector3.up * topRayLength,
                transform.position + edgeRayOffset + Vector3.up * topRayLength);
            transform.position = new Vector3(transform.position.x - newPos, transform.position.y, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, _yVelocity);

            Debug.Log("Corner Correct: Left!");
        }
    }

    private void Glide()
    {
        if (!isGliding)
        {
            isGliding = true;
        }

        float fallSpeed = Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 0);

        rb.AddForce(-transform.up * fallSpeed * Time.deltaTime * 75);//upforce by fallspeed

        rb.AddForce(transform.up * Mathf.Abs(rb.velocity.x) * Time.deltaTime * 75);//upforce by horizontal speed

        rb.AddForce(-transform.right * horizontalDir * fallSpeed * Time.deltaTime * 50);//horizontal speed by fallspeed * x input
    }

    private void OnDrawGizmos() {
        //Ground Check
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundRayOffset, transform.position + groundRayOffset + Vector3.down * groundRayLength);
        Gizmos.DrawLine(transform.position - groundRayOffset, transform.position - groundRayOffset + Vector3.down * groundRayLength);

        //Corner Check
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + edgeRayOffset, transform.position + edgeRayOffset + Vector3.up * topRayLength);
        Gizmos.DrawLine(transform.position - edgeRayOffset, transform.position - edgeRayOffset + Vector3.up * topRayLength);
        Gizmos.DrawLine(transform.position + innerRayOffset, transform.position + innerRayOffset + Vector3.up * topRayLength);
        Gizmos.DrawLine(transform.position - innerRayOffset, transform.position - innerRayOffset + Vector3.up * topRayLength);

        //Corner Distance Check
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - innerRayOffset + Vector3.up * topRayLength,
                        transform.position - innerRayOffset + Vector3.up * topRayLength + Vector3.left * topRayLength);
        Gizmos.DrawLine(transform.position + innerRayOffset + Vector3.up * topRayLength,
                        transform.position + innerRayOffset + Vector3.up * topRayLength + Vector3.right * topRayLength);
    }
}
