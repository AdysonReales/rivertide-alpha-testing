using UnityEngine;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel; // assign InventoryPanel here

    private bool isOpen = false;

    public void ToggleInventory()
    {
        if (inventoryPanel == null) return;

        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);
    }
}
