using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable()]
public class Budget
{
    public float budget = 10f;
    public float allowance = 5f;
    public float allowanceTime = 240f;
    internal float allowanceCountdown { get; set; }
}
