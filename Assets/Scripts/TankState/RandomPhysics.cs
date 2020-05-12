using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomPhysics : MonoBehaviour
{
    public float maxRotationForce = 0.1f;
    public float maxLateralForce = 0.1f;
    public float minTimeBetweenUpdates = 0.25f;
    public float maxTimeBetweenUpdates = 2f;

    private float updateCountdown = 0f;
    private Rigidbody rigidBody;
    // Start is called before the first frame update
    void Start()
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (rigidBody != null && updateCountdown <= 0f)
        {
            updateCountdown = Random.Range(minTimeBetweenUpdates, maxTimeBetweenUpdates);
            rigidBody.AddRelativeForce(new Vector3(Random.Range(0f, maxLateralForce), Random.Range(0f, maxLateralForce), Random.Range(0f, maxLateralForce)));
            rigidBody.AddRelativeTorque(new Vector3(Random.Range(0f, maxRotationForce), Random.Range(0f, maxRotationForce), Random.Range(0f, maxRotationForce)));

        }
        updateCountdown -= Time.deltaTime;
    }
}
