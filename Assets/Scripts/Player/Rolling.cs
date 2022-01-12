using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rolling : MonoBehaviour
{
    Rigidbody2D rb;

    Collider2D boxCol;
    Collider2D circleCol;

    [SerializeField]
    float speed;

    [SerializeField]
    float maxAngularVel = 300;
    private bool canRoll {
        get {
            return Input.GetAxis("Horizontal") != 0;
        }
    }
    public bool m_IsRolling {
        get {
            return this.isActiveAndEnabled && canRoll;
        }
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
        circleCol = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (canRoll)
        {
            Roll();
        }
    }

    void Roll()
    {
        rb.AddTorque(speed * maxAngularVel * 1000 * -Input.GetAxis("Horizontal") * Time.deltaTime / Mathf.Max(Mathf.Abs(rb.angularVelocity), maxAngularVel));
    }

    private void OnDisable()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        boxCol.enabled = true;
        circleCol.enabled = false;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        boxCol.enabled = false;
        circleCol.enabled = true;
        rb.freezeRotation = false;
        rb.drag = 0.5f;
    }
}
