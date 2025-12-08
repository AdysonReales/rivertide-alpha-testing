using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Inventory
{
    public List<FishInstance> fishes = new List<FishInstance>();
    public Dictionary<string,int> items = new Dictionary<string,int>(); // bait, magnet, etc.

    public void AddFish(FishInstance f)
    {
        fishes.Add(f);
        SaveLoad.SaveInventory(this);
    }

    public void RemoveFish(FishInstance f)
    {
        fishes.Remove(f);
        SaveLoad.SaveInventory(this);
    }
}
