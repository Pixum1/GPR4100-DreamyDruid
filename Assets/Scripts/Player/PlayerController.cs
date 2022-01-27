using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {
    #region Variables
    [Header("Components")]
    private Rigidbody2D rb;
    [SerializeField] 
    private SpriteRenderer playerSprite;
    [SerializeField] 
    private Animator anim;
    [SerializeField]

    [Header("Movement Scripts")]
    private Rolling rollingScript;
    [SerializeField]
    private Grappling grapplingScript;
    [SerializeField]
    private Gliding glidingScript;

    [Header("Layer Masks")]
    [SerializeField] [Tooltip("The layer that marks the player as 'grounded' when standing on")]
    private LayerMask groundLayer;
    [SerializeField] [Tooltip("The 'corner correction' layer")]
    private LayerMask CCLayer;
    [SerializeField]
    private LayerMask wallHangLayer;

    [Header("Movement")]
    [SerializeField] [Tooltip("The movement speed acceleration of the player")]
    private float acceleration = 70f;
    [SerializeField] [Tooltip("The maximum speed of the player")]
    private float maxSpeed = 12f;
    [SerializeField] [Tooltip("The friction applied when not moving <= decceleration")]
    private float groundLinDrag = 7f;
    public float m_HorizontalDir {
        get {
            return Input.GetAxisRaw("Horizontal");
        }
    }
    private bool m_ChangingDir {
        get {
            //returns true if the player changes his direction from left to right and vice versa
            return (rb.velocity.x > 0f && m_HorizontalDir < 0f) 
                   || (rb.velocity.x < 0f && m_HorizontalDir > 0f);
        }
    }
    public bool m_CanMove {
        get {
            return m_HorizontalDir != 0f
                   && !grapplingScript.m_IsGrappling
                   && !m_CanWallHang;
        }
    }

    [Header("Jump")]
    [SerializeField] [Tooltip("The jump height of the object in units(metres)")]
    private float jumpHeight;
    [SerializeField]
    private float frogJumpHeight;
    [SerializeField]
    private float owlJumpHeight;
    [SerializeField] [Tooltip("The air resistance while jumping")]
    private float airLinDrag = 2.5f;
    [SerializeField] [Tooltip("Gravity applied when doing a full jump")]
    private float fullJumpFallMultiplier = 8f;
    [SerializeField] [Tooltip("Gravity applied when doing half jump")]
    private float halfJumpFallMultiplier = 5f;
    [SerializeField] [Tooltip("The amount of additional jumps the player can make")]
    private int amountOfJumps = 1;
    private int jumpsCounted;
    private Vector2 lastJumpPos;
    public bool m_CanJump {
        get {
            //return true if the player performs an input in the given time window and has additional jumps left
            return jumpBufferTimer < jumpBufferTime
                   && (coyoteTimeTimer < coyoteTimeTime || jumpsCounted < amountOfJumps)
                   && !rollingScript.isActiveAndEnabled;
        }
    }
    public bool m_CanWallJump {
        get {
            return wallJumpBufferTimer < wallJumpBufferTime;
        }
    }

    [Header("Jump Buffer & Coyote Time")]
    [SerializeField] [Tooltip("The time window that allows the player to perform an action before it is allowed")]
    private float jumpBufferTime = .1f; //WARNING: if the player can not jump this number is probably = 0
    private float jumpBufferTimer = 1000f;

    [SerializeField] [Tooltip("The time window in which the player can jump after walking over an edge")]
    private float coyoteTimeTime = .1f; //WARNING: if the player can not jump this number is probably = 0
    private float coyoteTimeTimer = 1000f;

    [SerializeField]
    [Tooltip("The time window that allows the player to perform a wall jump after leaving the wall")]
    private float wallJumpBufferTime = .1f;
    private float wallJumpBufferTimer = 1000f;

    [Header("Wall Hanging")]
    [SerializeField]
    private float onWallGravityMultiplier;
    public bool m_CanWallHang {
        get {
            return Input.GetKey(KeyCode.LeftShift)
                   && (m_IsOnLeftWall || m_IsOnRightWall)
                   && !m_IsGrounded
                   && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                   && !rollingScript.isActiveAndEnabled
                   && !glidingScript.isActiveAndEnabled
                   && !grapplingScript.isActiveAndEnabled;
        }
    }

    [Header("Corner Correction")]
    [SerializeField] [Tooltip("The distance at which the corner correction should be calculated (Higher numbers = faster detection of corners)")]
    private float CCRayLength = 1f;
    [SerializeField] [Tooltip("The outer offset of the corner correction ray")]
    private Vector3 CCedgeRayOffset;
    [SerializeField] [Tooltip("The inner offset of the corner correction ray")]
    private Vector3 CCinnerRayOffset;
    private bool m_CanCornerCorrect {
        get {
            //returns true if a corner was detected
            return Physics2D.Raycast(transform.position + CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer) 
                   && !Physics2D.Raycast(transform.position + CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer) 
                   || Physics2D.Raycast(transform.position - CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer) 
                   && !Physics2D.Raycast(transform.position - CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer);
        }
    }

    [Header("Collision Check")]
    [SerializeField] 
    private float groundRayLength = 1f;
    [SerializeField] 
    private Vector3 groundRayOffset;
    public bool m_IsGrounded {
        get {
            //returns true if the players feet touch the ground layer
            return Physics2D.Raycast(transform.position + groundRayOffset, Vector2.down, groundRayLength, groundLayer) 
                   || Physics2D.Raycast(transform.position - groundRayOffset, Vector2.down, groundRayLength, groundLayer);
        }
    }
    [SerializeField]
    private float wallRayLength = 1f;
    private float wallRaySave;
    [SerializeField]
    private Vector3 wallRayOffset;
    public bool m_IsOnLeftWall {
        get {
            return Physics2D.Raycast(transform.position + wallRayOffset, -Vector3.right, wallRayLength, wallHangLayer)
                   || Physics2D.Raycast(transform.position - wallRayOffset, -Vector3.right, wallRayLength, wallHangLayer);
        }
    }
    public bool m_IsOnRightWall { 
        get {
            return Physics2D.Raycast(transform.position + wallRayOffset, Vector3.right, wallRayLength, wallHangLayer)
                   || Physics2D.Raycast(transform.position - wallRayOffset, Vector3.right, wallRayLength, wallHangLayer);
        }
    }

    public Action e_PlayerDied;

    #endregion

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        wallRaySave = wallRayLength;
    }

    void Update() {
        if (Input.GetButtonDown("Jump")) {
            jumpBufferTimer = 0; //reset the jump buffer
        }

        if (m_IsGrounded) {
            ApplyGroundLinearDrag();
            jumpsCounted = 0; //reset jumps counter
            coyoteTimeTimer = 0; //reset coyote time counter
        }
        else {
            ApplyAirLinearDrag();

            if (m_CanWallHang) {
                ApplyWallHangGravity();
            }
            else {
                ApplyFallGravity();
            }
        }

        if (m_CanWallHang) {
            jumpsCounted = amountOfJumps - 1;

            if((m_IsOnLeftWall && m_HorizontalDir > 0f) || (m_IsOnRightWall && m_HorizontalDir < 0f))
                wallJumpBufferTimer = 0;
        }

        coyoteTimeTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;
        wallJumpBufferTimer += Time.deltaTime;

        if (Input.GetKey(KeyCode.R)) {
            if (e_PlayerDied == null)
                Debug.LogWarning("Null Reference");

            e_PlayerDied?.Invoke();
        }


        ApplyRotation();
    }

    private void FixedUpdate() {
        if(m_CanMove)
            Move();

        if (m_CanJump) {
            if (m_CanWallJump) {
                Jump(jumpHeight, new Vector2(m_HorizontalDir * 1.5f, 1f));
            }
            if (m_CanWallHang) {
                Jump(jumpHeight / .5f, new Vector2(-m_HorizontalDir, 1f));
            }
            if (grapplingScript.isActiveAndEnabled) {
                jumpsCounted = amountOfJumps;
                Jump(frogJumpHeight, Vector2.up);
            }
            if (glidingScript.isActiveAndEnabled) {
                jumpsCounted = amountOfJumps;
                Jump(owlJumpHeight, Vector2.up);
            }
            else {
                Jump(jumpHeight, Vector2.up);
            }
        }

        if (m_CanCornerCorrect) {
            CheckForCornerCorrection(rb.velocity.y);
        }
    }

    #region Animations and Sprite Management
    private void ApplyRotation() {
        if (m_HorizontalDir > 0) {
            playerSprite.flipX = false;
        }
        else if (m_HorizontalDir < 0) {
            playerSprite.flipX = true;
        }
    }
    #endregion
    
    #region Input & Movement

    /// <summary>
    /// Makes the player move into the desired horinzontal direction and limits his speed
    /// </summary>
    private void Move() {
        rb.AddForce(new Vector2(m_HorizontalDir, 0f) * acceleration);

        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y); //Clamp velocity when max speed is reached!
    }

    /// <summary>
    /// Makes the player jump with a specific force to reach an exact amount of units in vertical space
    /// </summary>
    private void Jump(float _jumpHeight, Vector2 _dir) {
        if (coyoteTimeTimer > coyoteTimeTime && jumpsCounted < 1) {
            jumpsCounted = amountOfJumps;
        }

        lastJumpPos = transform.position;
        coyoteTimeTimer = coyoteTimeTime;
        jumpBufferTimer = jumpBufferTime;
        wallJumpBufferTimer = wallJumpBufferTime;
        jumpsCounted++;

        ApplyAirLinearDrag();

        rb.gravityScale = fullJumpFallMultiplier;

        rb.velocity = new Vector2(rb.velocity.x, 0f); //set y velocity to 0
        float jumpForce;

        StartCoroutine(DisableWallRay());

        jumpForce = Mathf.Sqrt((_jumpHeight + .5f) * -2f * (Physics.gravity.y * rb.gravityScale));
        rb.AddForce(_dir * jumpForce, ForceMode2D.Impulse);
    }

    private IEnumerator DisableWallRay() {
        wallRayLength = 0f;
        yield return new WaitForEndOfFrame();
        wallRayLength = wallRaySave;
    }

    #endregion
    
    #region LinearDrag & Gravity Management
    /// <summary>
    /// Applies the ground friction based on wether the player is moving or giving no horizontal inputs
    /// </summary>
    private void ApplyGroundLinearDrag() {
        if(Mathf.Abs(m_HorizontalDir) < .4f || m_ChangingDir) {
            rb.drag = groundLinDrag;
        }
        else {
            rb.drag = 0f;
        }
    }
    /// <summary>
    /// Applies the air resistance when the player is jumping
    /// </summary>
    private void ApplyAirLinearDrag() {
        rb.drag = airLinDrag;
    }
    /// <summary>
    /// Applies the fall gravity based on the players jump height and input
    /// </summary>
    private void ApplyFallGravity() {
        if(!grapplingScript.isActiveAndEnabled) {
            if (rb.velocity.y < 0f || transform.position.y - lastJumpPos.y > jumpHeight) {
                rb.gravityScale = fullJumpFallMultiplier;
            }
            else if (rb.velocity.y > 0f && !Input.GetButton("Jump")) {
                rb.gravityScale = halfJumpFallMultiplier;
            }
            else {
                rb.gravityScale = 1f;
            }
        }
        else {
            if (rb.velocity.y < 0f || transform.position.y - lastJumpPos.y > frogJumpHeight) {
                rb.gravityScale = fullJumpFallMultiplier;
            }
            else if (rb.velocity.y > 0f && !Input.GetButton("Jump")) {
                rb.gravityScale = halfJumpFallMultiplier;
            }
            else {
                rb.gravityScale = 1f;
            }
        }
    }
    private void ApplyWallHangGravity() {
        rb.gravityScale = onWallGravityMultiplier;
        rb.velocity = new Vector2(0, 0f); //set y velocity to 0
    }
    #endregion
    
    #region Corner Correction
    /// <summary>
    /// Applies the corner correction when the player hits a block above him in a very specific angle
    /// </summary>
    /// <param name="_verticalVelocity">The vertical velocity of the player</param>
    private void CheckForCornerCorrection(float _verticalVelocity) {
        //Check for corner on the left
        RaycastHit2D hit = Physics2D.Raycast(transform.position - CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.left, CCRayLength, CCLayer);
        if (hit.collider != null) {
            int dir = 1; //right
            ApplyCornerCorrection(_verticalVelocity, CalculateCornerCorrection(hit, dir), dir);
            return;
        }

        //Check for corner on the right
        hit = Physics2D.Raycast(transform.position + CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.right, CCRayLength, CCLayer);
        if (hit.collider != null) {
            int dir = -1; //left
            ApplyCornerCorrection(_verticalVelocity, CalculateCornerCorrection(hit, dir), dir);
        }
    }
    /// <summary>
    /// Calculate the position the player should be put in after the corner correction
    /// </summary>
    /// <returns>New Position</returns>
    private float CalculateCornerCorrection(RaycastHit2D _hit, int _direction) {
        return Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * CCRayLength,
                                            transform.position - (Mathf.Clamp(_direction, -1, 1) * CCedgeRayOffset) + Vector3.up * CCRayLength);
    }
    /// <summary>
    /// Moves the player to the new position and remain his vertical velocity
    /// </summary>
    private void ApplyCornerCorrection(float _verticalVelocity, float _newPos, int _direction) {
        transform.position = new Vector3(transform.position.x + (Mathf.Clamp(_direction, -1, 1) * _newPos), transform.position.y, transform.position.z);

        rb.velocity = new Vector2(rb.velocity.x, _verticalVelocity);
    }
    #endregion

    #region Debugging
    private void OnDrawGizmos() {
        DrawGroundRays();
        DrawCornerCheckRays();
        DrawCornerDistanceRays();
        DrawWallDistanceRays();
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
    private void DrawWallDistanceRays() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + wallRayOffset, transform.position + wallRayOffset + Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position - wallRayOffset, transform.position - wallRayOffset + Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position + wallRayOffset, transform.position + wallRayOffset - Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position - wallRayOffset, transform.position - wallRayOffset - Vector3.right * wallRayLength);
    }
#endregion
}
