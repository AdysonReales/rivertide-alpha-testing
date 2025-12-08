using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    public FishInventory inventoryManager; // Assign the GameObject that has FishInventory
    public Transform contentParent; // The "Content" GameObject inside the ScrollView
    public GameObject fishSlotPrefab; // Prefab for each fish slot

    private List<GameObject> spawnedSlots = new List<GameObject>();

    private void OnEnable()
    {
        RefreshInventory();
    }

    public void RefreshInventory()
    {
        ClearSlots();

        if (inventoryManager == null || inventoryManager.inventory == null) return;

        foreach (var fish in inventoryManager.inventory.fishes)
        {
            GameObject slot = Instantiate(fishSlotPrefab, contentParent);
            spawnedSlots.Add(slot);

            // Setup slot UI
            FishSlotUI slotUI = slot.GetComponent<FishSlotUI>();
            if (slotUI != null)
            {
                slotUI.SetFish(fish);
            }
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in spawnedSlots)
            Destroy(slot);

        spawnedSlots.Clear();
    }
}
