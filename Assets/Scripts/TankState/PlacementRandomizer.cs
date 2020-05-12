using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementRandomizer : MonoBehaviour
{
    public TankBoundary boundary;
    // Start is called before the first frame update
    void Start()
    {
        for (int x = 0; x < this.transform.childCount; x++)
        {
            this.transform.GetChild(x).SetPositionAndRotation(
                boundary.RandomPosition(),
                Quaternion.Euler(new Vector3(Random.value, Random.value, 0f)));
        }
    }
}
