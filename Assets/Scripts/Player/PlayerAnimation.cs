using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private SpriteRenderer playerSprite;
    [SerializeField]
    private Sprite wallHangSprite;
    [SerializeField]
    private Sprite wallHangLookAwaySprite;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private ParticleSystem particles;
    private string currentAnimation;

    const string idleAnim = "PlayerIdle_anim";
    const string runAnim = "PlayerRun_anim";
    const string jumpAnim = "PlayerJump_anim";
    const string fallAnim = "PlayerFalling_anim";
    const string wallAnim = "PlayerOnWall_anim";
    const string wallLookAnim = "PlayerOnWallLookAway_anim";

    private bool jumping { get { return !player.pCollision.m_IsGrounded && player.rb.velocity.y > 0f; } }
    private bool falling { get { return !player.pCollision.m_IsGrounded && !jumping; } }
    private bool running { get { return player.pMovement.m_CanMove && (!jumping && !falling); } }
    private bool idle { get { return !running && !jumping && !falling; } }
    private bool wallHang { get { return player.pMovement.m_CanWallHang; } }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        if (!wallHang) {
            ApplyRotation();

            if (Input.GetButtonDown("Jump") && player.pCollision.m_IsGrounded)
                particles.Play();

            if (jumping) {
                ChangeAnimationState(jumpAnim);
            }

            if (falling)
                ChangeAnimationState(fallAnim);

            if (running && !jumping)
                ChangeAnimationState(runAnim);

            if (idle)
                ChangeAnimationState(idleAnim);
        }

        if (wallHang) {
            if ((player.pCollision.m_IsOnLeftWall && player.pMovement.m_HorizontalDir > 0f) || (player.pCollision.m_IsOnRightWall && player.pMovement.m_HorizontalDir < 0f))
                ChangeAnimationState(wallLookAnim);
            else
                ChangeAnimationState(wallAnim);
        }
    }

    private void ApplyRotation() {
        if (player.pMovement.m_HorizontalDir > 0) {
            playerSprite.flipX = false;
        }
        else if (player.pMovement.m_HorizontalDir < 0) {
            playerSprite.flipX = true;
        }
    }

    private void ChangeAnimationState(string _animationName) {
        if (currentAnimation == _animationName) return;

        animator.Play(_animationName);
        currentAnimation = _animationName;
    }
}
