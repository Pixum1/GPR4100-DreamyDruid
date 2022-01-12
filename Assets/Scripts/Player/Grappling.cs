using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour {
    private LineRenderer lr;

    private Vector3 mousePos;

    [SerializeField]
    LayerMask grappable;

    Rigidbody2D rb;

    [SerializeField]
    private float maxDistance = 50f;

    private DistanceJoint2D joint;

    GameObject grapplingPoint;

    public bool m_IsGrappling {
        get {
            return this.isActiveAndEnabled && grapplingPoint != null;
        }
    }

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CaptureMousePosition();
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }        
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        grapplingPoint = new GameObject("GrapplingPoint");
        RaycastHit2D hit= Physics2D.Raycast(origin: transform.position, direction: mousePos- transform.position, maxDistance, grappable);
        if (hit)
        {
            grapplingPoint.transform.position = hit.point;
            grapplingPoint.transform.SetParent(hit.collider.gameObject.transform);

            joint = transform.gameObject.AddComponent<DistanceJoint2D>();
            joint.enableCollision = true;
            joint.maxDistanceOnly = true;
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = false;
            joint.distance = hit.distance;
            joint.connectedBody = rb;
            joint.connectedAnchor = grapplingPoint.transform.position;

            lr.positionCount = 2;
        }        
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(grapplingPoint);
        Destroy(joint);
    }
    void DrawRope()
    {
        if (joint)
        {
            lr.SetPosition(index: 0, transform.position);
            lr.SetPosition(index: 1, grapplingPoint.transform.position);
        }
    }

    void CaptureMousePosition()
    {
        mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                        Camera.main.ScreenToWorldPoint(Input.mousePosition).y,0);
    }

    private void OnDisable()
    {
        StopGrapple();
    }
}
