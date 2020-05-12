using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable()]
public class Food : Decayable, IPurchasable, IFood
{
    public int SegmentCount { get; set; }
    public List<TransformState> relativeSegmentPositions { get; set; }
    public TransformState position { get; set; }

    public string Name { get; set; } = "Pellet Food";
    public string Description { get; set; } = "Standard Pelletized Fish Food.  Good for all types of fish.";
    public float Price { get; set; } = 5f;
    public ItemType ItemType { get; set; } = ItemType.Food;
    public int Quantity { get; set; } = 200;
    public float HungerReduction { get; set; } = 0.05f;
    public float StressReduction { get; set; } = 0.01f;
    public float Dirtiness { get; set; } = 0.075f;

    public TankDirtiness tankDirtiness;
    public GameObject[] segments;

    public string foodTypeName = "Standard";


    public string ObjectKey { get { return foodTypeName; } set { foodTypeName = value; } }


    // Start is called before the first frame update
    void Start()
    {
        if (tankDirtiness == null)
            tankDirtiness = FindObjectOfType<TankDirtiness>();

        SetLength(Random.Range(1, segments.Length));

        if (position != null)
        {
            this.transform.position = position.Position;
            this.transform.rotation = position.Rotation;
        }
    }

    void Update()
    {
        //segmentUpdateCountdown -= Time.deltaTime;
        //if (segmentUpdateCountdown <= 0f)
        //{
        //    segmentUpdateCountdown = segmentUpdateInterval;
        //    relativeSegmentPositions = segments.Select(s => new Tuple<Vector3, Quaternion>(s.transform.position, s.transform.rotation)).ToList();
        //}

    }
    public void OnBeforeSave()
    {
        position = new TransformState(this.transform.position, this.transform.rotation);
        relativeSegmentPositions = segments.Select(s => new TransformState(s.transform.position, s.transform.rotation)).ToList();
    }

    public void SetLength(int length)
    {
        SegmentCount = length;
        SetLength();
    }
    public void SetLength()
    {
        if (segments != null && segments.Length > 0)
        {
            for (int x = 0; x < segments.Length; x++)
            {
                segments[x].SetActive(x < SegmentCount);

                if (relativeSegmentPositions != null && relativeSegmentPositions.Count <= x)
                {
                    segments[x].transform.position = relativeSegmentPositions[x].Position;
                    segments[x].transform.rotation = relativeSegmentPositions[x].Rotation;
                }
            }
        }
    }

    public override void BeforeDestroying(Decayable item)
    {
        base.BeforeDestroying(item);
        tankDirtiness.AddDirtiness((item as Food).Dirtiness);
    }

    //TODO: Replace this with something better
    internal FoodSave ToSave()
    {
        return new FoodSave()
        {
            Description = this.Description,
            Dirtiness = this.Dirtiness,
            HungerReduction = this.HungerReduction,
            ItemType = this.ItemType,
            Name = this.Name,
            position = new TransformState(transform.position, transform.rotation),
            Price = this.Price,
            Quantity = this.Quantity,
            relativeSegmentPositions = segments.Select(s => new TransformState(s.transform.position, s.transform.rotation)).ToList(),
            SegmentCount = this.SegmentCount,
            StressReduction = this.StressReduction
        };
    }
}


