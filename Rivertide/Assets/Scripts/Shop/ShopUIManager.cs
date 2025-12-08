using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUIManager : MonoBehaviour
{
    public static ShopUIManager Instance;
    public TextMeshProUGUI  worldMoneyText;
    [Header("World UI")]
    public WorldUIManager worldUIManager;

    [Header("Panels")]
    public GameObject inventoryButton; 
    public GameObject shopPanel;
    public GameObject dialogueBox;
    public GameObject shopItemsParent;
    public GameObject sellItemPrefab;

    [Header("Buttons")]
    public Button buyButton;
    public Button sellButton;
    public Button exitButton;

    [Header("Dialogue Texts")]
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI moneyText;
    public TextMeshProUGUI sellText;

    [Header("Prefabs")]
    public GameObject shopItemPrefab;

    private ShopSystem shopSystem;

    private void Awake()
    {
        Instance = this;
        shopSystem = GetComponent<ShopSystem>();

        buyButton.onClick.AddListener(OnBuyClicked);
        sellButton.onClick.AddListener(OnSellClicked);
        exitButton.onClick.AddListener(CloseShop);

        Button yesBtn = sellItemPrefab.transform.Find("YesButton").GetComponent<Button>();
        Button noBtn = sellItemPrefab.transform.Find("NoButton").GetComponent<Button>();
        yesBtn.onClick.AddListener(SellAllFish);
        noBtn.onClick.AddListener(() =>
        {
            sellItemPrefab.SetActive(false);
            dialogueBox.SetActive(true);
        });
    }

   public void OpenShop()
{
    if (UIController.Instance.isUIBlocked) return;

    shopPanel.SetActive(true);
    dialogueBox.SetActive(true);
    sellItemPrefab.SetActive(false);
    shopItemsParent.SetActive(false);

    // Block world UI
    UIController.Instance.BlockUI(true);

    UpdateMoneyText();
    dialogueText.text = "Welcome! What do you want to do?";
    
    // Disable world UI clicks
    WorldUIManager.Instance.DisableWorldUI();
}




public void CloseShop()
{
    shopPanel.SetActive(false);
    sellItemPrefab.SetActive(false);
    shopItemsParent.SetActive(false);

    // Unblock world UI
    UIController.Instance.BlockUI(false);
        // Re-enable world UI clicks
    WorldUIManager.Instance.EnableWorldUI();
}

    private void UpdateInventoryButton(bool show)
    {
        if (inventoryButton != null)
            inventoryButton.SetActive(show);
    }

    void UpdateMoneyText()
    {
        moneyText.text = GameManager.Instance.money + "g";
    }

    void OnBuyClicked()
    {
        // FIXED: Only show the Buy items panel
        dialogueBox.SetActive(false);
        sellItemPrefab.SetActive(false);
        shopItemsParent.SetActive(false);

        shopItemPrefab.SetActive(true);  // <-- THIS IS NOW THE BUY UI
    }

    void OnSellClicked()
    {
        dialogueBox.SetActive(false);
        shopItemPrefab.SetActive(false);
        sellItemPrefab.SetActive(true);

        int total = 0;
        foreach (var kvp in GameManager.Instance.playerInventory.items)
        {
            ShopItemData itemData = shopSystem.items.Find(x => x.displayName == kvp.Key);
            if (itemData != null)
                total += itemData.cost * kvp.Value;
        }

        sellText.text = $"Sell all your fish for {total}g?";
    }

    void SellAllFish()
    {
        foreach (var kvp in GameManager.Instance.playerInventory.items)
        {
            ShopItemData itemData = shopSystem.items.Find(x => x.displayName == kvp.Key);
            if (itemData != null)
            {
                GameManager.Instance.money += itemData.cost * kvp.Value;
            }
        }

        GameManager.Instance.playerInventory.items.Clear();
        UpdateMoneyText();

        sellItemPrefab.SetActive(false);
        shopItemPrefab.SetActive(false);

        dialogueBox.SetActive(true);
        dialogueText.text = "Thank you for selling!";
    }
}
