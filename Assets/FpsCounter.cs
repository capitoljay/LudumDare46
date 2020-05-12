using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour
{
    private TMPro.TextMeshProUGUI tmPro;
    private float updateCountdown = 0.5f;
    private float deltaTimeRolling = 0f;

    // Start is called before the first frame update
    void Start()
    {
        tmPro = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        deltaTimeRolling = (deltaTimeRolling + Time.deltaTime) / 2f;
        updateCountdown -= Time.deltaTime;
        if (updateCountdown <= 0f)
        {
            updateCountdown = 1.0f;
            if (tmPro != null)
                tmPro.text = $"{Mathf.Round(1f / deltaTimeRolling)} fps";
        }
    }
}
