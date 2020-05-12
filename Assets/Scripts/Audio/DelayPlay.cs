using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayPlay : MonoBehaviour
{
    public AudioSource audioSource;
    public float delay = 4.3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (delay > 0f)
        {
            delay -= Time.deltaTime;
            if (delay <= 0f)
                audioSource.Play();
        }
    }
}
