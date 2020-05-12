using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MedicineApplicator : MonoBehaviour
{
    public FishController[] fishInstances;
    public float medicineEffectiveness = 0.90f;
    // Start is called before the first frame update
    void Start()
    {
        if (fishInstances == null || fishInstances.Length == 0)
            fishInstances = FindObjectsOfType<FishController>();
    }

    public void AddMedicine()
    {
        foreach(var fish in fishInstances)
        {
            //Get a "Effectiveness" chance of being completely cured
            if (Random.value <= medicineEffectiveness)
            {
                fish.Sickness = 0f;
            }
            else //Or you get 1/2 effectiveness percent removed from your sickness level.
            {
                fish.Sickness = fish.Sickness * (1 - (medicineEffectiveness / 2));
            }
        }

        
    }
}
