using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("World UI Icons")]
    public UnityEngine.UI.Image baitIcon;
    public UnityEngine.UI.Image magnetIcon;
    public UnityEngine.UI.Image rageIcon;
    public UnityEngine.UI.Image trapperIcon;
    public UnityEngine.UI.Image spinnerIcon;
    public UnityEngine.UI.Image barbedHookIcon;

    public TMP_Text baitText;
public TMP_Text magnetText;
public TMP_Text rageText;
public TMP_Text trapperText;
public TMP_Text spinnerText;
public TMP_Text barbedHookText;
    

    public static UIManager Instance;
    public GameObject inventoryButton; 
    public WorldUIManager worldUIManager;

    public GameObject shopPanel;
    public GameObject displayPanel;
    public GameObject inventoryPanel;

    public TMP_Text moneyText;

    public GameObject catchPanel;
    public TMP_Text catchText;

    public GameObject pullPrompt;
    public TMP_Text interactText; 

    private bool isCatchPanelActive = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Safety Check for Money
        if (GameManager.Instance != null)
        {
            UpdateMoneyUI(GameManager.Instance.money);
        }

        // Check for Last Caught Fish
        if (LastCaughtFish.fish != null)
        {
            ShowCatchPanelWithBlock(LastCaughtFish.fish);
            LastCaughtFish.fish = null;
        }

        if (GameManager.Instance != null)
    {
        UpdateMoneyUI(GameManager.Instance.money);
        UpdateWorldIcons(); // <--- ADD THIS LINE
    }
    }

    private void Update()
    {
        if (isCatchPanelActive && Input.GetMouseButtonDown(0))
        {
            CloseCatchPanel();
        }
    }

public void UpdateWorldIcons()
    {
        if (GameManager.Instance == null) return;

        // Helper to check count > 0
        // Note: Make sure strings match your Data exactly!
        SetIconState(baitIcon, GameManager.Instance.GetItemCount("Bait") > 0);
        SetIconState(magnetIcon, GameManager.Instance.GetItemCount("Magnet") > 0);
        SetIconState(rageIcon, GameManager.Instance.GetItemCount("Rage Bait") > 0);
        SetIconState(trapperIcon, GameManager.Instance.GetItemCount("Trapper") > 0);
        SetIconState(spinnerIcon, GameManager.Instance.GetItemCount("Spinner") > 0);
        SetIconState(barbedHookIcon, GameManager.Instance.GetItemCount("Barbed Hook") > 0);
    }
    // Add this small helper function at the bottom of UIManager
    void SetIconState(UnityEngine.UI.Image icon, bool isOwned)
    {
        if (icon == null) return;

        // Option A: Hide completely if not owned (Current behavior)
        // icon.enabled = isOwned; 

        // Option B: Make transparent/grey if not owned (Better visuals)
        icon.enabled = true; // Always keep the object on
        if (isOwned)
        {
            icon.color = Color.white; // Fully visible
        }
        else
        {
            icon.color = new Color(1f, 1f, 1f, 0.15f); // Very faint/transparent
        }
    }


    // ------------------ CATCH PANEL FUNCTIONS ------------------
   public void ShowCatchPanelWithBlock(FishInstance fi)
    {
        if (fi == null || catchPanel == null || catchText == null) return;

        // Stop player movement
        var player = FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.canMove = false;

        bool isUnlocked = false;
        if (GameManager.Instance != null)
            isUnlocked = GameManager.Instance.IsFishUnlocked(fi.data);

        if (!isUnlocked)
        {
            // --- TEXT UPDATED HERE ---
            catchText.text = $"{fi.data.fishName}\n{fi.kg:F2} KG\nGo to the Truck to Display!";
        }
        else
        {
            catchText.text = $"{fi.data.fishName}\n{fi.kg:F2} KG";
        }

        catchPanel.SetActive(true);
        isCatchPanelActive = true;
    }


    private void CloseCatchPanel()
    {
        if (catchPanel != null) catchPanel.SetActive(false);
        isCatchPanelActive = false;

        // Re-enable player movement
        var player = GameObject.FindFirstObjectByType<PlayerController>();
        if (player != null)
            player.canMove = true;
    }

    // ------------------ EXISTING FUNCTIONS ------------------
    public void UpdateMoneyUI(int money)
    {
        if (moneyText != null)
            moneyText.text = money.ToString() + "g";
    }

   public void OpenShop()
    {
        // 1. Hide World UI
        if (inventoryButton) inventoryButton.SetActive(false);
        if (WorldUIManager.Instance) WorldUIManager.Instance.DisableWorldUI();

        // 2. Ensure the physical GameObject is ON first
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        // 3. Trigger the Shop Logic
        // Try the static instance first
        if (ShopUIManager.Instance != null)
        {
            ShopUIManager.Instance.OpenShop();
        }
        // Fallback: If Instance is null (first run), find the script on the panel we just turned on
        else if (shopPanel != null)
        {
            var shopScript = shopPanel.GetComponent<ShopUIManager>();
            if (shopScript != null)
            {
                shopScript.OpenShop();
            }
        }
    }

    public void CloseShop()
    {
        if (shopPanel) shopPanel.SetActive(false);
        if (WorldUIManager.Instance) WorldUIManager.Instance.EnableWorldUI();
    }

    public void OpenDisplay()
    {
        if (displayPanel) displayPanel.SetActive(true);
        PopulateDisplay();
    }

    public void CloseDisplay()
    {
        if (displayPanel) displayPanel.SetActive(false);
    }

   public void OpenInventory()
    {
        // 1. Turn on the panel
        if (inventoryPanel != null) 
        {
            inventoryPanel.SetActive(true);

            // 2. FORCE THE REFRESH
            // First, check if the script is attached directly to the panel (Fastest)
            InventoryUI invUI = inventoryPanel.GetComponent<InventoryUI>();
            
            if (invUI != null)
            {
                invUI.RefreshInventory();
            }
            else
            {
                // Fallback: Search the whole scene if not found on the panel
                invUI = FindFirstObjectByType<InventoryUI>();
                if(invUI != null) 
                {
                    invUI.RefreshInventory();
                }
                else
                {
                    Debug.LogWarning("UIManager: Could not find InventoryUI script anywhere!");
                }
            }
        }
    }

    public void CloseInventory()
    {
        if (inventoryPanel) inventoryPanel.SetActive(false);
    }

    

  // Replace your existing ShowCatchPanel(FishInstance fi) with this:
public void ShowCatchPanel(FishInstance fi)
{
    if (catchPanel == null || catchText == null)
    {
        Debug.LogError("UIManager: catchPanel or catchText is NULL!");
        return;
    }

    // CASE 1 — FishInstance is NULL
    if (fi == null)
    {
        Debug.LogWarning("ShowCatchPanel: FishInstance is NULL");
        catchText.text = "Unknown catch";
        catchPanel.SetActive(true);
        return;
    }

    // CASE 2 — FishData is NULL
    if (fi.data == null)
    {
        Debug.LogWarning("ShowCatchPanel: fi.data is NULL");
        catchText.text = "Unknown catch";
        catchPanel.SetActive(true);
        return;
    }

    string name = fi.data.fishName;
    float kg = fi.kg;

    bool isUnlocked = false;
    if (GameManager.Instance != null)
        isUnlocked = GameManager.Instance.IsFishUnlocked(fi.data);

    if (!isUnlocked)
    {
        // Only show Name + KG + "Put on Aquarium to Learn More"
        catchText.text = $"{name}\n{kg:F2} KG\nPut on Aquarium to Learn More";
    }
    else
    {
        // Known fish: only Name + KG
        catchText.text = $"{name}\n{kg:F2} KG";
    }

    catchPanel.SetActive(true);
}




    public void ShowEscape()
    {
        if (catchPanel) catchPanel.SetActive(true);
        if (catchText) catchText.text = "The fish escaped!";
    }

    public void ShowPullPrompt(bool show)
    {
        if (pullPrompt) pullPrompt.SetActive(show);
    }
    
    void PopulateDisplay()
    {
        // Delegate to the specialized Display Manager
        if (DisplayUIManager.Instance != null)
        {
            // This ensures the slots inside the display are refreshed/unlocked
            // We assume DisplayUIManager handles its own setup
            var slots = FindObjectsByType<DisplaySlotUI>(FindObjectsSortMode.None);
            foreach (var slot in slots)
            {
                slot.RefreshSlot();
            }
        }
    }

    void PopulateInventory()
    {
        // Find the InventoryUI script and tell it to refresh
        InventoryUI invUI = FindFirstObjectByType<InventoryUI>();
        
        if (invUI != null)
        {
            invUI.RefreshInventory();
        }
        else
        {
            // Fallback if the script isn't found (Safety check)
            Debug.LogWarning("UIManager: Could not find InventoryUI script in the scene!");
        }
    }
} // End of Class