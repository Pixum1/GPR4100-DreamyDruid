using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField]
    private Transform platform;
    [SerializeField]
    private Transform leftWaypoint;
    [SerializeField]
    private Transform rightWaypoint;
    [SerializeField]
    private float speed;

    private Vector2 destLeftPos;
    private Vector2 destRightPos;

    private bool movingLeft;
    private bool movingRight;

    private void Awake() {
        destLeftPos = new Vector2(leftWaypoint.position.x + platform.GetComponent<Collider2D>().bounds.extents.x, leftWaypoint.position.y);
        destRightPos = new Vector2(rightWaypoint.position.x - platform.GetComponent<Collider2D>().bounds.extents.x, rightWaypoint.position.y);
    }

    private void Start() {
        StartCoroutine(Move());
    }

    private IEnumerator Move() {
        while(Vector2.Distance(platform.position, destLeftPos) > 0) {
            movingLeft = true;
            movingRight = false;
            platform.position = Vector2.MoveTowards(platform.position, destLeftPos, speed * Time.deltaTime);
            yield return null;
        }
        while(Vector2.Distance(platform.position, destRightPos) > 0) {
            movingLeft=false;
            movingRight = true;
            platform.position = Vector2.MoveTowards(platform.position, destRightPos, speed * Time.deltaTime);
            yield return null;
        }
        StartCoroutine(Move());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.transform.SetParent(null);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(leftWaypoint.position, leftWaypoint.localScale);
        Gizmos.DrawWireCube(rightWaypoint.position, rightWaypoint.localScale);
    }
}
