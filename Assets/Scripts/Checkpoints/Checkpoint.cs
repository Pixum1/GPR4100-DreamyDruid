using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public Vector2 playerPosWhenCollected;

    private void OnTriggerEnter2D(Collider2D _other) {
        if (_other.CompareTag("Player")) {
            playerPosWhenCollected = _other.GetComponent<Transform>().position;
            isActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D _other) {
        if (_other.CompareTag("Player")) {
            playerPosWhenCollected = _other.GetComponent<Transform>().position;
            isActive = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1, 0, 0, .5f);
        Gizmos.DrawCube(transform.position, transform.localScale);
    }
}
