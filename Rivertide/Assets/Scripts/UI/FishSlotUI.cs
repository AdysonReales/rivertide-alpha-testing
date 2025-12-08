using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishSlotUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fishImage;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishKgText;

    private FishInstance fishData;

    // Call this to populate the UI for this slot
    public void SetFish(FishInstance fish)
    {
        if (fish == null) return;

        fishData = fish;

        if (fishImage != null)
            fishImage.sprite = fish.data.sprite;

        if (fishNameText != null)
            fishNameText.text = fish.data.fishName;

        if (fishKgText != null)
            fishKgText.text = $"{fish.kg:F2} kg";
    }

    // Optional: add a button to use/fish/inspect
    public void OnClickSlot()
    {
        Debug.Log($"Clicked on {fishData.data.fishName}, weight: {fishData.kg:F2} kg");
        // You can trigger other actions like moving it to display panel
    }
}
