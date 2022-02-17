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
    AudioClip nightmareStart;
    [SerializeField]
    AudioClip nightmare;

    [SerializeField]
    AudioSource musicSource;
    [SerializeField]
    AudioClip defaultMusic;
    [SerializeField]
    AudioClip nightmareMusic;

    [SerializeField]
    AudioSource crowSource;
    [SerializeField]
    AudioClip crowIdle;
    [SerializeField]
    AudioClip crowFly;

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
            StartCoroutine(NightmareSound());
        }
        else if (nightmareScript.active == false && nightmareSource.isPlaying)
        {
            nightmareSource.Stop();
        }


    }

    IEnumerator NightmareSound()
    {
        crowSource.PlayOneShot(crowFly);
        nightmareSource.PlayOneShot(nightmareStart);
        yield return new WaitForSeconds(4);
        musicSource.Stop();
        musicSource.clip = nightmareMusic;
        musicSource.Play();
        yield return new WaitForSeconds(2);
        nightmareSource.Play();
    }
}
