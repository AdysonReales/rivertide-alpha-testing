using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;

    [Header("Main Parents")]
    public GameObject shopPanel;        // The root "shopPanel"
    public GameObject rightBox;         // The "RightBox" container

    [Header("Sub-Panels")]
    public GameObject dialogueBox;      // "DialogueBox"
    public GameObject shopItemPrefab;   // "ShopItemPrefab" (The container for bait/tackle items)
    public GameObject sellItemPrefab;   // "SellItemPrefab" (The container for Yes/No)

    [Header("Right Box Buttons")]
    public Button buyButton;
    public Button sellButton;
    public Button exitButton;
    public TextMeshProUGUI moneyText;   // "Money" inside RightBox

    [Header("Sell Panel Elements")]
    public TextMeshProUGUI sellText;    // "SellText" inside SellItemPrefab
    public Button yesButton;            // "YesButton" inside SellItemPrefab
    public Button noButton;             // "NoButton" inside SellItemPrefab

    [Header("Dialogue Text")]
    public TextMeshProUGUI dialogueText; // Text inside DialogueBox (optional)

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // --- DELETE OR COMMENT OUT THIS LINE ---
        // if (shopPanel != null) shopPanel.SetActive(false); 
        // ---------------------------------------

        // Instead, we assume you have unchecked the box in the Inspector 
        // to hide it by default.

        // 2. Setup Main Buttons
        if (buyButton) buyButton.onClick.AddListener(OnBuyModeClicked);
        if (sellButton) sellButton.onClick.AddListener(OnSellModeClicked);
        if (exitButton) exitButton.onClick.AddListener(CloseShop);

        // 3. Setup Sell Confirmation Buttons
        if (yesButton) yesButton.onClick.AddListener(ConfirmSellAll);
        if (noButton) noButton.onClick.AddListener(CancelSell);
    }

    // --- MAIN STATE MANAGEMENT ---

    // Called when player presses 'E'
    public void OpenShop()
    {
        shopPanel.SetActive(true);
        
        // INITIAL STATE:
        // Dialogue: ON, RightBox: ON
        // ShopItems: OFF, SellItems: OFF
        
        dialogueBox.SetActive(true);
        rightBox.SetActive(true);
        
        shopItemPrefab.SetActive(false); 
        sellItemPrefab.SetActive(false);

        UpdateMoneyUI();
        if (dialogueText) dialogueText.text = "Welcome! How can I help you?";
    }

  public void CloseShop()
    {
        // 1. Hide the Shop Panel
        shopPanel.SetActive(false);
        
        // 2. Re-enable World UI interactions
        if (WorldUIManager.Instance) WorldUIManager.Instance.EnableWorldUI();

        // 3. FIX: Turn the Backpack Button back ON!
        if (UIManager.Instance != null && UIManager.Instance.inventoryButton != null)
        {
            UIManager.Instance.inventoryButton.SetActive(true);
        }
    }
    // --- BUTTON LOGIC ---

    void OnBuyModeClicked()
    {
        // Requirement: "DialogueBox and Sellitemprefab hidden, ShopItemPrefab visible"
        dialogueBox.SetActive(false);
        sellItemPrefab.SetActive(false);
        
        shopItemPrefab.SetActive(true); 
    }

    void OnSellModeClicked()
    {
        // Requirement: "Dialoguebox and ShopItemPrefab hidden, sellitemprefab visible"
        dialogueBox.SetActive(false);
        shopItemPrefab.SetActive(false);
        
        sellItemPrefab.SetActive(true);

        CalculateSellTotal();
    }

    // --- SELLING LOGIC ---

    void CalculateSellTotal()
    {
        if (GameManager.Instance == null) return;

        int totalValue = 0;
        int fishCount = 0;

        // Loop through all fish in inventory
        foreach (var fish in GameManager.Instance.playerInventory.fishes)
        {
            // Formula: Price * Weight
            int value = Mathf.RoundToInt(fish.data.price * fish.kg);
            totalValue += value;
            fishCount++;
        }

        // Update the text
        if (sellText) 
        {
            if (fishCount > 0)
                sellText.text = $"Sell {fishCount} fish for {totalValue}g?";
            else
                sellText.text = "You have no fish to sell.";
        }
        
        // Optional: Disable Yes button if no fish
        if (yesButton) yesButton.interactable = (fishCount > 0);
    }

    void ConfirmSellAll()
    {
        if (GameManager.Instance == null) return;

        int totalValue = 0;

        // 1. Calculate total
        foreach (var fish in GameManager.Instance.playerInventory.fishes)
        {
            totalValue += Mathf.RoundToInt(fish.data.price * fish.kg);
        }

        // 2. Add Money
        GameManager.Instance.money += totalValue;

        // 3. Clear Inventory
        GameManager.Instance.playerInventory.fishes.Clear();

        // 4. Save Game (Optional)
        // SaveLoad.SaveGame(); 

        // 5. UPDATE BOTH UI ELEMENTS
        
        // A. Update the Shop UI (RightBox)
        UpdateMoneyUI(); 
        
        // B. Update the World UI (Main Canvas) <--- THIS IS THE MISSING FIX
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateMoneyUI(GameManager.Instance.money);
        }
        
        // Refresh Inventory UI if it's open elsewhere (Safety check)
        InventoryUI invUI = FindFirstObjectByType<InventoryUI>();
        if (invUI != null) invUI.RefreshInventory();

        // 6. Return to "Dialogue" state
        CancelSell(); 
        
        if (dialogueText) dialogueText.text = $"Sold everything for {totalValue}g!";
    }

    void CancelSell()
    {
        // Requirement: "DialogueBox Goes back to visible"
        sellItemPrefab.SetActive(false);
        dialogueBox.SetActive(true);
        
        // Also hide shop items just in case
        shopItemPrefab.SetActive(false);
    }

    public void UpdateMoneyUI()
    {
        if (GameManager.Instance != null && moneyText != null)
            moneyText.text = GameManager.Instance.money + "g";
    }
}