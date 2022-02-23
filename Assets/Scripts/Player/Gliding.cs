    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D rb;
    [SerializeField]
    private float gravityScale;
    [SerializeField]
    float jumpForce;
    int jumps;
    bool jump;

    private bool m_CanGlide
    {
        get
        {
            return Input.GetAxisRaw("Ability") == 1f && !player.pCollision.m_IsGrounded;
        }
    }
    public bool m_IsGliding
    {
        get
        {
            return this.isActiveAndEnabled && m_CanGlide;
        }
    }


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
        jump = false;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate()
    {
        if (m_CanGlide)
        {
            Glide();
        }
        else if (jumps != 0)
        {
            jumps = 0;
        }
    }
    private void Glide()
    {
        ///
        /// Adjust Gravity???
        /// Because player gravity is changed based on jump/fall speed
        ///

        rb.gravityScale = gravityScale;
        float fallSpeed = Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 0);
        rb.AddForce(-transform.up * fallSpeed * Time.deltaTime * 75);//upforce by fallspeed
        rb.AddForce(transform.up * Mathf.Abs(rb.velocity.x) * Time.deltaTime * 75);//upforce by horizontal speed
        rb.AddForce(-transform.right * player.pMovement.m_HorizontalDir * fallSpeed * Time.deltaTime * 50);//horizontal speed by fallspeed * x input

        if (jump && jumps < 5)
        {
            jumps += 1;
            if (jumps >= 2)
            {
                rb.AddForce(transform.up * jumpForce / (jumps), ForceMode2D.Impulse);
            }
            jump = false;
        }
    }
}
