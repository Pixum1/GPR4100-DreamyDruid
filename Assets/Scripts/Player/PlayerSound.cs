using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    [SerializeField]
    AudioSource walkSource;
    [SerializeField]
    AudioClip walk;

    [SerializeField]
    AudioSource rollSource;
    [SerializeField]
    AudioClip roll;

    [SerializeField]
    AudioSource jumpSource;
    [SerializeField]
    AudioClip jump;

    [SerializeField]
    AudioSource nightmareSource;
    [SerializeField]
    AudioClip nightmare;

    [SerializeField]
    Rigidbody2D rb;

    [SerializeField]
    PlayerCollision playerCollision;

    [SerializeField]
    PlayerMovement playerMovement;

    [SerializeField]
    Rolling rollingScript;

    [SerializeField]
    Nightmare nightmareScript;

    void Start()
    {

    }

    void Update()
    {
        if (!rollingScript.m_IsRolling)
        {
            if (rollSource.isPlaying)
            {
                rollSource.Stop();
            }
            if (playerCollision.m_IsGrounded)
            {
                if (!walkSource.isPlaying)
                    walkSource.PlayOneShot(walk);
                walkSource.pitch = Mathf.RoundToInt(Mathf.Clamp(rb.velocity.magnitude * 0.5f, 0, 10));
            }
            else
            {
                walkSource.Stop();
            }

            if (playerMovement.PlayerJumped)
            {
                jumpSource.PlayOneShot(jump);
                playerMovement.PlayerJumped = false;
            }
        }
        else if (rollingScript.isActiveAndEnabled)
        {
            if (!rollSource.isPlaying && Mathf.Abs(rb.angularVelocity) > 10f)
            {
                rollSource.PlayOneShot(roll);
            }
            else if (Mathf.Abs(rb.angularVelocity) < 10f)
            {
                rollSource.Stop();
            }
        }

        if (nightmareScript.active && !nightmareSource.isPlaying)
        {
            nightmareSource.PlayOneShot(nightmare);
        }
        else if (nightmareScript.active == false && nightmareSource.isPlaying)
        {
            nightmareSource.Stop();
        }


    }
}
