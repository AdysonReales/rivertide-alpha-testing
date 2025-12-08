using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fishIcon;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI weightText;
    public TextMeshProUGUI priceText;
    // Sell Button Removed

    private FishInstance myFish;

    public void SetFish(FishInstance fish)
    {
        myFish = fish;

        // Safety Check: Ensure fish data exists
        if (fish == null || fish.data == null) return;

        // 1. Set Icon (With Safety Check)
        if (fishIcon != null) 
        {
            fishIcon.sprite = fish.data.sprite;
            fishIcon.preserveAspect = true; // Keeps the fish from looking stretched
        }

        // 2. Set Name
        if (fishNameText != null) 
            fishNameText.text = fish.data.fishName;

        // 3. Set Weight
        if (weightText != null) 
            weightText.text = $"{fish.kg:F2} kg";
        
        // 4. Set Price (Just for display now)
        if (priceText != null)
        {
            int price = Mathf.RoundToInt(fish.data.price * fish.kg);
            priceText.text = price + "g";
        }

        // Sell Button Logic Removed
    }
}