using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private SpriteRenderer playerSprite;

    [SerializeField]
    private Animator animator;
    private string currentAnimation;

    const string idleAnim = "PlayerIdle_anim";
    const string runAnim = "PlayerRun_anim";
    const string jumpAnim = "PlayerJump_anim";
    const string fallAnim = "PlayerFalling_anim";

    private bool jumping { get { return !player.pCollision.m_IsGrounded && player.rb.velocity.y > 0f; } }
    private bool falling { get { return !player.pCollision.m_IsGrounded && !jumping; } }
    private bool running { get { return player.pMovement.m_CanMove && (!jumping && !falling); } }
    private bool idle { get { return !running && !jumping && !falling; } }

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        ApplyRotation();

        if (jumping) {
            ChangeAnimationState(jumpAnim);
        }
        if (falling) {
            ChangeAnimationState(fallAnim);
        }
        if (running && !jumping) {
            ChangeAnimationState(runAnim);
        }
        if (idle) {
            ChangeAnimationState(idleAnim);
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
