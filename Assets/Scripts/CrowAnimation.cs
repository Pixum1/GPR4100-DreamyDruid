using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrowAnimation : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer crowSprite;
    [SerializeField]
    private Sprite crowIdleSprite;
    [SerializeField]
    private Sprite crowFlySprite;

    [SerializeField]
    private Animator animator;
    [SerializeField]
    float speed;
    public bool active;
    private string currentAnimation;
    private string idleAnim = "CrowIdle";
    private string flyAnim = "CrowFly";

    void Update()
    {
        if (active)
        {
            ChangeAnimationState(flyAnim);
        }
        else
        {
            ChangeAnimationState(idleAnim);
        }
    }

    private void FixedUpdate()
    {
        if (active)
            transform.position += new Vector3(-0.5f * speed * Time.fixedDeltaTime, 1 * speed * Time.fixedDeltaTime, 0);
    }

    private void ChangeAnimationState(string _animationName)
    {
        if (currentAnimation == _animationName) return;

        animator.Play(_animationName);
        currentAnimation = _animationName;
    }

}
