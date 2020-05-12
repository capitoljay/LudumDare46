using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class FishPoop : MonoBehaviour, IPoop
{
    public float Dirtiness { get; set; } = 0.2f;
    public float DecayState { get; set; } = 1f;
    public float DecayStepAmount { get; set; } = 0.25f;
    public float DecayTimer { get; set; } = 0f;
    public float DecayCountdown { get; set; } = 5f;
    public int MinSegments { get; set; } = 3;
    public int SegmentCount { get; set; } = 4;

    public GameObject[] segments;
    public List<TransformState> segmentPositions { get; set; }
    public TransformState position { get; set; }

    private TankDirtiness tankDirtiness;
    public string poopTypeName = "Standard";


    public string ObjectKey { get { return poopTypeName; } set { poopTypeName = value; } }

    private void Start()
    {
        if (tankDirtiness == null)
            tankDirtiness = FindObjectOfType<TankDirtiness>();

        SetLength(UnityEngine.Random.Range(MinSegments,segments.Length));
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

                if (segmentPositions != null && segmentPositions.Count <= x)
                {
                    segments[x].transform.localPosition = segmentPositions[x].Position;
                    segments[x].transform.localRotation = segmentPositions[x].Rotation;
                }
            }
        }

        if (position != null)
        {
            this.transform.position = position.Position;
            this.transform.rotation = position.Rotation;
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(new Ray(this.transform.position, Vector3.down), out hit, 20f))
                this.transform.position = hit.point;
            
        }

    }

    private void Update()
    {
        //If the game is paused, we want to skip the update loop
        if (Time.timeScale <= 0f)
            return;

        DecayTimer -= Time.deltaTime;
        if (DecayTimer <= 0f)
        {
            DecayTimer = DecayCountdown;
            DecayState -= DecayStepAmount;
            if (DecayState <= 0f)
            {
                tankDirtiness.AddDirtiness(Dirtiness);
                for(int x = this.transform.childCount - 1; x >= 0; x--)
                {
                    Destroy(this.transform.GetChild(x).gameObject);
                }
                Destroy(this.gameObject);
            }
            else
            {
                //position = new Tuple<Vector3, Quaternion>(transform.position, transform.rotation);
                //segmentPositions = segments.Select(s => new Tuple<Vector3, Quaternion>(s.transform.position, s.transform.rotation)).ToList();
            }
        }
    }
    public void OnBeforeSave()
    {
        position = new TransformState(this.transform.position, this.transform.rotation);
        segmentPositions = segments.Select(s => new TransformState(s.transform.localPosition, s.transform.localRotation)).ToList();
    }

    public static float GetTotalDirtiness()
    {
        return FindObjectsOfType<FishPoop>().Sum(p => p.TotalDirtiness);
    }

    internal void SetLengthRatio(float length)
    {
        SetLength(Mathf.RoundToInt(Mathf.Lerp(2f, segments.Length, length)));
    }

    public float TotalDirtiness
    {
        get
        {
            return Dirtiness * SegmentCount;
        }
    }

    //TODO: Replace this with something better
    internal PoopSave ToSave()
    {
        position = new TransformState(transform.position, transform.rotation);
        segmentPositions = segments.Select(s => new TransformState(s.transform.position, s.transform.rotation)).ToList();

        return new PoopSave()
        {
            DecayCountdown = this.DecayCountdown,
            DecayState = this.DecayState,
            DecayStepAmount = this.DecayStepAmount,
            DecayTimer = this.DecayTimer,
            Dirtiness = this.Dirtiness,
            MinSegments = this.MinSegments,
            position = this.position,
            SegmentCount = this.SegmentCount,
            segmentPositions = this.segmentPositions
        };
    }
}
