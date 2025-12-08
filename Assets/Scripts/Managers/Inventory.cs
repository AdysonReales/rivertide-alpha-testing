using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Inventory
{
    // The list of fish the player has caught
    public List<FishInstance> fishes = new List<FishInstance>();

    // The dictionary for shop items (Bait, Magnet, etc.)
    public Dictionary<string, int> items = new Dictionary<string, int>();

    // --- THIS WAS MISSING: The list of unlocked fish for the Aquarium ---
    public List<string> unlockedFishNames = new List<string>(); 

    public void AddFish(FishInstance fish)
    {
        fishes.Add(fish);
    }

    public void RemoveFish(FishInstance fish)
    {
        if (fishes.Contains(fish))
            fishes.Remove(fish);
    }
}