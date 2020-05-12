using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodFeeder : MonoBehaviour
{
    public GameObject foodPrefab;
    public GameObject foodParent;
    public TankBoundary feedLocation;

    private float FoodCheckTime = 5.0f;
    private float FoodCheckCountdown = 0f;

    public Food[] foodAvailable = new Food[0];

    // Start is called before the first frame update
    void Start()
    {
        if (foodParent == null)
            foodParent = this.gameObject;

        if (feedLocation == null)
            feedLocation = this.GetComponent<TankBoundary>();
    }

    private void Update()
    {
        //Look for any food in the tank periodically
        if (FoodCheckCountdown < 0f)
        {
            FoodCheckCountdown = FoodCheckTime;
            foodAvailable = foodParent.GetComponentsInChildren<Food>();
        }
        else
        {
            FoodCheckCountdown -= Time.deltaTime;
        }
    }

    public void AddFood(int count)
    {
        for (int x = 0; x < count; x++)
        {
            var newFood = Instantiate(foodPrefab);
            newFood.transform.position = feedLocation.RandomPosition();
            if (foodParent != null)
                newFood.transform.SetParent(foodParent.transform);
        }
    }

    public void AddFood(Food food)
    {
        if (foodParent != null)
            food.transform.SetParent(foodParent.transform);
    }
}
