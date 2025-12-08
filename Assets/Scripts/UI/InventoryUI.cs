using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public Transform contentParent;
    public GameObject fishSlotPrefab;

    private List<GameObject> spawnedSlots = new List<GameObject>();

    private void OnEnable()
    {
        Debug.Log("InventoryUI: OnEnable called.");
        RefreshInventory();
    }

    // Allow us to click the 3 dots in Inspector and choose "Force Refresh"
    [ContextMenu("Force Refresh")]
    public void RefreshInventory()
    {
        Debug.Log("InventoryUI: RefreshInventory STARTED.");

        // 1. Check Manager
        if (GameManager.Instance == null)
        {
            Debug.LogError("InventoryUI: GameManager.Instance is NULL! Retrying in 0.1s...");
            Invoke(nameof(RefreshInventory), 0.1f);
            return;
        }

        // 2. Check Inventory
        if (GameManager.Instance.playerInventory == null)
        {
            Debug.LogError("InventoryUI: PlayerInventory is NULL!");
            return;
        }

        // 3. Check List Count
        int count = GameManager.Instance.playerInventory.fishes.Count;
        Debug.Log($"InventoryUI: Found {count} fish in inventory.");

        if (count == 0)
        {
            Debug.LogWarning("InventoryUI: Inventory is empty. Nothing to spawn.");
            ClearSlots(); // Make sure we clear old slots even if empty
            return;
        }

        // 4. Check Prefab
        if (fishSlotPrefab == null)
        {
            Debug.LogError("InventoryUI: Fish Slot Prefab is Missing in Inspector!");
            return;
        }

        if (contentParent == null)
        {
            Debug.LogError("InventoryUI: Content Parent is Missing in Inspector!");
            return;
        }

        ClearSlots();

        // 5. Spawn Loop
        Debug.Log("InventoryUI: Starting Spawn Loop...");
        foreach (var fish in GameManager.Instance.playerInventory.fishes)
        {
            GameObject slot = Instantiate(fishSlotPrefab, contentParent);
            spawnedSlots.Add(slot);

            FishSlotUI slotUI = slot.GetComponent<FishSlotUI>();
            if (slotUI != null)
            {
                slotUI.SetFish(fish);
            }
            else
            {
                Debug.LogError("InventoryUI: The Prefab does not have the 'FishSlotUI' script!");
            }
        }
        Debug.Log($"InventoryUI: Finished. Spawned {spawnedSlots.Count} slots.");
    }

   private void ClearSlots()
    {
        // 1. Destroy all objects we track in the list
        foreach (var slot in spawnedSlots)
        {
            if (slot != null) Destroy(slot);
        }
        spawnedSlots.Clear();

        // 2. SAFETY NET: Destroy any "Ghost" children that might be lingering in the Hierarchy
        // (This happens if the list got cleared but the UI objects didn't)
        if (contentParent != null)
        {
            foreach (Transform child in contentParent)
            {
                Destroy(child.gameObject);
            }
        }
    }
}