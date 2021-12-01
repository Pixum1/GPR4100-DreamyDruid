using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //THe Players movmenet-speed
    public float moveSpeed = 3f;
    //variable necessary for the jumping-method
    bool isInAir = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //Horizontal movement
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), 0f, 0f);
        transform.position += movement * Time.deltaTime * moveSpeed;

        //Vertical movement. See method "Jump()"
        if (Input.GetKeyDown(KeyCode.Space) & isInAir == false)
        {
            isInAir = true;
            Jump();
            isInAir = false;
        }
    }
    void Jump()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, 5f), ForceMode2D.Impulse);
        //Starts the jumping cooldown
        StartCoroutine(JumpCooldown());
    }
    //time the player has to wait between jumps
    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(2);
    }

}
