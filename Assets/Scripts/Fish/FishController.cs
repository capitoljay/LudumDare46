using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class FishController : MonoBehaviour, IFish
{
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

    public Vector3? MoveTarget { get; set; } = null;
    public TransformState position { get; set; }
    
    public float MoveSpeed { get; set; } = 100f;
    public float TurnSpeed { get; set; } = 100f;
    public float PitchSpeed { get; set; } = 50f;
    public float MaxTravelDistance { get; set; } = 2f;

    public string speciesName;
    public string ObjectKey 
    { 
        get { return speciesName; }
        set { speciesName = value; }
    }

    //NonSerialized members
    private float TargetReachedDistance = 0.1f;

    private float ColliderTimeout = 1.0f;
    private bool ResetColliders = false;

    private float OutOfBoundsResetMultiplier = 1.0f;


    private bool previousIsAlive = true;
    private Rigidbody rigidBody;
    private TankBoundary tankBoundary;

    public FishAction? action = null;
    private GameObject actionTarget = null;
    //public float actionCountdown = 0f;

    public GameObject target;
    public MeshFilter tankParameters;

    public GameObject fishPoopPrefab;
    public FishManager fishManager;
    public FoodFeeder feeder;
    private float outOfBoundsCountdown = 0f;
    public GameObject poopTarget;

    private Collider[] colliders;

    private static List<string> takenNames = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        IsAlive = true;
        FishName = NewName();

        if (rigidBody == null)
            rigidBody = this.GetComponent<Rigidbody>();

        if (tankBoundary == null)
            tankBoundary = GetComponent<TankBoundary>();

        if (tankBoundary == null)
            tankBoundary = GetComponentInParent<TankBoundary>();

        if (target == null)
            target = this.transform.Find("Target")?.gameObject;

        if (target == null)
            target = this.gameObject;

        if (colliders == null)
            colliders = this.GetComponentsInChildren<Collider>().Where(c => c.enabled).ToArray();

        if (fishManager == null)
            fishManager = FindObjectOfType<FishManager>();

        if (fishManager != null)
            fishManager.fish.Add(this);

        if (feeder == null)
            feeder = FindObjectOfType<FoodFeeder>();

        if (position != null)
        {
            this.transform.position = position.Position;
            this.transform.rotation = position.Rotation;
        }

    }

    internal void AddCleaningStress()
    {
        Stress += UnityEngine.Random.value * StressMultiplier;
    }
    private string NewName()
    {
        int nameCount = 15;
        int val = Mathf.RoundToInt(UnityEngine.Random.Range(0, nameCount));
        while(takenNames.Contains(GetName(val)) && takenNames.Count <= nameCount)
        {
            val = Mathf.RoundToInt(UnityEngine.Random.Range(0, nameCount));
        }
        var name = GetName(val);
        takenNames.Add(name);
        return name;
    }
    private string GetName(int index)
    {
        switch (index)
        {
            case 0:
                return "Bubbles";
            case 1:
                return "Flipper";
            case 2:
                return "Jaws";
            case 3:
                return "Max";
            case 4:
                return "Sam";
            case 5:
                return "Shelly";
            case 6:
                return "Sally";
            case 7:
                return "Jane";
            case 8:
                return "Tom";
            case 9:
                return "Nimo";
            case 10:
                return "Dory";
            case 11:
                return "Rita";
            case 12:
                return "Edward";
            case 13:
                return "Rose";
            case 14:
                return "Ivan";
            default:
                return "Bob";
        }
    }
    private void OnDrawGizmos()
    {
        if (MoveTarget.HasValue)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(MoveTarget.Value, 0.025f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        if (IsAlive)
            Age += Time.deltaTime;

        if (ColliderTimeout <= 0f && ResetColliders)
        {
            EnableColliders(true);
            ColliderTimeout = 0f;
            ResetColliders = false;
        }
        else
        {
            if (ResetColliders)
                ColliderTimeout -= Time.deltaTime;
        }

        if (IsAlive)
        {
            CheckHunger();
            CheckSickness();
            CheckStress();

            CheckBoundaries();
            DoActions();
        }
        else
        {
            if (previousIsAlive == true)
            {
                Hunger = 0;
                Sickness = 0;
                Stress = 0;

                CheckBoundaries(true);
                transform.rotation = Quaternion.Euler(200f, transform.rotation.y, transform.rotation.z);
                rigidBody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
            }
            PlayDead();
            previousIsAlive = IsAlive;
        }

        //positionUpdateCountdown -= Time.deltaTime;
        //if (positionUpdateCountdown <= 0f)
        //{
        //    positionUpdateCountdown = positionUpdateTimeout;
        //    position = new Tuple<Vector3, Quaternion>(transform.position, transform.rotation);
        //}
    }
    public void OnBeforeSave()
    {
        position = new TransformState(this.transform.position, this.transform.rotation);
    }

    private void PlayDead()
    {
        //If we're within the tank boundary, keep floating upwards.
        if (tankBoundary.tankBounds.Contains(transform.position))
        {
            rigidBody.AddForce(Vector3.up * (2 * rigidBody.drag));
        }
        else
        {
            rigidBody.AddForce(Vector3.down * (rigidBody.drag));
        }
    }

    private void EnableColliders(bool enabled)
    {
        foreach (var collider in colliders)
            collider.enabled = enabled;
    }
    private void CheckBoundaries(bool forceImmediateReset = false)
    {
        //Check out of bounds and reset the fish if needed
        if (outOfBoundsCountdown > 0f)
        {
            //Count down until it's time to reset the fish.
            outOfBoundsCountdown -= Time.deltaTime;

            //If the fish came back inside the boundary, reset the countdown.
            if (tankBoundary.tankBounds.Contains(transform.position))
            {
                outOfBoundsCountdown = 0f;
                if (OutOfBoundsResetMultiplier > 1.0f)
                    OutOfBoundsResetMultiplier -= 0.25f;
            }
            else if (outOfBoundsCountdown < 0f)
                outOfBoundsCountdown = -10f;
        }
        else if (outOfBoundsCountdown <= -10f)
        {
            ResetColliders = true;
            ColliderTimeout = 0.75f;
            EnableColliders(false);

            //Reset fish to within bounds
            if (forceImmediateReset)
            {
                while (!tankBoundary.tankBounds.Contains(transform.position))
                {
                    transform.position = Vector3.Lerp(transform.position, tankBoundary.tankBounds.center, 0.1f * OutOfBoundsResetMultiplier);
                }
            }
            else
            { 
                transform.position = Vector3.Lerp(transform.position, tankBoundary.tankBounds.center, 0.01f * OutOfBoundsResetMultiplier);
                OutOfBoundsResetMultiplier += 0.25f;
                outOfBoundsCountdown = 0f;
            }
        }
        else if (outOfBoundsCountdown == 0f && !tankBoundary.tankBounds.Contains(transform.position))
        {
            outOfBoundsCountdown = 5f;
        }
    }

    private void DoActions()
    {
        //If the action isn't specified and we're not performing the action, or the counter reaches zero, start a new action
        if (ActionCounter <= 0f || !PerformingAction)
        {
            action = null;
            ActionCounter = Mathf.Lerp(MinimumTimeBetweenActions, MaximumTimeBetweenActions, Random.value);
            float actionVal = Random.value;

            //If there's food available decide whether we should look for food.  If the fish is over 35% hungry always look for food.
            if (feeder.foodAvailable.Length > 0 && ((Hunger > 0.1f && actionVal > 0.15f) || Hunger > 0.35f))
            {
                var foodTopIdx = feeder.foodAvailable.Length - (feeder.foodAvailable.Length > 4 ? feeder.foodAvailable.Length / 4 : 1);
                if (foodTopIdx >= 0 && foodTopIdx < feeder.foodAvailable.Length)
                {
                    foodTopIdx = feeder.foodAvailable.Length - 1;

                    var targetFood = feeder.foodAvailable[Random.Range(0, foodTopIdx)];
                    actionTarget = targetFood != null ? targetFood.gameObject : null;
                    MoveTarget = actionTarget.transform.position;
                    action = FishAction.Eat;
                }
                else
                {
                    //Move to the top of the tank in preparation for more food.
                    MoveTarget = GetFishTarget(transform.position, true);
                }
            }
            else if (TankDirtiness.IsCleaning() || (Stress + actionVal > 0.75)) //If fish stress is high, decide if we should hide.  If we're cleaning the tank, always hide
            {
                action = FishAction.Hide;

            }
            else if (actionVal > 0.2f && NeedsToPoop > 0.2f) //If the fish needs to poop, decide if it should poop.
            {
                action = FishAction.Poop;
            }
            else
            {
                //Otherwise we should just move around in the tank.
                if (actionVal < 0.05f || (Hunger > 0.35f && feeder.foodAvailable.Length == 0)) //Move to top of tank
                {
                    MoveTarget = GetFishTarget(transform.position, true);
                }
                else //Move
                {
                    MoveTarget = GetFishTarget(transform.position, false);
                }
            }
            PerformingAction = true;
        }
        else
        {
            //RaycastHit hit = new RaycastHit();
            ////If the path to the target is blocked, try a new target.
            //if (target != null && !CheckTarget(transform.position, target.transform.position, out hit))
            //{
            //    target.transform.position = RandomTarget(transform.position, false);
            //}
            ActionCounter -= Time.deltaTime;

            //If there's an actionTarget, ensure the MoveTarget is set to it.
            if (MoveTarget == null)
            {
                if (actionTarget != null)
                    MoveTarget = actionTarget.transform.position;
                else
                    MoveTarget = GetFishTarget(transform.position, false);
            }

                if (MoveTarget.HasValue)
            {
                //If we've reached the target.  Set it to null in preparation for the next target, otherwise move towards the target.
                var distance = Vector3.Distance(target.transform.position, MoveTarget.Value);
                if (distance <= TargetReachedDistance)
                {
                    if (actionTarget != null)
                    {
                        //If the actionTarget is food, eat it
                        var food = actionTarget.GetComponent<Food>();
                        if (food != null)
                            Eat(food);

                        Destroy(actionTarget);
                        actionTarget = null;
                    }
                    MoveTarget = null;
                    PerformingAction = false;
                }
                else
                {
                    //TODO: Change this to a relative motion.
                    transform.LookAt(MoveTarget.Value);
                    var distanceFactor = distance / tankBoundary.tankBounds.size.magnitude;
                    rigidBody.AddRelativeForce(Vector3.forward * MoveSpeed * Time.deltaTime * distanceFactor, ForceMode.Acceleration);
                }

            }

            //If we're supposed to poop, do so.
            if (action == FishAction.Poop && fishPoopPrefab != null)
            {
                var newPoop = Instantiate(fishPoopPrefab);
                newPoop.GetComponent<FishPoop>().SetLengthRatio(CreatePoop());
                newPoop.transform.SetPositionAndRotation(poopTarget.transform.position, poopTarget.transform.rotation);
                if (fishManager.fishPoopParent != null)
                    newPoop.transform.SetParent(fishManager.fishPoopParent.transform);

                action = null;
                PerformingAction = false;
            }

            ////Catch all for if we don't have a MoveTarget or an actionTarget
            //if (!MoveTarget.HasValue && !action.HasValue)
            //    PerformingAction = false;
        }
    }

    private Vector3 GetFishTarget(Vector3 fishPosition, bool maxY = false)
    {
        var newTarget = RandomTarget(fishPosition, maxY);
        var tryCount = 5;
        var hit = new RaycastHit();
        while (!CheckTarget(fishPosition, newTarget, out hit) && tryCount > 0)
        {
            newTarget = RandomTarget(fishPosition, maxY);
            tryCount--;
        }

        return newTarget;
    }

    private bool CheckTarget(Vector3 fishPosition, Vector3 target, out RaycastHit hit)
    {
        int layerMask = 1 << 8 & 1 << 9 & 1 << 10; //Need to raycast to layers 8 (Accessories), 9 (Terrain), and 10 (Equipment)
        var targetDirection = (target - fishPosition);

        return Physics.Raycast(new Ray(fishPosition, targetDirection), out hit, targetDirection.magnitude, layerMask);
    }

    private Vector3 RandomTarget(Vector3 fishPosition, bool maxY)
    {
        var x = Mathf.Lerp(
            Mathf.Max(tankBoundary.tankBounds.min.x, fishPosition.x - MaxTravelDistance),
            Mathf.Min(tankBoundary.tankBounds.max.x, fishPosition.x + MaxTravelDistance),
            Random.value);
        var y = maxY ?
            tankBoundary.tankBounds.max.y :
            Mathf.Lerp(
                Mathf.Max(tankBoundary.tankBounds.min.y, fishPosition.y - MaxTravelDistance),
                Mathf.Min(tankBoundary.tankBounds.max.y, fishPosition.y + MaxTravelDistance),
                Random.value);
        var z = Mathf.Lerp(
            Mathf.Max(tankBoundary.tankBounds.min.z, fishPosition.z - MaxTravelDistance),
            Mathf.Min(tankBoundary.tankBounds.max.z, fishPosition.z + MaxTravelDistance),
            Random.value);

        return new Vector3(x, y, z);
    }

    //TODO: Replace this with something better
    internal FishSave ToSave()
    {
        position = new TransformState(transform.position, transform.rotation);

        return new FishSave()
        {
            ActionCounter = this.ActionCounter,
            Age = this.Age,
            FishName = this.FishName,
            Hunger = this.Hunger,
            HungerMultiplier = this.HungerMultiplier,
            IsAlive = this.IsAlive,
            MaximumTimeBetweenActions = this.MaximumTimeBetweenActions,
            MaxScale = this.MaxScale,
            MoveSpeed = this.MoveSpeed,
            MoveTarget = this.MoveTarget,
            NeedsToPoop = this.NeedsToPoop,
            PerformingAction = this.PerformingAction,
            PitchSpeed = this.PitchSpeed,
            position = this.position,
            ScaleIncreaseMax = this.ScaleIncreaseMax,
            ScaleIncreaseMin = this.ScaleIncreaseMin,
            Sickness = this.Sickness,
            SicknessMultiplier = this.SicknessMultiplier,
            Stress = this.Stress,
            StressMultiplier = this.StressMultiplier,
            TurnSpeed = this.TurnSpeed
        };
    }

    public void CheckHunger()
    {
        Hunger += HungerMultiplier * Time.deltaTime * Mathf.Abs(UnityEngine.Random.value - 0.5f);
        if (Hunger >= 1.0f)
            IsAlive = false;
    }

    public void CheckSickness()
    {
        if (UnityEngine.Random.value < SicknessMultiplier / 5000f)
        {
            var contagionModifier = 1f + (TankDirtiness.SickFishCount);
            Sickness += UnityEngine.Random.value * SicknessMultiplier * 100f * (Stress * StressMultiplier) * contagionModifier;
            if (Sickness > 0.9f)
                Sickness = 0.9f;
        }
        else if (UnityEngine.Random.value < SicknessMultiplier)
        {
            //Adjusted stress is shifted so that under 20% stress will help to reduce sickness.
            var adjustedStress = (Stress - 0.2f);

            //The amount a fish gets sick is dependant on how much stress they're under.
            Sickness += UnityEngine.Random.value * SicknessMultiplier * adjustedStress * Time.deltaTime;
        }
        if (Sickness >= 1.0f)
            IsAlive = false;
    }

    public void Eat(Food food)
    {
        Hunger -= food.HungerReduction;
        Stress -= food.StressReduction;
        if (Hunger < 0f)
            Hunger = 0f;
        if (Stress < 0f)
            Stress = 0f;

        if (transform.localScale.magnitude < MaxScale)
        {
            var newSize = Random.Range(ScaleIncreaseMin, ScaleIncreaseMax - Mathf.Lerp(0,ScaleIncreaseMax, Random.Range(0.5f,1f) * (transform.localScale.x / MaxScale)));
            transform.localScale = new Vector3(transform.localScale.x + newSize, transform.localScale.y + newSize, transform.localScale.z + newSize);
        }

        NeedsToPoop += food.Dirtiness * Random.Range(0.5f,1.5f);

        if (NeedsToPoop > 1f)
            NeedsToPoop = 1f;
    }

    public void CheckStress()
    {
        if (UnityEngine.Random.value < StressMultiplier)
        {
            //Shift dirtiness factor down so the cleanest 15% of the tank reduces stress.
            var adjustedDirtiness = (TankDirtiness.Value / TankDirtiness.Maximum) - 0.25f;

            //Shift the hunger factor down so that under 35% hunger reduces stress
            var adjustedHunger = Hunger - 0.35f;

            //Stress is a factor of tank dirtiness and hunger and can increase or decrease stress in this equation.
            Stress += (adjustedDirtiness + adjustedHunger) * StressMultiplier * Time.deltaTime;
        }
    }

    private float CreatePoop()
    {
        float value = Mathf.Lerp(0f, NeedsToPoop, UnityEngine.Random.value);
        NeedsToPoop -= value;
        if (NeedsToPoop < 0)
            NeedsToPoop = 0;

        return value;
    }

    public enum FishAction
    {
        Eat,
        Attack,
        Hide,
        Poop
    }

    public static FishController AddFish(IFish fish)
    {
        var newFish = (FishController)fish;
        takenNames.Add(newFish.FishName);
        return newFish;
    }
}
