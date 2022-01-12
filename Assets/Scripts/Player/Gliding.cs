using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gliding : MonoBehaviour
{
    private PlayerController player;
    private Rigidbody2D rb;
    [SerializeField]
    private float gravityScale;

    private bool m_CanGlide {
        get {
            return Input.GetKey(KeyCode.LeftControl) && !player.m_IsGrounded;
        }
    }
    public bool m_IsGliding { 
        get {
            return this.isActiveAndEnabled && m_CanGlide;
        } 
    }


    private void Awake() {
        player = FindObjectOfType<PlayerController>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        if (m_CanGlide) {
            Glide();
        }
    }
    private void Glide() {

        ///
        /// Adjust Gravity???
        /// Because player gravity is changed based on jump/fall speed
        ///
        rb.gravityScale = gravityScale;

        float fallSpeed = Mathf.Clamp(rb.velocity.y, Mathf.NegativeInfinity, 0);

        rb.AddForce(-transform.up * fallSpeed * Time.deltaTime * 75);//upforce by fallspeed

        rb.AddForce(transform.up * Mathf.Abs(rb.velocity.x) * Time.deltaTime * 75);//upforce by horizontal speed

        rb.AddForce(-transform.right * player.m_HorizontalDir * fallSpeed * Time.deltaTime * 50);//horizontal speed by fallspeed * x input
    }
}
