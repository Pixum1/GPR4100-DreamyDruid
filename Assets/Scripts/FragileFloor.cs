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


    private void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            StartCoroutine(Fall());
        }
    }

    private IEnumerator Fall()
    {
        Vector3 endPos = new Vector3(startPos.x, startPos.y - 5f);
        yield return new WaitForSeconds(timeToFall);
        while (transform.position.y - endPos.y > 0f)
        {
            transform.position -= Vector3.up * fallSpeed * Time.deltaTime;
            yield return null;
        }
        SetState(false);
        yield return new WaitForSeconds(5f);
        SetState(true);
        transform.position = startPos;
    }
    public void SetState(bool _state)
    {
        this.GetComponent<SpriteRenderer>().enabled = _state;
        this.GetComponent<Collider2D>().enabled = _state;
    }
}
