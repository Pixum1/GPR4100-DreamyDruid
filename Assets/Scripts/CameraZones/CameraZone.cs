using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {
    public bool m_isActive { get { return isAsctive; } }

    private bool isAsctive;
    public LayerMask playerLayer;

    private void Update() {
        CheckForPlayer();
    }

    /// <summary>
    /// Checks if the player is within the bounds of the zone.
    /// </summary>
    private void CheckForPlayer() {
        Collider2D[] cols = Physics2D.OverlapBoxAll(transform.position, transform.localScale, 0f, playerLayer);
        if (cols.Length != 0) {
            isAsctive = true;
        }
        else {
            isAsctive = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, transform.localScale); //visualize zone bounds
    }
}
