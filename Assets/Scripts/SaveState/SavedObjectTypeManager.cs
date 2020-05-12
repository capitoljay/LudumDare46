using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SavedObjectTypeManager : MonoBehaviour
{
    public GameObject[] FishSpeciesPrefabs;
    public GameObject[] FoodPrefabs;
    public GameObject[] PoopPrefabs;

    public Dictionary<string, GameObject> FishSpeciesLookup = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> FoodPrefabLookup = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> PoopPrefabLookup = new Dictionary<string, GameObject>();

    private int previousFishSpecisCount = 0;
    private int previousFoodPrefabCount = 0;
    private int previousPoopPrefabCount = 0;
    // Start is called before the first frame update
    void Start()
    {
        CheckLookups();
    }

    // Update is called once per frame
    void Update()
    {
        CheckLookups();
    }

    private void CheckLookups()
    { 
        //Update the lookup for fish species
        if (FishSpeciesPrefabs != null && previousFishSpecisCount != FishSpeciesPrefabs.Length)
        {
            FishSpeciesLookup = FishSpeciesPrefabs
                .Where(f => f.GetComponent<FishController>() != null)
                .ToDictionary<GameObject, string>(f => f.GetComponent<FishController>().ObjectKey);
        }

        //Update the lookup for food types
        if (FoodPrefabs != null && previousFoodPrefabCount != FoodPrefabs.Length)
        {
            FoodPrefabLookup = FoodPrefabs
                .Where(f => f.GetComponent<Food>() != null)
                .ToDictionary(f => f.GetComponent<Food>().ObjectKey);
        }

        //Update the lookup for poop types
        if (PoopPrefabs != null && previousPoopPrefabCount != PoopPrefabs.Length)
        {
            PoopPrefabLookup = PoopPrefabs
                .Where(f => f.GetComponent<FishPoop>() != null)
                .ToDictionary(f => f.GetComponent<FishPoop>().ObjectKey);
        }
    }
}
