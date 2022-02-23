using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldTransitionScreen : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    Transform player;
    [SerializeField]
    ParticleSystem playerParticles;
    float distanceToPlayer;
    Color color;

    void Start()
    {
        color = new Color(1, 1, 1, 0);
    }

    void Update()
    {
        distanceToPlayer = Vector2.Distance(spriteRenderer.transform.position, player.position);
        ColorByDistance();
        LoadScene();
        SetPlayerParticles();
    }

    void ColorByDistance()
    {
        if (distanceToPlayer < 10)
        {
            color.a = 1 / Mathf.Pow(distanceToPlayer, 1.2f);

        }
        else if (color.a != 0)
        {
            color.a = 0;
        }
        if (spriteRenderer.color != color)
        {
            spriteRenderer.color = color;
        }
    }

    void LoadScene()
    {
        if (distanceToPlayer < 1)
        {
            if(PlayerPrefs.GetInt("WorldUnlock") < SceneManager.GetActiveScene().buildIndex + 1)
            {
                PlayerPrefs.SetInt("WorldUnlock", SceneManager.GetActiveScene().buildIndex + 1);
            }
            PlayerPrefs.SetInt("DeathCount", 0);
            SceneManager.LoadSceneAsync("Main Menu");
        }
    }

    void SetPlayerParticles()
    {
        if (distanceToPlayer < 21)
        {
            if (!playerParticles.isPlaying)
                playerParticles.Play();
            playerParticles.transform.position = player.position;
        }
        else
        {
            if (playerParticles.isPlaying)
                playerParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            playerParticles.transform.position = transform.position;
        }

    }
}
