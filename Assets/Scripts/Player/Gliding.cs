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

    public bool m_CanGlide
    {
        get
        {
            return Input.GetAxisRaw("Ability") == 1f && !player.pCollision.m_IsGrounded;
        }
    }


    private void Awake()
    {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        jumps = 4;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump") && jumps > 0 && m_CanGlide)
        {
            BirdJump();
        }
        if (player.pCollision.m_IsGrounded)
        {
            jumps = 4;
        }
    }

    private void FixedUpdate()
    {
        if (m_CanGlide)
        {
            Glide();
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
    }

    private void BirdJump()
    {
        jumps -= 1;
        rb.AddForce(transform.up * jumpForce * Mathf.Pow(jumps, 0.85f), ForceMode2D.Impulse);
    }
}
