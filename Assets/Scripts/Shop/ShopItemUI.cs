using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("Data (Logic Only)")]
    public ShopItemData itemData; 

    [Header("UI References")]
    public Button buyButton;
    public TextMeshProUGUI amountOwnedText; // The text that shows the number (e.g. "5")

    // We don't need Image/CostText references because we are keeping your Scene design!

    private void Start()
    {
        if (itemData == null)
        {
            Debug.LogError($"ShopItemUI on {gameObject.name} is missing ItemData!");
            return;
        }
        
        Initialize();
    }

    public void Initialize()
    {
        // 1. Setup Button Listener
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(TryBuyItem);
        }
        
        // 2. Update the Count immediately on start
        UpdateOwnedCount();
    }

    private void TryBuyItem()
    {
        if (GameManager.Instance == null) return;

        // Check Money vs Cost from Data
        if (GameManager.Instance.money >= itemData.cost)
        {
            // Deduct Money
            GameManager.Instance.money -= itemData.cost;

            // Add to Inventory
            if (GameManager.Instance.playerInventory.items.ContainsKey(itemData.displayName))
            {
                GameManager.Instance.playerInventory.items[itemData.displayName]++;
            }
            else
            {
                GameManager.Instance.playerInventory.items.Add(itemData.displayName, 1);
            }

            // Enable Effect
            ApplyEffect(itemData.effect);

            // Update the "5" or "10" text
            UpdateOwnedCount();
            
            // Update World UI (Main Screen)
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateMoneyUI(GameManager.Instance.money);
                UIManager.Instance.UpdateWorldIcons();
            }

            // Update Shop UI (Right Box Money)
            if (ShopUIManager.Instance != null)
            {
                ShopUIManager.Instance.UpdateMoneyUI();
            }
        }
        else
        {
            Debug.LogWarning($"Not enough money! Cost is {itemData.cost}");
        }
    }

    private void UpdateOwnedCount()
    {
        if (amountOwnedText == null) return;

        int count = 0;
        if (GameManager.Instance != null && GameManager.Instance.playerInventory.items.ContainsKey(itemData.displayName))
        {
            count = GameManager.Instance.playerInventory.items[itemData.displayName];
        }

        // CHANGE: Only show the number
        amountOwnedText.text = count.ToString(); 
        // amountOwnedText.text = "x" + count; // Use this if you want "x5"
    }

    private void ApplyEffect(ShopItemData.Effect effect)
    {
        switch (effect)
        {
            case ShopItemData.Effect.Bait: GameManager.Instance.baitEquipped = true; break;
            case ShopItemData.Effect.Magnet: GameManager.Instance.magnetEquipped = true; break;
            case ShopItemData.Effect.RageBait: GameManager.Instance.rageEquipped = true; break;
            case ShopItemData.Effect.Trapper: GameManager.Instance.trapperEquipped = true; break;
            case ShopItemData.Effect.Spinner: GameManager.Instance.spinnerEquipped = true; break;
            case ShopItemData.Effect.BarbedHook: GameManager.Instance.barbedHookEquipped = true; break;
        }
    }
}