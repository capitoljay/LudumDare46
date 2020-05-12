using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class TankDirtiness : MonoBehaviour
{
    public float dirtinessMultiplier = 1.0f;
    public float maximumDirtiness = 5f;
    public float totalDirtiness = 0f;
    public GameObject poopParent;
    public GameObject foodParent;

    private static TankDirtiness instance;

    internal int totalFood;
    internal int totalPoop;

    private static float _sickFishCount = 0f;
    private static TankDirtiness GetInstance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<TankDirtiness>();

            return instance;
        }
    }

    public static float SickFishCount 
    { 
        get
        {
            return _sickFishCount;
        }
    }

    public static float Value
    {
        get
        {
            return GetInstance.totalDirtiness;
        }
    }
    public static float Maximum
    {
        get
        {
            return GetInstance.maximumDirtiness;
        }
    }


    public float dirtinessRatio
    {
        get
        {
            var val = totalDirtiness / maximumDirtiness;
            if (val < 1.0f)
                return val;

            return 1.0f;
        }
    }

    private float updateTimer = 0f;

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

        updateTimer -= Time.deltaTime;
        if (updateTimer <= 0f)
        {
            updateTimer = 1.25f;
            if (poopParent != null)
                totalPoop = poopParent.transform.childCount;
            if (foodParent != null)
                totalFood = foodParent.transform.childCount;

            _sickFishCount = FindObjectsOfType<FishController>().Count(f => f.Sickness > 0.25f);

            AddDirtiness(Random.Range(0, 0.005f) * dirtinessMultiplier);
        }
    }

    public void AddDirtiness(float value)
    {
        totalDirtiness += value;
    }

    internal static void Clean()
    {

        if (instance == null)
            instance = FindObjectOfType<TankDirtiness>();

        instance.CleanTank();
    }
    internal void CleanTank()
    {
        for (int x = poopParent.transform.childCount - 1; x >= 0; x--)
        {
            var child = poopParent.transform.GetChild(x);
            Destroy(child.gameObject);
        }
        for (int x = foodParent.transform.childCount - 1; x >= 0; x--)
        {
            var child = foodParent.transform.GetChild(x);
            Destroy(child.gameObject);
        }
        foreach(var fish in FindObjectsOfType<FishController>())
        {
            fish.AddCleaningStress();
        }
        totalDirtiness = 0f;
    }

    internal static bool IsCleaning()
    {
        //TODO: Once tank cleaning is not immediate, implement a check on it.
        return false;
    }
}
