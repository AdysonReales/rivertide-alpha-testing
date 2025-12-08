using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplaySlotUI : MonoBehaviour
{
    public Image fishImage; // <-- change from RawImage to Image
    public TextMeshProUGUI fishNameText;
    public TextMeshProUGUI fishDetailsText;

    public FishData targetFish; // assign in Inspector for each slot
    private bool unlocked = false;

    // Default blacked-out sprite (assign in inspector)
    public Sprite lockedSprite;

    public void SetupSlot()
    {
        unlocked = false;
        fishImage.sprite = lockedSprite; // now you can directly assign Sprite
        fishNameText.text = "????";
        fishDetailsText.text = "????";
    }

    public void OnClick()
    {
        if (!unlocked)
        {
            // Open inventory to select fish
            DisplayUIManager.Instance.OpenInventoryForSlot(this);
        }
    }

    public bool TryUnlock(FishInstance fish)
    {
        if (fish.data == targetFish)
        {
            unlocked = true;
            fishImage.sprite = fish.data.sprite; // direct assignment
            fishNameText.text = fish.data.fishName;
            fishDetailsText.text = $"{fish.kg:F2} kg\n{fish.data.description}";
            return true;
        }
        return false;
    }
}
