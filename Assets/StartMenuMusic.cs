using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenuMusic : MonoBehaviour
{
    AudioSource audio;
    float vol;
    [SerializeField]
    float fadeInSpeed;
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        vol = audio.volume;
        audio.volume = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (audio.volume < vol)
        {
            audio.volume += Time.deltaTime*fadeInSpeed;
        }
        else
        {
            audio.volume = vol;
        }
    }
}
