using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Medicine : MonoBehaviour, IPurchasable
{
    public string Name { get { return "Fish Medicine"; } }
    public string Description { get { return "Standard Cure-all Fish Medicine."; } }
    public float Price { get { return 50f; } }
    public ItemType ItemType { get { return ItemType.Medicine; } }
    public int Quantity { get { return 10; } }
}
