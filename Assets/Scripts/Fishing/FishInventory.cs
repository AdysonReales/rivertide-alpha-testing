using UnityEngine;

public class FishInventory : MonoBehaviour
{
    public static FishInventory Instance;

    public Inventory inventory;   // This uses your existing Inventory class

    void Awake()
    {
        // Singleton pattern
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInventory();
    }

    // ------------------------------
    // LOAD INVENTORY FROM SAVE FILE
    // ------------------------------
    void LoadInventory()
    {
        inventory = SaveLoad.LoadInventory();

        if (inventory == null)
        {
            Debug.Log("No inventory found — creating new one.");
            inventory = new Inventory();
            SaveLoad.SaveInventory(inventory);
        }
    }

    // ------------------------------
    // ADD FISH TO INVENTORY
    // ------------------------------
    public void AddFish(FishInstance fish)
    {
        inventory.AddFish(fish);
        Debug.Log($"Fish added: {fish.data.fishName} ({fish.kg}kg)");
    }

    // ------------------------------
    // REMOVE FISH FROM INVENTORY
    // ------------------------------
    public void RemoveFish(FishInstance fish)
    {
        inventory.RemoveFish(fish);
        Debug.Log($"Fish removed: {fish.data.fishName}");
    }

    // ------------------------------
    // GET ALL FISH
    // ------------------------------
    public System.Collections.Generic.List<FishInstance> GetFishes()
    {
        return inventory.fishes;
    }

    // ------------------------------
    // CLEAR INVENTORY
    // (optional for testing)
    // ------------------------------
    public void Clear()
    {
        inventory.fishes.Clear();
        SaveLoad.SaveInventory(inventory);
        Debug.Log("Inventory cleared.");
    }
}
