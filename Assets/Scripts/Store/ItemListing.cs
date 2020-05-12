using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemListing : MonoBehaviour
{
    private BuyDialog buyDialog;
    public IPurchasable item;
    public TMPro.TextMeshProUGUI nameText;
    public TMPro.TextMeshProUGUI descriptionText;
    public TMPro.TextMeshProUGUI priceText;
    public TMPro.TextMeshProUGUI quantityText;
    public Button buyButton;

    // Start is called before the first frame update
    void Start()
    {
        //if (uiManager == null)
        //    uiManager = FindObjectOfType<UIManager>();
        if (buyDialog == null)
            buyDialog = gameObject.GetComponentInParent<BuyDialog>();

        if (item != null)
        {
            if (nameText != null)
                nameText.text = item.Name;

            if (descriptionText != null)
                descriptionText.text = item.Description;

            if (priceText != null)
                priceText.text = $"{item.Price:C2}";

            if (quantityText != null)
                quantityText.text = $"({item.Quantity})";

            if (buyButton != null)
            {
                buyButton.interactable = buyDialog.CanBuy(item);
                buyButton.onClick.AddListener(BuyItem);
            }
        }

    }

    void BuyItem()
    {
        buyDialog.Buy(item, 1);
        buyButton.interactable = buyDialog.CanBuy(item);
    }
}
