using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZone : MonoBehaviour {
    public bool m_isActive { get { return isAsctive; } }

    private bool isAsctive;
    public LayerMask playerLayer;
    [SerializeField]
    private PlayerController player;

    [SerializeField]
    private float followBorderSize;
    public Vector3 followBorderDimension {
        get {
            return new Vector3(transform.localScale.x - followBorderSize, transform.localScale.y, transform.localScale.z);
        }
    }
    public Vector3 followBorderPosition {
        get {
            return new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
    }

    private void Awake() {
        player = FindObjectOfType<PlayerController>();
    }

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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(followBorderPosition, followBorderDimension);
    }
}
