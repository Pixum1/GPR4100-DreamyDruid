using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour {

    [Header("Components")]
    public Rigidbody2D rb;
    [SerializeField]
    public PlayerCollision pCollision;
    [SerializeField]
    public PlayerMovement pMovement;
    [SerializeField]
    public PlayerHealth pHealth;
    [SerializeField]
    public PlayerAnimation pAnimation;

    [Header("Movement Scripts")]
    public Rolling rollingScript;
    [SerializeField]
    public Grappling grapplingScript;
    [SerializeField]
    public Gliding glidingScript;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
}
