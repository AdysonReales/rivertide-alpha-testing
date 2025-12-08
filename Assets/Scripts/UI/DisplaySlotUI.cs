using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySlotUI : MonoBehaviour
{
    [Header("Configuration")]
    public FishData targetFish; // Assign specific fish in Inspector (e.g., Slot 1 = Carp)
    public Sprite lockedSprite; // Black silhouette

    [Header("UI References")]
    public Image fishImage;
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI detailsText;
    public Button slotButton;

    private bool isUnlocked = false;

    private void Start()
    {
        RefreshSlot();
        slotButton.onClick.AddListener(OnSlotClicked);
    }

    public void RefreshSlot()
    {
        isUnlocked = GameManager.Instance.IsFishUnlocked(targetFish);

        if (isUnlocked)
        {
            // Show full details
            fishImage.sprite = targetFish.sprite;
            fishImage.color = Color.white;
            fishNameText.text = targetFish.fishName;
        }
        else
        {
            // Show locked state
            fishImage.sprite = lockedSprite; // Or targetFish.sprite with black color
            fishImage.color = Color.black;   // Silhouette effect
            fishNameText.text = "???";
        }
    }

    private void OnSlotClicked()
    {
        if (!isUnlocked)
        {
            // Open the specific inventory picker for this slot
            DisplayUIManager.Instance.OpenPickerForSlot(this);
        }
    }

    public void Unlock()
    {
        GameManager.Instance.UnlockFish(targetFish);
        RefreshSlot();
    }
}