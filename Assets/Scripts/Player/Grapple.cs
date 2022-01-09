using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    private LineRenderer lr;

    private Vector3 mousePos;

    [SerializeField]
    LayerMask grappable;

    Rigidbody2D rb;

    [SerializeField]
    private float maxDistance = 50f;

    private DistanceJoint2D joint;

    GameObject grapplePointObj;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        grapplePointObj= GameObject.Find("GrapplePoint");
    }
    
    void Update()
    {
        CaptureMousePosition();        

        if (Input.GetMouseButtonDown(0))
        {
            StartGrapple();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            StopGrapple();
        }        
    }

    private void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple()
    {
        RaycastHit2D hit= Physics2D.Raycast(origin: transform.position, direction: mousePos- transform.position, maxDistance, grappable);
        if (hit)
        {
            
            grapplePointObj.transform.position = hit.point;
            grapplePointObj.transform.SetParent(hit.collider.gameObject.transform);

            joint = transform.gameObject.AddComponent<DistanceJoint2D>();
            joint.enableCollision = true;
            joint.maxDistanceOnly = true;
            joint.autoConfigureConnectedAnchor = false;
            joint.autoConfigureDistance = false;
            joint.distance = hit.distance;
            joint.connectedBody = rb;

            joint.connectedAnchor = grapplePointObj.transform.position;
            lr.positionCount = 2;
        }        
    }

    void StopGrapple()
    {
        lr.positionCount = 0;
        Destroy(joint);
    }
    void DrawRope()
    {
        if (joint)
        {
            lr.SetPosition(index: 0, transform.position);
            lr.SetPosition(index: 1, grapplePointObj.transform.position);
        }
    }

    void CaptureMousePosition()
    {
        mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                        Camera.main.ScreenToWorldPoint(Input.mousePosition).y,0);
    }
}
