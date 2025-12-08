using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int money = 0;
    public Inventory playerInventory;
    public List<FishData> allFishData; // assign in inspector (12 fish)
    public List<ShopItemData> shopItems; // assign in inspector

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else Destroy(gameObject);
        if(playerInventory == null) playerInventory = new Inventory();
    }
}
