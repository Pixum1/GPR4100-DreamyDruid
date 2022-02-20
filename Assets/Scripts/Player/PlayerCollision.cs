using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    [SerializeField]
    private PlayerController player;

    [Header("Layer Masks")]
    [SerializeField]
    [Tooltip("The layer that marks the player as 'grounded' when standing on")]
    private LayerMask groundLayer;
    [SerializeField]
    [Tooltip("The 'corner correction' layer")]
    private LayerMask CCLayer;
    [SerializeField]
    private LayerMask wallHangLayer;

    [Header("Collision Check")]
    [SerializeField]
    private float groundRayLength = 1f;
    [SerializeField]
    private Vector3 groundRayOffset;
    [SerializeField]
    private Vector3 groundRayVerticalOffset;
    [SerializeField]
    private float wallRayLength = 1f;
    [SerializeField]
    private Vector3 wallRayOffset;
    private float wallRaySave;
    

    [Header("Corner Correction")]
    [SerializeField, Tooltip("The distance at which the corner correction should be calculated (Higher numbers = faster detection of corners)")]
    private float CCRayLength = 1f;
    [SerializeField, Tooltip("The outer offset of the corner correction ray")]
    private Vector3 CCedgeRayOffset;
    [SerializeField, Tooltip("The inner offset of the corner correction ray")]
    private Vector3 CCinnerRayOffset;

    [HideInInspector]
    public bool m_IsGrounded;
    [HideInInspector]
    public bool m_IsOnLeftWall;
    [HideInInspector]
    public bool m_IsOnRightWall;
    [HideInInspector]
    public bool m_CanCornerCorrect;
    [HideInInspector]

    private void Start() {
        wallRaySave = wallRayLength;
    }

    private void Update() {
        m_IsGrounded = Physics2D.Raycast(transform.position + groundRayOffset + groundRayVerticalOffset, Vector2.down, groundRayLength, groundLayer)
                   || Physics2D.Raycast(transform.position - groundRayOffset + groundRayVerticalOffset, Vector2.down, groundRayLength, groundLayer);

        m_IsOnLeftWall = Physics2D.Raycast(transform.position + wallRayOffset, -Vector3.right, wallRayLength, wallHangLayer)
                   || Physics2D.Raycast(transform.position - wallRayOffset, -Vector3.right, wallRayLength, wallHangLayer);

        m_IsOnRightWall = Physics2D.Raycast(transform.position + wallRayOffset, Vector3.right, wallRayLength, wallHangLayer)
                   || Physics2D.Raycast(transform.position - wallRayOffset, Vector3.right, wallRayLength, wallHangLayer);

        m_CanCornerCorrect = Physics2D.Raycast(transform.position + CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer)
                   && !Physics2D.Raycast(transform.position + CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer)
                   || Physics2D.Raycast(transform.position - CCedgeRayOffset, Vector2.up, CCRayLength, CCLayer)
                   && !Physics2D.Raycast(transform.position - CCinnerRayOffset, Vector2.up, CCRayLength, CCLayer);

        if (m_CanCornerCorrect) {
            CheckForCornerCorrection(player.rb.velocity.y);
        }
    }

    public IEnumerator DisableWallRay() {
        wallRayLength = 0f;
        yield return new WaitForEndOfFrame();
        wallRayLength = wallRaySave;
    }

    #region Corner Correction
    /// <summary>
    /// Applies the corner correction when the player hits a block above him in a very specific angle
    /// </summary>
    /// <param name="_verticalVelocity">The vertical velocity of the player</param>
    public void CheckForCornerCorrection(float _verticalVelocity) {
        //Check for corner on the left
        RaycastHit2D hit = Physics2D.Raycast(transform.position - CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.left, CCRayLength, CCLayer);
        if (hit.collider != null) {
            int dir = 1; //right
            ApplyCornerCorrection(_verticalVelocity, CalculateCornerCorrection(hit, dir), dir);
            return;
        }

        //Check for corner on the right
        hit = Physics2D.Raycast(transform.position + CCinnerRayOffset + Vector3.up * CCRayLength, Vector3.right, CCRayLength, CCLayer);
        if (hit.collider != null) {
            int dir = -1; //left
            ApplyCornerCorrection(_verticalVelocity, CalculateCornerCorrection(hit, dir), dir);
        }
    }
    /// <summary>
    /// Calculate the position the player should be put in after the corner correction
    /// </summary>
    /// <returns>New Position</returns>
    private float CalculateCornerCorrection(RaycastHit2D _hit, int _direction) {
        return Vector3.Distance(new Vector3(_hit.point.x, transform.position.y, 0f) + Vector3.up * CCRayLength,
                                            transform.position - (Mathf.Clamp(_direction, -1, 1) * CCedgeRayOffset) + Vector3.up * CCRayLength);
    }
    /// <summary>
    /// Moves the player to the new position and remain his vertical velocity
    /// </summary>
    private void ApplyCornerCorrection(float _verticalVelocity, float _newPos, int _direction) 
    {
        if (!player.rollingScript.m_IsRolling)
        {

            transform.position = new Vector3(transform.position.x + (Mathf.Clamp(_direction, -1, 1) * _newPos), transform.position.y, transform.position.z);

            player.rb.velocity = new Vector2(player.rb.velocity.x, _verticalVelocity);
        }
    }
    #endregion

    #region Debugging
    private void OnDrawGizmos() {
        DrawGroundRays();
        DrawCornerCheckRays();
        DrawCornerDistanceRays();
        DrawWallDistanceRays();
    }
    private void DrawGroundRays() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + groundRayOffset + groundRayVerticalOffset, transform.position + groundRayOffset + groundRayVerticalOffset + Vector3.down * groundRayLength);
        Gizmos.DrawLine(transform.position - groundRayOffset + groundRayVerticalOffset, transform.position - groundRayOffset + groundRayVerticalOffset + Vector3.down * groundRayLength);
    }
    private void DrawCornerCheckRays() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + CCedgeRayOffset, transform.position + CCedgeRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position - CCedgeRayOffset, transform.position - CCedgeRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position + CCinnerRayOffset, transform.position + CCinnerRayOffset + Vector3.up * CCRayLength);
        Gizmos.DrawLine(transform.position - CCinnerRayOffset, transform.position - CCinnerRayOffset + Vector3.up * CCRayLength);
    }
    private void DrawCornerDistanceRays() {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position - CCinnerRayOffset + Vector3.up * CCRayLength,
                        transform.position - CCinnerRayOffset + Vector3.up * CCRayLength + Vector3.left * CCRayLength);
        Gizmos.DrawLine(transform.position + CCinnerRayOffset + Vector3.up * CCRayLength,
                        transform.position + CCinnerRayOffset + Vector3.up * CCRayLength + Vector3.right * CCRayLength);
    }
    private void DrawWallDistanceRays() {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + wallRayOffset, transform.position + wallRayOffset + Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position - wallRayOffset, transform.position - wallRayOffset + Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position + wallRayOffset, transform.position + wallRayOffset - Vector3.right * wallRayLength);
        Gizmos.DrawLine(transform.position - wallRayOffset, transform.position - wallRayOffset - Vector3.right * wallRayLength);
    }
    #endregion
}
