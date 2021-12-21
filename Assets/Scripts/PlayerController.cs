using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private float moveSpeed = 5f; //The Players movmenet-speed
    
    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    //private float maxSpeed = 5f;
    private float maxSpeed = 8f;

    [SerializeField]
    private bool isGrounded; //True if the player stands on an object with 'Ground' tag

    private Rigidbody2D rb;

    private float x;

    [SerializeField]
    private int maxJumps = 2;

    private int jumpsLeft;

    private Vector2 startPos;

    private bool isGliding;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }


    void Update()
    {
        x = Input.GetAxisRaw("Horizontal"); //Horizontal Axis

        //Horizontal movement
        if (x != 0 && !isGliding)
        {
            Move();
        }

        if (Input.GetButtonDown("Jump") && jumpsLeft > 0)
        {
            Jump();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (!isGrounded)
            {
                Glide();
            }
        }
        else if (isGliding)
        {
            isGliding = false;
        }

        //reset player position
        if (Input.GetKey(KeyCode.R))
        {
            transform.position = startPos;
        }
    }



    private void Move()
    {
        //Physics based movement <-- more 'realistic' no turn mid-air
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(transform.right * x * moveSpeed);
        }

        //Non physics based movment <-- mid-air turns, more direct, no acceleration
        //transform.position += (transform.right * x) * Time.deltaTime * moveSpeed;
    }

    private void Jump()
    {
        jumpsLeft -= 1;
        rb.velocity -= new Vector2(0, rb.velocity.y);
        rb.AddForce(transform.up * jumpForce * 100);
    }

    private void Glide()
    {
        if (!isGliding)
        {
            isGliding = true;
        }

        float fallSpeed = Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 0);

        rb.AddForce(-transform.up * fallSpeed * Time.deltaTime * 75);//upforce by fallspeed

        rb.AddForce(transform.up * Mathf.Abs(rb.velocity.x) * Time.deltaTime * 75);//upforce by horizontal speed

        rb.AddForce(-transform.right * x * fallSpeed * Time.deltaTime * 50);//horizontal speed by fallspeed * x input

    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        if (_other.collider.CompareTag("Ground"))
        {
            CancelInvoke("CoyoteTime");
            jumpsLeft = maxJumps;
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D _other)
    {
        if (_other.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            Invoke("CoyoteTime",0.3f);
        }
    }

    void CoyoteTime()
    {
        jumpsLeft = maxJumps - 1;
    }
}
