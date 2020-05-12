using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipPlayer : MonoBehaviour
{
    public AudioSource audioSource;


    public void PlayStop(bool play, bool oneShot = false)
    {
        if (play && !audioSource.isPlaying)
        {
            if (oneShot)
                audioSource.PlayOneShot(audioSource.clip);
            else
                audioSource.Play();
        }

        if (!play && audioSource.isPlaying)
            audioSource.Stop();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
}
