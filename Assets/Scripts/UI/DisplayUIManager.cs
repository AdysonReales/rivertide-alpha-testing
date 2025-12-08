using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class DisplayUIManager : MonoBehaviour
{
    public static DisplayUIManager Instance;

    public GameObject displayPanel;
    public GameObject pickerPanel; // Re-use Inventory UI or a separate panel
    public Transform pickerContent; // Where fish slots spawn
    public GameObject pickerSlotPrefab; // A simplified fish slot button

    private DisplaySlotUI currentSelectedSlot;

    private void Awake()
    {
        Instance = this;
        displayPanel.SetActive(false);
        pickerPanel.SetActive(false);
    }

    public void OpenDisplay()
    {
        displayPanel.SetActive(true);
        // Refresh all slots visually
        DisplaySlotUI[] slots = displayPanel.GetComponentsInChildren<DisplaySlotUI>();
        foreach (var slot in slots) slot.RefreshSlot();
    }

    public void CloseDisplay()
    {
        displayPanel.SetActive(false);
        pickerPanel.SetActive(false);
    }

    // Called when clicking a locked slot
    public void OpenPickerForSlot(DisplaySlotUI slot)
    {
        currentSelectedSlot = slot;
        pickerPanel.SetActive(true);
        PopulatePicker();
    }
    // --- ADD THIS FUNCTION ---
    public void ClosePicker()
    {
        if (pickerPanel != null) 
        {
            pickerPanel.SetActive(false);
        }
    }

    private void PopulatePicker()
    {
        // Clear old
        foreach (Transform child in pickerContent) Destroy(child.gameObject);

        // Fill with player's fish
        foreach (var fish in GameManager.Instance.playerInventory.fishes)
        {
            GameObject go = Instantiate(pickerSlotPrefab, pickerContent);
            
            // Setup visual (Reuse FishSlotUI or make a simple one)
            FishSlotUI slotUI = go.GetComponent<FishSlotUI>();
            if(slotUI) slotUI.SetFish(fish);

            // Add click listener to TRY placing this fish
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() => TryPlaceFish(fish));
        }
    }

    private void TryPlaceFish(FishInstance fish)
    {
        // Check if the picked fish matches the slot's target
        if (fish.data == currentSelectedSlot.targetFish)
        {
            // Success!
            currentSelectedSlot.Unlock();
            
            // Remove from inventory (Donated to museum)
            GameManager.Instance.playerInventory.RemoveFish(fish);

            // Close picker
            pickerPanel.SetActive(false);
        }
        else
        {
            Debug.Log("Wrong fish for this slot!");
            // Optional: Play error sound
        }
    }
}