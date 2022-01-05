using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    private Rigidbody2D rb;

    [Header("Layer Masks")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Ground Collision")]
    [SerializeField] private float groundRayLength;
    [SerializeField] private Vector3 groundRayOffset;
    private bool isGrounded;

    [Header("Movement")]
    [SerializeField] private float acceleration;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float groundLinDrag;
    private float horizontalDir;
    private bool changingDir => (rb.velocity.x > 0f && horizontalDir < 0f) || (rb.velocity.x < 0f && horizontalDir > 0f); //returns true when switching from left to right and vice versa

    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float airLinDrag;
    [SerializeField] private float fallMultiplier;
    [SerializeField] private float lowJumpFallMultiplier;
    [SerializeField] private int extraJumps = 1;
    private int extraJumpsValue;
    private bool canJump => Input.GetButtonDown("Jump") && (isGrounded || extraJumpsValue > 0);

    private Vector2 startPos;

    private bool isGliding;
    
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer playerSprite;
    //[SerializeField] private Animator anim;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }


    void Update()
    {
        CheckCollision();

        horizontalDir = GetInput().x;

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
        //HandleAnimations();
    }

    private void FixedUpdate() {

        Move();
        ApplyLinearDrag();

        if (canJump) {
            Jump();
        }

        if (isGrounded) {
            extraJumpsValue = extraJumps;
            ApplyLinearDrag();
        }
        else {
            ApplyAirLinearDrag();
            ApplyFallGravity();
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

    //private void HandleAnimations() {
    //    if (rb.velocity.y > 0) {
    //        anim.SetBool("jump", true);
    //    }
    //    else if (rb.velocity.y < 0) {
    //        anim.SetBool("falling", true);
    //    }
    //    else {
    //        anim.SetBool("jump", false);
    //        anim.SetBool("falling", false);
    //    }
    //}
    #endregion

    private Vector2 GetInput() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void CheckCollision() {
        isGrounded = Physics2D.Raycast(transform.position +groundRayOffset, Vector2.down, groundRayLength, groundLayer) ||
                     Physics2D.Raycast(transform.position - groundRayOffset, Vector2.down, groundRayLength, groundLayer);
    }

    private void Move() {
        rb.AddForce(new Vector2(horizontalDir, 0f) * acceleration);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed) {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //Clamp velocity when max speed is reached!
        }
    }

    private void ApplyLinearDrag() {
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
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundRayOffset, transform.position + groundRayOffset + Vector3.down * groundRayLength);
        Gizmos.DrawLine(transform.position - groundRayOffset, transform.position - groundRayOffset + Vector3.down * groundRayLength);
    }
}
