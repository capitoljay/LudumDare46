using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPurchasable
{
    string Name { get; }
    string Description { get; }
    float Price { get;  }
    ItemType ItemType { get; }
    int Quantity { get; }
}

public enum ItemType
{
    Food,
    Medicine,
    TankAccessory,
    Plant,
    Fish
}
