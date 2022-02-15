using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    private float length;
    private float startPos;
    [SerializeField]
    private float parallaxEffect;

    private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    private void Update()
    {
        float temp = Camera.main.transform.position.x * (1 - parallaxEffect);
        float distance = Camera.main.transform.position.x * parallaxEffect;

        transform.position = new Vector2(startPos + distance, transform.position.y);

        if(temp > startPos + length)
        {
            startPos += length;
        }
        else if(temp < startPos - length)
        {
            startPos -= length;
        }
    }
}
