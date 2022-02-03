using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    [SerializeField]
    private PlayerController player;

    [Header("Movement")]
    [SerializeField]
    [Tooltip("The movement speed acceleration of the player")]
    private float acceleration = 70f;
    [SerializeField]
    [Tooltip("The maximum speed of the player")]
    private float maxSpeed = 12f;
    [SerializeField]
    [Tooltip("The friction applied when not moving <= decceleration")]
    private float groundLinDrag = 20f;
    

    [Header("Jump")]
    [SerializeField]
    [Tooltip("The jump height of the object in units(metres)")]
    private float jumpHeight;
    [SerializeField]
    private float frogJumpHeight;
    private float frogJumpHeightMultiplier;
    [SerializeField]
    private float frogJumpMaxMulti = 15f;
    [SerializeField]
    private float frogJumpTimeToMaxForce = 2;
    private bool frogJump;
    [SerializeField]
    private float owlJumpHeight;
    [SerializeField]
    [Tooltip("The air resistance while jumping")]
    private float airLinDrag = 2.5f;
    [SerializeField]
    [Tooltip("Gravity applied when doing a full jump")]
    private float fullJumpFallMultiplier = 8f;
    [SerializeField]
    [Tooltip("Gravity applied when doing half jump")]
    private float halfJumpFallMultiplier = 5f;
    [SerializeField]
    [Tooltip("The amount of additional jumps the player can make")]
    private int amountOfJumps = 1;
    private int jumpsCounted;
    private Vector2 lastJumpPos;
    

    [Header("Jump Buffer & Coyote Time")]
    [SerializeField]
    [Tooltip("The time window that allows the player to perform an action before it is allowed")]
    private float jumpBufferTime = .1f; //WARNING: if the player can not jump this number is probably = 0
    private float jumpBufferTimer = 1000f;

    [SerializeField]
    [Tooltip("The time window in which the player can jump after walking over an edge")]
    private float coyoteTimeTime = .1f; //WARNING: if the player can not jump this number is probably = 0
    private float coyoteTimeTimer = 1000f;

    [SerializeField]
    [Tooltip("The time window that allows the player to perform a wall jump after leaving the wall")]
    private float wallJumpBufferTime = .1f;
    private float wallJumpBufferTimer = 1000f;

    [Header("Wall Hanging")]
    [SerializeField]
    private float onWallGravityMultiplier;

    #region bool / movement conditions
    public float m_HorizontalDir {
        get {
            return Input.GetAxisRaw("Horizontal");
        }
    }
    public bool m_ChangingDir {
        get {
            return (player.rb.velocity.x > 0f && m_HorizontalDir < 0f)
                   || (player.rb.velocity.x < 0f && m_HorizontalDir > 0f);
        }
    }
    public bool m_CanMove {
        get {
            return m_HorizontalDir != 0f
                   && !player.grapplingScript.m_IsGrappling
                   && !m_CanWallHang;
        }
    }
    public bool m_CanJump {
        get {
            if(amountOfJumps > 1) {
                return jumpBufferTimer < jumpBufferTime
                    && (coyoteTimeTimer < coyoteTimeTime || jumpsCounted < amountOfJumps)
                    && !player.rollingScript.isActiveAndEnabled;
            }
            else if (m_CanWallHang) {
                return jumpBufferTimer < jumpBufferTime;
            }
            else {
                return jumpBufferTimer < jumpBufferTime
                    && coyoteTimeTimer < coyoteTimeTime
                    && !player.rollingScript.isActiveAndEnabled;
            }
        }
    }
    public bool m_CanWallJump {
        get {
            return wallJumpBufferTimer < wallJumpBufferTime;
        }
    }
    public bool m_CanWallHang {
        get {
            return Input.GetKey(KeyCode.LeftShift)
                   && (player.pCollision.m_IsOnLeftWall || player.pCollision.m_IsOnRightWall)
                   /*&& !m_IsGrounded*/
                   && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                   && !player.rollingScript.isActiveAndEnabled
                   && !player.glidingScript.isActiveAndEnabled
                   && !player.grapplingScript.isActiveAndEnabled;
        }
    }

    public bool PlayerJumped;
    #endregion

    private void Update() {

        if (Input.GetButtonDown("Jump")) {
            frogJump = false;
            jumpBufferTimer = 0; //reset the jump buffer
        }
        if (frogJump) {
            FrogJump();
        }

        if (player.pCollision.m_IsGrounded) {
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

            if ((player.pCollision.m_IsOnLeftWall && m_HorizontalDir > 0f) || (player.pCollision.m_IsOnRightWall && m_HorizontalDir < 0f)) {
                wallJumpBufferTimer = 0;
            }
        }

        coyoteTimeTimer += Time.deltaTime;
        jumpBufferTimer += Time.deltaTime;
        wallJumpBufferTimer += Time.deltaTime;
    }

    private void FixedUpdate() {
        if (m_CanMove)
            Move();

        if (m_CanJump) {
            if (m_CanWallJump) {
                Jump(jumpHeight, new Vector2(m_HorizontalDir * 1.5f, 1f));
            }
            else if (m_CanWallHang) {
                Jump(jumpHeight / .5f, new Vector2(-m_HorizontalDir/2f, 1f));
            }
            if (player.grapplingScript.isActiveAndEnabled) {
                jumpsCounted = amountOfJumps;
                frogJump = true;
            }
            if (player.glidingScript.isActiveAndEnabled) {
                jumpsCounted = amountOfJumps;
                Jump(owlJumpHeight, Vector2.up);
            }
            else if (!player.grapplingScript.isActiveAndEnabled) {
                Jump(jumpHeight, Vector2.up);
            }
        }
    }

    public void Move() {
        player.rb.AddForce(new Vector2(m_HorizontalDir, 0f) * acceleration);

        if (Mathf.Abs(player.rb.velocity.x) > maxSpeed)
            player.rb.velocity = new Vector2(Mathf.Sign(player.rb.velocity.x) * maxSpeed, player.rb.velocity.y); //Clamp velocity when max speed is reached!
    }

    /// <summary>
    /// Makes the player jump with a specific force to reach an exact amount of units in vertical space
    /// </summary>
    public void Jump(float _jumpHeight, Vector2 _dir) {
        if (coyoteTimeTimer > coyoteTimeTime && jumpsCounted < 1) {
            jumpsCounted = amountOfJumps;
        }

        lastJumpPos = transform.position;
        coyoteTimeTimer = coyoteTimeTime;
        jumpBufferTimer = jumpBufferTime;
        wallJumpBufferTimer = wallJumpBufferTime;
        jumpsCounted++;

        ApplyAirLinearDrag();

        player.rb.gravityScale = fullJumpFallMultiplier;

        player.rb.velocity = new Vector2(player.rb.velocity.x, 0f); //set y velocity to 0
        float jumpForce;

        player.pCollision.StartCoroutine(player.pCollision.DisableWallRay());

        jumpForce = Mathf.Sqrt((_jumpHeight + .5f) * -2f * (Physics.gravity.y * player.rb.gravityScale));
        player.rb.AddForce(_dir * jumpForce, ForceMode2D.Impulse);
    }
    void FrogJump() {
        if (Input.GetKey(KeyCode.Space)) {
            if (frogJumpHeightMultiplier < frogJumpMaxMulti) {
                frogJumpHeightMultiplier += Time.fixedDeltaTime * frogJumpMaxMulti / frogJumpTimeToMaxForce;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            frogJump = false;
            Jump(jumpHeight * frogJumpHeightMultiplier, Vector2.up);
            frogJumpHeightMultiplier = 0;
        }
    }

    /// <summary>
    /// Applies the ground friction based on wether the player is moving or giving no horizontal inputs
    /// </summary>
    private void ApplyGroundLinearDrag() {
        if (Mathf.Abs(m_HorizontalDir) < .4f || m_ChangingDir) {
            player.rb.drag = groundLinDrag;
        }
        else {
            player.rb.drag = 0f;
        }
    }
    /// <summary>
    /// Applies the air resistance when the player is jumping
    /// </summary>
    private void ApplyAirLinearDrag() {
        player.rb.drag = airLinDrag;
    }
    /// <summary>
    /// Applies the fall gravity based on the players jump height and input
    /// </summary>
    private void ApplyFallGravity() {
        if (!player.grapplingScript.isActiveAndEnabled) {
            if (player.rb.velocity.y < 0f || transform.position.y - lastJumpPos.y > jumpHeight) {
                player.rb.gravityScale = fullJumpFallMultiplier;
            }
            else if (player.rb.velocity.y > 0f && !Input.GetButton("Jump")) {
                player.rb.gravityScale = halfJumpFallMultiplier;
            }
            else {
                player.rb.gravityScale = 1f;
            }
        }
        else {
            if (player.rb.velocity.y < 0f || transform.position.y - lastJumpPos.y > frogJumpHeight) {
                player.rb.gravityScale = fullJumpFallMultiplier;
            }
            else if (player.rb.velocity.y > 0f && !Input.GetButton("Jump")) {
                player.rb.gravityScale = halfJumpFallMultiplier;
            }
            else {
                player.rb.gravityScale = 1f;
            }
        }
    }
    private void ApplyWallHangGravity() {
        player.rb.gravityScale = onWallGravityMultiplier;
        player.rb.velocity = new Vector2(0, 0f); //set y velocity to 0
    }
}
