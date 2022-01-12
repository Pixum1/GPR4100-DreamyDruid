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

    Rigidbody2D rb;

    Collider2D boxCol;
    CircleCollider2D circleCol;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCol = GetComponent<BoxCollider2D>();
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
        Destroy(circleCol);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        boxCol.enabled = true;
        circleCol.enabled = false;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        circleCol= gameObject.AddComponent<CircleCollider2D>();
        circleCol.radius = 0.9f;
        circleCol.sharedMaterial = physicsMaterial;
        boxCol.enabled = false;
        circleCol.enabled = true;
        rb.freezeRotation = false;
        rb.drag = 0.5f;
    }
}
