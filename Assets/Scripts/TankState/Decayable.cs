using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Decayable : MonoBehaviour
{

    public float decayState = 1f;
    public float decayStepAmount = 0.25f;
    public float decayTimer = 0f;
    public float decayCountdown = 5f;
    private void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        decayTimer -= Time.deltaTime;

        if (decayTimer <= 0f)
        {
            decayTimer = decayCountdown;
            decayState -= decayStepAmount;
            if (decayState <= 0f)
            {
                BeforeDestroying(this);
                //tankDirtiness.Add(dirtiness);
                Destroy(this);
            }
        }
    }

    public virtual void BeforeDestroying(Decayable item)
    {

    }

}
