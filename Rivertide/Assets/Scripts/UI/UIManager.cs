using UnityEngine;
using TMPro;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject inventoryButton; // assign InventoryButton here
    public WorldUIManager worldUIManager;



    public GameObject shopPanel;
    public GameObject displayPanel;
    public GameObject inventoryPanel;

    public TMP_Text moneyText;

    public GameObject catchPanel;
    public TMP_Text catchText;

    public GameObject pullPrompt;

    public TMP_Text interactText; // assigned to PlayerController.interactText

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        UpdateMoney();
    }

    public void UpdateMoney()
    {
        if (moneyText)
            moneyText.text = GameManager.Instance.money + "g";
    }

    public void OpenShop() { shopPanel.SetActive(true); 
        // Hide inventory button while shop is open
        if (inventoryButton != null)
            inventoryButton.SetActive(false);
        
        WorldUIManager.Instance.DisableWorldUI();
            
    }

    public void CloseShop() { shopPanel.SetActive(false); 
    WorldUIManager.Instance.EnableWorldUI();
    
    }

    public void OpenDisplay()
    {
        displayPanel.SetActive(true);
        PopulateDisplay();
    }
    public void CloseDisplay()
    {
        displayPanel.SetActive(false);
    }

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        PopulateInventory();
    }
    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
    }

    public void ShowCatchPanel(FishInstance fi)
    {
        catchPanel.SetActive(true);
        catchText.text = $"{fi.data.fishName}\n{fi.kg:F2} kg\nRarity: {fi.data.rarity}\n{fi.data.tagline}";
        UpdateMoney();
    }

    public void ShowEscape()
    {
        catchPanel.SetActive(true);
        catchText.text = "The fish escaped!";
    }

    public void ShowPullPrompt(bool show)
    {
        pullPrompt.SetActive(show);
    }

    void PopulateDisplay()
    {
        // show 12 icons, if owned show details.
        // implement UI binding in Unity editor
    }

    void PopulateInventory()
    {
        // show list of FishInstance in GameManager.Instance.playerInventory
    }
}
