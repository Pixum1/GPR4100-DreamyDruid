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

    private SpriteRenderer sp;
    private Nightmare nm;
    [SerializeField]
    private Color nmColor;
    [SerializeField]
    private float colorChangeSpeed;

    private void Start()
    {
        nm = FindObjectOfType<Nightmare>();
        sp=GetComponent<SpriteRenderer>();
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }
    private void Update()
    {
        if (nm.active)
        {
            if (sp.color.g >= nmColor.g && sp.color.b >= nmColor.b)
            {
                sp.color = new Color(sp.color.r, sp.color.g - (Time.deltaTime * colorChangeSpeed), sp.color.b - (Time.deltaTime * colorChangeSpeed));
            }
            foreach (Transform child in transform)
            {
                SpriteRenderer childSp = child.GetComponent<SpriteRenderer>();
                if (childSp.color.g >= nmColor.g && childSp.color.b >= nmColor.b)
                {
                    childSp.color = new Color(childSp.color.r, childSp.color.g - (Time.deltaTime * colorChangeSpeed), childSp.color.b - (Time.deltaTime * colorChangeSpeed));
                }
            }
        }

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
            else if(transform.position.x < startPos - length)
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
