using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDifficulty : MonoBehaviour
{
    public string difficultyName = "Normal";
    public float dirtinessMultiplier = 1f;
    public float hungerMultiplier = 0.01f;
    public float sicknessMultiplier = 0.25f;
    public float stressMultiplier = 0.5f;
    public float allowanceTime = 240;

    public bool updateTimerEnabled = false;
    private float updateTimer = 10f;
    private float updateCountdown = 0f;

    private UIManager uiManager;
    private TankDirtiness tankDirtiness;
    private FishController[] fish;

    // Start is called before the first frame update
    void Start()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();

        if (tankDirtiness == null)
            tankDirtiness = FindObjectOfType<TankDirtiness>();

        if (fish == null || fish.Length == 0)
            fish = FindObjectsOfType<FishController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTimerEnabled)
        {
            if (updateCountdown <= 0f)
            {
                updateCountdown = updateTimer;
                ApplyDifficulty();
            }
        }
    }

    public void ApplyDifficulty()
    {
        if (tankDirtiness != null)
        {
            tankDirtiness.dirtinessMultiplier = dirtinessMultiplier;
        }

        if (uiManager != null)
        {
            uiManager.budget.allowanceTime = allowanceTime;
        }

        if (fish != null)
        {
            for(int x = 0; x < fish.Length; x++)
            {
                fish[x].HungerMultiplier = hungerMultiplier;
                fish[x].SicknessMultiplier = sicknessMultiplier;
                fish[x].StressMultiplier = stressMultiplier;
            }
        }
    }
}
