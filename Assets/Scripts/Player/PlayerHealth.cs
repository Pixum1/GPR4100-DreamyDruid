using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    public Action e_PlayerDied;

    private void Update() {
        if (e_PlayerDied == null)
            Debug.LogWarning("Null Reference");

        if (Input.GetKey(KeyCode.R))
            e_PlayerDied?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D _other) {
        if (_other.CompareTag("Obstacle")) {
            e_PlayerDied?.Invoke();
        }
    }
}
