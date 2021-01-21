using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioClip[] clips;
    public AudioSource audioSource;

    private AudioClip justPlayed;
    private bool foundNewClip = false;

    void Start()
    {
        audioSource.loop = false;
    }

    private AudioClip GetRandomClip()
    {
        return clips[Random.Range(0, clips.Length)];
    }

    void Update()
    {
        
        if (!audioSource.isPlaying)
        {
            foundNewClip = false;
            while (!foundNewClip)
            {
                audioSource.clip = GetRandomClip();
                if (justPlayed != audioSource.clip)
                    foundNewClip = true;

                justPlayed = audioSource.clip;
            }
            
            audioSource.Play();
        }
    }
}
