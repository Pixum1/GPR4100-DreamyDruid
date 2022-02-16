using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragileFloor : MonoBehaviour
{
    public Vector3 startPos;
    [SerializeField]
    private float timeToFall = .2f;
    [SerializeField]
    private float fallSpeed = 2f;

    private bool called;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(Fall());
            called = true;
        }
    }

    private IEnumerator Fall()
    {
        if (!called)
        {
            Vector3 endPos = new Vector3(startPos.x, startPos.y - 5f);
            yield return new WaitForSeconds(timeToFall);
            while (transform.position.y - endPos.y > 0f)
            {
                transform.position -= Vector3.up * fallSpeed * Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
