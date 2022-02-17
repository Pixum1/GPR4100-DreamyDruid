using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    [SerializeField]
    PhysicsMaterial2D physicsMaterial;
    [SerializeField]
    float speed;
    [SerializeField]
    float duration;
    [SerializeField]
    float cooldown = 3;
    float cooldownLeft;
    [SerializeField]
    SpriteRenderer playerSpriteRenderer;
    private bool canRoll;
    public bool m_IsRolling
    {
        get
        {
            return this.isActiveAndEnabled && !canRoll;
        }
    }

    Rigidbody2D rb;

    Collider2D boxCol;
    CircleCollider2D circleCol;
    float timer;
    int flipped;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        circleCol = gameObject.AddComponent<CircleCollider2D>();
        circleCol.enabled = false;
        canRoll = true;
    }

    void Update()
    {

        if (Input.GetButtonDown("Ability"))
        {

            if (canRoll)
            {
                if (cooldownLeft <= 0)
                {
                    if (playerSpriteRenderer.flipX)
                    {
                        flipped = 1;
                    }
                    else
                    {
                        flipped = -1;
                    }
                    StartCoroutine(RollProperties());
                }
            }
            else
            {
                StopCoroutine(RollProperties());
                circleCol.enabled = false;
                transform.rotation = Quaternion.Euler(0, 0, 0);
                boxCol.enabled = true;
                rb.freezeRotation = true;
                canRoll = true;
                timer = 0;
                cooldownLeft = cooldown;
            }
        }

        if (Input.GetButtonUp("Ability"))
        {
            StopCoroutine(RollProperties());
            circleCol.enabled = false;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            boxCol.enabled = true;
            rb.freezeRotation = true;
            canRoll = true;
            timer = 0;
            cooldownLeft = cooldown;
        }
    }


    private void FixedUpdate()
    {
        Roll();
    }

    void Roll()
    {
        if (m_IsRolling == true)
        {
            timer += Time.fixedDeltaTime;
            rb.AddTorque(speed * 100 * Mathf.Min(timer + 1,1.5f) * flipped * Time.fixedDeltaTime);
            rb.AddForce(-transform.right * speed * 10 * Mathf.Min(timer + 1,1.5f) * flipped * Time.fixedDeltaTime);
        }
        else if (cooldownLeft > 0)
        {
            cooldownLeft -= Time.fixedDeltaTime;
        }
    }


    IEnumerator RollProperties()
    {
        canRoll = false;
        circleCol.enabled = true;
        circleCol.radius = 0.45f;
        circleCol.sharedMaterial = physicsMaterial;
        boxCol.enabled = false;
        circleCol.enabled = true;
        rb.freezeRotation = false;
        rb.drag = 0.5f;
        yield return new WaitForSeconds(duration);
        circleCol.enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        boxCol.enabled = true;
        rb.freezeRotation = true;
        canRoll = true;
        timer = 0;
        cooldownLeft = cooldown;
    }

    private void OnDisable()
    {
        circleCol.enabled = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        boxCol.enabled = true;
        rb.freezeRotation = true;
        canRoll = true;
    }

    private void OnEnable()
    {
        canRoll = true;
    }
}
