using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyDialog : MonoBehaviour
{
    public UIManager uIManager;
    public GameObject itemListingParent;
    public GameObject itemListingPrefab;

    private IPurchasable[] itemsForSale;

    // Start is called before the first frame update
    void Start()
    {
        if (itemsForSale == null || itemsForSale.Length == 0)
        {
            itemsForSale = new IPurchasable[]{
                CreateItemForSale(typeof(Food)),
                CreateItemForSale(typeof(Medicine)),
            };
        }

        RefreshView();
    }

    private IPurchasable CreateItemForSale(Type itemType)
    {
        if (typeof(IPurchasable).IsAssignableFrom(itemType))
        {
            GameObject newItem = new GameObject();
            newItem.AddComponent(itemType);
            newItem.SetActive(false);
            return (IPurchasable)newItem.GetComponent(itemType);
        }
        return null;
    }

    public void Buy(IPurchasable item, int quantity)
    {
        bool purchased = false;
        if (!CanBuy(item, quantity))
            return;

        switch(item.ItemType)
        {
            case ItemType.Food:
                uIManager.foodInventory += item.Quantity * quantity;
                purchased = true;
                break;
            case ItemType.Medicine:
                uIManager.medicineInventory += item.Quantity * quantity;
                purchased = true;
                break;
        }

        //Subtract the cost from your budget if we made the purchase.
        if (purchased)
        {
            uIManager.budget.budget -= item.Price * quantity;
            RefreshView();
        }
    }

    public void RefreshView()
    {
        if (itemListingParent != null && itemListingPrefab != null)
        {
            for(int x = itemListingParent.transform.childCount - 1; x >= 0; x--)
            {
                Destroy(itemListingParent.transform.GetChild(x).gameObject);
            }

            foreach (var item in itemsForSale)
            {
                var prefab = Instantiate(itemListingPrefab);
                ItemListing prefabItem = prefab.GetComponent<ItemListing>();
                prefabItem.item = item;
                prefab.transform.SetParent(itemListingParent.transform);
            }
        }
    }

    public bool CanBuy(IPurchasable item, int quantity = 1)
    {
        return item.Price * quantity <= uIManager.budget.budget;
    }
}