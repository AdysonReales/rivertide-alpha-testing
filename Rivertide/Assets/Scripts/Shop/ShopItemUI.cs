using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    public Button buyButton;
    public Button sellButton;

    private ShopItemData data;

    public void SetItem(ShopItemData item)
    {
        data = item;
        nameText.text = item.displayName;
        costText.text = item.cost + "g";
    }

    public ShopItemData GetItem()
    {
        return data;
    }
}
