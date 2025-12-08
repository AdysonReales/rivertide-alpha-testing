using UnityEngine;
using System.Linq;

public class AquariumDisplay : MonoBehaviour
{
    public Transform slotParent; // 12 slots
    public GameObject slotPrefab;

    void Start(){ PopulateSlots(); }

    public void PopulateSlots()
    {
        // instantiate 12 slot UI elements showing ??? if not unlocked
        // On click of a slot, open inventory and allow drag-drop. If correct fish dropped, unlock.
    }

    public bool TryPlaceFish(int slotIndex, FishInstance fish)
    {
        // intended: each slot corresponds to a specific FishData from GameManager.Instance.allFishData order.
        var target = GameManager.Instance.allFishData[slotIndex];
        if (fish.data == target)
        {
            // unlock, show details and remove fish from inventory
            GameManager.Instance.playerInventory.RemoveFish(fish);
            SaveLoad.SaveGame();
            // update UI
            return true;
        }
        return false;
    }
}
