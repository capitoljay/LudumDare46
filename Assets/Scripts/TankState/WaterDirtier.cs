using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDirtier : MonoBehaviour
{
    public float cleanAlpha = 0.05f;
    public float dirtyAlpha = 0.45f;

    public Material waterMaterial;

    private TankDirtiness tankDirtiness;

    private float dirtyUpdateCountdown = 0f;
    private float dirtyUpdateLength = 2.5f;
    // Start is called before the first frame update
    void Start()
    {
        if (tankDirtiness == null)
            tankDirtiness = FindObjectOfType<TankDirtiness>();

        if (waterMaterial == null)
            waterMaterial = this.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (waterMaterial != null)
        {
            if (dirtyUpdateCountdown <= 0f && waterMaterial.HasProperty("_Color"))
            {
                dirtyUpdateCountdown = dirtyUpdateLength;
                waterMaterial.color = new Color(
                    waterMaterial.color.r,
                    waterMaterial.color.g,
                    waterMaterial.color.b,
                    Mathf.Lerp(cleanAlpha, dirtyAlpha, tankDirtiness.dirtinessRatio));
            }
            dirtyUpdateCountdown -= Time.deltaTime;
        }
    }
}
