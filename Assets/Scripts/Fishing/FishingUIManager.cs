using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishingUIManager : MonoBehaviour
{
    public static FishingUIManager Instance;

    [Header("UI Panels")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;
    public GameObject pullPanel;
    public Slider pullSlider;
    public TextMeshProUGUI pullPromptText;
    public GameObject catchPanel; 
    public TextMeshProUGUI catchText;
    public GameObject escapePanel;
    public TextMeshProUGUI escapeText;

    // We don't need individual icon references anymore since we are using the full Panel!
    // But keep them if you still have the old icons on screen.

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        HideAll();
        
        // --- NEW: UPDATE THE BAIT/TACKLE PANEL COUNTS ---
        UpdateBaitPanelCounts();
    }

    void UpdateBaitPanelCounts()
    {
        // CHANGE: Find objects EVERYWHERE in the scene, not just in children
        // Note: Use FindObjectsByType (Unity 6/2023+) or FindObjectsOfType (Older Unity)
        
        ShopItemUI[] shopItems = FindObjectsByType<ShopItemUI>(FindObjectsSortMode.None);
        
        // If you get an error on the line above, use this old version instead:
        // ShopItemUI[] shopItems = FindObjectsOfType<ShopItemUI>(true); 

        if (shopItems.Length == 0)
        {
            Debug.LogWarning("FishingUIManager: No Shop Items found! Did you paste the panel?");
        }

        foreach (var item in shopItems)
        {
            // 1. Force the text to update (shows "10")
            item.Initialize();

            // 2. Disable the Buy button so you can't buy while fishing
            if (item.buyButton != null)
            {
                item.buyButton.interactable = false; 
            }
        }
    }

    public void HideAll()
    {
        if(promptPanel) promptPanel.SetActive(false);
        if(pullPanel) pullPanel.SetActive(false);
        if(catchPanel) catchPanel.SetActive(false);
        if(escapePanel) escapePanel.SetActive(false);
    }

    public void ShowPrompt(string text)
    {
        promptPanel.SetActive(true);
        promptText.text = text;
    }

    public void HidePrompt() => promptPanel.SetActive(false);

    public void ShowPull(float progress, string instruction)
    {
        pullPanel.SetActive(true);
        pullPromptText.text = instruction;
        pullSlider.value = progress;
    }

    public void UpdatePull(float progress)
    {
        if (pullPanel.activeSelf)
            pullSlider.value = progress;
    }

    public void HidePull() => pullPanel.SetActive(false);

    public void ShowEscape()
    {
        escapePanel.SetActive(true);
        escapeText.text = "The fish escaped!";
        Invoke(nameof(HideEscape), 1.2f);
    }

    void HideEscape() => escapePanel.SetActive(false);
    
    public void ShowCatchSummary(string name, float weight) { } 
}