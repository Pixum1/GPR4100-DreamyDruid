﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private float dragValue;
    [SerializeField]
    private LayerMask grappable;
    private LineRenderer lr;
    private Vector3 mousePos;
    [SerializeField]
    SpriteRenderer playerSprite;
    [SerializeField]
    private Material ropeMaterial;
    [SerializeField]
    private Color ropeColor;
    [SerializeField]
    private float maxDistance = 50f;
    [SerializeField]
    private float ropeSegmentLength = 0.25f;
    [SerializeField]
    private float initialDrawDelay = 0.2f;
    [SerializeField]
    private float ropeWidth = 0.2f;
    public bool m_IsGrappling
    {
        get
        {
            return this.isActiveAndEnabled && grapplingAnchor != null;
        }
    }
    private List<RopeSegment> ropeSegments = new List<RopeSegment>();
    private DistanceJoint2D distJoint;
    private SpringJoint2D springJoint;
    private GameObject grapplingAnchor;
    private GameObject playerAnchor;
    private int segmentAmount;
    private bool isGrappling;
    private float drawDelay;
    bool isAddingMomentum;
    private void Awake()
    {
        drawDelay = initialDrawDelay;
        rb = GetComponent<Rigidbody2D>();
    }
    //private void Update()
    //{
    //    if (isAddingMomentum)
    //    {
    //        AddMomentum();
    //    }
    //    if (Input.GetButtonDown("Ability"))
    //    {
    //        CaptureMousePosition();
    //        StartGrapple();
    //    }
    //    else if (Input.GetButtonUp("Ability"))
    //    {
    //        StopGrapple();
    //    }
    //}
    //private void FixedUpdate()
    //{
    //    if (isGrappling)
    //    {
    //        SimulateRope();
    //    }
    //}
    //private void LateUpdate()
    //{
    //    if (isGrappling)
    //    {
    //        if (drawDelay > 0)
    //        {
    //            drawDelay -= Time.deltaTime;
    //        }
    //        else
    //        {
    //            DrawRope();
    //        }
    //    }
    //}
    private void OnDisable()
    {
        StopGrapple();
    }
    public struct RopeSegment
    {
        public Vector2 posNow;
        public Vector2 posOld;

        public RopeSegment(Vector2 pos)
        {
            posNow = pos;
            posOld = pos;
        }
    }
    private void CaptureMousePosition()
    {
        mousePos = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x,
                                        Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0);
    }
    private void StartGrapple()
    {
        if (isGrappling)
        {
            StopGrapple();
        }
        RaycastHit2D hit = Physics2D.Raycast(origin: transform.position, direction: mousePos - transform.position, maxDistance, grappable);
        if (hit)
        {
            rb.drag = dragValue; //adjust drag
            //rb.AddForce(Vector2.right * Mathf.Clamp(rb.velocity.x, -1 * 10f, 1 * 10f) * 5, ForceMode2D.Impulse); <- Kümmer sich lars drum

            lr = gameObject.AddComponent<LineRenderer>();
            segmentAmount = Mathf.RoundToInt(hit.distance / (ropeSegmentLength + ropeSegmentLength * 0.15f));
            grapplingAnchor = new GameObject("GrapplingAnchor");
            playerAnchor = new GameObject("PlayerAnchor");
            CreateRope();
            isGrappling = true;

            playerAnchor.transform.position = transform.position;
            playerAnchor.transform.SetParent(transform);

            grapplingAnchor.transform.position = hit.point;
            grapplingAnchor.transform.SetParent(hit.collider.gameObject.transform);

            distJoint = playerAnchor.AddComponent<DistanceJoint2D>();
            distJoint.maxDistanceOnly = true;
            distJoint.autoConfigureConnectedAnchor = false;
            distJoint.autoConfigureDistance = false;
            distJoint.distance = hit.distance;
            distJoint.connectedAnchor = grapplingAnchor.transform.position;

            springJoint = playerAnchor.transform.gameObject.AddComponent<SpringJoint2D>();
            springJoint.connectedBody = transform.GetComponent<Rigidbody2D>();
            springJoint.autoConfigureDistance = false;
            springJoint.distance = 0f;
            springJoint.frequency = 5f;
            springJoint.dampingRatio = 1f;
            StartCoroutine("AddMomentumIE");
        }
    }
    IEnumerator AddMomentumIE()
    {
        isAddingMomentum = true;
        yield return new WaitForSeconds(0.5f);
        isAddingMomentum = false;
    }
    void AddMomentum()
    {
        if (grapplingAnchor != null)
        {
            float playerAnchorDistX = grapplingAnchor.transform.position.x - transform.position.x;
            float playerAnchorDistY = grapplingAnchor.transform.position.y - transform.position.y;
            float f = .1f;
            rb.AddForce(new Vector2(Mathf.Clamp(f * playerAnchorDistX, -maxDistance, maxDistance), -Mathf.Clamp(f * playerAnchorDistY, -maxDistance * .25f, maxDistance * .25f)), ForceMode2D.Impulse);
        }
    }
    private void CreateRope()
    {
        Vector3 ropeStartPoint = grapplingAnchor.transform.position;

        for (int i = 0; i < segmentAmount; i++)
        {
            ropeSegments.Add(new RopeSegment(ropeStartPoint));
            ropeStartPoint.y -= ropeSegmentLength;
        }
    }
    private void SimulateRope()
    {
        Vector2 forceGravity = new Vector2(0f, -0.5f);

        RopeSegment lastSegment = ropeSegments[segmentAmount - 1];

        lastSegment.posNow = transform.position;
        lastSegment.posOld = lastSegment.posNow;

        for (int i = 0; i < segmentAmount; i++)
        {
            RopeSegment firstSegment = ropeSegments[i];
            Vector2 velocity = firstSegment.posNow - firstSegment.posOld;
            firstSegment.posOld = firstSegment.posNow;
            firstSegment.posNow += velocity;
            firstSegment.posNow += forceGravity * Time.deltaTime;
            ropeSegments[i] = firstSegment;
        }

        for (int i = 0; i < 50; i++)
        {
            ApplyRopeConstraint();
        }
    }
    private void ApplyRopeConstraint()
    {
        RopeSegment firstSegment = ropeSegments[0];
        firstSegment.posNow = grapplingAnchor.transform.position;
        ropeSegments[0] = firstSegment;

        RopeSegment lastSegment = ropeSegments[segmentAmount - 1];
        lastSegment.posNow = playerAnchor.transform.position;
        ropeSegments[segmentAmount - 1] = lastSegment;

        for (int i = 0; i < segmentAmount - 1; i++)
        {
            RopeSegment firstSeg = ropeSegments[i];
            RopeSegment secondSeg = ropeSegments[i + 1];

            float dist = (firstSeg.posNow - secondSeg.posNow).magnitude;
            float error = dist - ropeSegmentLength;

            Vector2 changeDir = (firstSeg.posNow - secondSeg.posNow).normalized;
            Vector2 changeAmount = changeDir * error;

            if (i != 0)
            {
                firstSeg.posNow -= changeAmount * 0.75f;
                ropeSegments[i] = firstSeg;
                secondSeg.posNow += changeAmount * 0.75f;
                ropeSegments[i + 1] = secondSeg;
            }
            else
            {
                secondSeg.posNow += changeAmount;
                ropeSegments[i + 1] = secondSeg;
            }
        }
    }
    private void DrawRope()
    {
        if (distJoint)
        {
            lr.numCapVertices = 3;
            lr.numCornerVertices = 1;
            lr.startWidth = ropeWidth;
            lr.endWidth = ropeWidth;
            lr.material = ropeMaterial;
            lr.startColor = ropeColor;
            lr.endColor = ropeColor;

            Vector3[] ropePositions = new Vector3[segmentAmount];

            for (int i = 0; i < segmentAmount - 1; i++)
            {
                ropePositions[i] = ropeSegments[i].posNow;
            }
            ropePositions[segmentAmount - 1] = playerAnchor.transform.position;
            lr.positionCount = ropePositions.Length;
            lr.SetPositions(ropePositions);

        }
    }
    private void StopGrapple()
    {
        isGrappling = false;
        drawDelay = initialDrawDelay;
        Destroy(lr);
        Destroy(grapplingAnchor);
        Destroy(playerAnchor);
        Destroy(springJoint);
        Destroy(distJoint);
    }
}
