using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FishManager : MonoBehaviour
{
    public GameObject fishPoopParent;
    public GameObject fishFoodParent;
    public List<FishController> fish = new List<FishController>();

    // Start is called before the first frame update
    void Start()
    {
        fish = FindObjectsOfType<FishController>().ToList();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
