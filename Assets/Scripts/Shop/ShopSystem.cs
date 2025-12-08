using UnityEngine;
using System.Collections.Generic;

public class ShopSystem : MonoBehaviour
{
    public List<ShopItemData> items; // assign from GameManager.Instance.shopItems

    public void Buy(ShopItemData item, int quantity = 1)
    {
        int total = item.cost * quantity;
        if (GameManager.Instance.money >= total)
        {
            GameManager.Instance.money -= total;
            // apply item effect
            ApplyItem(item, quantity);
            UIManager.Instance.UpdateMoneyUI(GameManager.Instance.money);
            // Save purchases in inventory items dictionary
            if (!GameManager.Instance.playerInventory.items.ContainsKey(item.displayName))
                GameManager.Instance.playerInventory.items[item.displayName] = 0;
            GameManager.Instance.playerInventory.items[item.displayName] += quantity;
            SaveLoad.SaveGame();
        }
        else
        {
            Debug.Log("Not enough money");
        }
    }

    void ApplyItem(ShopItemData item, int quantity)
    {
        switch(item.effect)
        {
            case ShopItemData.Effect.Bait:
                // Increase uncommon chance by adding a temporary bonus
                FishingManager.Instance.magnetBonusPercent += 0; // none, or implement as needed
                break;
            case ShopItemData.Effect.Magnet:
                FishingManager.Instance.magnetBonusPercent += 15f * quantity;
                break;
            case ShopItemData.Effect.RageBait:
                FishingManager.Instance.rageBaitBonusPercent += 15f * quantity;
                break;
            case ShopItemData.Effect.Trapper:
                FishingManager.Instance.trapperEquipped = true;
                break;
            case ShopItemData.Effect.Spinner:
                FishingManager.Instance.spinnerEquipped = true;
                break;
            case ShopItemData.Effect.BarbedHook:
                FishingManager.Instance.barbedHookEquipped = true;
                break;
        }
    }
}
