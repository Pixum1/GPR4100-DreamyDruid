using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float startPos;
    [SerializeField]
    private float parallaxEffect;
    [SerializeField]
    private float autoScrollEffect;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    private void Update()
    {
        float temp = Camera.main.transform.position.x * (1 - parallaxEffect);
        float distance = Camera.main.transform.position.x * parallaxEffect;

        if (autoScrollEffect == 0)
        {
            transform.position = new Vector2(startPos + distance, transform.position.y);
        }
        else
        {
            transform.position += Vector3.right * autoScrollEffect;

            if(transform.position.x > startPos + length)
            {
                transform.position = new Vector2(startPos, transform.position.y);
            }
        }

        if (temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }
}
