using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayUIManager : MonoBehaviour
{
    public static DisplayUIManager Instance;

    [Header("Panels")]
    public WorldUIManager worldUIManager;

    public GameObject darkBackground; // assign the dark background Image
    public GameObject inventoryButton; // your toggle button
    public TextMeshProUGUI worldMoneyText;
  
    public GameObject displayPanel;           // Main DisplayPanel
    public GameObject inventoryPanel;         // Inventory panel for choosing fish
    public Transform inventoryContentParent;  // Content of ScrollView in InventoryPanel
    public GameObject fishSlotPrefab;         // Prefab for inventory fish
    public FishInventory inventoryManager;    // Reference to player's inventory

    [Header("Fish Slots in Display")]
    public List<DisplaySlotUI> displaySlots = new List<DisplaySlotUI>(); // List of 12 slots

    private void Awake()
    {
        Instance = this;
        displayPanel.SetActive(false); // Hide by default
        inventoryPanel.SetActive(false);
    }
public void OpenDisplay()
{
    if (UIController.Instance.isUIBlocked) return;

    displayPanel.SetActive(true);
    foreach (var slot in displaySlots)
        slot.SetupSlot();

    // Block world UI
    UIController.Instance.BlockUI(true);
    WorldUIManager.Instance.DisableWorldUI();
}

public void CloseDisplay()
{
    displayPanel.SetActive(false);
    inventoryPanel.SetActive(false);

    // Unblock world UI
    UIController.Instance.BlockUI(false);
    WorldUIManager.Instance.EnableWorldUI();
}

    // Called when a locked fish slot is clicked
    public void OpenInventoryForSlot(DisplaySlotUI slot)
    {
        darkBackground.SetActive(true);
        //inventoryButton.SetActive(false);
        //orldMoneyText.gameObject.SetActive(false); 
        inventoryPanel.SetActive(true);
        PopulateInventory(slot);
    }

    private void PopulateInventory(DisplaySlotUI targetSlot)
    {
        // Clear previous inventory items
        foreach (Transform t in inventoryContentParent)
            Destroy(t.gameObject);

        foreach (var fish in inventoryManager.inventory.fishes)
        {
            GameObject go = Instantiate(fishSlotPrefab, inventoryContentParent);
            FishSlotUI slotUI = go.GetComponent<FishSlotUI>();
            slotUI.SetFish(fish);

            // Add listener to choose this fish
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                TryPlaceFishInSlot(targetSlot, fish);
            });
        }
    }

    private void TryPlaceFishInSlot(DisplaySlotUI slot, FishInstance fish)
    {
        bool success = slot.TryUnlock(fish);
        if (success)
        {
            inventoryManager.RemoveFish(fish); // remove from player's inventory
            inventoryPanel.SetActive(false);   // hide inventory after success
        }
        else
        {
            // Optional: play error sound or flash UI to indicate wrong fish
        }
    }


public void CloseInventoryFromDisplay()
{
    inventoryPanel.SetActive(false);
    UIController.Instance.ShowDarkBackground(false);
    UIController.Instance.BlockUI(false);
}
}
