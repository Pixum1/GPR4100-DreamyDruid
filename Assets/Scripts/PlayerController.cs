using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour{
    //THe Players movmenet-speed
    [SerializeField]
    private float moveSpeed = 3f;
    //variable necessary for the jumping-method
    private bool isInAir = false;

    [SerializeField]
    private float jumpCD = 2f;
    private float jumpTimer;

    void Start(){
        jumpTimer = jumpCD;
    }

    void Update(){
        jumpTimer -= Time.deltaTime;

        #region Horizontal movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * moveSpeed;
        #endregion

        #region Vertical movement. See method "Jump()"
        if (Input.GetKeyDown(KeyCode.Space) & jumpTimer <= 0){
            Jump();
        }
        #endregion
    }
    void Jump(){
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        jumpTimer = jumpCD;
    }
}
