using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    
    [SerializeField]
    private float moveSpeed = 3f; //The Players movmenet-speed
    [SerializeField]
    private float jumpForce = 5f;

    [SerializeField]
    private float maxSpeed = 5f;

    private bool isGrounded; //True if the player stands on an object with 'Ground' tag

    private Rigidbody2D rb;

    private float x;
    private float y;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }


    void Update(){
        x = Input.GetAxisRaw("Horizontal"); //Horizontal Axis
        y = Input.GetAxisRaw("Jump"); //Vertical 'Jump' Axis

        //Horizontal movement
        if(x != 0) {
            Move();
        }
    }

    private void FixedUpdate() {

        //Horizontal Movement
        //if(x != 0) {
        //    Move();
        //}

        //Vertical Movement
        if (y != 0 && isGrounded) {
            Jump();
        }
    }

    private void Move() {
        //Physics based movement <-- more 'realistic' no turn mid-air
        //if (x != 0 && rb.velocity.magnitude < maxSpeed) {
        //    rb.AddForce(transform.right * x * moveSpeed, ForceMode2D.Force);
        //}

        //Non physics based movment <-- mid-air turns, more direct, no acceleration
        transform.position += (transform.right * x) * Time.deltaTime * moveSpeed;
    }

    private void Jump(){
        rb.AddForce(transform.up * y * jumpForce, ForceMode2D.Impulse);
    }

    private void OnCollisionEnter2D(Collision2D _other) {
        if (_other.collider.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D _other) {
        if (_other.collider.CompareTag("Ground")) {
            isGrounded = false;
        }
    }
}
