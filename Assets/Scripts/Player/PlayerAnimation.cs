using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour {
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private SpriteRenderer playerSprite;

    void Update() {
        ApplyRotation();
    }

    private void ApplyRotation() {
        if (player.pMovement.m_HorizontalDir > 0) {
            playerSprite.flipX = false;
        }
        else if (player.pMovement.m_HorizontalDir < 0) {
            playerSprite.flipX = true;
        }
    }
}
