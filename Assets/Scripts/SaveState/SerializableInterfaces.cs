using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public interface IPresave
{
    void OnBeforeSave();
}
public interface IKeyedObject
{
    string ObjectKey { get; set; }
}
public interface IFood : IKeyedObject
{
    int SegmentCount { get; set; }
    List<TransformState> relativeSegmentPositions { get; set; }
    TransformState position { get; set; }

    string Name { get; set; }
    string Description { get; set; }
    float Price { get; set; }
    ItemType ItemType { get; set; }
    int Quantity { get; set; }
    float HungerReduction { get; set; }
    float StressReduction { get; set; }
    float Dirtiness { get; set; }

}
[Serializable()]
public class FoodSave : IFood, IPresave
{
    public string ObjectKey { get; set; } = "Standard";
    public int SegmentCount { get; set; }
    public List<TransformState> relativeSegmentPositions { get; set; }
    public TransformState position { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }
    public float Price { get; set; }
    public ItemType ItemType { get; set; }
    public int Quantity { get; set; }
    public float HungerReduction { get; set; }
    public float StressReduction { get; set; }
    public float Dirtiness { get; set; }
    public void OnBeforeSave()
    {

    }
}


public interface IPoop: IKeyedObject, IPresave
{
    float Dirtiness { get; set; }
    float DecayState { get; set; }
    float DecayStepAmount { get; set; }
    float DecayTimer { get; set; }
    float DecayCountdown { get; set; }
    int MinSegments { get; set; }
    int SegmentCount { get; set; }

    List<TransformState> segmentPositions { get; set; }
    TransformState position { get; set; }
}
public class PoopSave : IPoop
{
    public string ObjectKey { get; set; } = "Standard";
    public float Dirtiness { get; set; }
    public float DecayState { get; set; }
    public float DecayStepAmount { get; set; }
    public float DecayTimer { get; set; }
    public float DecayCountdown { get; set; }
    public int MinSegments { get; set; }
    public int SegmentCount { get; set; }

    public List<TransformState> segmentPositions { get; set; }
    public TransformState position { get; set; }
    public void OnBeforeSave()
    {

    }

}

public interface IFish : IKeyedObject, IPresave
{
    float Sickness { get; set; }
    float Hunger { get; set; }
    float Stress { get; set; }
    double Age { get; set; }

    float NeedsToPoop { get; set; }

    float HungerMultiplier { get; set; }
    float SicknessMultiplier { get; set; }
    float StressMultiplier { get; set; }

    float MaxScale { get; set; }
    float ScaleIncreaseMin { get; set; }
    float ScaleIncreaseMax { get; set; }
    bool IsAlive { get; set; }

    string FishName { get; set; }

    float ActionCounter { get; set; }
    float MaximumTimeBetweenActions { get; set; }
    float MinimumTimeBetweenActions { get; set; }
    bool PerformingAction { get; set; }

    TransformState position { get; set; }
    Vector3? MoveTarget { get; set; }

    float MoveSpeed { get; set; }
    float TurnSpeed { get; set; }
    float PitchSpeed { get; set; }
    float MaxTravelDistance { get; set; }
}
public class FishSave : IFish, IKeyedObject
{
    public string ObjectKey { get; set; } = "Standard";
    public float Sickness { get; set; } = 0.0f; //Once the sickness reaches 1 the fish dies of the illness
    public float Hunger { get; set; } = 0.0f; //Once hunger reaches 1 the fish dies of starvation
    public float Stress { get; set; } = 0.0f; //Stress causes the fish to get hungry faster and sick easier
    public double Age { get; set; } = 0f;

    public float NeedsToPoop { get; set; } = 0;

    public float HungerMultiplier { get; set; } = 0.01f;
    public float SicknessMultiplier { get; set; } = 0.25f;
    public float StressMultiplier { get; set; } = 0.5f;

    public float MaxScale { get; set; } = 0.08f;
    public float ScaleIncreaseMin { get; set; } = 0.001f;
    public float ScaleIncreaseMax { get; set; } = 0.005f;

    public bool IsAlive { get; set; }

    public string FishName { get; set; }

    public float ActionCounter { get; set; } = 0f;
    public float MaximumTimeBetweenActions { get; set; } = 15f;
    public float MinimumTimeBetweenActions { get; set; } = 5f;
    public bool PerformingAction { get; set; } = false;

    public TransformState position { get; set; }
    public Vector3? MoveTarget { get; set; } = null;

    public float MoveSpeed { get; set; } = 2f;
    public float TurnSpeed { get; set; } = 5f;
    public float PitchSpeed { get; set; } = 0.5f;
    public float MaxTravelDistance { get; set; } = 2f;

    public void OnBeforeSave()
    {

    }
}

[Serializable()]
public class TransformState
{
    public TransformState(Vector3 position, Quaternion rotation) : this()
    {
        Position = position;
        Rotation = rotation;
    }
    public TransformState()
    {

    }
    public Vector3 Position { get; set; }
    public Quaternion Rotation { get; set; }
}