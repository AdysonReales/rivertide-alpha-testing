using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Game State")]
    public int money = 0;
    public Inventory playerInventory = new Inventory();

    [Header("Database")]
    public List<FishData> allFishData; // Assign all 12 FishData assets here
    public List<ShopItemData> shopItems; // Assign all ShopItemData assets here

    [Header("Equipped Upgrades")]
    public bool baitEquipped = false;
    public bool magnetEquipped = false;
    public bool rageEquipped = false;
    public bool trapperEquipped = false;
    public bool spinnerEquipped = false;
    public bool barbedHookEquipped = false;

    // Helper to check upgrades
    public bool HasBait() => baitEquipped;
    public bool HasMagnet() => magnetEquipped;
    public bool HasRage() => rageEquipped;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- DISPLAY SYSTEM LOGIC ---
    public bool IsFishUnlocked(FishData fish)
    {
        return playerInventory.unlockedFishNames.Contains(fish.fishName);
    }

    public void UnlockFish(FishData fish)
    {
        if (!IsFishUnlocked(fish))
        {
            playerInventory.unlockedFishNames.Add(fish.fishName);
            // SaveLoad.SaveGame(); // Uncomment when ready
        }
    }
    // Check how many of an item we have
    public int GetItemCount(string itemName)
    {
        if (playerInventory.items.ContainsKey(itemName))
        {
            return playerInventory.items[itemName];
        }
        return 0;
    }

    // Remove 1 item from inventory
    public void ConsumeItem(string itemName)
    {
        if (playerInventory.items.ContainsKey(itemName))
        {
            playerInventory.items[itemName]--;
            
            // Optional: Keep it at 0, don't go negative
            if (playerInventory.items[itemName] < 0) 
                playerInventory.items[itemName] = 0;

            // Save changes
            // SaveLoad.SaveGame(); 
        }
    }
}

// Simple Inventory Class
