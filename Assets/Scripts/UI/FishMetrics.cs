using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishMetrics : MonoBehaviour
{
    public FishController fish;
    public Slider sicknessSlider;
    public Slider hungerSlider;
    public Slider stressSlider;
    public TMPro.TextMeshProUGUI nameTextbox;
    public TMPro.TextMeshProUGUI ageTextbox;
    public GameObject IsDeadImage;

    private float updateCountdown = 0f;
    private float updateInterval = 1f;
    // Start is called before the first frame update
    void Start()
    {
        nameTextbox.text = fish.FishName;
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
            sicknessSlider.value = fish.Sickness;
            sicknessSlider.gameObject.SetActive(fish.IsAlive);

            hungerSlider.value = fish.Hunger;
            hungerSlider.gameObject.SetActive(fish.IsAlive);

            stressSlider.value = fish.Stress;
            stressSlider.gameObject.SetActive(fish.IsAlive);

            if (ageTextbox != null)
                ageTextbox.text = GetAge();
            if (IsDeadImage != null)
                IsDeadImage.SetActive(!fish.IsAlive);
        }
        
    }

    private string GetAge()
    {
        var time = TimeSpan.FromSeconds(fish.Age);
        if (time.Days > 0)
        {
            return time.ToString("d'd 'h'h'");
        }
        if (time.Hours > 0)
            return time.ToString("h'h 'm'm'");

        return time.ToString("m'm 's's'");
    }
}
