using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public bool isActive;

    private void OnTriggerEnter2D(Collider2D _other) {
        if (_other.CompareTag("Player")) {
            isActive = true;
        }
    }
    private void OnTriggerExit2D(Collider2D _other) {
        if (_other.CompareTag("Player")) {
            isActive = false;
        }
    }
}
