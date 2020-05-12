using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankMetrics : MonoBehaviour
{
    public TankDirtiness tankDirtiness;
    public Slider dirtinessSlider;
    public TextMeshProUGUI foodCountText;
    public TextMeshProUGUI poopCountText;
    public TextMeshProUGUI moneyText;
    public UIManager uiManager;

    private float updateCountdown = 0f;
    private float updateInterval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        if (uiManager == null)
            uiManager = FindObjectOfType<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        updateCountdown -= Time.deltaTime;
        if (updateCountdown <= 0f)
        {
            updateCountdown = updateInterval;
            if (tankDirtiness.totalDirtiness < dirtinessSlider.maxValue)
                dirtinessSlider.value = tankDirtiness.totalDirtiness;
            else
                dirtinessSlider.value = dirtinessSlider.maxValue;

            foodCountText.text = $"Total Food: {tankDirtiness.totalFood}";
            poopCountText.text = $"Total Poop: {tankDirtiness.totalPoop}";
            moneyText.text = $"Budget: {uiManager.budget.budget:C2}";

        }
    }
}
