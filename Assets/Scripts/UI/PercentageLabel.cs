using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PercentageLabel : MonoBehaviour
{
    public string text;
    public TextMeshProUGUI textbox;
    public Slider slider;

    public float updateInterval = 2.0f;
    private float updateCountdown = 0f;
    // Start is called before the first frame update
    void Start()
    {
        if (textbox == null)
            textbox = this.GetComponent<TextMeshProUGUI>();
        if (textbox == null)
            textbox = this.GetComponentInChildren<TextMeshProUGUI>();

        if (slider == null)
            slider = this.GetComponent<Slider>();
        if (slider == null)
            slider = this.GetComponentInChildren<Slider>();
}

    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (updateCountdown <= 0 && textbox != null)
        {
            textbox.text = $"{text} ({Math.Round(slider.value / (slider.maxValue - slider.minValue) * 100, 1)}%)";
            updateCountdown = updateInterval;
        }

        updateCountdown -= Time.deltaTime;
    }
}
